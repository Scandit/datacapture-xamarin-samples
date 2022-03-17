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
using System.Text;
using CoreFoundation;
using CoreGraphics;
using Foundation;
using IdCaptureExtendedSample.ModeCollection;
using IdCaptureExtendedSample.Result;
using Scandit.DataCapture.Core.Data;
using Scandit.DataCapture.Core.Source;
using Scandit.DataCapture.Core.UI;
using Scandit.DataCapture.ID.Capture;
using Scandit.DataCapture.ID.Data;
using Scandit.DataCapture.ID.UI;
using Scandit.DataCapture.ID.UI.Overlay;
using UIKit;

namespace IdCaptureExtendedSample
{
    public partial class ViewController : UIViewController, IIdCaptureListener
    {
        private IdCapture IdCapture => DataCaptureManager.Instance.IdCapture;
        private Camera Camera => DataCaptureManager.Instance.Camera;

        private DataCaptureView dataCaptureView;
        private IdCaptureOverlay captureOverlay;

        private bool isScanningBackSide = false;
        private const float modeCollectionHeight = 80;

        public ViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            this.SetupRecognition();
            this.SetupModeCollection();
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            this.IdCapture.Reset();
            this.isScanningBackSide = false;
            this.IdCapture.Enabled = true;

            // Switch camera on to start streaming frames. The camera is started asynchronously and will take some time to
            // completely turn on.
            this.Camera?.SwitchToDesiredStateAsync(FrameSourceState.On);
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);

