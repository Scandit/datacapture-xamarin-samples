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
using Android.Views;
using AndroidX.RecyclerView.Widget;
using BarcodeCaptureViewsSample.Modes.Base;
using Scandit.DataCapture.Barcode.UI.Overlay;
using Scandit.DataCapture.Core.UI;
using Scandit.DataCapture.Core.UI.Viewfinder;

namespace BarcodeCaptureViewsSample.Modes.SplitView
{
    [Activity(Label = "@string/split_view", Theme = "@style/Theme.AppCompat.Light", ParentActivity = typeof(MainActivity))]
    public class SplitViewScanActivity : CameraPermissionActivity
    {
        public static Intent GetIntent(Context context)
        {
            return new Intent(context, typeof(SplitViewScanActivity));
        }

        private readonly SplitViewScanViewModel viewModel = new SplitViewScanViewModel();

        private SplitViewAdapter adapter;
        private RecyclerView recyclerResults;
        private View tapToContinueLabel;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            this.SetContentView(Resource.Layout.activity_split_view);
            this.SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            this.SetupDataCaptureView();

            this.tapToContinueLabel = this.FindViewById<View>(Resource.Id.tap_to_continue_label);
            this.tapToContinueLabel.Click += OnTapToContinueLabelClick;

            this.recyclerResults = this.FindViewById<RecyclerView>(Resource.Id.recycler_scan_results);
            this.recyclerResults.SetLayoutManager(new LinearLayoutManager(this));
            this.recyclerResults.SetItemAnimator(new DefaultItemAnimator());
            this.recyclerResults.AddItemDecoration(new DividerItemDecoration(this, RecyclerView.Vertical));

            this.adapter = new SplitViewAdapter(this.viewModel.Barcodes);
            this.viewModel.BarcodeInserted += this.OnBarcodeInserted;
            this.viewModel.BarcodesCleared += this.OnBarcodesCleared;
            this.viewModel.ScannerPaused += this.OnScannerPaused;

            this.recyclerResults.SetAdapter(this.adapter);
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            base.OnCreateOptionsMenu(menu);
            this.MenuInflater.Inflate(Resource.Menu.settings_menu, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.action_clear:
                    this.viewModel.ClearResults();
                    return true;
                case Android.Resource.Id.Home:
                    this.OnBackPressed();
                    return true;
                default:
                    return base.OnOptionsItemSelected(item);
            }
        }

        protected override void OnResume()
        {
            base.OnResume();

            // Check for camera permission and request it, if it hasn't yet been granted.
            // Once we have the permission the onCameraPermissionGranted() method will be called.
            this.RequestCameraPermission();
        }

        protected override void OnCameraPermissionGranted()
        {
            // Switch camera on to start streaming frames.
            // The camera is started asynchronously and will take some time to completely turn on.
            this.viewModel.StartFrameSourceAsync();
            this.viewModel.ResumeScanning();
            this.tapToContinueLabel.Visibility = ViewStates.Gone;
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

        private void OnTapToContinueLabelClick(object sender, EventArgs e)
        {
            this.tapToContinueLabel.Visibility = ViewStates.Gone;
            this.viewModel.ResumeScanning();
        }

        private void OnScannerPaused(object sender, EventArgs e)
        {
            this.tapToContinueLabel.Visibility = ViewStates.Visible;
        }

        private void OnBarcodesCleared(object sender, int clearedBarcodesCount)
        {
            this.adapter?.NotifyItemRangeRemoved(0, clearedBarcodesCount);
        }

        private void OnBarcodeInserted(object sender, int itemPosition)
        {
            this.adapter?.NotifyItemInserted(itemPosition);
        }

        private void SetupDataCaptureView()
        {
            // To visualize the on-going barcode capturing process on screen,
            // setup a data capture view that renders the camera preview.
            // The view must be connected to the data capture context.
            DataCaptureView view = DataCaptureView.Create(this, this.viewModel.DataCaptureContext);

            // Add a barcode capture overlay to the data capture view to render the tracked
            // barcodes on top of the video preview.
            // This is optional, but recommended for better visual feedback.
            BarcodeCaptureOverlay overlay = BarcodeCaptureOverlay.Create(this.viewModel.BarcodeCapture, view, BarcodeCaptureOverlayStyle.Frame);

            // Add the aimer viewfinder to the overlay.
            overlay.Viewfinder = AimerViewfinder.Create();

            // We put the dataCaptureView in its container.
            ((ViewGroup)this.FindViewById(Resource.Id.scanner_container)).AddView(view);
        }
    }
}
