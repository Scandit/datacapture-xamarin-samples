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
using CoreFoundation;
using MatrixScanBubblesSample.Extensions;
using MatrixScanBubblesSample.Overlay;
using MatrixScanBubblesSampleiOS.Extensions;
using Scandit.DataCapture.Barcode.Data;
using Scandit.DataCapture.Barcode.Tracking.Capture;
using Scandit.DataCapture.Barcode.Tracking.Data;
using Scandit.DataCapture.Barcode.Tracking.UI.Overlay;
using Scandit.DataCapture.Core.Capture;
using Scandit.DataCapture.Core.Common.Geometry;
using Scandit.DataCapture.Core.Data;
using Scandit.DataCapture.Core.Source;
using Scandit.DataCapture.Core.UI;
using Scandit.DataCapture.Core.UI.Style;
using UIKit;

namespace MatrixScanBubblesSample
{
    public partial class ScanViewController : UIViewController, IBarcodeTrackingListener, IBarcodeTrackingAdvancedOverlayListener
    {
        private static readonly nfloat BarcodeToScreenTresholdRatio = 0.1f;
        private static readonly int ShelfCount = 4;
        private static readonly int BackroomCount = 8;

        private DataCaptureContext context;
        // Use the default camera
        private readonly Camera camera = Camera.GetDefaultCamera();
        private BarcodeTracking barcodeTracking;
        private DataCaptureView captureView;
        private BarcodeTrackingBasicOverlay basicOverlay;
        private BarcodeTrackingAdvancedOverlay advancedOverlay;

        private IDictionary<int, UIView> overlays = new Dictionary<int, UIView>();

        public ScanViewController(IntPtr handle) : base(handle) { }

