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
using System.Timers;
using Android.Content;
using Android.OS;
using Android.Views;
using AndroidX.AppCompat.App;
using AndroidX.Lifecycle;
using Scandit.DataCapture.Core.Common.Geometry;
using Scandit.DataCapture.Core.UI;
using Scandit.DataCapture.Core.UI.Control;

namespace BarcodeCaptureSettingsSample.Scanning
{
    public class BarcodeScanFragment : CameraPermissionFragment, IBarcodeScanViewModelListener 
    {
        private const int dialogAutoDissmissInterval = 500;
        private readonly Timer continuousResultTimer = new Timer(dialogAutoDissmissInterval);
        
        private BarcodeScanViewModel viewModel;
        private DataCaptureView dataCaptureView;
        private AlertDialog dialog;

        public BarcodeScanFragment()
        {
            this.continuousResultTimer.Elapsed += (object sender, ElapsedEventArgs e) =>
            {
                if (this.viewModel.ContinuousScanningEnabled)
                {
                    this.DismissDialog();
                }
            };
        }

        public static BarcodeScanFragment Create() 
        {
            return new BarcodeScanFragment();
        }

        public bool ShowingDialog => this.dialog?.IsShowing ?? false;

        public override void OnCreate(Bundle savedInstanceState) 
        {
            base.OnCreate(savedInstanceState);
            this.HasOptionsMenu = true;
            this.viewModel = ViewModelProviders.Of(this)
                                               .Get(Java.Lang.Class.FromType(typeof(BarcodeScanViewModel))) as BarcodeScanViewModel;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            this.dataCaptureView = DataCaptureView.Create(this.RequireContext(), null);
            return this.dataCaptureView;
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);
            this.SetupDataCaptureView(this.viewModel.SettingsManager);
            this.SetupBarcodeCaptureOverlay(this.viewModel.SettingsManager);
        }

        public override void OnResume()
        {
            base.OnResume();

            if (this.dialog != null)
            {
                this.DismissDialog();
                this.dialog = null;
            }

            this.viewModel.SetListener(this);

            // Check for camera permission and request it, if it hasn't yet been granted.
            // Once we have the permission the onCameraPermissionGranted() method will be called.
            this.RequestCameraPermission();
        }

        public override void OnPause()
        {
            this.PauseFrameSource();
            base.OnPause();
        }

        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        {
            inflater.Inflate(Resource.Menu.settings_menu, menu);
        }

        public void ShowDialog(string symbologyName, string data, int symbolCount)
        {
            string textFormat = this.RequireContext().GetString(Resource.String.result_parametrised);
            string text = string.Format(textFormat, symbologyName, data, symbolCount);

            if (this.viewModel.ContinuousScanningEnabled)
            {
                this.ShowDialogForContinuousScanning(text);
            }
            else
            {
                this.ShowDialogForOneShotScanning(text);
            }
        }

        protected override bool ShouldShowBackButton() => false;

        protected override string GetTitle() => this.Context.GetString(Resource.String.app_title);

        protected override void OnCameraPermissionGranted()
        {
            this.ResumeFrameSource();
        }

        private void SetupDataCaptureView(SettingsManager settings)
        {
            this.dataCaptureView.DataCaptureContext = settings.DataCaptureContext;
            this.dataCaptureView.PointOfInterest = settings.PointOfInterest;
            this.dataCaptureView.ScanAreaMargins = settings.ScanAreaMargins;
            this.dataCaptureView.LogoAnchor = settings.LogoAnchor;
            this.dataCaptureView.LogoOffset = new PointWithUnit(settings.AnchorXOffset, settings.AnchorYOffset);
            
            if (settings.TorchButtonEnabled)
            {
                this.dataCaptureView.AddControl(new TorchSwitchControl(this.RequireContext()));
            }
        }

        private void SetupBarcodeCaptureOverlay(SettingsManager settings)
        {
            this.dataCaptureView.AddOverlay(settings.BarcodeCaptureOverlay);
        }

        private void ResumeFrameSource()
        {
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

        private void ShowDialogForContinuousScanning(string text)
        {
            this.continuousResultTimer.Start();

            if (this.ShowingDialog)
            {
                this.dialog.SetMessage(text);
            }
            else
            {
                this.dialog = this.BuildAutoDismissDialog(text);
                this.dialog.Show();
            }
        }

        private void ShowDialogForOneShotScanning(string text)
        {
            this.dialog = this.BuildPermanentDialog(text);
            this.dialog.Show();
        }

        private AlertDialog BuildPermanentDialog(string text)
        {
            return this.BuildBaseDialog(text)
                       .SetPositiveButton(
                           Resource.String.ok,
                           new EventHandler<DialogClickEventArgs>((object sender, DialogClickEventArgs args) => 
                           {
                               this.viewModel.ResumeScanning();
                           }))
                       .Create();
        }

        private AlertDialog BuildAutoDismissDialog(string text)
        {
            return this.BuildBaseDialog(text).Create();
        }

        private AlertDialog.Builder BuildBaseDialog(string text)
        {
            var context = this.RequireContext();
            return new AlertDialog.Builder(context)
                                  .SetCancelable(false)
                                  .SetMessage(text)
                                  .SetTitle(Resource.String.result_title);
        }

        private void DismissDialog()
        {
            if (this.ShowingDialog)
            {
                this.dialog.Dismiss();
            }
        }
    }
}