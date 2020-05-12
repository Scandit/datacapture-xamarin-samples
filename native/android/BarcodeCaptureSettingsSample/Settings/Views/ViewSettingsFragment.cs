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
using AndroidX.Lifecycle;
using AndroidX.RecyclerView.Widget;
using BarcodeCaptureSettingsSample.Base;
using BarcodeCaptureSettingsSample.Settings.Views.Controls;
using BarcodeCaptureSettingsSample.Settings.Views.Logo;
using BarcodeCaptureSettingsSample.Settings.Views.Overlays;
using BarcodeCaptureSettingsSample.Settings.Views.PointOfInterests;
using BarcodeCaptureSettingsSample.Settings.Views.ScanAreas;
using BarcodeCaptureSettingsSample.Settings.Views.Viewfinder;

namespace BarcodeCaptureSettingsSample.Settings.Views
{
    public class ViewSettingsFragment : NavigationFragment
    {
        private ViewSettingsViewModel viewModel;

        public static ViewSettingsFragment Create()
        {
            return new ViewSettingsFragment();
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            this.viewModel = ViewModelProviders.Of(this)
                                               .Get(Java.Lang.Class.FromType(typeof(ViewSettingsViewModel))) as ViewSettingsViewModel;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return inflater.Inflate(Resource.Layout.fragment_view_settings, container, false);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);
            RecyclerView recyclerViewOptions = view.FindViewById<RecyclerView>(Resource.Id.recycler_view_options);
            recyclerViewOptions.SetLayoutManager(new LinearLayoutManager(this.RequireContext()));
            recyclerViewOptions.SetAdapter(new ViewSettingsAdapter(this.viewModel.GetItems(), onClickCallback: item => this.OnViewItemClicked(item.Type)));
        }

        protected override bool ShouldShowBackButton() => true;

        protected override string GetTitle() => this.Context.GetString((int)SettingsOverviewType.View);

        public void OnViewItemClicked(ViewSettingsType type)
        {
            switch (type)
            {
                case ViewSettingsType.ScanArea:
                    this.MoveToFragment(ScanAreaSettingsFragment.Create(), true, null);
                    break;
                case ViewSettingsType.PointOfInterest:
                    this.MoveToFragment(PointOfInterestSettingsFragment.Create(), true, null);
                    break;
                case ViewSettingsType.Overlay:
                    this.MoveToFragment(OverlaySettingsFragment.Create(), true, null);
                    break;
                case ViewSettingsType.Viewfinder:
                    this.MoveToFragment(ViewfinderSettingsFragment.Create(), true, null);
                    break;
                case ViewSettingsType.Logo:
                    this.MoveToFragment(LogoSettingsFragment.Create(), true, null);
                    break;
                case ViewSettingsType.Controls:
                    this.MoveToFragment(ControlsSettingsFragment.Create(), true, null);
                    break;
                default:
                    break;
            }
        }
    }
}
