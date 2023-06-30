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
using Android.OS;
using Android.Widget;
using AndroidX.Activity.Result;
using AndroidX.Activity.Result.Contract;
using MatrixScanCountSimpleSample.Views;
using Scandit.DataCapture.Barcode.Count.Capture;
using Scandit.DataCapture.Barcode.Count.UI;
using Scandit.DataCapture.Barcode.Data;
using Scandit.DataCapture.Core.Capture;

namespace MatrixScanCountSimpleSample
{
    [Activity(Label = "@string/app_title", MainLauncher = true)]
    public class MainActivity : CameraPermissionActivity
    {
        // There is a Scandit sample license key set below here.
        // This license key is enabled for sample evaluation only.
        // If you want to build your own application, get your license key by signing up for a trial at https://ssl.scandit.com/dashboard/sign-up?p=test
        public static string SCANDIT_LICENSE_KEY = "AQIzpSC5AyYeKA6KZgjthjEmMbJBFJEpiUUjkCJu72AUVSWyGjN0xNt0OVgASxKO6FwLejYDRFGraFReiUwL8wp3a8mgX0elHhmx0JhY/QYrbQHJjGIhQAhjcW1cYr+ogWCDUmhM2KuWPlJXBkSGmbwinMAqKusC5zQHGoY6JDKJXbzv97CRhGdjlfgjhTZErgfs+P/fLp0cCCAmP+TTZ6jiyA/my9Ojy7ugt7DKay2ZAkezAO8OwAtnl0GUIflPz6KI68hRPaAV18wwS030+riqfDIcFQ+3BAfqRMpJxrYfKZOvvwyTAbC+5ZzgFmwd9YR0vbFToSmHDemEyRVufdMw0s+jqCHsCY5ox8jBfV1RkmDQxCckkJoS3rhPmLgEyiTm+gI0y30swn2orZ4aaml+aoA55vhN4jY+ZAkMkmhipAXK/TMzyHo4iUDA4/v3TgiJbodw27iI/+f6YxIpA+/nAEItRH7C3vuxAdo8lmk5q0QeCkc6QA0FhQa6S/cu8yrehTi+Lb8khFmt3gkwEubowGdg3cg8KoBsDgY59lAKWy55rmVznq7REv6ugw1KwgW724K4s5ILfgQ2NcV/jFgeTReaTSVYUWKZGXdJmDrteX7tgmdfkpjaCrijgSGwYRaATxVKitCYIPyfuipsSHdC0iLqCoJ8CIc2UclvimPXDzDLk83uIRFjgspykVm+eIsKiMuxrW6OlB7o7NWPcJtEcyO74Mq6scB8+bWP5eJFIPazUcZEtxG2u3UpWz7+EoBADwbUI9G63HcTwt2bi8JZo16pfGxsWti3DJ1HWooGSIVvyZ2jePvhBcuu+EbtOucgdPDvDTCTpm/V";

        private BarcodeCount barcodeCount;
        private BarcodeCountView barcodeCountView;
        private DataCaptureContext dataCaptureContext;

        public class ExitLauncherResultCallback : Java.Lang.Object, IActivityResultCallback
        {
            private readonly MainActivity mainActivity;

            public ExitLauncherResultCallback(MainActivity mainActivity)
            {
                this.mainActivity = mainActivity;
            }

            public void OnActivityResult(Java.Lang.Object arg)
            {
                this.mainActivity.ResetSession();
            }
        }

        public class ListLauncherResultCallback : Java.Lang.Object, IActivityResultCallback
        {
            private readonly MainActivity mainActivity;

            public ListLauncherResultCallback(MainActivity mainActivity)
            {
                this.mainActivity = mainActivity;
            }

            public void OnActivityResult(Java.Lang.Object arg)
            {
                ActivityResult result;

                if (arg is ActivityResult)
                {
                    result = arg as ActivityResult;

                    if (result.ResultCode == (int)Android.App.Result.FirstUser)
                    {
                        this.mainActivity.ResetSession();
                    }
                }
            }
        }

        // The launcher to use when starting the result activity clicking the exit button
        private readonly ActivityResultLauncher exitLauncher;
        private readonly ActivityResultLauncher listLauncher;

