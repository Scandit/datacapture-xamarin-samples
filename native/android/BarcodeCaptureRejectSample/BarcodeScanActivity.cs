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
using Android.App;
using Android.Content;
using Android.OS;
using Scandit.DataCapture.Barcode.Capture;
using Scandit.DataCapture.Barcode.Data;
using Scandit.DataCapture.Barcode.UI.Overlay;
using Scandit.DataCapture.Core.Capture;
using Scandit.DataCapture.Core.Common.Feedback;
using Scandit.DataCapture.Core.Data;
using Scandit.DataCapture.Core.Source;
using Scandit.DataCapture.Core.UI;
using Scandit.DataCapture.Core.UI.Style;
using Scandit.DataCapture.Core.UI.Viewfinder;

namespace BarcodeCaptureRejectSample
{
    [Activity(MainLauncher = true, Label = "@string/app_name")]
    public class BarcodeScanActivity : CameraPermissionActivity, IBarcodeCaptureListener
    {
        // Enter your Scandit License key here.
        public const string SCANDIT_LICENSE_KEY = "-- ENTER YOUR SCANDIT LICENSE KEY HERE --";

        private DataCaptureContext dataCaptureContext;
        private BarcodeCapture barcodeCapture;
        private Camera camera;
        private DataCaptureView dataCaptureView;
        private BarcodeCaptureOverlay overlay;
        private Brush highlightingBrush;
        private readonly Feedback feedback = Feedback.DefaultFeedback;

        private AlertDialog dialog;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Initialize and start the barcode recognition.
            this.InitializeAndStartBarcodeScanning();
        }

        private void InitializeAndStartBarcodeScanning()
        {
            // Create data capture context using your license key.
            this.dataCaptureContext = DataCaptureContext.ForLicenseKey(SCANDIT_LICENSE_KEY);

            // Use the default camera with the recommended camera settings for the BarcodeCapture mode
            // and set it as the frame source of the context. The camera is off by default and must be
            // turned on to start streaming frames to the data capture context for recognition.
            // See resumeFrameSource and pauseFrameSource below.
            this.camera = Camera.GetDefaultCamera();
            if (this.camera == null)
            {
                throw new NullReferenceException(
                   "Sample depends on a camera, which failed to initialize.");
            }

            // Use the settings recommended by barcode capture.
            this.dataCaptureContext.SetFrameSourceAsync(this.camera);

            // The settings instance initially has all types of barcodes (symbologies) disabled. For the purpose of this
            // sample we enable the QR symbology. In your own app ensure that you only enable the symbologies that your app
            // requires as every additional enabled symbology has an impact on processing times.
            var barcodeCaptureSettings = BarcodeCaptureSettings.Create();
            barcodeCaptureSettings.EnableSymbology(Symbology.Qr, true);

            // Create new barcode capture mode with the settings from above.
            barcodeCapture = BarcodeCapture.Create(dataCaptureContext, barcodeCaptureSettings);

            // By default, every time a barcode is scanned, a sound (if not in silent mode) and a
            // vibration are played. In the following we are setting a success feedback without sound
            // and vibration.
            barcodeCapture.Feedback.Success = new Feedback();

            // Register self as a listener to get informed whenever a new barcode got recognized.
            barcodeCapture.AddListener(this);

            // To visualize the on-going barcode capturing process on screen, setup a data capture view
            // that renders the camera preview. The view must be connected to the data capture context.
            this.dataCaptureView = DataCaptureView.Create(this, this.dataCaptureContext);

            // Add a barcode capture overlay to the data capture view to render the location of captured
            // barcodes on top of the video preview.
            // This is optional, but recommended for better visual feedback.
            overlay = BarcodeCaptureOverlay.Create(this.barcodeCapture, this.dataCaptureView);

            // Add a square viewfinder as we are only scanning square QR codes.
            overlay.Viewfinder = RectangularViewfinder.Create(RectangularViewfinderStyle.Square, RectangularViewfinderLineStyle.Light);

            // Adjust the overlay's barcode highlighting to match the new viewfinder styles and improve the visibility of feedback.
            // With 6.10 we will introduce this visual treatment as a new style for the overlay.
            this.highlightingBrush = new Brush(fillColor: Android.Graphics.Color.Transparent,
                                               strokeColor: Android.Graphics.Color.White,
                                               strokeWidth: 3);
            overlay.Brush = this.highlightingBrush;

            SetContentView(this.dataCaptureView);
        }

