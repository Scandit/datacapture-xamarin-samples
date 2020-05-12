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
using System.Linq;
using System.Threading.Tasks;
using Android.OS;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using AndroidX.Lifecycle;
using AndroidX.RecyclerView.Widget;
using BarcodeCaptureSettingsSample.Base;
using Scandit.DataCapture.Core.Source;

namespace BarcodeCaptureSettingsSample.Settings.Camera
{
    public class CameraSettingsFragment : NavigationFragment
    {
        private const float ZoomStep = 0.1f;
        private const float ZoomMin = 1f;
        private const float ZoomMax = 20f;

        private CameraSettingsViewModel viewModel;

        private CameraSettingsPositionAdapter positionAdapter;

        private RecyclerView recyclerCameraPositions;
        private Switch torchSwitch;
        private EditText editFrameRate;
        private View containerResolution;
        private TextView textResolution, textZoomFactor;
        private SeekBar seekbarZoomFactor;

        public static CameraSettingsFragment Create()
        {
            return new CameraSettingsFragment();
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            this.viewModel = ViewModelProviders.Of(this)
                                               .Get(Java.Lang.Class.FromType(typeof(CameraSettingsViewModel))) as CameraSettingsViewModel;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return inflater.Inflate(Resource.Layout.fragment_camera_settings, container, false);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);
            this.recyclerCameraPositions = view.FindViewById<RecyclerView>(Resource.Id.recycler_camera_positions);
            this.SetupCameraPositionRecycler();

            this.torchSwitch = view.FindViewById<Switch>(Resource.Id.switch_torch_state);
            this.RefreshTorchData();
            this.SetupTorchSwitch();

            this.editFrameRate = view.FindViewById<EditText>(Resource.Id.edit_frame_rate);
            this.RefreshFrameRateData();
            this.SetupEditTextFrameRate();

            this.containerResolution = view.FindViewById<View>(Resource.Id.container_preferred_resolution);
            this.textResolution = view.FindViewById<TextView>(Resource.Id.text_preferred_resolution);
            this.RefreshResolutionData();
            this.SetupResolution();

            this.seekbarZoomFactor = view.FindViewById<SeekBar>(Resource.Id.seekbar_zoom_factor);
            this.textZoomFactor = view.FindViewById<TextView>(Resource.Id.text_zoom_factor);
            this.SetupZoomFactor();
            this.RefreshZoomFactorData();
        }

        protected override bool ShouldShowBackButton() => true;

        protected override string GetTitle() => this.Context.GetString((int)SettingsOverviewType.Camera);

        private void SetupCameraPositionRecycler()
        {
            this.recyclerCameraPositions.SetLayoutManager(new LinearLayoutManager(this.RequireContext()));
            async Task onClickCallback(CameraSettingsPositionItem item)
            {
                await this.viewModel.SetCameraPositionAsync(item.CameraPosition);
                this.RefreshCameraPositionData();
                this.RefreshFrameRateData();
                this.RefreshResolutionData();
                this.RefreshZoomFactorData();
            }
            this.positionAdapter = new CameraSettingsPositionAdapter(this.viewModel.GetItems(), onClickCallback);
            this.recyclerCameraPositions.SetAdapter(this.positionAdapter);
        }

        private void SetupTorchSwitch()
        {
            this.torchSwitch.CheckedChange += (object sender, CompoundButton.CheckedChangeEventArgs args) =>
            {
                this.viewModel.TorchEnabled = args.IsChecked;
                this.RefreshTorchData();
            };
        }

        private void SetupEditTextFrameRate()
        {
            this.editFrameRate.EditorAction += async (object sender, TextView.EditorActionEventArgs args) =>
            {
                if (args.ActionId == ImeAction.Done)
                {
                    await this.ApplyChangeAsync(this.editFrameRate.Text);
                    this.DismissKeyboard(this.editFrameRate);
                    this.editFrameRate.ClearFocus();
                }
            };
        }

        private void SetupResolution()
        {
            this.containerResolution.Click += (object sender, EventArgs args) =>
            {
                this.BuildAndShowResolutionMenu();
            };
        }

        private void SetupZoomFactor()
        {
            this.seekbarZoomFactor.Max = (int)((ZoomMax - ZoomMin) / ZoomStep);
            this.seekbarZoomFactor.ProgressChanged += async (object sender, SeekBar.ProgressChangedEventArgs args) =>
            {
                if (args.FromUser)
                {
                    float decimalProgress = ZoomMin + (args.Progress * ZoomStep);
                    await this.viewModel.SetZoomFactorAsync(decimalProgress);
                    this.RefreshZoomFactorData();
                }
            };
        }

        private async Task ApplyChangeAsync(string text)
        {
            if (float.TryParse(text, out float result))
            {
                await this.viewModel.SetMaxFrameRateAsync(result);
                this.RefreshFrameRateData();
            }
            else
            {
                this.ShowInvalidNumberToast();
            }
        }

        private void ShowInvalidNumberToast()
        {
            Toast.MakeText(this.RequireContext(), Resource.String.number_not_valid, ToastLength.Long).Show();
        }

        private void BuildAndShowResolutionMenu()
        {
            using PopupMenu menu = new PopupMenu(this.RequireContext(), containerResolution, GravityFlags.End);

            /*
             * UHD4K is not supported in the Camera API 1. To use the Camera API 2, please contact
             * us at support@scandit.com.
             */
            VideoResolution.GetValues()
                           .Where(v => v != VideoResolution.Uhd4k)
                           .Select(v => menu.Menu.Add(v.Name()))
                           .ToList();

            menu.MenuItemClick += async (object sender, PopupMenu.MenuItemClickEventArgs args) =>
            {
                string selectedResolution = args.Item.TitleFormatted.ToString();
                await this.viewModel.SetVideoResolutionAsync(Java.Lang.Enum.ValueOf(Java.Lang.Class.FromType(typeof(VideoResolution)), selectedResolution) as VideoResolution);
                this.RefreshResolutionData();
            };

            menu.Show();
        }

        private void RefreshCameraPositionData()
        {
            this.positionAdapter.UpdateData(this.viewModel.GetItems());
        }

        private void RefreshTorchData()
        {
            this.torchSwitch.Checked = this.viewModel.TorchEnabled;
        }

        private void RefreshFrameRateData()
        {
            string textFormat = this.Context.GetString(Resource.String.size_no_unit);
            this.editFrameRate.Text = string.Format(textFormat, this.viewModel.MaxFrameRate);
        }

        private void RefreshResolutionData()
        {
            this.textResolution.Text = this.viewModel.VideoResolution.Name();
        }

        private void RefreshZoomFactorData()
        {
            float value = this.viewModel.ZoomFactor;
            string textFormat = this.Context.GetString(Resource.String.size_no_unit);
            this.textZoomFactor.Text = string.Format(textFormat, value);
            this.seekbarZoomFactor.Progress = (int)((value - ZoomMin) / ZoomStep);
        }
    }
}
