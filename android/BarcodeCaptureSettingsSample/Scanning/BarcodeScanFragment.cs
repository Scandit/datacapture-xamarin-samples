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
using Android.Support.Design.Widget;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.Lifecycle;
using BarcodeCaptureSettingsSample.Utils;
using Scandit.DataCapture.Barcode.Data;
using Scandit.DataCapture.Core.Common.Geometry;
using Scandit.DataCapture.Core.UI;
using Scandit.DataCapture.Core.UI.Control;
using Scandit.DataCapture.Core.UI.Gesture;

namespace BarcodeCaptureSettingsSample.Scanning
{
    public class BarcodeScanFragment : CameraPermissionFragment, IBarcodeScanViewModelListener 
    {
        private const int snackbarAutoDissmissInterval = 500;
        private readonly Timer continuousResultTimer;

        private BarcodeScanViewModel viewModel;
        private DataCaptureView dataCaptureView;
        private AlertDialog dialog;
        private Snackbar snackbar;

        private class SnackbarCallback : BaseTransientBottomBar.BaseCallback
        {
            public override void OnShown(Java.Lang.Object transientBottomBar)
            {
                base.OnShown(transientBottomBar);
                var view = ((Snackbar)transientBottomBar).View;
                view.Visibility = ViewStates.Visible;
            }
        }

        public BarcodeScanFragment()
        {
            this.continuousResultTimer = new Timer(snackbarAutoDissmissInterval)
            {
                AutoReset = false,
                Enabled = false
            };
            this.continuousResultTimer.Elapsed += (object sender, ElapsedEventArgs e) =>
            {
                if (this.viewModel.ContinuousScanningEnabled)
                {
                    this.DismissSnackbar();
                }
            };
        }

        public static BarcodeScanFragment Create() 
        {
            return new BarcodeScanFragment();
        }

        public bool ShowingDialog => this.dialog?.IsShowing ?? false;

        public bool ShowingSnackbar => this.snackbar?.IsShown ?? false;

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

            if (this.snackbar != null)
            {
                this.DismissSnackbar();
                this.snackbar = null;
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

        public void ShowDialog(Barcode barcode)
        {
            if (!this.IsAdded)
            {
                return;
            }

            string compositeType = string.Empty;
            string data = barcode.Data;

            if (!string.IsNullOrEmpty(barcode.AddOnData))
            {
                data += " " + barcode.AddOnData;
            }

            if (!string.IsNullOrEmpty(barcode.CompositeData))
            {
                data += " " + barcode.CompositeData;
                compositeType = this.StringFromCompositeFlag(barcode.CompositeFlag);
            }

            using SymbologyDescription description = SymbologyDescription.Create(barcode.Symbology);
            string symbology = description.ReadableName;
            int symbolCount = barcode.SymbolCount;
            string text;

            if (string.IsNullOrEmpty(compositeType))
            {
                string textFormat = this.RequireContext().GetString(Resource.String.result_parametrised);
                text = string.Format(textFormat, symbology, data, symbolCount);
            }
            else
            {
                string textFormat = this.RequireContext().GetString(Resource.String.cc_result_parametrised);
                text = string.Format(textFormat, compositeType, symbology, data, symbolCount);
            }

            if (this.viewModel.ContinuousScanningEnabled)
            {
                this.Activity.RunOnUiThread(() => this.ShowDialogForContinuousScanning(text));
            }
            else
            {
                this.Activity.RunOnUiThread(() => this.ShowDialogForOneShotScanning(text));
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
            if (settings.ZoomSwitchButtonEnabled)
            {
                this.dataCaptureView.AddControl(new ZoomSwitchControl(this.RequireContext()));
            }

            this.dataCaptureView.FocusGesture = settings.TapToFocusEnabled ? TapToFocus.Create() : null;
            this.dataCaptureView.ZoomGesture = settings.SwipeToZoomEnalbed ? SwipeToZoom.Create() : null;
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
            this.continuousResultTimer.Reset();

            if (this.ShowingSnackbar)
            {
                this.snackbar.SetText(text);
            }
            else
            {
                this.snackbar = this.BuildSnackbarDialog(text);
                this.snackbar.Show();
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

        private Snackbar BuildSnackbarDialog(string text)
        {
            Snackbar snackbar = Snackbar.Make(this.dataCaptureView, text, Snackbar.LengthShort);
            snackbar.SetDuration(int.MaxValue);
            CoordinatorLayout.LayoutParams layoutParams = (CoordinatorLayout.LayoutParams)snackbar.View.LayoutParameters;
            snackbar.View.Visibility = ViewStates.Invisible;
            layoutParams.Gravity = (int)GravityFlags.Top;
            snackbar.View.LayoutParameters = layoutParams;
            snackbar.View.SetBackgroundColor(Android.Graphics.Color.White);
            TextView textView = (TextView)snackbar.View.FindViewById(Resource.Id.snackbar_text);
            textView.SetTextColor(Android.Graphics.Color.Black);
            snackbar.AddCallback(new SnackbarCallback());

            return snackbar;
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
                this.Activity.RunOnUiThread(() => this.dialog.Dismiss());
            }
        }

        private void DismissSnackbar()
        {
            if (this.ShowingSnackbar)
            {
                this.Activity.RunOnUiThread(() => this.snackbar.Dismiss());
            }
        }

        private string StringFromCompositeFlag(CompositeFlag compositeFlag)
        {
            return compositeFlag switch
            {
                CompositeFlag.Gs1TypeA => "A",
                CompositeFlag.Gs1TypeB => "B",
                CompositeFlag.Gs1TypeC => "C",
                _ => string.Empty,
            };
        }
    }
}