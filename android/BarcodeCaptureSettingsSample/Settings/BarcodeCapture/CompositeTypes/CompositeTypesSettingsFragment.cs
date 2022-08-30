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
using BarcodeCaptureSettingsSample.Settings.BarcodeCapture.CompositeTypes.Type;

namespace BarcodeCaptureSettingsSample.Settings.BarcodeCapture.CompositeTypes
{
    public class CompositeTypesSettingsFragment : NavigationFragment
    {
        private CompositeTypesSettingsViewModel viewModel;
        private CompositeTypesAdapter adapter;

        public static CompositeTypesSettingsFragment Create()
        {
            return new CompositeTypesSettingsFragment();
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            this.viewModel = ViewModelProviders.Of(this)
                                               .Get(Java.Lang.Class.FromType(typeof(CompositeTypesSettingsViewModel))) as CompositeTypesSettingsViewModel;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return inflater.Inflate(Resource.Layout.fragment_composite_types_settings, container, false);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);
            RecyclerView recyclerCompositeTypes = view.FindViewById<RecyclerView>(Resource.Id.recycler_composite_types);

            this.adapter = new CompositeTypesAdapter(this.viewModel.GetCompositeTypeEntries(), onClickCallback: async (CompositeTypeItem item) =>
            {
                await this.viewModel.ToggleCompositeType(item);
                this.RefreshCompositeTypesAdapterData();
            });

            recyclerCompositeTypes.SetLayoutManager(new LinearLayoutManager(this.RequireContext()));
            recyclerCompositeTypes.SetAdapter(adapter);
        }

        private void RefreshCompositeTypesAdapterData()
        {
            this.adapter.UpdateData(this.viewModel.GetCompositeTypeEntries());
        }

        protected override bool ShouldShowBackButton() => true;

        protected override string GetTitle() => this.Context.GetString((int)BarcodeCaptureSettingsType.CompositeTypes);
    }
}
