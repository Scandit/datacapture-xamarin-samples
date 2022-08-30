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
using Foundation;
using Scandit.DataCapture.Barcode.Capture;
using Scandit.DataCapture.Barcode.Data;
using Scandit.DataCapture.Barcode.UI.Overlay;
using Scandit.DataCapture.Core.Capture;
using Scandit.DataCapture.Core.Source;
using Scandit.DataCapture.Core.UI;
using UIKit;

namespace BarcodeCaptureViewsSample.Modes
{
    public partial class CommonScannerViewController : UIViewController
    {
        private DataCaptureView dataCaptureView;
        private DataCaptureContext dataCaptureContext;
        private Camera camera;
        private BarcodeCapture barcodeCapture;
        private BarcodeCaptureOverlay barcodeCaptureOverlay;

        public CommonScannerViewController(IntPtr handle) : base(handle)
        { }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            this.InitializeBarcodeScanning();
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            // Switch camera on to start streaming frames. The camera is started asynchronously and will take some time to
            // completely turn on.
            this.barcodeCapture.Enabled = true;
            this.camera?.SwitchToDesiredStateAsync(FrameSourceState.On);
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);

            // Switch camera off to stop streaming frames. The camera is stopped asynchronously and will take some time to
            // completely turn off. Until it is completely stopped, it is still possible to receive further results, hence
            // it's a good idea to first disable barcode tracking as well.
            this.barcodeCapture.Enabled = false;
            this.camera?.SwitchToDesiredStateAsync(FrameSourceState.Off);
        }

        protected void InitializeBarcodeScanning()
        {
            // Create data capture context using your license key.
            this.dataCaptureContext = DataCaptureContext.ForLicenseKey(Application.SCANDIT_LICENSE_KEY);

            // Use the default camera and set it as the frame source of the context.
            // The camera is off by default and must be turned on to start streaming frames to the data
            // capture context for recognition.
            this.camera = Camera.GetDefaultCamera();

            if (this.camera != null)
            {
                // Use the settings recommended by barcode capture.
                this.camera.ApplySettingsAsync(BarcodeCapture.RecommendedCameraSettings);
                this.dataCaptureContext.SetFrameSourceAsync(this.camera);
            }

            BarcodeCaptureSettings barcodeCaptureSettings = BarcodeCaptureSettings.Create();

            // The settings instance initially has all types of barcodes (symbologies) disabled.
            // For the purpose of this sample we enable a very generous set of symbologies.
            // In your own app ensure that you only enable the symbologies that your app requires as
            // every additional enabled symbology has an impact on processing times.
            HashSet<Symbology> symbologies = new HashSet<Symbology>()
            {
                Symbology.Ean13Upca,
                Symbology.Ean8,
                Symbology.Upce,
                Symbology.Qr,
                Symbology.DataMatrix,
                Symbology.Code39,
                Symbology.Code128,
                Symbology.InterleavedTwoOfFive
            };

            barcodeCaptureSettings.EnableSymbologies(symbologies);

            // Create new barcode capture mode with the settings from above.
            this.barcodeCapture = BarcodeCapture.Create(this.dataCaptureContext, barcodeCaptureSettings);

            // Subsribe to BarcodeScanned event to get informed whenever a new barcode got recognized.
            this.barcodeCapture.BarcodeScanned += BarcodeScanned;

            // To visualize the on-going barcode capturing process on screen, setup a data capture view
            // that renders the camera preview. The view must be connected to the data capture context.
            this.dataCaptureView = DataCaptureView.Create(this.dataCaptureContext, this.View.Bounds);
            this.dataCaptureView.AutoresizingMask = UIViewAutoresizing.FlexibleHeight |
                                                    UIViewAutoresizing.FlexibleWidth;
            this.View.AddSubview(this.dataCaptureView);

            this.barcodeCaptureOverlay = BarcodeCaptureOverlay.Create(this.barcodeCapture, this.dataCaptureView, BarcodeCaptureOverlayStyle.Frame);
        }

        private void BarcodeScanned(object sender, BarcodeCaptureEventArgs args)
        {
            var barcode = args.Session?.NewlyRecognizedBarcodes.FirstOrDefault();

            if (barcode == null)
            {
                return;
            }

            // Stop recognizing barcodes for as long as we are displaying the result. There won't be any new
            // results until the capture mode is enabled again. Note that disabling the capture mode does
            // not stop the camera, the camera continues to stream frames until it is turned off.
            this.barcodeCapture.Enabled = false;

            // Get the human readable name of the symbology and assemble the result to be shown.
            string symbology = SymbologyDescription.Create(barcode.Symbology).ReadableName;
            string result = string.Format("Scanned {0} ({1})", barcode.Data, symbology);
            this.ShowResult(result);
        }

        public void ShowResult(string result)
        {
            DispatchQueue.MainQueue.DispatchAsync(() => {
                UIAlertController alert = UIAlertController.Create(result, message: null, preferredStyle: UIAlertControllerStyle.Alert);
                var action = UIAlertAction.Create("OK", UIAlertActionStyle.Default, (UIAlertAction) => {
                    this.barcodeCapture.Enabled = true;
                });
                alert.AddAction(action);
                this.PresentViewController(alert, animated: true, completionHandler: () => { });
            });
        }

        partial void DismissModal(NSObject sender)
        {
            this.DismissViewControllerAsync(true);
        }
    }
}
