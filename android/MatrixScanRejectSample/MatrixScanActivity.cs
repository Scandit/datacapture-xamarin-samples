//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
//
//        http://www.apache.org/licenses/LICENSE-2.0
//
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using MatrixScanRejectSample.Data;
using Scandit.DataCapture.Barcode.Data;
using Scandit.DataCapture.Barcode.Tracking.Capture;
using Scandit.DataCapture.Barcode.Tracking.Data;
using Scandit.DataCapture.Barcode.Tracking.UI.Overlay;
using Scandit.DataCapture.Core.Capture;
using Scandit.DataCapture.Core.Data;
using Scandit.DataCapture.Core.Source;
using Scandit.DataCapture.Core.UI;
using Scandit.DataCapture.Core.UI.Style;

using Camera = Scandit.DataCapture.Core.Source.Camera;

namespace MatrixScanRejectSample
{
    [Activity(MainLauncher = true)]
    public class MatrixScanActivity : CameraPermissionActivity, IBarcodeTrackingListener, IBarcodeTrackingBasicOverlayListener
    {
        // There is a Scandit sample license key set below here.
        // This license key is enabled for sample evaluation only.
        // If you want to build your own application, get your license key by signing up for a trial at https://ssl.scandit.com/dashboard/sign-up?p=test
        public const string SCANDIT_LICENSE_KEY = "AZ707AsCLmJWHbYO4RjqcVAEgAxmNGYcF3Ytg4RiKa/lWTQ3IXkfVZhSSi0yOzuabn9STRdnzTLybIiJVkVZU2QK5jeqbn1HGCGXQ+9lqsN8VUaTw1IeuHJo+9mYVdv3I1DhedtSy89aKA4QugKI5d9ykKaXGohXjlI+PB5ju8Tyc80FPAC3WP9D8oKBcWyemTLQjoUu0Nl3T7mVyFIXMPshQeYddkjMQ1sVV9Jcuf1CbI9riUJWzbNUb4NcB4MoV0BHuyALUPtuM2+cBkX3bPN0AxjD9WC7KflL2UrsZeenvl/aDx2yU4t5vsa2BImNTyEqdVs+rmrGUzRdbYvSUFzKBeiBncLAASqnexTuSzh9KfEm/cKrVlWekP+zOkrilICkn3KVNY6g9RQd8FrsHTBI9OBbMpC79BTwuzHcnlFUG5S3ru/viJ2+f9JEEejxDbdJ7u4JohfBuUYBSEBQ/XzEPMdpqWcmxHGWF4j7jQy83B9Wlgrhd8xNWKjgAViI0bcebjnB7o6yuKacXICH/lo787RhnXSjqjQhJBCbEvwxHQZiEfWPdVKtY7EM+x8HFr6j3orKllKOMJ9asZ5bJYz9aIHlOWeRGm90guQn0KWiPwuKbUOQIMxFAOem2zcSTt4OfqS6Ci0Y6lk7FIrgpbaz8L1PW64kkjrZB6FtQ8OppmsyZ/QTvrHYFQFTH7MpamDviRjEKMyiD2ID6ypl+Meeme6cZYRJVujr6b4tweQCsfNEYhuDiMJaWQ57R0n2XdF0zkepLVc0yA2Q3wWhxSIASLnv6GTCYYVnDJnkrr6VaTv8RVUOp8h8U34wGDanamQ+39+rESMD59E288OKgFvZZWN9Ltu/VQCcjYCYT1RTDcA9co3Y18aGpDxvtLVEGJ8QDPv1E//IYAYEhXqu8r9xbsx/hTwZmLpNKyXGPRr9+hpufTAcAj908f2kuQ==";

        public const int REQUEST_CODE_SCAN_RESULTS = 1;

        private Camera camera;
        private BarcodeTracking barcodeTracking;
        private DataCaptureContext dataCaptureContext;

        private readonly HashSet<ScanResult> scanResults = new HashSet<ScanResult>();

        private Brush defaultBrush;
        private Brush rejectedBrush;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_matrix_scan);

            SetTitle(Resource.String.app_title);

            Initialize();

