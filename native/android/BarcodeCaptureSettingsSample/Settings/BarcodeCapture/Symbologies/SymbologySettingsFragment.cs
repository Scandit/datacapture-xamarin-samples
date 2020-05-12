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
using Android.OS;
using Android.Views;
using AndroidX.Lifecycle;
using AndroidX.RecyclerView.Widget;
using BarcodeCaptureSettingsSample.Base;
using BarcodeCaptureSettingsSample.Settings.BarcodeCapture.Symbologies.SpecificSymbology;
using Scandit.DataCapture.Barcode.Data;

namespace BarcodeCaptureSettingsSample.Settings.BarcodeCapture.Symbologies
{
    public class SymbologySettingsFragment : NavigationFragment
    {
        private SymbologySettingsViewModel viewModel;
        private SymbologySettingsAdapter adapter;

        public static SymbologySettingsFragment Create()
        {
            return new SymbologySettingsFragment();
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            this.viewModel = ViewModelProviders.Of(this)
                                               .Get(Java.Lang.Class.FromType(typeof(SymbologySettingsViewModel))) as SymbologySettingsViewModel;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return inflater.Inflate(Resource.Layout.fragment_symbology_settings, container, false);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

            view.FindViewById<View>(Resource.Id.button_enable_all).Click += async (object sender, EventArgs args) =>
            {
                await this.viewModel.EnableAllSymbologyAsync(true);
                this.RefreshSymbologyAdapterData();
            };

            view.FindViewById<View>(Resource.Id.button_disable_all).Click += async (object sender, EventArgs args) =>
            {
                await this.viewModel.EnableAllSymbologyAsync(false);
                this.RefreshSymbologyAdapterData();
            };

            this.adapter = new SymbologySettingsAdapter(this.viewModel.GetItems().ToList(), onClickCallback: (SymbologyDescription symbology) =>
            {
                this.MoveToFragment(SpecificSymbologyFragment.Create(symbology.Identifier), true, null);
            });

            RecyclerView recyclerSymbologies = view.FindViewById<RecyclerView>(Resource.Id.recycler_symbologies);
            recyclerSymbologies.SetLayoutManager(new LinearLayoutManager(this.RequireContext()));
            recyclerSymbologies.SetAdapter(this.adapter);
        }

        protected override bool ShouldShowBackButton() => true;

        protected override string GetTitle() => this.Context.GetString((int)BarcodeCaptureSettingsType.Symbologies);

        private void RefreshSymbologyAdapterData()
        {
            this.adapter.UpdateData(this.viewModel.GetItems().ToList());
        }
    }
}