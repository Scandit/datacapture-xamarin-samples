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
using BarcodeCaptureSettingsSample.Settings.BarcodeCapture.CompositeTypes;
using BarcodeCaptureSettingsSample.Settings.BarcodeCapture.Feedback;
using BarcodeCaptureSettingsSample.Settings.BarcodeCapture.Location;
using BarcodeCaptureSettingsSample.Settings.BarcodeCapture.Symbologies;

namespace BarcodeCaptureSettingsSample.Settings.BarcodeCapture
{
    public class BarcodeCaptureSettingsFragment : NavigationFragment
    {
        private BarcodeCaptureSettingsViewModel viewModel;

        public static BarcodeCaptureSettingsFragment Create()
        {
            return new BarcodeCaptureSettingsFragment();
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            this.viewModel = ViewModelProviders.Of(this)
                                               .Get(Java.Lang.Class.FromType(typeof(BarcodeCaptureSettingsViewModel))) as BarcodeCaptureSettingsViewModel;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return inflater.Inflate(Resource.Layout.fragment_barcode_capture_settings, container, false);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);
            RecyclerView recyclerOptions = view.FindViewById<RecyclerView>(Resource.Id.recycler_barcode_capture_settings);
            recyclerOptions.SetLayoutManager(new LinearLayoutManager(this.RequireContext()));
            recyclerOptions.SetAdapter(new BarcodeCaptureSettingsAdapter(this.viewModel.GetItems(), onClickCallback: (BarcodeCaptureSettingsItem item) =>
            {
                this.MoveToDeeperSettings(item);
            }));
        }

        protected override bool ShouldShowBackButton() => true;

        protected override string GetTitle() => this.Context.GetString((int)SettingsOverviewType.BarcodeCaputre);

        private void MoveToDeeperSettings(BarcodeCaptureSettingsItem item)
        {
            switch (item.Type) 
            {
                case BarcodeCaptureSettingsType.Symbologies:
                    this.MoveToFragment(SymbologySettingsFragment.Create(), true, null);
                    break;
                case BarcodeCaptureSettingsType.LocationSelection:
                    this.MoveToFragment(LocationSettingsFragment.Create(), true, null);
                    break;
                case BarcodeCaptureSettingsType.Feedback:
                    this.MoveToFragment(FeedbackSettingsFragment.Create(), true, null);
                    break;
                case BarcodeCaptureSettingsType.CodeDuplicateFilter:
                    this.MoveToFragment(CodeDuplicateFilterSettingsFragment.Create(), true, null);
                    break;
                case BarcodeCaptureSettingsType.CompositeTypes:
                    this.MoveToFragment(CompositeTypesSettingsFragment.Create(), true, null);
                    break;
            }
        }
    }
}
