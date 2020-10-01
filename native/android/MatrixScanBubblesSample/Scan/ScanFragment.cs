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
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.Lifecycle;
using MatrixScanBubblesSample.Scan.Bubbles;
using MatrixScanBubblesSample.Scan.Bubbles.Data;
using Scandit.DataCapture.Barcode.Tracking.Data;
using Scandit.DataCapture.Barcode.Tracking.UI.Overlay;
using Scandit.DataCapture.Core.UI;

namespace MatrixScanBubblesSample.Scan
{
    public class ScanFragment : CameraPermissionFragment, IScanViewModelListener
    {
        private ScanViewModel viewModel;
        private BubbleSizeManager bubbleSizeManager;

        private DataCaptureView dataCaptureView;
        private BarcodeTrackingAdvancedOverlay bubblesOverlay;
        private BarcodeTrackingBasicOverlay highlightOverlay;
        private ImageButton freezeButton;

        // We reuse bubble views where possible.
        private SparseArray<Bubble> bubbles;

        private class OnClickListener : Java.Lang.Object, View.IOnClickListener
        {
            private readonly Action onClickAction;

            public OnClickListener(Action onClickAction) => this.onClickAction = onClickAction;

            public void OnClick(View view) => this.onClickAction?.Invoke();
        }

        public static ScanFragment Create()
        {
            return new ScanFragment();
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            this.viewModel = ViewModelProviders.Of(this).Get(Java.Lang.Class.FromType(typeof(ScanViewModel))) as ScanViewModel;
            this.bubbleSizeManager = new BubbleSizeManager(this.RequireContext());
            this.bubbles = new SparseArray<Bubble>();
        }

        public override View OnCreateView(
                LayoutInflater inflater,
                ViewGroup container,
                Bundle savedInstanceState)
        {
            View layout = inflater.Inflate(Resource.Layout.fragment_scan, container, false);

            // To visualize the on-going barcode capturing process on screen,
            // setup a data capture view that renders the camera preview.
            // The view must be connected to the data capture context.
            this.dataCaptureView = DataCaptureView.Create(
                this.RequireContext(),
                this.viewModel.GetDataCaptureContext());

            // We create an overlay to highlight the barcodes.
            this.highlightOverlay = BarcodeTrackingBasicOverlay.Create(
                this.viewModel.BarcodeTracking, dataCaptureView);
            this.highlightOverlay.Brush = this.viewModel.DefaultBrush;

            // We create an overlay for the bubbles.
            this.bubblesOverlay = BarcodeTrackingAdvancedOverlay.Create(
                    this.viewModel.BarcodeTracking,
                    this.dataCaptureView);
            this.bubblesOverlay.Listener = this.viewModel;

            // We add the data capture view to the root layout.
            ((ViewGroup)layout.FindViewById(Resource.Id.root)).AddView(
                    this.dataCaptureView,
                    new ViewGroup.LayoutParams(
                        ViewGroup.LayoutParams.MatchParent,
                        ViewGroup.LayoutParams.MatchParent));

            return layout;
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

            this.freezeButton = view.FindViewById<ImageButton>(Resource.Id.button_freeze);
            this.freezeButton.SetOnClickListener(new OnClickListener(() => this.viewModel.ToggleFreeze()));

            this.OnFrozenChanged(this.viewModel.IsFrozen());
        }

        public override void OnDestroyView()
        {
            base.OnDestroyView();
            this.dataCaptureView.RemoveOverlay(this.bubblesOverlay);
            this.bubblesOverlay.Listener = null;
            this.highlightOverlay.Listener = null;
        }

        public override void OnResume()
        {
            base.OnResume();

            // Check for camera permission and request it, if it hasn't yet been granted.
            // Once we have the permission the onCameraPermissionGranted() method will be called.
            this.RequestCameraPermission();
        }

        public override void OnPause()
        {
            this.PauseFrameSource();
            base.OnPause();
        }

        #region IScanViewModelListener
        public bool ShouldShowBubble(TrackedBarcode barcode)
        {
            var result = this.dataCaptureView.MapFrameQuadrilateralToView(barcode.Location);
            return this.bubbleSizeManager.IsBarcodeLargeEnoughForBubble(result);
        }

        public View GetOrCreateViewForBubbleData(
            TrackedBarcode barcode,
            BubbleData bubbleData,
            bool visible)
        {
            Bubble bubble = this.bubbles.Get(barcode.Identifier);

            if (bubble == null)
            {
                // There's no recyclable bubble for this tracking identifier, so we create one.
                string code = barcode.Barcode.Data ?? string.Empty;
                bubble = new Bubble(this.RequireContext(), bubbleData, code);

                // We store the newly created bubble to recycle it in subsequent frames.
                this.bubbles.Put(barcode.Identifier, bubble);
            }

            return bubble.Root;
        }

        public void SetBubbleVisibility(TrackedBarcode barcode, bool visible)
        {
            Bubble bubble = this.bubbles.Get(barcode.Identifier);

            if (visible)
            {
                bubble?.Show();
            }
            else
            {
                bubble?.Hide();
            }
        }

        public void RemoveBubbleView(int identifier)
        {
            // When a barcode is not tracked anymore, we can remove the bubble from our list.
            this.bubbles.Remove(identifier);
        }

        public void OnFrozenChanged(bool frozen)
        {
            this.freezeButton.SetImageResource(
                    frozen ? Resource.Drawable.freeze_disabled : Resource.Drawable.freeze_enabled
            );
        }
        #endregion

        protected override void OnCameraPermissionGranted()
        {
            this.ResumeFrameSource();
        }

        private void ResumeFrameSource()
        {
            this.viewModel.SetListener(this);

            // Switch camera on to start streaming frames.
            // The camera is started asynchronously and will take some time to completely turn on.
            this.viewModel.StartFrameSource();
            this.viewModel.ResumeScanning();
        }

        private void PauseFrameSource()
        {
            this.viewModel.SetListener(null);

            // Switch camera off to stop streaming frames.
            // The camera is stopped asynchronously and will take some time to completely turn off.
            // Until it is completely stopped, it is still possible to receive further results, hence
            // it's a good idea to first disable barcode tracking as well.
            this.viewModel.PauseScanning();
            this.viewModel.StopFrameSource();
        }
    }
}