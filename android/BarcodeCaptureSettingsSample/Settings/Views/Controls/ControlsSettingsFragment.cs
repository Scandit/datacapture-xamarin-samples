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

using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.Lifecycle;
using BarcodeCaptureSettingsSample.Base;

namespace BarcodeCaptureSettingsSample.Settings.Views.Controls
{
    public class ControlsSettingsFragment : NavigationFragment
    {
        private ControlsSettingsViewModel viewModel;
        private Switch torchSwitch;
        private Switch zoomSwitch;

        public static ControlsSettingsFragment Create()
        {
            return new ControlsSettingsFragment();
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            this.viewModel = ViewModelProviders.Of(this)
                                               .Get(Java.Lang.Class.FromType(typeof(ControlsSettingsViewModel))) as ControlsSettingsViewModel;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return inflater.Inflate(Resource.Layout.fragment_controls_settings, container, false);
        }


        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);
            this.torchSwitch = view.FindViewById<Switch>(Resource.Id.switch_torch_button);
            this.zoomSwitch = view.FindViewById<Switch>(Resource.Id.switch_zoom_button);
            this.RefreshSwitchStates();
            this.SetupSwitches();
        }

        protected override bool ShouldShowBackButton() => true;

        protected override string GetTitle() => this.Context.GetString((int)ViewSettingsType.Controls);

        private void RefreshSwitchStates()
        {
            this.torchSwitch.Checked = this.viewModel.TorchButtonEnabled;
            this.zoomSwitch.Checked = this.viewModel.ZoomSwitchButtonEnabled;
        }

        private void SetupSwitches()
        {
            this.torchSwitch.CheckedChange += (object sender, CompoundButton.CheckedChangeEventArgs args) =>
            {
                this.viewModel.TorchButtonEnabled = args.IsChecked;
                this.RefreshSwitchStates();
            };
            this.zoomSwitch.CheckedChange += (object sender, CompoundButton.CheckedChangeEventArgs args) =>
            {
                this.viewModel.ZoomSwitchButtonEnabled = args.IsChecked;
                this.RefreshSwitchStates();
            };
        }
    }
}