        private bool navigatingInternally = false;

        public MainActivity()
        {
            this.exitLauncher = this.RegisterForActivityResult(
                new ActivityResultContracts.StartActivityForResult(),
                new ExitLauncherResultCallback(this));
            this.listLauncher = this.RegisterForActivityResult(
                new ActivityResultContracts.StartActivityForResult(),
                new ListLauncherResultCallback(this));
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            this.SetContentView(Resource.Layout.activity_main);

            // Initialize and start the barcode recognition.
            this.Initialize();
        }

        protected override void OnPause()
        {
            // Pause camera if the app is going to background,
            // but keep it on if it goes to result screen.
            // That way the session is not lost when coming back from results.
            if (!this.navigatingInternally)
            {
                CameraManager.Instance.PauseFrameSource();

                // Save current barcodes as additional barcodes.
                BarcodeManager.Instance.SaveCurrentBarcodesAsAdditionalBarcodes();
            }

            // Unsubscribe from barcode count.
            this.barcodeCount.Scanned -= this.BarcodeCountScanned;
            base.OnPause();
        }

        protected override void OnResume()
        {
            base.OnResume();
            this.navigatingInternally = false;

            // Subscribe for barcode count events.
            this.barcodeCount.Scanned += this.BarcodeCountScanned;

            // Enable the mode to start processing frames.
            this.barcodeCount.Enabled = true;

            // Check for camera permission and request it, if it hasn't yet been granted.
            // Once we have the permission the onCameraPermissionGranted() method will be called.
            this.RequestCameraPermission();
        }

        protected override void OnCameraPermissionGranted()
        {
            CameraManager.Instance.ResumeFrameSource();
        }

        private void Initialize()
        {
            // Create data capture context using your license key.
            this.dataCaptureContext = DataCaptureContext.ForLicenseKey(SCANDIT_LICENSE_KEY);

            // Initialize the shared camera manager.
            CameraManager.Instance.Initialize(dataCaptureContext);

            // The barcode count process is configured through barcode count settings
            // which are then applied to the barcode count instance that manages barcode count.
            BarcodeCountSettings barcodeCountSettings = new BarcodeCountSettings();

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
            barcodeCountSettings.EnableSymbologies(symbologies);

            // Create barcode count and attach to context.
            this.barcodeCount = BarcodeCount.Create(dataCaptureContext, barcodeCountSettings);

            // Initialize the shared barcode manager.
            BarcodeManager.Instance.Initialize(this.barcodeCount);

            // To visualize the on-going barcode count process on screen, setup a BarcodeCountView
            // that renders the camera preview. The view must be connected to the data capture context
            // and to the barcode count. This is optional, but recommended for better visual feedback.
            this.barcodeCountView = BarcodeCountView.Create(
                    this,
                    dataCaptureContext,
                    barcodeCount,
                    BarcodeCountViewStyle.Icon
            );

            // Subscribe to BarcodeCountView events.
            this.barcodeCountView.ListButtonTapped += BarcodeCountViewListButtonTapped;
            this.barcodeCountView.ExitButtonTapped += BarcodeCountViewExitButtonTapped;

            // Add the BarcodeCountView to the container.
            FrameLayout container = this.FindViewById<FrameLayout>(Resource.Id.data_capture_view_container);
            container.AddView(barcodeCountView);
        }

        private void ResetSession()
        {
            BarcodeManager.Instance.Reset();
            barcodeCount.ClearAdditionalBarcodes();
            barcodeCount.Reset();
        }

        private void BarcodeCountScanned(object sender, BarcodeCountEventArgs args)
        {
            BarcodeManager.Instance.UpdateWithSession(args.Session);
        }

        private void BarcodeCountViewExitButtonTapped(object sender, ExitButtonTappedEventArgs args)
        {
            this.navigatingInternally = true;
            this.exitLauncher.Launch(ResultsActivity.GetIntent(this, DoneButtonStyle.NewScan));
        }

        private void BarcodeCountViewListButtonTapped(object sender, ListButtonTappedEventArgs args)
        {
            this.navigatingInternally = true;
            this.listLauncher.Launch(ResultsActivity.GetIntent(this, DoneButtonStyle.Resume));
        }
    }
}
