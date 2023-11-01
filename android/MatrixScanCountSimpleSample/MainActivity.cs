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
        public static string SCANDIT_LICENSE_KEY = "AWHjFzlFHa+fLq/kfS8GCBU/hT60NkQeVGQOWhhtRVcDZxJfsD0OY9NK0YErLuxTtTKLC1BLdrvDdsJ1dnxmcx9fDIeeaQlxawtkiq1pmEFxHOvYa3emcbAfOeiwbFPtQEWCWvdc95KoIFxAuDiYcfccdywzH2KONgwmnV9cEcX11FhIPLtX74RLua7VkOukFfNTOGExxhiCq96qZnzGgrgViuagpL0ekK6xv8K4bYt7lVkxloUMM6dFRSZ4aummJ2Q1uZNR78kSGCpCn/uJjaf/5lyNbYWpnxYvsYRPI7jOFYZykI0nIjhjt/ncukCEsz4BQLAh5hp1qocvQ2+dw3ADD8LJLXcnX7JaCOKV5cfHEHGSLR4moTxNtxPXdUNlM5w75iHZub5BsIfkJCknKrLn5oJ15k5Rx4/JnFj11tGLqtfRs+jdtXSGxAb86BxwPM1mEBO/Va1yV//CGku5UWR5MwspCf7pl8OUH7frkCtV4kDB6y5jusSMSIEGnKCLd2sWKE04mAURrpWt8pgsIB89xXPPTgPh1C+nAeMuuEN3dPYAJYrJKvy44w130JrUvxWLcTM1oFVWikC6CluLC7WGgRhZCew0eROnv9neITolB6Gmy04dlF0euA595dJcw2lLTwwxEydGp5gGIIDtofviho7JdHtPrMer/Ptz1/LOVeF55OY9eg8z1Lq2CkZf6cgWZBPa1uakuZzxWXZUprJMdTquhInmqP4ELLxGXhv+CXoT2n0p022+wyiWAXatmhvcK+n2uCWX30SL0Sri1qPmf6Ldtgqj2aFEMLM+LouJg6Ukv0PKUTXlgPW7L0vYrNGtPjvRlaR7Nwph";

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