        #region View lifecycle

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            this.SetupRecognition();
            this.freezeButton.TouchUpInside += (sender, eventArgs) =>
            {
                if (!(sender is UIButton button))
                {
                    return;
                }
                button.Selected = !button.Selected;
                if (button.Selected)
                {
                    this.Freeze();
                }
                else
                {
                    this.UnFreeze();
                }
            };
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            this.UnFreeze();
        }

        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);
            this.Freeze();
        }

        #endregion

        private void SetupRecognition()
        {
            // Create data capture context using your license key.
            this.context = DataCaptureContextExtensions.LicensedContext;

            // Use the camera and set it as the frame source of the context. The camera is off by
            // default and must be turned on to start streaming frames to the data capture context for recognition.
            // See viewWillAppear and viewDidDisappear above.
            this.context.SetFrameSourceAsync(this.camera);

            // Use the recommended camera settings for the BarcodeTracking mode as default settings.
            // The preferred resolution is automatically chosen, which currently defaults to HD on all devices.
            // Setting the preferred resolution to full HD helps to get a better decode range.
            var cameraSettings = BarcodeTracking.RecommendedCameraSettings;
            cameraSettings.PreferredResolution = VideoResolution.Uhd4k;
            this.camera?.ApplySettingsAsync(cameraSettings);

            // The barcode tracking process is configured through barcode tracking settings
            // and are then applied to the barcode tracking instance that manages barcode tracking.
            var settings = BarcodeTrackingSettings.Create(BarcodeTrackingScenario.A);

            // The settings instance initially has all types of barcodes (symbologies) disabled. For the purpose of this
            // sample we enable a very generous set of symbologies. In your own app ensure that you only enable the
            // symbologies that your app requires as every additional enabled symbology has an impact on processing times.
            settings.EnableSymbologies(new HashSet<Symbology>
            {
                Symbology.Ean13Upca,
                Symbology.Ean8,
                Symbology.Upce,
                Symbology.Code39,
                Symbology.Code128
            });

            // Create new barcode tracking mode with the settings from above.
            this.barcodeTracking = BarcodeTracking.Create(this.context, settings);

            this.barcodeTracking.AddListener(this);

            // To visualize the on-going barcode tracking process on screen, setup a data capture view that renders the
            // camera preview. The view must be connected to the data capture context.
            this.captureView = DataCaptureView.Create(this.context, this.View.Bounds);
            this.captureView.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
            this.View.AddSubview(this.captureView);
            this.View.SendSubviewToBack(this.captureView);

            // Add a barcode tracking overlay to the data capture view to render the tracked barcodes on top of the video
            // preview. This is optional, but recommended for better visual feedback. The overlay is automatically added
            // to the view.
            this.basicOverlay = BarcodeTrackingBasicOverlay.Create(this.barcodeTracking, this.captureView, BarcodeTrackingBasicOverlayStyle.Dot);

            // Add another barcode tracking overlay to the data capture view to render other views. The overlay is 
            // automatically added to the view.
            this.advancedOverlay = BarcodeTrackingAdvancedOverlay.Create(this.barcodeTracking, this.captureView);
            this.advancedOverlay.Listener = this;
        }

        private void Freeze()
        {
            // First, disable barcode tracking to stop processing frames.
            this.barcodeTracking.Enabled = false;
            // Switch the camera off to stop streaming frames. The camera is stopped asynchronously.
            this.camera?.SwitchToDesiredStateAsync(FrameSourceState.Off);
        }

        private void UnFreeze()
        {
            // First, enable barcode tracking to resume processing frames.
            this.barcodeTracking.Enabled = true;
            // Switch camera on to start streaming frames. The camera is started asynchronously and will take some time to
            // completely turn on.
            this.camera?.SwitchToDesiredStateAsync(FrameSourceState.On);
        }

        private bool ShouldHideOverlay(TrackedBarcode trackedCode, nfloat captureViewWidth)
        {
            // If the barcode is wider than the desired percent of the data capture view's width,
            // show it to the user.
            var width = trackedCode.Location.Width(this.captureView);
            return (width / captureViewWidth) <= BarcodeToScreenTresholdRatio;
        }

        private UIView CreateStockOverlay(TrackedBarcode trackedBarcode)
        {
            var identifier = trackedBarcode.Identifier;
            UIView overlay;
            if (this.overlays.Keys.Contains(identifier))
            {
                overlay = this.overlays[identifier];
            }
            else
            {
                // Get the information you want to show from your back end system/database.
                overlay = StockOverlay.Create(ShelfCount, BackroomCount, trackedBarcode.Barcode.Data);
                this.overlays[identifier] = overlay;
            }
            overlay.Hidden = this.ShouldHideOverlay(trackedBarcode, this.captureView.Frame.Width);
            return overlay;
        }

        #region IBarcodeTrackingListener

        // This method is called whenever objects are updated and it's the right place to react to the tracking results.
        public void OnSessionUpdated(BarcodeTracking barcodeTracking, BarcodeTrackingSession session, IFrameData frameData)
        {
            DispatchQueue.MainQueue.DispatchAsync(() =>
            {
                if (!this.barcodeTracking.Enabled)
                {
                    return;
                }
                foreach (var identifier in session.RemovedTrackedBarcodes)
                {
                    this.overlays.Remove(identifier);
                }
                var filteredTrackedCodes = session.TrackedBarcodes.Values.Where(code => code != null && code.Barcode != null);
                foreach (var trackedCode in filteredTrackedCodes)
                {
                    var success = this.overlays.TryGetValue(trackedCode.Identifier, out UIView overlay);
                    if (!success)
                    {
                        return;
                    }
                    overlay.Hidden = this.ShouldHideOverlay(trackedCode, this.captureView.Frame.Width);
                }
            });
            // Dispose the frame when you have finished processing it. If the frame is not properly disposed,
            // different issues could arise, e.g. a frozen, non-responsive, or "severely stuttering" video feed.
            frameData.Dispose();
        }

        public void OnObservationStarted(BarcodeTracking barcodeTracking) { }

        public void OnObservationStopped(BarcodeTracking barcodeTracking) { }

        #endregion

        #region IBarcodeTrackingAdvancedOverlayListener

        public UIView ViewForTrackedBarcode(BarcodeTrackingAdvancedOverlay overlay, TrackedBarcode trackedBarcode)
        {
            return CreateStockOverlay(trackedBarcode);
        }

        public Anchor AnchorForTrackedBarcode(BarcodeTrackingAdvancedOverlay overlay, TrackedBarcode trackedBarcode)
        {
            // The offset of our overlay will be calculated from the top center anchoring point.
            return Anchor.TopCenter;
        }

        public PointWithUnit OffsetForTrackedBarcode(BarcodeTrackingAdvancedOverlay overlay, TrackedBarcode trackedBarcode)
        {
            // We set the offset's height to be equal of the 100 percent of our overlay.
            // The minus sign means that the overlay will be above the barcode.
            return new PointWithUnit()
            {
                X = new FloatWithUnit()
                {
                    Value = 0,
                    Unit = MeasureUnit.Fraction
                },
                Y = new FloatWithUnit()
                {
                    Value = -1,
                    Unit = MeasureUnit.Fraction
                }
            };
        }

        #endregion
    }
}
