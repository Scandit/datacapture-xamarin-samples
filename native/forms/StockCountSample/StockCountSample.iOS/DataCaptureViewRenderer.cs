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

using System.Collections.Generic;
using System.Linq;
using Scandit.DataCapture.Barcode.Tracking.Capture;
using Scandit.DataCapture.Barcode.Tracking.Data;
using Scandit.DataCapture.Barcode.Tracking.UI.Overlay;
using Scandit.DataCapture.Core.Capture;
using Scandit.DataCapture.Core.Data;
using Scandit.DataCapture.Core.Source;
using StockCountSample.Bridging;
using StockCountSample.iOS;
using StockCountSample.iOS.Extensions;
using StockCountSample.ViewModel;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

using NativeDataCaptureView = Scandit.DataCapture.Core.UI.DataCaptureView;
using NativeBrush = Scandit.DataCapture.Core.UI.Style.Brush;
using Scandit.DataCapture.Barcode.Data;
using System;

[assembly: ExportRenderer(typeof(DataCaptureView), typeof(DataCaptureViewRenderer))]
namespace StockCountSample.iOS
{
    public class DataCaptureViewRenderer : ViewRenderer<DataCaptureView, NativeDataCaptureView>, IBarcodeTrackingBasicOverlayListener, IBarcodeTrackingListener
    {
        private NativeDataCaptureView captureView;
        private BarcodeTracking barcodeTracking;
        private DataCaptureContext context;
        
        // Use the default camera.
        private readonly Camera camera = Camera.GetDefaultCamera();
        private BarcodeTrackingBasicOverlay overlay;

        protected override void OnElementChanged(ElementChangedEventArgs<DataCaptureView> e)
        {
            base.OnElementChanged(e);
            if (e.OldElement != null)
            {
                // Unsubscribe from event handlers and cleanup any resources
                e.OldElement.PauseScanningRequested -= OnPauseScanningRequested;
                e.OldElement.ResumeScanningRequested -= OnResumeScanningRequested;

                this.context?.Dispose();
            }

            if (e.NewElement != null)
            {
                // Instantiate the native control and assign it to the Control property with
                // the SetNativeControl method
                if (this.Control == null)
                {
                    e.NewElement.PauseScanningRequested += OnPauseScanningRequested;
                    e.NewElement.ResumeScanningRequested += OnResumeScanningRequested;

                    this.context = DataCaptureContext.ForLicenseKey(e.NewElement.LicenseKey);
                    // Set the camera as the frame source of the context. The camera is off by
                    // default and must be turned on to start streaming frames to the data capture context for recognition.
                    if (this.camera != null)
                    {
                        this.context.SetFrameSourceAsync(this.camera);
                    }

                    // The barcode tracking process is configured through barcode tracking settings
                    // and are then applied to the barcode tracking instance that manages barcode tracking.
                    var settings = BarcodeTrackingSettings.Create();

                    // The settings instance initially has all types of barcodes (symbologies) disabled. For the purpose of this
                    // sample we enable a very generous set of symbologies. In your own app ensure that you only enable the
                    // symbologies that your app requires as every additional enabled symbology has an impact on processing times.
                    var symbologiesToEnable = new List<Symbology> {
                        Symbology.Ean8,
                        Symbology.Ean13Upca,
                        Symbology.Code128,
                        Symbology.DataMatrix
                    };
                    settings.EnableSymbologies(symbologiesToEnable);

                    // Create new barcode tracking mode with the settings from above.
                    this.barcodeTracking = BarcodeTracking.Create(this.context, settings);

                    // Register this as a listener to get informed of tracked barcodes.
                    this.barcodeTracking.AddListener(this);

                    // To visualize the on-going barcode tracking process on screen, setup a data capture view that renders the
                    // camera preview. The view must be connected to the data capture context.
                    this.captureView = NativeDataCaptureView.Create(this.context, NativeView.Frame);

                    this.captureView.FocusGesture = null;
                    this.captureView.ZoomGesture = null;

                    // Once the native DataCaptureView is created it needs to be set as the native control.
                    this.SetNativeControl(this.captureView);

                    // Add a barcode tracking overlay to the data capture view to render the tracked barcodes on top of the video
                    // preview. This is optional, but recommended for better visual feedback.
                    this.overlay = BarcodeTrackingBasicOverlay.Create(this.barcodeTracking, this.captureView, BarcodeTrackingBasicOverlayStyle.Frame);
                    this.overlay.Listener = this;

                    var cameraSettings = BarcodeTracking.RecommendedCameraSettings;
                    this.camera?.ApplySettingsAsync(cameraSettings);

                    this.camera?.SwitchToDesiredStateAsync(FrameSourceState.On);
                    this.barcodeTracking.Enabled = true;
                }
            }
        }

        public NativeBrush BrushForTrackedBarcode(BarcodeTrackingBasicOverlay overlay, TrackedBarcode trackedBarcode)
        {
            return this.Element.TrackedBarcodesBrush.ConvertToNative();
        }

        public void OnTrackedBarcodeTapped(BarcodeTrackingBasicOverlay overlay, TrackedBarcode trackedBarcode)
        {}

        public void OnSessionUpdated(BarcodeTracking barcodeTracking, BarcodeTrackingSession session, IFrameData frameData)
        {
            // This method is called whenever objects are updated and it's the right place to react to the tracking results.
            var products = session.TrackedBarcodes.Values
                .Select(trackedBarcode => trackedBarcode.Barcode)
                .Where(barcode => barcode.Data != null)
                .Select(barcode => Model.Product.Create(barcode.Data, barcode.Symbology.HumanReadableName()));

            if (this.Element?.BindingContext is MainViewModel viewModel)
            {
                viewModel.ScannedProducts = products.ToList();
            }

            frameData.Dispose();
        }

        public void OnObservationStarted(BarcodeTracking barcodeTracking) {}

        public void OnObservationStopped(BarcodeTracking barcodeTracking) {}

        private void OnResumeScanningRequested(object sender, EventArgs e)
        {
            this.barcodeTracking.Enabled = true;
            this.camera?.SwitchToDesiredStateAsync(FrameSourceState.On);
        }

        private void OnPauseScanningRequested(object sender, EventArgs e)
        {
            this.barcodeTracking.Enabled = false;
            this.camera?.SwitchToDesiredStateAsync(FrameSourceState.Off);
        }
    }
}