        protected override void OnPause()
        {
            PauseFrameSource();
            base.OnPause();
        }

        private void PauseFrameSource()
        {
            // Switch camera off to stop streaming frames.
            // The camera is stopped asynchronously and will take some time to completely turn off.
            // Until it is completely stopped, it is still possible to receive further results, hence
            // it's a good idea to first disable barcode capture as well.
            barcodeCapture.Enabled = false;
            camera.SwitchToDesiredState(FrameSourceState.Off, null);
        }

        protected override void OnDestroy()
        {
            barcodeCapture.RemoveListener(this);
            dataCaptureContext.RemoveMode(barcodeCapture);
            base.OnDestroy();
        }

        protected override void OnResume()
        {
            base.OnResume();

            // Check for camera permission and request it, if it hasn't yet been granted.
            // Once we have the permission the onCameraPermissionGranted() method will be called.
            RequestCameraPermission();
        }

        public void OnBarcodeScanned(BarcodeCapture barcodeCapture, BarcodeCaptureSession session, IFrameData data)
        {
            if (session.NewlyRecognizedBarcodes.Count == 0)
            {
                return;
            }

            var barcode = session.NewlyRecognizedBarcodes[0];

            // If the code scanned doesn't start with "09:", we will just ignore it and continue
            // scanning.
            if (barcode.Data?.StartsWith("09:") == false)
            {
                // We temporarily change the brush, used to highlight recognized barcodes, to a
                // transparent brush.
                overlay.Brush = Brush.TransparentBrush;
                return;
            }

            // If the code is recognized, we want to make sure to use a brush to highlight the code.
            overlay.Brush = this.highlightingBrush;

            // We also want to emit a feedback (vibration and, if enabled, sound).
            feedback.Emit();

            // Stop recognizing barcodes for as long as we are displaying the result. There won't be any
            // new results until the capture mode is enabled again. Note that disabling the capture mode
            // does not stop the camera, the camera continues to stream frames until it is turned off.
            barcodeCapture.Enabled = false;

            // If you are not disabling barcode capture here and want to continue scanning, consider
            // setting the codeDuplicateFilter when creating the barcode capture settings to around 500
            // or even -1 if you do not want codes to be scanned more than once.

            // Get the human readable name of the symbology and assemble the result to be shown.
            using (var description = SymbologyDescription.Create(barcode.Symbology))
            {
                var result = "Scanned: " + barcode.Data + " (" + description.ReadableName + ")";

                RunOnUiThread(() => ShowResult(result));
            }
        }

        private void ShowResult(string result)
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(this);
            this.dialog = builder.SetCancelable(false)
                                 .SetTitle(result)
                                 .SetPositiveButton(Android.Resource.String.Ok, (Object sender, DialogClickEventArgs args) =>
                                 {
                                     this.barcodeCapture.Enabled = true;
                                 })
                                 .Create();
            this.dialog.Show();
        }

        public void OnObservationStarted(BarcodeCapture barcodeCapture)
        {
            //NOP
        }

        public void OnObservationStopped(BarcodeCapture barcodeCapture)
        {
            //NOP
        }

        public void OnSessionUpdated(BarcodeCapture barcodeCapture, BarcodeCaptureSession session, IFrameData data)
        {
            //NOP
        }

        protected override void OnCameraPermissionGranted()
        {
            ResumeFrameSource();
        }

        private void ResumeFrameSource()
        {
            DismissScannedCodesDialog();

            // Switch camera on to start streaming frames.
            // The camera is started asynchronously and will take some time to completely turn on.
            barcodeCapture.Enabled = true;
            camera.SwitchToDesiredState(FrameSourceState.On, null);
        }

        private void DismissScannedCodesDialog()
        {
            dialog?.Dismiss();
            dialog = null;
        }
    }
}
