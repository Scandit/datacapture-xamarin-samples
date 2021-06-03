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

namespace BarcodeCaptureSettingsSample.Settings.Views.Gestures
{
    public class GesturesSettingsFragment : NavigationFragment
    {
        public static GesturesSettingsFragment Create()
        {
            return new GesturesSettingsFragment();
        }

        private GesturesSettingsViewModel viewModel;

        private Switch switchTapToFocus;
        private Switch switchSwipeToZoom;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            this.viewModel = ViewModelProviders.Of(this)
                                               .Get(Java.Lang.Class.FromType(typeof(GesturesSettingsViewModel))) as GesturesSettingsViewModel;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return inflater.Inflate(Resource.Layout.fragment_gestures_settings, container, false);
        }

    
        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

            this.switchTapToFocus = view.FindViewById<Switch>(Resource.Id.switch_tap_to_focus);
            this.RefreshSwitchTapToFocus();
            this.SetupTapToFocus();

            this.switchSwipeToZoom = view.FindViewById<Switch>(Resource.Id.switch_swipe_to_zoom);
            this.RefreshSwitchSwipeToZoom();
            this.SetupSwipeToZoom();
        }

        private void RefreshSwitchTapToFocus()
        {
            this.switchTapToFocus.Checked = this.viewModel.TapToFocusEnabled;
        }

        private void SetupTapToFocus()
        {
            this.switchTapToFocus.CheckedChange += (object sender, CompoundButton.CheckedChangeEventArgs args) =>
            {
                this.viewModel.TapToFocusEnabled = args.IsChecked;
                this.RefreshSwitchTapToFocus();
            };
        }

        private void RefreshSwitchSwipeToZoom()
        {
            this.switchSwipeToZoom.Checked = this.viewModel.SwipeToZoomEnabled;
        }

        private void SetupSwipeToZoom()
        {
            this.switchSwipeToZoom.CheckedChange += (object sender, CompoundButton.CheckedChangeEventArgs args) =>
            {
                this.viewModel.SwipeToZoomEnabled = args.IsChecked;
                this.RefreshSwitchSwipeToZoom();
            };
        }

        protected override bool ShouldShowBackButton() => true;

        protected override string GetTitle() => this.Context.GetString((int) ViewSettingsType.Gestures);
    }
}
