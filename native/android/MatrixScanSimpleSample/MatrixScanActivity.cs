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
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using Scandit.DataCapture.Barcode.Data;
using Scandit.DataCapture.Barcode.Tracking.Capture;
using Scandit.DataCapture.Barcode.Tracking.UI.Overlay;
using Scandit.DataCapture.Core.Capture;
using Scandit.DataCapture.Core.Data;
using Scandit.DataCapture.Core.Source;
using Scandit.DataCapture.Core.UI;

namespace MatrixScanSimpleSample
{
    [Activity(MainLauncher = true, Label = "MatrixScan Simple Sample")]
    public class BarcodeScanActivity : CameraPermissionActivity, IBarcodeTrackingListener
    {
        // Enter your Scandit License key here.
        public const string SCANDIT_LICENSE_KEY = "-- ENTER YOUR LICENSE KEY --";

        public readonly static int RequestCodeScanResults = 1;

        private Camera camera;
        private DataCaptureContext dataCaptureContext;
        private BarcodeTracking barcodeTracking;
        private DataCaptureView dataCaptureView;

        private HashSet<ScanResult> scanResults = new HashSet<ScanResult>();

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            this.SetContentView(Resource.Layout.activity_main);

            // Initialize and start the barcode recognition.
            this.InitializeAndStartBarcodeScanning();

            Button doneButton = FindViewById<Button>(Resource.Id.done_button);
            doneButton.Click += this.DoneButtonClick;
        }

        private void DoneButtonClick(object sender, System.EventArgs e)
        {
            lock (this.scanResults)
            {
                // Show new screen displaying a list of all barcodes that have been scanned.
                Intent intent = ResultsActivity.GetIntent(this, this.scanResults);
                this.StartActivityForResult(intent, RequestCodeScanResults);
            }
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
            // Switch camera on to start streaming frames.
            // The camera is started asynchronously and will take some time to completely turn on.
            this.barcodeTracking.Enabled = true;
            this.camera?.SwitchToDesiredStateAsync(FrameSourceState.On);
        }

        private void InitializeAndStartBarcodeScanning()
        {
            // Create data capture context using your license key.
            this.dataCaptureContext = DataCaptureContext.ForLicenseKey(SCANDIT_LICENSE_KEY);

            // Use the default camera and set it as the frame source of the context.
            // The camera is off by default and must be turned on to start
            // streaming frames to the data capture context for recognition.
            // See ResumeFrameSource and OnPause methods.
            this.camera = Camera.DefaultCamera;

            if (this.camera != null)
            {
                // Use the recommended camera settings for the BarcodeTracking mode.
                CameraSettings cameraSettings = BarcodeTracking.RecommendedCameraSettings();
                // Adjust camera settings - set Full HD resolution.
                cameraSettings.PreferredResolution = VideoResolution.FullHd;
                this.camera.ApplySettingsAsync(cameraSettings);
                this.dataCaptureContext.SetFrameSourceAsync(this.camera);
            }

            // The barcode tracking process is configured through barcode tracking settings
            // which are then applied to the barcode tracking instance that manages barcode tracking.
            BarcodeTrackingSettings barcodeTrackingSettings = BarcodeTrackingSettings.Create();

            // The settings instance initially has all types of barcodes (symbologies) disabled.
            // For the purpose of this sample we enable a very generous set of symbologies.
            // In your own app ensure that you only enable the symbologies that your app requires
            // as every additional enabled symbology has an impact on processing times.
            HashSet<Symbology> symbologies = new HashSet<Symbology>();
            symbologies.Add(Symbology.Ean13Upca);
            symbologies.Add(Symbology.Ean8);
            symbologies.Add(Symbology.Upce);
            symbologies.Add(Symbology.Code39);
            symbologies.Add(Symbology.Code128);

            barcodeTrackingSettings.EnableSymbologies(symbologies);

            // Create barcode tracking and attach to context.
            this.barcodeTracking = BarcodeTracking.Create(this.dataCaptureContext, barcodeTrackingSettings);

            // Register self as a listener to get informed of tracked barcodes.
            this.barcodeTracking.AddListener(this);

            // To visualize the on-going barcode tracking process on screen, setup a data capture view
            // that renders the camera preview. The view must be connected to the data capture context.
            this.dataCaptureView = DataCaptureView.Create(this, this.dataCaptureContext);

            // Create barcode tracking overlay to the data capture view to render the tracked barcodes on
            // top of the video preview. This is optional, but recommended for better visual feedback.
            BarcodeTrackingBasicOverlay.Create(this.barcodeTracking, this.dataCaptureView);

            // Add the DataCaptureView to the container.
            FrameLayout container = this.FindViewById<FrameLayout>(Resource.Id.data_capture_view_container);
            container.AddView(this.dataCaptureView);
        }

        public void OnObservationStarted(BarcodeTracking barcodeTracking)
        {
        }

        public void OnObservationStopped(BarcodeTracking barcodeTracking)
        {
        }

        public void OnSessionUpdated(BarcodeTracking barcodeCapture, BarcodeTrackingSession session, IFrameData frameData)
        {
            lock (this.scanResults)
            {
                foreach (var trackedBarcode in session.AddedTrackedBarcodes)
                {
                    this.scanResults.Add(new ScanResult
                    {
                        Data = trackedBarcode.Barcode.Data,
                        Symbology = SymbologyDescription.Create(trackedBarcode.Barcode.Symbology).ReadableName
                    });
                }
            }
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            if (requestCode == RequestCodeScanResults && resultCode == Result.Ok)
            {
                lock (this.scanResults)
                {
                    this.scanResults.Clear();
                }
            }

            base.OnActivityResult(requestCode, resultCode, data);
        }

        protected override void OnDestroy()
        {
            this.dataCaptureContext.RemoveMode(this.barcodeTracking);
            base.OnDestroy();
        }
    }
}

