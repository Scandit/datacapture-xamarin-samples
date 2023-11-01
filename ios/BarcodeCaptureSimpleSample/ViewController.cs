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
using Scandit.DataCapture.Barcode.Capture;
using Scandit.DataCapture.Barcode.Data;
using Scandit.DataCapture.Barcode.UI.Overlay;
using Scandit.DataCapture.Core.Capture;
using Scandit.DataCapture.Core.Data;
using Scandit.DataCapture.Core.Source;
using Scandit.DataCapture.Core.UI;
using Scandit.DataCapture.Core.UI.Style;
using Scandit.DataCapture.Core.UI.Viewfinder;
using UIKit;

namespace BarcodeCaptureSimpleSample_iOS_
{
    public partial class ViewController : UIViewController, IBarcodeCaptureListener
    {
        // There is a Scandit sample license key set below here.
        // This license key is enabled for sample evaluation only.
        // If you want to build your own application, get your license key by signing up for a trial at https://ssl.scandit.com/dashboard/sign-up?p=test
        public const string SCANDIT_LICENSE_KEY = "AWHjFzlFHa+fLq/kfS8GCBU/hT60NkQeVGQOWhhtRVcDZxJfsD0OY9NK0YErLuxTtTKLC1BLdrvDdsJ1dnxmcx9fDIeeaQlxawtkiq1pmEFxHOvYa3emcbAfOeiwbFPtQEWCWvdc95KoIFxAuDiYcfccdywzH2KONgwmnV9cEcX11FhIPLtX74RLua7VkOukFfNTOGExxhiCq96qZnzGgrgViuagpL0ekK6xv8K4bYt7lVkxloUMM6dFRSZ4aummJ2Q1uZNR78kSGCpCn/uJjaf/5lyNbYWpnxYvsYRPI7jOFYZykI0nIjhjt/ncukCEsz4BQLAh5hp1qocvQ2+dw3ADD8LJLXcnX7JaCOKV5cfHEHGSLR4moTxNtxPXdUNlM5w75iHZub5BsIfkJCknKrLn5oJ15k5Rx4/JnFj11tGLqtfRs+jdtXSGxAb86BxwPM1mEBO/Va1yV//CGku5UWR5MwspCf7pl8OUH7frkCtV4kDB6y5jusSMSIEGnKCLd2sWKE04mAURrpWt8pgsIB89xXPPTgPh1C+nAeMuuEN3dPYAJYrJKvy44w130JrUvxWLcTM1oFVWikC6CluLC7WGgRhZCew0eROnv9neITolB6Gmy04dlF0euA595dJcw2lLTwwxEydGp5gGIIDtofviho7JdHtPrMer/Ptz1/LOVeF55OY9eg8z1Lq2CkZf6cgWZBPa1uakuZzxWXZUprJMdTquhInmqP4ELLxGXhv+CXoT2n0p022+wyiWAXatmhvcK+n2uCWX30SL0Sri1qPmf6Ldtgqj2aFEMLM+LouJg6Ukv0PKUTXlgPW7L0vYrNGtPjvRlaR7Nwph";

        private DataCaptureContext dataCaptureContext;
        private Camera camera;
        private DataCaptureView dataCaptureView;
        private BarcodeCapture barcodeCapture;
        private BarcodeCaptureOverlay overlay;

        public ViewController (IntPtr handle) : base (handle)
        {
        }

        public override void ViewDidLoad ()
        {
            base.ViewDidLoad ();
            this.InitializeAndStartBarcodeScanning();
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            this.barcodeCapture.Enabled = true;

            // Switch camera on to start streaming frames. The camera is started asynchronously and will
            // take some time to completely turn on.
            this.camera?.SwitchToDesiredStateAsync(FrameSourceState.On);
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);

