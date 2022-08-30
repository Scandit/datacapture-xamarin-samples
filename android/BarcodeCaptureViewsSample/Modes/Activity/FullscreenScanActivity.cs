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

using Android.App;
using Android.Content;
using Android.OS;
using BarcodeCaptureViewsSample.Modes.Base;
using Scandit.DataCapture.Barcode.Data;
using Scandit.DataCapture.Barcode.UI.Overlay;
using Scandit.DataCapture.Core.UI;

namespace BarcodeCaptureViewsSample.Modes.Activity
{
    [Activity(Label = "@string/picker_activity", Theme = "@style/Theme.AppCompat.Light", ParentActivity = typeof(MainActivity))]
    public class FullscreenScanActivity : CameraPermissionActivity
    {
        public static Intent GetIntent(Context context)
        {
            return new Intent(context, typeof(FullscreenScanActivity));
        }

        private readonly ScanViewModel viewModel = new ScanViewModel();
        private AlertDialog dialog = null;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            this.viewModel.BarcodeScanned += ShowResults;

            // To visualize the on-going barcode capturing process on screen,
            // setup a data capture view that renders the camera preview.
            // The view must be connected to the data capture context.
            DataCaptureView view = DataCaptureView.Create(this, this.viewModel.DataCaptureContext);

            // Add a barcode capture overlay to the data capture view to render the tracked
            // barcodes on top of the video preview.
            // This is optional, but recommended for better visual feedback.
            BarcodeCaptureOverlay.Create(this.viewModel.BarcodeCapture, view, BarcodeCaptureOverlayStyle.Frame);

            this.SetContentView(view);
        }

        protected override void OnResume()
        {
            base.OnResume();

            // Check for camera permission and request it, if it hasn't yet been granted.
            // Once we have the permission the OnCameraPermissionGranted() method will be called.
            this.RequestCameraPermission();
        }

        protected override void OnPause()
        {
            base.OnPause();

            // Switch camera off to stop streaming frames.
            // The camera is stopped asynchronously and will take some time to completely turn off.
            // Until it is completely stopped, it is still possible to receive further results, hence
            // it's a good idea to first disable barcode tracking as well.
            this.viewModel.PauseScanning();
            this.viewModel.StopFrameSourceAsync();
        }

        protected override void OnCameraPermissionGranted()
        {
            // Switch camera on to start streaming frames.
            // The camera is started asynchronously and will take some time to completely turn on.
            this.viewModel.StartFrameSourceAsync();

            if (!this.IsShowingDialog())
            {
                this.viewModel.ResumeScanning();
            }
        }

        private bool IsShowingDialog()
        {
            return this.dialog != null && this.dialog.IsShowing;
        }

        private void ShowResults(object sender, Barcode barcodeResult)
        {
            string scanResultFormat = this.GetString(Resource.String.scan_result_format);
            string message = string.Format(
                scanResultFormat,                     
                SymbologyDescription.Create(barcodeResult.Symbology).ReadableName,
                barcodeResult.Data,
                barcodeResult.SymbolCount);

            this.dialog = new AlertDialog.Builder(this)
                                         .SetTitle("Scanned")
                                         .SetMessage(message)
                                         .SetCancelable(false)
                                         .SetPositiveButton(Android.Resource.String.Ok, (object sender, DialogClickEventArgs args) => {
                                             this.viewModel.ResumeScanning();
                                         })
                                         .Create();
            this.dialog.Show();
        }
    }
}
