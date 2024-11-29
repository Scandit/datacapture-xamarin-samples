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
using Android.Views;
using AndroidX.AppCompat.App;
using AndroidX.AppCompat.Widget;
using AndroidX.Lifecycle;
using Google.Android.Material.BottomNavigation;
using Scandit.DataCapture.Core.UI;
using Scandit.DataCapture.ID.Data;
using Scandit.DataCapture.ID.UI;
using Scandit.DataCapture.ID.UI.Overlay;

namespace IdCaptureExtendedSample.Scan
{
    public class ScanFragment : CameraPermissionFragment, BottomNavigationView.IOnNavigationItemSelectedListener, IScanViewModelListener
    {
        private Mode mode = Mode.Barcode;
        private ScanViewModel viewModel;
        private BottomNavigationView modeNavigation;
        private DataCaptureView dataCaptureView;
        private IdCaptureOverlay idCaptureOverlay;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            this.viewModel = ViewModelProviders.Of(this)
                                               .Get(Java.Lang.Class.FromType(typeof(ScanViewModel))) as ScanViewModel;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View layout = inflater.Inflate(Resource.Layout.scan_screen, container, false);

            this.AttachDataCaptureView(layout);
            this.InitViews(layout);

            return layout;
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

            this.modeNavigation.SetOnNavigationItemSelectedListener(this);
        }

        public override void OnDestroyView()
        {
            base.OnDestroyView();

            this.dataCaptureView = null;
            this.idCaptureOverlay = null;
        }

        public override void OnResume()
        {
            base.OnResume();

            // Check for camera permission and request it, if it hasn't yet been granted.
            // Once we have the permission the OnCameraPermissionGranted() method will be called.
            this.RequestCameraPermission();
        }

        public override void OnPause()
        {
            base.OnPause();

            this.PauseFrameSource();
        }

        public bool OnNavigationItemSelected(IMenuItem menuItem)
        {
            switch (menuItem.ItemId)
            {
                case Resource.Id.barcode:
                    this.Select(Mode.Barcode);
                    return true;
                case Resource.Id.mrz:
                    this.Select(Mode.MRZ);
                    return true;
                case Resource.Id.viz:
                    this.Select(Mode.VIZ);
                    return true;
                default:
                    return false;
            }
        }

        #region IScanViewModelListener
        public void ShowIdCaptured(CapturedId capturedId)
        {
            ((MainActivity)this.RequireActivity()).GoToResultScreen(capturedId);
        }

        public void ShowError(string error)
        {
            this.viewModel.PauseScanning();

            ((MainActivity)this.RequireActivity()).ShowAlert(Resource.String.error, error, completion: () =>
            {
                this.viewModel.ResumeScanning();
            });
        }

        public void ShowIdRejected()
        {
            this.viewModel.PauseScanning();

            var message = this.Context.Resources.GetString(Resource.String.document_not_supported_message);
            ((MainActivity)this.RequireActivity()).ShowAlert(Resource.String.error, message, completion: () =>
            {
                this.viewModel.ResumeScanning();
            });
        }
        #endregion

        protected override void OnCameraPermissionGranted()
        {
            this.ResumeFrameSource();
        }

        private void AttachDataCaptureView(View root)
        {
            ViewGroup dataCaptureViewContainer = root.FindViewById<ViewGroup>(Resource.Id.data_capture_view_container);

            this.dataCaptureView = DataCaptureView.Create(this.RequireContext(), this.viewModel.DataCaptureContext);
            dataCaptureViewContainer.AddView(
                this.dataCaptureView,
                new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent));
        }

        private void InitViews(View root)
        {
            this.modeNavigation = root.FindViewById<BottomNavigationView>(Resource.Id.bottom_bar);
            this.Select(this.mode);
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
            // it's a good idea to first disable barcode batch as well.
            this.viewModel.PauseScanning();
            this.viewModel.StopFrameSource();
        }

        private void Select(Mode mode)
        {
            this.mode = mode;

            if (this.idCaptureOverlay != null)
            {
                this.dataCaptureView?.RemoveOverlay(this.idCaptureOverlay);
            }

            this.viewModel.OnModeSelected(mode);
            this.idCaptureOverlay = IdCaptureOverlay.Create(this.viewModel.IdCapture, this.dataCaptureView);
            this.idCaptureOverlay.IdLayoutStyle = IdLayoutStyle.Rounded;
        }
    }
}
