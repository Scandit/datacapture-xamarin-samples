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
        public static string SCANDIT_LICENSE_KEY = "AfUkdmKlRiP5FdlOFQnOhu4V3j5LFKttPGTWXFd7CkuRaTAstDqq78RrBm2ZG9LRu1T8CNgP6oLScGrUoEwfmP1TUXonIGCl2g9Fo5NYtmK/aEV8FX/YcdRKfWS5bJrTcWGDHdcsJxT6Me5C3RMdWZkdqeR5GEjDzT6dO4ZPWOBbNLjpkgZ0/MjtYQPKqSV+bSZC7+ekFaXovSKWfXV89BXtta/6sZHFJOMKxyvzh6zw5yA+NDR67OXoWKCrrNq4AOuBlt1ZelIHCqjQgTy/SZG110eJr5e4pth38Bx0fXE8FGX92BoxwJr1EG+P5CEJF8EFMy2zf87aJQYuzHmg0nM7czcNqLUd9F23uxntZYjKlwgWmmSzev/ozaumEvbW9RVW1bUQmV8pQ1SWILBuzQPeAw8iWOWgnTH18tH7cT+fUJumvM2rn7LWx9JYLAKBKRuwe2sDh3l5eqobZKdarIRsKVgXa4pw+gkYKuplzTo+Bzh70rbmtgq3IJ8hSpdoZITzfUQSwXkrgdQa5Cmrpxz9gXManBRt01h3eFXG7znZU9w0+uzzV/b5e6MQcPncODrCQOq0kfEBYgRoLAwVCOKnxyWQkqRbUpsTN2wy2MTg10flYhR/zf1eXdiUjgPUhWj8LtmgxJELYky7uMu46abfCkAw73e+12iJmlf9/tmTFk34La9ZQiF/BYps5h327ZW8qobay+Esx1i9dsaFKYt/nCN8jZdUYD/df+/vApyK4PMbph9EPRe5u0alg8BqpEExnkQsy1W7r85yngO/rxSXsY6rTMoTXb/87ul8uQnsrD41ZLtFdzo0OlbNTeNOI1mJz/E6/SOLbRRK";

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
