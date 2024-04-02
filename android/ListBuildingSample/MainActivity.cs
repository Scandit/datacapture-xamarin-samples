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
using System.Threading.Tasks;
using Android.App;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using ListBuildingSample.Extensions;
using ListBuildingSample.Models;
using ListBuildingSample.Views;
using Scandit.DataCapture.Barcode.Data;
using Scandit.DataCapture.Barcode.Spark.Capture;
using Scandit.DataCapture.Barcode.Spark.Feedback;
using Scandit.DataCapture.Barcode.Spark.UI;
using Scandit.DataCapture.Core.Capture;

namespace ListBuildingSample
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true, ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : CameraPermissionActivity, ISparkScanFeedbackDelegate
    {
        // There is a Scandit sample license key set below here.
        // This license key is enabled for sample evaluation only.
        // If you want to build your own application, get your license key by signing up for a trial at https://ssl.scandit.com/dashboard/sign-up?p=test
        public static string SCANDIT_LICENSE_KEY = "Aa2k0xbKMtvDJWNgLU02Cr8aLxUjNtOuqXCjHUxVAUf/d66Y5Tm74sJ+8L0rGQUZ20e52VlMY9I7YW4W13kWbvp36R8jbqQy6yZUGS50G5n4fRItJD6525RcbTYZQjoIGHQqle9jj08ra19ZUy9RliVlOn3hHz4WrGO8vORyATmFXJpULzk0I5RpiT84ckXhG2Ri8jtIzoISX3zsoiLtXVRGjjrkbuGZzGbKA180JKEpdfSQwVyupLti5yNYHAeKihS6IOklCTz8CM1BfRC4zBdIDjbVEJPFgAsLvMU0rTyJhHkB5Ds4wfHbKNFhW0T2XkYLKkvZ7X/HnEVD5oz9Kl4T4rtRkepJfsXUWHUgVugjLO5vqwhMcHNV5XpK2Pk/SLrzGF1PDRu8f4ZhBLrWKknWq+5TSK8GWi4wmGpVvbxqHhLljzOzplYs8I5TtphZ3otJNLs10lhk1YN9cmdaxpdUuF4k0WDU1Qfco75p5G+MBlsAVVFrs0xMF9fSMJkQ+4UU+G+py5781HPkpw4kaGwmJhGrzA/Lbhf4tL+XfynseLw42oygpfVabYEYRHSQx+1j5RpFSR6V9t4jlKsJu2xgYz0A96I82gIHItRRxZkT2oEsZCgYlgCiQsFcsFdo9N9bzDL9mVR5Nj0RPIVvKc01AVtKvXLx86g2rNPv45eBaJFrdsWmv97V8+Pv6M9d+Wr1qcTeT1BY8fvWUEDmU1HF6eCJ1A6cDAM+Nq4sAP9D2lH7D6rHwK+x07F56bMZibLeDoGKanE8PhhamhxBVemE/ByCoMoItBtSbpeBubHVsSHlGF3/AAKi6flY6j0htptgPOM8eOwGXx6YvVxu3KOMF+2RBIQai8LP0YEuhVJ0ST7WX5seeVSu5RMKUx/euHoQB6qID+ydzkXGzYZLTPPskmJSWqrboJQPIjZ/ruCtJepZ/+Lr7g5nCyb01w==";

        private SparkScanBarcodeErrorFeedback errorFeedback;
        private SparkScanBarcodeSuccessFeedback successFeedback;

        private DataCaptureContext dataCaptureContext;
        private SparkScan sparkScan;
        private SparkScanView sparkScanView;

        private ResultListAdapter resultListAdapter = new ResultListAdapter();
        private TextView resultCountTextView;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            this.SetContentView(Resource.Layout.activity_main);

            // Initialize spark scan and start the barcode recognition.
            this.Initialize();

            // Setup RecyclerView for results
            RecyclerView recyclerView = this.FindViewById<RecyclerView>(Resource.Id.result_recycler);
            recyclerView.SetLayoutManager(new LinearLayoutManager(this));
            DividerItemDecoration divider =
                new DividerItemDecoration(this, LinearLayoutManager.Vertical);
            divider.Drawable = this.GetDrawable(Resource.Drawable.recycler_divider);
            recyclerView.AddItemDecoration(divider);
            recyclerView.SetAdapter(this.resultListAdapter);

            this.resultCountTextView = this.FindViewById<TextView>(Resource.Id.item_count);
            this.SetItemCount(0);

            // Set up the button that clears the result list
            Button clearButton = this.FindViewById<Button>(Resource.Id.clear_list);
            clearButton.Click += this.ClearButtonClick;

            this.resultListAdapter.ListChanged += this.ResultListAdapterListChanged;
        }

        private void ResultListAdapterListChanged(object sender, EventArgs e)
        {
            this.SetItemCount(this.resultListAdapter.ItemCount);
        }

        protected override void OnPause()
        {
            base.OnPause();
            this.sparkScanView.OnPause();
        }

        protected override void OnResume()
        {
            base.OnResume();
            this.sparkScanView.OnResume();

            // Check for camera permission and request it, if it hasn't yet been granted.
            this.RequestCameraPermission();
        }

        protected override void OnCameraPermissionGranted()
        {
        }

        private void ClearButtonClick(object sender, EventArgs e)
        {
            this.resultListAdapter.ClearResults();
        }

        private void SetItemCount(int count)
        {
            this.RunOnUiThread(() =>
            {
                this.resultCountTextView.Text =
                    Resources.GetQuantityString(Resource.Plurals.results_amount, count, count);
            });
        }

        private void Initialize()
        {
            // Create data capture context using your license key.
            this.dataCaptureContext = DataCaptureContext.ForLicenseKey(SCANDIT_LICENSE_KEY);

            // The spark scan process is configured through SparkScan settings
            // which are then applied to the spark scan instance that manages the spark scan.
            SparkScanSettings settings = new SparkScanSettings();

            // The settings instance initially has all types of barcodes (symbologies) disabled.
            // For the purpose of this sample we enable a very generous set of symbologies.
            // In your own app ensure that you only enable the symbologies that your app requires
            // as every additional enabled symbology has an impact on processing times.
            HashSet<Symbology> symbologies = new HashSet<Symbology>()
            {
                Symbology.Ean13Upca,
                Symbology.Ean8,
                Symbology.Upce,
                Symbology.Code39,
                Symbology.Code128,
                Symbology.InterleavedTwoOfFive
            };
            settings.EnableSymbologies(symbologies);

            // Some linear/1d barcode symbologies allow you to encode variable-length data.
            // By default, the Scandit Data Capture SDK only scans barcodes in a certain length range.
            // If your application requires scanning of one of these symbologies, and the length is
            // falling outside the default range, you may need to adjust the "active symbol counts"
            // for this symbology. This is shown in the following few lines of code for one of the
            // variable-length symbologies.
            settings.GetSymbologySettings(Symbology.Code39).ActiveSymbolCounts =
                new short[] { 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 };

            // Create new spark scan mode with the settings from above.
            this.sparkScan = new SparkScan(settings);

            // Subsribe to BarcodeScanned event to get informed whenever a new barcode got recognized.
            this.sparkScan.BarcodeScanned += this.BarcodeScanned;

            // The SparkScanCoordinatorLayout container will make sure that the main layout of the view
            // will not break when the SparkScanView will be attached.
            // When creating the SparkScanView instance use the SparkScanCoordinatorLayout
            // as a parent view.
            SparkScanCoordinatorLayout container = this.FindViewById<SparkScanCoordinatorLayout>(Resource.Id.spark_scan_coordinator);

            // You can customize the SparkScanView using SparkScanViewSettings.
            SparkScanViewSettings viewSettings = new SparkScanViewSettings();

            // Creating the instance of SparkScanView. The instance will be automatically added
            // to the container.
            this.sparkScanView =
                SparkScanView.Create(container, this.dataCaptureContext, this.sparkScan, viewSettings);

            this.SetupSparkScanFeedback();
        }

        private void SetupSparkScanFeedback()
        {
            this.errorFeedback = new SparkScanBarcodeErrorFeedback(
                message: "This code should not have been scanned",
                resumeCapturingDelay: TimeSpan.FromSeconds(60));

            this.successFeedback = new SparkScanBarcodeSuccessFeedback();
            this.sparkScanView.Feedback = this;
        }

        private void BarcodeScanned(object sender, SparkScanEventArgs args)
        {
            if (args.Session.NewlyRecognizedBarcodes.Count == 0)
            {
                return;
            }

            var frame = args.FrameData?.ImageBuffer.ToBitmap();
            var barcode = args.Session.NewlyRecognizedBarcodes.First();

            Task.Factory.StartNew(() =>
            {
                var thumbnail = barcode.GetBarcodeImage(frame) ?? Bitmap.CreateBitmap(1, 1, Bitmap.Config.Argb8888);

                this.RunOnUiThread(() =>
                {
                    if (IsBarcodeValid(barcode))
                    {
                        var itemNumber = this.resultListAdapter.ItemCount + 1;
                        this.resultListAdapter.AddListItem(
                            new ListItem(
                                thumbnail,
                                itemNumber,
                                barcode.Symbology,
                                barcode.Data));
                    }
                });
            });
        }

        SparkScanBarcodeFeedback ISparkScanFeedbackDelegate.GetFeedbackForBarcode(Barcode barcode)
        {
            if (IsBarcodeValid(barcode))
            {
                return this.successFeedback;
            }

            return this.errorFeedback;
        }

        private static bool IsBarcodeValid(Barcode barcode)
        {
            return barcode.Data != "123456789";
        }
    }
}