            // Switch camera off to stop streaming frames. The camera is stopped asynchronously and will take some time to
            // completely turn off. Until it is completely stopped, it is still possible to receive further results, hence
            // it's a good idea to first disable id capture as well.
            this.IdCapture.Enabled = false;
            this.Camera?.SwitchToDesiredStateAsync(FrameSourceState.Off);
        }

        #region IIdCaptureListener
        public void OnIdCaptured(IdCapture capture, IdCaptureSession session, IFrameData frameData)
        {
            try
            {
                CapturedId capturedId = session.NewlyCapturedId;

                // Pause the IdCapture to not capture while showing the result.
                this.IdCapture.Enabled = false;

                // Viz documents support multiple sides scanning.
                // In case the back side is supported and not yet captured we inform the user about the feature.
                if (capturedId.Viz != null &&
                    capturedId.Viz.BackSideCaptureSupported &&
                    capturedId.Viz.CapturedSides == SupportedSides.FrontOnly)
                {
                    // Until the back side is scanned, IdCapture will keep reporting the front side.
                    // If we are looking for the back side we just return.
                    if (this.isScanningBackSide)
                    {
                        this.IdCapture.Enabled = true;
                    }
                    else
                    {
                        DispatchQueue.MainQueue.DispatchAsync(() =>
                        {
                            this.DisplayBackOfCardAlert(capturedId: capturedId);
                        });
                    }
                }
                else
                {
                    // Show the result
                    DispatchQueue.MainQueue.DispatchAsync(() =>
                    {
                        this.Display(capturedId: capturedId);
                    });
                }
            }
            finally
            {
                // Dispose the frame when you have finished processing it. If the frame is not properly disposed,
                // different issues could arise, e.g. a frozen, non-responsive, or "severely stuttering" video feed.
                frameData?.Dispose();
            }
        }

        public void OnIdLocalized(IdCapture capture, IdCaptureSession session, IFrameData frameData)
        {
            // Implement to handle a personal identification document or its part localized within
            // a frame. A document or its part is considered localized when it's detected in a frame,
            // but its data is not yet extracted.

            // In this sample we are not interested in this callback.

            // Dispose the frame when you have finished processing it. If the frame is not properly disposed,
            // different issues could arise, e.g. a frozen, non-responsive, or "severely stuttering" video feed.
            frameData?.Dispose();
        }

        public void OnIdRejected(IdCapture capture, IdCaptureSession session, IFrameData frameData)
        {
            try
            {
                // Implement to handle documents recognized in a frame, but rejected.
                // A document or its part is considered rejected when (a) it's valid, but not enabled in the settings,
                // (b) it's a barcode of a correct symbology or a Machine Readable Zone (MRZ),
                // but the data is encoded in an unexpected/incorrect format.

                // Pause the IdCapture to not capture while showing the result.
                this.IdCapture.Enabled = false;
                this.ShowResult(title: string.Empty, message: "Document not supported", completion: () => {
                    // Resume the idCapture.
                    this.IdCapture.Enabled = true;
                });
            }
            finally
            {
                // Dispose the frame when you have finished processing it. If the frame is not properly disposed,
                // different issues could arise, e.g. a frozen, non-responsive, or "severely stuttering" video feed.
                frameData?.Dispose();
            }
        }

        public void OnErrorEncountered(IdCapture capture, NSError error, IdCaptureSession session, IFrameData frameData)
        {
            try
            {
                // Implement to handle an error encountered during the capture process.
                // The error message can be retrieved from the Error localizedDescription.
                this.ShowResult(title: "Error", message: GetErrorMessage(error));
            }
            finally
            {
                // Dispose the frame when you have finished processing it. If the frame is not properly disposed,
                // different issues could arise, e.g. a frozen, non-responsive, or "severely stuttering" video feed.
                frameData?.Dispose();
            }
        }

        public void OnObservationStarted(IdCapture idCapture)
        { }

        public void OnObservationStopped(IdCapture idCapture)
        { }
        #endregion

        private void SetupRecognition()
        {
            DataCaptureManager.Instance.InitializeCamera();

            // To visualize the on-going capturing process on screen, setup a data capture view
            // that renders the camera preview. The view must be connected to the data capture context.
            this.dataCaptureView = DataCaptureView.Create(DataCaptureManager.Instance.DataCaptureContext, CGRect.Empty);
            this.View.AddSubview(this.dataCaptureView);
            this.dataCaptureView.TranslatesAutoresizingMaskIntoConstraints = false;
            this.View.AddConstraints(new[] {
                this.dataCaptureView.LeadingAnchor.ConstraintEqualTo(this.View.LeadingAnchor),
                this.dataCaptureView.TopAnchor.ConstraintEqualTo(this.View.SafeAreaLayoutGuide.TopAnchor),
                this.dataCaptureView.TrailingAnchor.ConstraintEqualTo(this.View.TrailingAnchor),
                this.dataCaptureView.BottomAnchor.ConstraintEqualTo(this.View.BottomAnchor, constant: -modeCollectionHeight)
            });

            this.Configure(Mode.Barcode);
        }

        private void SetupModeCollection()
        {
            var layout = new UICollectionViewFlowLayout();
            layout.EstimatedItemSize = UICollectionViewFlowLayout.AutomaticSize;
            layout.MinimumInteritemSpacing = 10;
            layout.ScrollDirection = UICollectionViewScrollDirection.Horizontal;
            var modeCollection = new ModeCollectionViewController(layout, (Mode mode) => this.Configure(mode));
            this.AddChildViewController(modeCollection);
            this.View.AddSubview(modeCollection.View);
            modeCollection.View.TranslatesAutoresizingMaskIntoConstraints = false;
            modeCollection.View.BottomAnchor.ConstraintEqualTo(this.View.BottomAnchor).Active = true;
            modeCollection.View.LeadingAnchor.ConstraintEqualTo(this.View.LeadingAnchor).Active = true;
            modeCollection.View.TrailingAnchor.ConstraintEqualTo(this.View.TrailingAnchor).Active = true;
            modeCollection.View.HeightAnchor.ConstraintEqualTo(modeCollectionHeight).Active = true;
            modeCollection.SelectItem(0);
        }

        private void Configure(Mode mode)
        {
            this.IdCapture?.RemoveListener(this);
            this.isScanningBackSide = false;

            if (this.captureOverlay != null)
            {
                this.dataCaptureView?.RemoveOverlay(this.captureOverlay);
            }

            DataCaptureManager.Instance.ConfigureIdCapture(mode);
            this.IdCapture.AddListener(this);
            this.captureOverlay = IdCaptureOverlay.Create(this.IdCapture, this.dataCaptureView);
            this.captureOverlay.IdLayoutStyle = IdLayoutStyle.Rounded;
        }

        private void ShowResult(string title, string message, Action completion = null)
        {
            DispatchQueue.MainQueue.DispatchAsync(() => {
                UIAlertController alert = UIAlertController.Create(title, message, preferredStyle: UIAlertControllerStyle.Alert);
                var action = UIAlertAction.Create("OK", UIAlertActionStyle.Default, (UIAlertAction) => {
                    completion?.Invoke();
                });
                alert.AddAction(action);
                this.PresentViewController(alert, animated: true, completionHandler: null);
            });
        }

        private void Display(CapturedId capturedId)
        {
            var resultView = new ResultViewController(capturedId);
            this.NavigationController?.PushViewController(resultView, animated: true);
        }

        private void DisplayBackOfCardAlert(CapturedId capturedId)
        {
            string message = "This documents has additional data in the visual inspection zone on the back of the card";
            UIAlertController alertController = UIAlertController.Create(title: "Back of Card", message, UIAlertControllerStyle.Alert);

            var scanAction = UIAlertAction.Create(title: "Scan",
                                                    UIAlertActionStyle.Default,
                                                    (UIAlertAction handler) =>
                                                    {
                                                        this.isScanningBackSide = true;
                                                        this.IdCapture.Enabled = true;
                                                    });
            alertController.AddAction(scanAction);

            var skipAction = UIAlertAction.Create(title: "Skip",
                                                    UIAlertActionStyle.Cancel,
                                                    (UIAlertAction handler) =>
                                                    {
                                                        this.IdCapture.Reset();
                                                        this.Display(capturedId);
                                                    });
            alertController.AddAction(skipAction);
            this.PresentViewController(alertController, animated: true, completionHandler: null);
        }

        private static string GetErrorMessage(NSError error)
        {
            int errCode = (int)error.Code;
            StringBuilder messageBuilder = new StringBuilder(((IdCaptureErrorCode)errCode).ToString());

            if (!string.IsNullOrEmpty(error.LocalizedDescription))
            {
                messageBuilder.Append($": {error.LocalizedDescription}");
            }

            return messageBuilder.ToString();
        }
    }
}
