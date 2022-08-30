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

namespace BarcodeCaptureSettingsSample.Settings.BarcodeCapture.Feedback
{
    public class FeedbackSettingsFragment : NavigationFragment
    {
        private FeedbackSettingsViewModel viewModel;

        public static FeedbackSettingsFragment Create()
        {
            return new FeedbackSettingsFragment();
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            this.viewModel = ViewModelProviders.Of(this).Get(Java.Lang.Class.FromType(typeof(FeedbackSettingsViewModel))) as FeedbackSettingsViewModel;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return inflater.Inflate(Resource.Layout.fragment_feedback_settings, container, false);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

            Switch switchSound = view.FindViewById<Switch>(Resource.Id.switch_sound);
            switchSound.Checked = this.viewModel.SoundEnabled;
            switchSound.CheckedChange += (object sender, CompoundButton.CheckedChangeEventArgs args) =>
            { 
                this.viewModel.SoundEnabled = args.IsChecked;
            };

            Switch vibrationSwitch = view.FindViewById<Switch>(Resource.Id.switch_vibration);
            vibrationSwitch.Checked = this.viewModel.VibrationEnabled;
            vibrationSwitch.CheckedChange += (object sender, CompoundButton.CheckedChangeEventArgs args) =>
            {
                this.viewModel.VibrationEnabled = args.IsChecked;
            };
        }

        protected override bool ShouldShowBackButton() => true;

        protected override string GetTitle() => this.Context.GetString((int) BarcodeCaptureSettingsType.Feedback);
    }
}