            FindViewById<Button>(Resource.Id.done_button).Click += (sender, e) =>
            {
                var intent = ResultsActivity.GetIntent(
                            this, scanResults);
                StartActivityForResult(intent, REQUEST_CODE_SCAN_RESULTS);
            };
        }

        private void Initialize()
        {
            // Create data capture context using your license key.
            dataCaptureContext = DataCaptureContext.ForLicenseKey(SCANDIT_LICENSE_KEY);

            // Use the recommended camera settings for the BarcodeTracking mode.
            var cameraSettings = BarcodeTracking.RecommendedCameraSettings;
            // Adjust camera settings - set Full HD resolution.
            cameraSettings.PreferredResolution = VideoResolution.FullHd;
            // Use the default camera and set it as the frame source of the context.
            // The camera is off by default and must be turned on to start streaming frames to the data
            // capture context for recognition.
            // See resumeFrameSource and pauseFrameSource below.
            camera = Camera.GetDefaultCamera(cameraSettings);

            if (camera == null)
            {
                throw new NullReferenceException(
                   "Sample depends on a camera, which failed to initialize.");
            }

            dataCaptureContext.SetFrameSourceAsync(camera);

            // The barcode tracking process is configured through barcode tracking settings
            // which are then applied to the barcode tracking instance that manages barcode tracking.
            var barcodeTrackingSettings = BarcodeTrackingSettings.Create();

            // The settings instance initially has all types of barcodes (symbologies) disabled.
            // For the purpose of this sample we enable a very generous set of symbologies.
            // In your own app ensure that you only enable the symbologies that your app requires
            // as every additional enabled symbology has an impact on processing times.
            var symbologies = new HashSet<Symbology>
            {
                Symbology.Ean13Upca,
                Symbology.Ean8,
                Symbology.Upce,
                Symbology.Code39,
                Symbology.Code128
            };

            barcodeTrackingSettings.EnableSymbologies(symbologies);

            // Create barcode tracking and attach to context.
            barcodeTracking = BarcodeTracking.Create(dataCaptureContext, barcodeTrackingSettings);

            // Register self as a listener to get informed of tracked barcodes.
            barcodeTracking.AddListener(this);

            // To visualize the on-going barcode tracking process on screen, setup a data capture view
            // that renders the camera preview. The view must be connected to the data capture context.
            var dataCaptureView = DataCaptureView.Create(this, dataCaptureContext);

            // Add a barcode tracking overlay to the data capture view to render the tracked barcodes on
            // top of the video preview. This is optional, but recommended for better visual feedback.
            var overlay =
                    BarcodeTrackingBasicOverlay.Create(barcodeTracking, dataCaptureView, BarcodeTrackingBasicOverlayStyle.Frame);

            // Configure how barcodes are highlighted - apply default brush or create your own.
            this.rejectedBrush = new Brush(Color.Transparent, Color.Red, 3f);
            this.defaultBrush = new Brush(Color.Transparent, Color.Green, 3f);
            overlay.Listener = this;

            // Add the DataCaptureView to the container.
            var container = FindViewById<FrameLayout>(Resource.Id.data_capture_view_container);
            container.AddView(dataCaptureView);
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
            // it's a good idea to first disable barcode tracking as well.
            barcodeTracking.Enabled = false;
            camera.SwitchToDesiredState(FrameSourceState.Off, null);
        }

        protected override void OnResume()
        {
            base.OnResume();

            // Check for camera permission and request it, if it hasn't yet been granted.
            // Once we have the permission the onCameraPermissionGranted() method will be called.
            RequestCameraPermission();
        }

        public void OnObservationStarted(BarcodeTracking barcodeTracking)
        {
            // NOP
        }

        public void OnObservationStopped(BarcodeTracking barcodeTracking)
        {
            // NOP
        }

        public void OnSessionUpdated(BarcodeTracking barcodeTracking, BarcodeTrackingSession session, IFrameData frameData)
        {
            lock (scanResults)
            {
                foreach (var trackedBarcode in
                    session.AddedTrackedBarcodes.Where(trb => IsValidBarcode(trb.Barcode)))
                {
                    scanResults.Add(new ScanResult(trackedBarcode.Barcode));
                }
            }
        }

        protected override void OnCameraPermissionGranted()
        {
            ResumeFrameSource();
        }

        private void ResumeFrameSource()
        {
            // Switch camera on to start streaming frames.
            // The camera is started asynchronously and will take some time to completely turn on.
            barcodeTracking.Enabled = true;
            camera.SwitchToDesiredState(FrameSourceState.On, null);
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            if (requestCode == REQUEST_CODE_SCAN_RESULTS
                && (int)resultCode == ResultsActivity.RESULT_CODE_CLEAN)
            {
                lock (scanResults)
                {
                    scanResults.Clear();
                }
            }
            base.OnActivityResult(requestCode, resultCode, data);
        }

        protected override void OnDestroy()
        {
            dataCaptureContext.RemoveMode(barcodeTracking);
            base.OnDestroy();
        }

        public Brush BrushForTrackedBarcode(BarcodeTrackingBasicOverlay overlay, TrackedBarcode trackedBarcode)
        {
            if (IsValidBarcode(trackedBarcode.Barcode))
            {
                return defaultBrush;
            }
            else
            {
                return rejectedBrush;
            }
        }

        private bool IsValidBarcode(Barcode barcode)
        {
            // Reject invalid barcodes.
            if (String.IsNullOrEmpty(barcode.Data)) return false;

            // Reject barcodes based on your logic.
            if (barcode.Data.StartsWith("7")) return false;

            return true;
        }

        public void OnTrackedBarcodeTapped(BarcodeTrackingBasicOverlay overlay, TrackedBarcode trackedBarcode)
        {
            // Handle barcode click if necessary.
        }
    }
}
