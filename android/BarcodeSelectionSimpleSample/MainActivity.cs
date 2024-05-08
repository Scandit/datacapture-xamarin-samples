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
using System.Timers;
using Android.App;
using Android.OS;
using Android.Widget;

using Scandit.DataCapture.Barcode.Data;
using Scandit.DataCapture.Barcode.Selection.Capture;
using Scandit.DataCapture.Barcode.Selection.UI.Overlay;
using Scandit.DataCapture.Core.Capture;
using Scandit.DataCapture.Core.Data;
using Scandit.DataCapture.Core.Source;
using Scandit.DataCapture.Core.UI;

namespace BarcodeSelectionSimpleSample
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : CameraPermissionActivity, IBarcodeSelectionListener
    {
        // There is a Scandit sample license key set below here.
        // This license key is enabled for sample evaluation only.
        // If you want to build your own application, get your license key by signing up for a trial at https://ssl.scandit.com/dashboard/sign-up?p=test
        public const string SCANDIT_LICENSE_KEY = "AZ707AsCLmJWHbYO4RjqcVAEgAxmNGYcF3Ytg4RiKa/lWTQ3IXkfVZhSSi0yOzuabn9STRdnzTLybIiJVkVZU2QK5jeqbn1HGCGXQ+9lqsN8VUaTw1IeuHJo+9mYVdv3I1DhedtSy89aKA4QugKI5d9ykKaXGohXjlI+PB5ju8Tyc80FPAC3WP9D8oKBcWyemTLQjoUu0Nl3T7mVyFIXMPshQeYddkjMQ1sVV9Jcuf1CbI9riUJWzbNUb4NcB4MoV0BHuyALUPtuM2+cBkX3bPN0AxjD9WC7KflL2UrsZeenvl/aDx2yU4t5vsa2BImNTyEqdVs+rmrGUzRdbYvSUFzKBeiBncLAASqnexTuSzh9KfEm/cKrVlWekP+zOkrilICkn3KVNY6g9RQd8FrsHTBI9OBbMpC79BTwuzHcnlFUG5S3ru/viJ2+f9JEEejxDbdJ7u4JohfBuUYBSEBQ/XzEPMdpqWcmxHGWF4j7jQy83B9Wlgrhd8xNWKjgAViI0bcebjnB7o6yuKacXICH/lo787RhnXSjqjQhJBCbEvwxHQZiEfWPdVKtY7EM+x8HFr6j3orKllKOMJ9asZ5bJYz9aIHlOWeRGm90guQn0KWiPwuKbUOQIMxFAOem2zcSTt4OfqS6Ci0Y6lk7FIrgpbaz8L1PW64kkjrZB6FtQ8OppmsyZ/QTvrHYFQFTH7MpamDviRjEKMyiD2ID6ypl+Meeme6cZYRJVujr6b4tweQCsfNEYhuDiMJaWQ57R0n2XdF0zkepLVc0yA2Q3wWhxSIASLnv6GTCYYVnDJnkrr6VaTv8RVUOp8h8U34wGDanamQ+39+rESMD59E288OKgFvZZWN9Ltu/VQCcjYCYT1RTDcA9co3Y18aGpDxvtLVEGJ8QDPv1E//IYAYEhXqu8r9xbsx/hTwZmLpNKyXGPRr9+hpufTAcAj908f2kuQ==";

        public readonly static int RequestCodeScanResults = 1;

        private Camera camera;
        private DataCaptureContext dataCaptureContext;
        private DataCaptureView dataCaptureView;
        private BarcodeSelection barcodeSelection;
        private BarcodeSelectionSettings barcodeSelectionSettings;

        private const int dialogAutoDissmissInterval = 500;
        private readonly Timer dialogAutoDismissTimer;
        private AlertDialog dialog;

        private Button aimToSelectButton;
        private Button tapToSelectButton;

        public MainActivity()
        {
            this.dialogAutoDismissTimer = new Timer(dialogAutoDissmissInterval)
            {
                AutoReset = false,
                Enabled = false
            };
            this.dialogAutoDismissTimer.Elapsed += (object sender, ElapsedEventArgs e) =>
            {
                this.DismissScannedCodesDialog();
            };
        }

        #region IBarcodeSelectionListener
        public void OnObservationStarted(BarcodeSelection barcodeSelection)
        { }

        public void OnObservationStopped(BarcodeSelection barcodeSelection)
        { }

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
            string result = barcode.Data + " (" + description.ReadableName + ")" + " Times: " + selectionCount;

            this.RunOnUiThread(() => this.ShowResults(result));
        }

        public void OnSessionUpdated(BarcodeSelection barcodeSelection, BarcodeSelectionSession session, IFrameData frameData)
        { }
        #endregion

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            this.SetContentView(Resource.Layout.activity_main);
            this.InitializeAndStartBarcodeScanning();

            this.aimToSelectButton = this.FindViewById<Button>(Resource.Id.aim_to_select_button);
            this.aimToSelectButton.Click += this.AimToSelectButtonClick;
            this.tapToSelectButton = this.FindViewById<Button>(Resource.Id.tap_to_select_button);
            this.tapToSelectButton.Click += this.TapToSelectButtonClick;
        }

        private void AimToSelectButtonClick(object sender, System.EventArgs e)
        {
            if (this.barcodeSelectionSettings.SelectionType is BarcodeSelectionTapSelection)
            {
                this.barcodeSelectionSettings.SelectionType = BarcodeSelectionAimerSelection.Create();
                this.barcodeSelection.ApplySettingsAsync(this.barcodeSelectionSettings)
                                     .ContinueWith((task) =>
                                         // Switch the camera to On state in case it froze through
                                         // double tap while on TapToSelect selection type.
                                         this.camera.SwitchToDesiredStateAsync(FrameSourceState.On));
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
            this.barcodeSelection.Enabled = true;
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
            this.camera = Camera.GetDefaultCamera();

            if (this.camera != null)
            {
                // Use the recommended camera settings for the BarcodeSelection mode.
                CameraSettings cameraSettings = BarcodeSelection.RecommendedCameraSettings;
                // Adjust camera settings - set Full HD resolution.
                cameraSettings.PreferredResolution = VideoResolution.FullHd;
                this.camera.ApplySettingsAsync(cameraSettings);
                this.dataCaptureContext.SetFrameSourceAsync(this.camera);
            }

            // The barcode selection process is configured through barcode selection settings
            // which are then applied to the barcode selection instance that manages barcode selection.
            this.barcodeSelectionSettings = BarcodeSelectionSettings.Create();

            // The settings instance initially has all types of barcodes (symbologies) disabled.
            // For the purpose of this sample we enable a very generous set of symbologies.
            // In your own app ensure that you only enable the symbologies that your app requires
            // as every additional enabled symbology has an impact on processing times.
            HashSet<Symbology> symbologies = new HashSet<Symbology>
            {
                Symbology.Ean13Upca,
                Symbology.Ean8,
                Symbology.Upce,
                Symbology.Code39,
                Symbology.Code128
            };

            this.barcodeSelectionSettings.EnableSymbologies(symbologies);

            // Create barcode selection and attach to context.
            this.barcodeSelection = BarcodeSelection.Create(this.dataCaptureContext, this.barcodeSelectionSettings);

            // Register self as a listener to get informed of selected barcodes.
            this.barcodeSelection.AddListener(this);

            // To visualize the on-going barcode selection process on screen, setup a data capture view
            // that renders the camera preview. The view must be connected to the data capture context.
            this.dataCaptureView = DataCaptureView.Create(this, this.dataCaptureContext);

            // Create barcode selection overlay to the data capture view to render the selected barcodes on
            // top of the video preview. This is optional, but recommended for better visual feedback.
            BarcodeSelectionBasicOverlay.Create(this.barcodeSelection, this.dataCaptureView);

            // Add the DataCaptureView to the container.
            FrameLayout container = this.FindViewById<FrameLayout>(Resource.Id.data_capture_view_container);
            container.AddView(this.dataCaptureView);
        }

        private void DismissScannedCodesDialog()
        {
            this.RunOnUiThread(() => this.dialog?.Dismiss());
        }

        private void ShowResults(string result)
        {
            if (this.dialog != null)
            {
                this.dialog.Dismiss();
            }

            this.dialogAutoDismissTimer.Stop();
            this.dialogAutoDismissTimer.Start();

            AlertDialog.Builder builder = new AlertDialog.Builder(this);
            this.dialog = builder.SetCancelable(false)
                                 .SetMessage(result)
                                 .Create();
            this.dialog.Show();
        }
    }
}
