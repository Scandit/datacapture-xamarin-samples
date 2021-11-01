/*
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using UIKit;

using Scandit.DataCapture.Barcode.Data;
using Scandit.DataCapture.Barcode.Selection.Capture;
using Scandit.DataCapture.Barcode.Selection.UI.Overlay;
using Scandit.DataCapture.Core.Capture;
using Scandit.DataCapture.Core.Source;
using Scandit.DataCapture.Core.UI;
using Scandit.DataCapture.Core.Data;
using CoreFoundation;
using CoreGraphics;

namespace BarcodeSelectionSimpleSample
{
    public partial class ViewController : UIViewController, IBarcodeSelectionListener
    {
        // Enter your Scandit License key here.
        public const string SCANDIT_LICENSE_KEY = "-- ENTER YOUR SCANDIT LICENSE KEY HERE --";

        private DataCaptureContext dataCaptureContext;
        private Camera camera;
        private BarcodeSelection barcodeSelection;
        private BarcodeSelectionSettings barcodeSelectionSettings;

        private const int dialogAutoDissmissInterval = 500;
        private UIButton aimToSelectButton;
        private UIButton tapToSelectButton;

        public ViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillAppear(animated);
            // First, disable barcode selection to stop processing frames.
            this.barcodeSelection.Enabled = false;
            // Switch the camera off to stop streaming frames. The camera is stopped asynchronously.
            this.camera?.SwitchToDesiredStateAsync(FrameSourceState.Off);
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            // First, enable barcode selection to resume processing frames.
            this.barcodeSelection.Enabled = true;
            // Switch camera on to start streaming frames. The camera is started asynchronously and will take some time to
            // completely turn on.
            this.camera?.SwitchToDesiredStateAsync(FrameSourceState.On);
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            this.View.BackgroundColor = UIColor.Black;
            this.InitializeAndStartBarcodeSelection();
            this.AddAimToSelectButton();
            this.AddTapToSelectButton();
            this.View.LayoutIfNeeded();
        }

        protected void InitializeAndStartBarcodeSelection()
        {
            // Create data capture context using your license key.
            this.dataCaptureContext = DataCaptureContext.ForLicenseKey(SCANDIT_LICENSE_KEY);

            // Use the default camera and set it as the frame source of the context.
            // The camera is off by default and must be turned on to start streaming frames to the data
            // capture context for recognition.
            // See resumeFrameSource and pauseFrameSource below.
            this.camera = Camera.GetDefaultCamera();
            if (this.camera != null)
            {
                // Use the settings recommended by barcode capture.
                this.dataCaptureContext.SetFrameSourceAsync(this.camera);
            }

            // Use the recommended camera settings for the BarcodeSelection mode as default settings.
            // The preferred resolution is automatically chosen, which currently defaults to HD on all devices.            
            CameraSettings cameraSettings = BarcodeSelection.RecommendedCameraSettings;

            // Setting the preferred resolution to full HD helps to get a better decode range.
            cameraSettings.PreferredResolution = VideoResolution.FullHd;
            camera?.ApplySettingsAsync(cameraSettings);
            this.barcodeSelectionSettings = BarcodeSelectionSettings.Create();

            // The settings instance initially has all types of barcodes (symbologies) disabled.
            // For the purpose of this sample we enable a very generous set of symbologies.
            // In your own app ensure that you only enable the symbologies that your app requires as
            // every additional enabled symbology has an impact on processing times.
            HashSet<Symbology> symbologies = new HashSet<Symbology>()
            {
                Symbology.Ean13Upca,
                Symbology.Ean8,
                Symbology.Upce,
                Symbology.Code39,
                Symbology.Code128
            };

            this.barcodeSelectionSettings.EnableSymbologies(symbologies);

            // Create new barcode selection mode with the settings from above.
            this.barcodeSelection = BarcodeSelection.Create(this.dataCaptureContext, this.barcodeSelectionSettings);

            // Register self as a listener to get informed whenever a new barcode got selected.
            this.barcodeSelection.AddListener(this);

            // To visualize the on-going barcode selection process on screen, setup a data capture view
            // that renders the camera preview. The view must be connected to the data capture context.
            var dataCaptureView = DataCaptureView.Create(this.dataCaptureContext,
                                                         new CGRect(
                                                             this.View.Bounds.Location,
                                                             new CGSize(
                                                                 this.View.Bounds.Width,
                                                                 this.View.Bounds.Height - 48)));
            dataCaptureView.AutoresizingMask = UIViewAutoresizing.FlexibleHeight |
                                               UIViewAutoresizing.FlexibleWidth;
            this.View.AddSubview(dataCaptureView);
            this.View.SendSubviewToBack(dataCaptureView);

            // Create barcode selection overlay to the data capture view to render the selected barcodes on
            // top of the video preview. This is optional, but recommended for better visual feedback.
            BarcodeSelectionBasicOverlay.Create(this.barcodeSelection, dataCaptureView);
        }

        protected void AddAimToSelectButton()
        {
            this.aimToSelectButton = new UIButton(UIButtonType.RoundedRect);
            this.aimToSelectButton.SetTitle("Aim to Select", UIControlState.Normal);
            this.aimToSelectButton.SetTitleColor(UIColor.White, UIControlState.Normal);
            this.aimToSelectButton.BackgroundColor = UIColor.Black;
            this.aimToSelectButton.TranslatesAutoresizingMaskIntoConstraints = false;
            this.aimToSelectButton.Font = UIFont.SystemFontOfSize(18);

            this.View.AddSubview(aimToSelectButton);
            this.View.AddConstraints(new[]
            {
                this.aimToSelectButton.LeadingAnchor.ConstraintEqualTo(this.View.LeadingAnchor, 32),
                this.aimToSelectButton.BottomAnchor.ConstraintEqualTo(this.View.BottomAnchor, -8)
            });
            this.aimToSelectButton.TouchUpInside += AimToSelectButtonClick;
        }

        protected void AddTapToSelectButton()
        {
            this.tapToSelectButton = new UIButton(UIButtonType.RoundedRect);
            this.tapToSelectButton.SetTitle("Tap to Select", UIControlState.Normal);
            this.tapToSelectButton.SetTitleColor(UIColor.White, UIControlState.Normal);
            this.tapToSelectButton.BackgroundColor = UIColor.Black;
            this.tapToSelectButton.TranslatesAutoresizingMaskIntoConstraints = false;
            this.tapToSelectButton.Font = UIFont.SystemFontOfSize(18);

            this.View.AddSubview(tapToSelectButton);
            this.View.AddConstraints(new[]
            {
                this.tapToSelectButton.TrailingAnchor.ConstraintEqualTo(this.View.TrailingAnchor, -32),
                this.tapToSelectButton.BottomAnchor.ConstraintEqualTo(this.View.BottomAnchor, -8)
            });
            this.tapToSelectButton.TouchUpInside += TapToSelectButtonClick;
        }

        private void AimToSelectButtonClick(object sender, System.EventArgs e)
        {
            if (this.barcodeSelectionSettings.SelectionType is BarcodeSelectionTapSelection)
            {
                this.barcodeSelectionSettings.SelectionType = BarcodeSelectionAimerSelection.Create();
                this.barcodeSelection.ApplySettingsAsync(this.barcodeSelectionSettings);
            }
        }

        private void TapToSelectButtonClick(object sender, System.EventArgs e)
        {
            if (this.barcodeSelectionSettings.SelectionType is BarcodeSelectionAimerSelection)
            {
                this.barcodeSelectionSettings.SelectionType = BarcodeSelectionTapSelection.Create();
                this.barcodeSelection.ApplySettingsAsync(this.barcodeSelectionSettings);
            }
        }

        #region IBarcodeSelectionListener
        public void OnSelectionUpdated(BarcodeSelection barcodeSelection, BarcodeSelectionSession session, IFrameData frameData)
        {
            if (!session.NewlySelectedBarcodes.Any())
            {
                return;
            }

            Barcode barcode = session.NewlySelectedBarcodes.First();

            // Get barcode selection count.
            int selectionCount = session.GetCount(barcode);

            // Get the human readable name of the symbology and assemble the result to be shown.
            using SymbologyDescription description = SymbologyDescription.Create(barcode.Symbology);
            string result = barcode.Data + " (" + description.ReadableName + ")" + "\nTimes: " + selectionCount; ;

            this.ShowResult(result);

            // Dispose the frame when you have finished processing it. If the frame is not properly disposed,
            // different issues could arise, e.g. a frozen, non-responsive, or "severely stuttering" video feed.
            frameData?.Dispose();
        }

        public void OnSessionUpdated(BarcodeSelection barcodeSelection, BarcodeSelectionSession session, IFrameData frameData)
        {
            // Dispose the frame when you have finished processing it. If the frame is not properly disposed,
            // different issues could arise, e.g. a frozen, non-responsive, or "severely stuttering" video feed.
            frameData?.Dispose();
        }

        public void OnObservationStarted(BarcodeSelection barcodeSelection)
        { }

        public void OnObservationStopped(BarcodeSelection barcodeSelection)
        { }
        #endregion

        private void ShowResult(string result)
        {
            DispatchQueue.MainQueue.DispatchAsync(() =>
            {
                var alert = UIAlertController.Create(result, message: null, UIAlertControllerStyle.Alert);
                this.PresentViewController(alert, true, () =>
                {
                    var delta = new TimeSpan(0, 0, 0, 0, dialogAutoDissmissInterval);
                    var dispatchTime = new DispatchTime(DispatchTime.Now, delta);
                    DispatchQueue.MainQueue.DispatchAfter(dispatchTime, () =>
                    {
                        this.DismissViewController(true, completionHandler: null);
                    });
                });
            });
        }
    }
}
