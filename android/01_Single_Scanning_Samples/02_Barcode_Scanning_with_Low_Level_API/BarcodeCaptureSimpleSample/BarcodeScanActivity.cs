﻿/*
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

using Android.App;
using Android.Content;
using Android.OS;

using Scandit.DataCapture.Barcode.Capture;
using Scandit.DataCapture.Core.Common.Feedback;
using Scandit.DataCapture.Barcode.Data;
using Scandit.DataCapture.Barcode.UI.Overlay;
using Scandit.DataCapture.Core.Capture;
using Scandit.DataCapture.Core.Data;
using Scandit.DataCapture.Core.Source;
using Scandit.DataCapture.Core.UI;
using Scandit.DataCapture.Core.UI.Style;
using Scandit.DataCapture.Core.UI.Viewfinder;

namespace BarcodeCaptureSimpleSample
{
    [Activity(MainLauncher = true, Label = "@string/app_name")]
    public class BarcodeScanActivity : CameraPermissionActivity, IBarcodeCaptureListener
    {
        // Enter your Scandit License key here.
        // Your Scandit License key is available via your Scandit SDK web account.
        public const string SCANDIT_LICENSE_KEY = "-- ENTER YOUR SCANDIT LICENSE KEY HERE --";

        private DataCaptureContext dataCaptureContext;
        private BarcodeCapture barcodeCapture;
        private Camera camera;
        private DataCaptureView dataCaptureView;
        private BarcodeCaptureOverlay overlay;
        private readonly Feedback feedback = Feedback.DefaultFeedback;
        private Brush highlightingBrush;

        private AlertDialog dialog;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            this.InitializeAndStartBarcodeScanning();
        }

        protected override void OnPause()
        {
            base.OnPause();

            this.camera?.SwitchToDesiredStateAsync(FrameSourceState.Off);
        }

        protected override void OnResume()
        {
            base.OnResume();

            // Handle permissions for Marshmallow and onwards...
            if (Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.M)
            {
                this.RequestCameraPermission();
            }
            else
            {
                // Once the activity is in the foreground again, restart scanning.
                this.ResumeFrameSource();
            }
        }

        protected override void OnCameraPermissionGranted()
        {
            this.ResumeFrameSource();
        }

        private void ResumeFrameSource()
        {
            this.DismissScannedCodesDialog();

            // Switch camera on to start streaming frames.
            // The camera is started asynchronously and will take some time to completely turn on.
            this.barcodeCapture.Enabled = true;
            this.camera?.SwitchToDesiredStateAsync(FrameSourceState.On);
        }

        private void InitializeAndStartBarcodeScanning()
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

            // The barcode capturing process is configured through barcode capture settings
            // which are then applied to the barcode capture instance that manages barcode recognition.
            BarcodeCaptureSettings barcodeCaptureSettings = BarcodeCaptureSettings.Create();

            // The settings instance initially has all types of barcodes (symbologies) disabled.
            // For the purpose of this sample we enable a very generous set of symbologies.
            // In your own app ensure that you only enable the symbologies that your app requires as
            // every additional enabled symbology has an impact on processing times.
            HashSet<Symbology> symbologies = new HashSet<Symbology>();
            symbologies.Add(Symbology.Ean13Upca);
            symbologies.Add(Symbology.Ean8);
            symbologies.Add(Symbology.Upce);
            symbologies.Add(Symbology.Qr);
            symbologies.Add(Symbology.DataMatrix);
            symbologies.Add(Symbology.Code39);
            symbologies.Add(Symbology.Code128);
            symbologies.Add(Symbology.InterleavedTwoOfFive);

            barcodeCaptureSettings.EnableSymbologies(symbologies);

            // Some linear/1d barcode symbologies allow you to encode variable-length data.
            // By default, the Scandit Data Capture SDK only scans barcodes in a certain length range.
            // If your application requires scanning of one of these symbologies, and the length is
            // falling outside the default range, you may need to adjust the "active symbol counts"
            // for this symbology. This is shown in the following few lines of code for one of the
            // variable-length symbologies.
            SymbologySettings symbologySettings =
                    barcodeCaptureSettings.GetSymbologySettings(Symbology.Code39);

            ICollection<short> activeSymbolCounts = new HashSet<short>(
                new short[] { 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 });

            symbologySettings.ActiveSymbolCounts = activeSymbolCounts;

            // Create new barcode capture mode with the settings from above.
            this.barcodeCapture = BarcodeCapture.Create(this.dataCaptureContext, barcodeCaptureSettings);

            // By default, every time a barcode is scanned, a sound (if not in silent mode) and a vibration are played.
            // Uncomment the following line to set a success feedback without sound and vibration.
            // barcodeCapture.Feedback.Success = new Feedback();

            // Register self as a listener to get informed whenever a new barcode got recognized.
            barcodeCapture.AddListener(this);

            // To visualize the on-going barcode capturing process on screen, setup a data capture view
            // that renders the camera preview. The view must be connected to the data capture context.
            this.dataCaptureView = DataCaptureView.Create(this, this.dataCaptureContext);

            // Add a barcode capture overlay to the data capture view to render the location of captured
            // barcodes on top of the video preview.
            // This is optional, but recommended for better visual feedback.
            overlay = BarcodeCaptureOverlay.Create(this.barcodeCapture, this.dataCaptureView, BarcodeCaptureOverlayStyle.Frame);
            overlay.Viewfinder = RectangularViewfinder.Create(RectangularViewfinderStyle.Square, RectangularViewfinderLineStyle.Light);
            this.highlightingBrush = overlay.Brush;

            SetContentView(this.dataCaptureView);
        }

        private void DismissScannedCodesDialog()
        {
            if (this.dialog != null)
            {
                this.dialog.Dismiss();
                this.dialog = null;
            }
        }

        public void OnBarcodeScanned(BarcodeCapture barcodeCapture, BarcodeCaptureSession session, IFrameData frameData)
        {
            if (session.NewlyRecognizedBarcode == null)
            {
                return;
            }

            Barcode barcode = session.NewlyRecognizedBarcode;

            // Use the following code to reject barcodes.
            // By uncommenting the following lines, barcodes not starting with 09: are ignored.
            // if (barcode.Data?.StartsWith("09:") == false)
            // {
            //     // We temporarily change the brush, used to highlight recognized barcodes, to a
            //     // transparent brush.
            //     overlay.Brush = Brush.TransparentBrush;
            //     return;
            // }
            // Otherwise, if the barcode is of interest, we want to use a brush to highlight it.
            // overlay.Brush = this.highlightingBrush;

            // We also want to emit a feedback (vibration and, if enabled, sound).
            // By default, every time a barcode is scanned, a sound (if not in silent mode) and a vibration are played.
            // To emit a feedback only when necessary, it is necessary to set a success feedback without sound and vibration
            // when setting up Barcode Capture (in this case in the `InitializeAndStartBarcodeScanning` method).
            // feedback.Emit();

            // Stop recognizing barcodes for as long as we are displaying the result. There won't be any new results until
            // the capture mode is enabled again. Note that disabling the capture mode does not stop the camera, the camera
            // continues to stream frames until it is turned off.
            barcodeCapture.Enabled = false;

            // If you don't want the codes to be scanned more than once, consider setting the codeDuplicateFilter when
            // creating the barcode capture settings to -1.
            // You can set any other value (e.g. 500) to set a fixed timeout and override the smart behaviour enabled
            // by default.

            // Get the human readable name of the symbology and assemble the result to be shown.
            using (SymbologyDescription description = SymbologyDescription.Create(barcode.Symbology))
            {
                string result = "Scanned: " + barcode.Data + " (" + description.ReadableName + ")";

                RunOnUiThread(() => ShowResults(result));
            }
        }

        public void OnObservationStarted(BarcodeCapture barcodeCapture)
        {
        }

        public void OnObservationStopped(BarcodeCapture barcodeCapture)
        {
        }

        public void OnSessionUpdated(BarcodeCapture barcodeCapture, BarcodeCaptureSession session, IFrameData frameData)
        {
        }

        private void ShowResults(string result)
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(this);
            this.dialog = builder.SetCancelable(false)
                                 .SetTitle(result)
                                 .SetPositiveButton(Android.Resource.String.Ok, (Object sender, DialogClickEventArgs args) => {
                                     this.barcodeCapture.Enabled = true;
                                 })
                                 .Create();
            this.dialog.Show();
        }
    }
}