            // Switch camera off to stop streaming frames. The camera is stopped asynchronously and will
            // take some time to completely turn off. Until it is completely stopped, it is still possible
            // to receive further results, hence it's a good idea to first disable barcode capture as well.
            this.camera?.SwitchToDesiredStateAsync(FrameSourceState.Off);
        }

        public override void DidReceiveMemoryWarning ()
        {
            base.DidReceiveMemoryWarning ();
        }

        protected void InitializeAndStartBarcodeScanning()
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

            // Some linear/1d barcode symbologies allow you to encode variable-length data.
            // By default, the Scandit Data Capture SDK only scans barcodes in a certain length range.
            // If your application requires scanning of one of these symbologies, and the length is
            // falling outside the default range, you may need to adjust the "active symbol counts"
            // for this symbology. This is shown in the following few lines of code for one of the
            // variable-length symbologies.
            SymbologySettings symbologySettings =
                    barcodeCaptureSettings.GetSymbologySettings(Symbology.Code39);

            HashSet<short> activeSymbolCounts = new HashSet<short>(
                new short[] { 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 });

            symbologySettings.ActiveSymbolCounts = activeSymbolCounts;

            // Create new barcode capture mode with the settings from above.
            this.barcodeCapture = BarcodeCapture.Create(this.dataCaptureContext, barcodeCaptureSettings);

            // Register self as a listener to get informed whenever a new barcode got recognized.
            this.barcodeCapture.AddListener(this);

            // To visualize the on-going barcode capturing process on screen, setup a data capture view
            // that renders the camera preview. The view must be connected to the data capture context.
            this.dataCaptureView = DataCaptureView.Create(this.dataCaptureContext, this.View.Bounds);
            this.dataCaptureView.AutoresizingMask = UIViewAutoresizing.FlexibleHeight |
                                                    UIViewAutoresizing.FlexibleWidth;
            this.View.AddSubview(this.dataCaptureView);

            this.overlay = BarcodeCaptureOverlay.Create(this.barcodeCapture, this.dataCaptureView, BarcodeCaptureOverlayStyle.Frame);
            this.overlay.Viewfinder = RectangularViewfinder.Create(RectangularViewfinderStyle.Square, RectangularViewfinderLineStyle.Light);
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

        #region IBarcodeCaptureListener

        public void OnBarcodeScanned(BarcodeCapture barcodeCapture, BarcodeCaptureSession session, IFrameData frameData)
        {
            var barcode = session?.NewlyRecognizedBarcodes.FirstOrDefault();

            if (barcode == null)
            {
                return;
            }

            // Stop recognizing barcodes for as long as we are displaying the result. There won't be any new
            // results until the capture mode is enabled again. Note that disabling the capture mode does
            // not stop the camera, the camera continues to stream frames until it is turned off.
            this.barcodeCapture.Enabled = false;

            // If you are not disabling barcode capture here and want to continue scanning, consider
            // setting the codeDuplicateFilter when creating the barcode capture settings to around 500
            // or even -1 if you do not want codes to be scanned more than once.

            // Get the human readable name of the symbology and assemble the result to be shown.
            string symbology = SymbologyDescription.Create(barcode.Symbology).ReadableName;
            string result = string.Format("Scanned {0} ({1})", barcode.Data, symbology);
            this.ShowResult(result);

            // Dispose the frame when you have finished processing it. If the frame is not properly disposed,
            // different issues could arise, e.g. a frozen, non-responsive, or "severely stuttering" video feed.
            frameData.Dispose();
        }

        public void OnSessionUpdated(BarcodeCapture barcodeCapture, BarcodeCaptureSession session, IFrameData frameData)
        {
            // Dispose the frame when you have finished processing it. If the frame is not properly disposed,
            // different issues could arise, e.g. a frozen, non-responsive, or "severely stuttering" video feed.
            frameData.Dispose();
        }

        public void OnObservationStarted(BarcodeCapture barcodeCapture)
        {
        }

        public void OnObservationStopped(BarcodeCapture barcodeCapture)
        {
        }

        #endregion
    }
}
