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
using AndroidX.RecyclerView.Widget;
using BarcodeCaptureSettingsSample.Base;
using BarcodeCaptureSettingsSample.Settings.BarcodeCapture;
using BarcodeCaptureSettingsSample.Settings.Camera;
using BarcodeCaptureSettingsSample.Settings.ResultHandling;
using BarcodeCaptureSettingsSample.Settings.Views;
using Scandit.DataCapture.Core.Capture;

namespace BarcodeCaptureSettingsSample.Settings
{
    public class SettingsOverviewFragment : NavigationFragment
    {
        private SettingsOverviewViewModel viewModel;

        public static SettingsOverviewFragment Create()
        {
            return new SettingsOverviewFragment();
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            this.HasOptionsMenu = false;
            this.viewModel = ViewModelProviders.Of(this)
                                               .Get(Java.Lang.Class.FromType(typeof(SettingsOverviewViewModel))) as SettingsOverviewViewModel;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return inflater.Inflate(Resource.Layout.fragment_settings_overview, container, false);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

            string versionFormat = this.Context.GetString(Resource.String.sdk_version);

            TextView sdkVersionTextView = view.FindViewById<TextView>(Resource.Id.text_scandit_sdk_version);
            sdkVersionTextView.Text = string.Format(versionFormat, DataCaptureVersion.Version);

            RecyclerView recyclerOverviewOptions = view.FindViewById<RecyclerView>(Resource.Id.recycler_overview);
            recyclerOverviewOptions.SetLayoutManager(new LinearLayoutManager(this.RequireContext()));

            recyclerOverviewOptions.SetAdapter(new SettingsOverviewAdapter(this.viewModel.GetItems(), onClickCallback: (SettingsOverviewItem settingsOverview) =>
            { 
                this.MoveToDeeperSettings(settingsOverview); 
            }));
        }

        protected override bool ShouldShowBackButton() => true;

        protected override string GetTitle() => this.Context.GetString(Resource.String.settings);

        private void MoveToDeeperSettings(SettingsOverviewItem settingsOverview)
        {
            switch (settingsOverview.Type)
            {
                case SettingsOverviewType.BarcodeCaputre:
                    this.MoveToFragment(BarcodeCaptureSettingsFragment.Create(), true, null);
                    break;
                case SettingsOverviewType.Camera:
                    this.MoveToFragment(CameraSettingsFragment.Create(), true, null);
                    break;
                case SettingsOverviewType.View:
                    this.MoveToFragment(ViewSettingsFragment.Create(), true, null);
                    break;
                case SettingsOverviewType.ResultHandling:
                    this.MoveToFragment(ResultHandlingSettingsFragment.Create(), true, null);
                    break;
            }
        }
    }
}
