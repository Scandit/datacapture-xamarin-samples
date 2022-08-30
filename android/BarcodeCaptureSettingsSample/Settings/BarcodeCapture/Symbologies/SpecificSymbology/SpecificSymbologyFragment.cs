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
using System.Threading.Tasks;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using BarcodeCaptureSettingsSample.Base;
using BarcodeCaptureSettingsSample.Utils;
using Range = Scandit.DataCapture.Core.Data.Range;

namespace BarcodeCaptureSettingsSample.Settings.BarcodeCapture.Symbologies.SpecificSymbology
{
    public class SpecificSymbologyFragment : NavigationFragment
    {
        private const string KeySymbologyIdentifier = "symbology_identifier";
        private SpecificSymbologyViewModel viewModel;
        private SymbologyAdapter adapter;

        private Switch switchEnabled, switchColorInverted;
        private View containerRange, containerRangeMin, containerRangeMax, containerExtensions;
        private TextView textRangeMin, textRangeMax;
        private RecyclerView recyclerExtensions;

        public static SpecificSymbologyFragment Create(string symbologyIdentifier)
        {
            SpecificSymbologyFragment fragment = new SpecificSymbologyFragment();
            Bundle args = new Bundle();
            args.PutString(KeySymbologyIdentifier, symbologyIdentifier);
            fragment.Arguments = args;
            return fragment;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            this.Arguments.RequireNotNull(nameof(Arguments), "No identifier given to symbology fragment");
            string symbologyIdentifier = this.Arguments
                                             .GetString(KeySymbologyIdentifier, null)
                                             .RequireNotNull(nameof(symbologyIdentifier), "No identifier given to symbology fragment");
            this.viewModel = new SpecificSymbologyViewModel(symbologyIdentifier);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return inflater.Inflate(Resource.Layout.fragment_specific_symbology_settings, container, false);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

            this.switchEnabled = view.FindViewById<Switch>(Resource.Id.switch_enabled);
            this.SetupEnabledSwitch();

            this.switchColorInverted = view.FindViewById<Switch>(Resource.Id.switch_color_inverted);
            this.SetupColorInvertedSwitch();

            this.containerRange = view.FindViewById<View>(Resource.Id.card_range);
            this.containerRangeMin = view.FindViewById<View>(Resource.Id.container_range_min);
            this.containerRangeMax = view.FindViewById<View>(Resource.Id.container_range_max);
            this.textRangeMin = view.FindViewById<TextView>(Resource.Id.text_range_min);
            this.textRangeMax = view.FindViewById<TextView>(Resource.Id.text_range_max);
            this.SetupRange();

            this.containerExtensions = view.FindViewById<View>(Resource.Id.card_extensions);
            this.recyclerExtensions = view.FindViewById<RecyclerView>(Resource.Id.recycler_extensions);
            this.SetupExtensions();
        }

        protected override bool ShouldShowBackButton() => true;

        protected override string GetTitle() => this.viewModel.SymbologyReadableName;

        private void SetupEnabledSwitch()
        {
            this.switchEnabled.Checked = this.viewModel.CurrentSymbologyEnabled;
            this.switchEnabled.CheckedChange += async (object sender, CompoundButton.CheckedChangeEventArgs args) =>
            {
                await this.viewModel.SetCurrentSymbologyEnabledAsync(args.IsChecked);
            };
        }

        private void SetupColorInvertedSwitch()
        {
            if (this.viewModel.ColorInvertedSettingsAvailable)
            {
                this.switchColorInverted.Visibility = ViewStates.Visible;
                this.switchColorInverted.Checked = this.viewModel.CurrentSymbologyColorInverted;
                this.switchColorInverted.CheckedChange += async (object sender, CompoundButton.CheckedChangeEventArgs args) =>
                {
                    await this.viewModel.SetCurrentSymbologyColorInvertedAsync(args.IsChecked);
                };
            }
        }

        private void SetupRange()
        {
            if (this.viewModel.IsRangeSettingsAvailable())
            {
                this.containerRange.Visibility = ViewStates.Visible;
                this.UpdateRangeValues();

                this.containerRangeMin.Click += (object sender, EventArgs args) =>
                {
                    this.BuildAndShowDropdownForMinRange();
                };

                this.containerRangeMax.Click += (object sender, EventArgs args) =>
                {
                    this.BuildAndShowDropdownForMaxRange();
                };
            }
        }

        private void SetupExtensions()
        {
            if (this.viewModel.ExtensionsAvailable)
            {
                this.containerExtensions.Visibility = ViewStates.Visible;
                async Task onClickCallback(string extension)
                {
                    await this.viewModel.ToggleExtensionAsync(extension);
                    this.RefreshSymbologyAdapterData();
                }
                this.adapter = new SymbologyAdapter(this.viewModel.GetItems(), onClickCallback);
                recyclerExtensions.SetLayoutManager(new LinearLayoutManager(this.RequireContext()));
                recyclerExtensions.SetAdapter(this.adapter);
            }
        }

        private void UpdateRangeValues()
        {
            this.textRangeMin.Text = this.viewModel.CurrentMinActiveSymbolCount.ToString();
            this.textRangeMax.Text = this.viewModel.CurrentMaxActiveSymbolCount.ToString();
        }

        private void BuildAndShowDropdownForMinRange()
        {
            using PopupMenu menu = new PopupMenu(this.RequireContext(), this.textRangeMin, GravityFlags.End);
            Range range = this.viewModel.SymbolCountRange;

            // We allow selection from the minimum symbol count allowed by the symbology until the
            // currently selected maximum symbol count.
            int minAllowedSymbolCount = range.Minimum;
            int maxAllowedSymbolCount = this.viewModel.CurrentMaxActiveSymbolCount;
            int step = range.Step;
            
            for (int i = minAllowedSymbolCount; i <= maxAllowedSymbolCount; i += step)
            {
                menu.Menu.Add(0, i, i, "" + i);
            }

            menu.MenuItemClick += async (object sender, PopupMenu.MenuItemClickEventArgs args) => 
            {
                int selectedSymbolCount = args.Item.ItemId;
                await this.viewModel.SetCurrentMinActiveSymbolCountAsync(selectedSymbolCount);
                this.UpdateRangeValues();
            };
            
            menu.Show();
        }

        private void BuildAndShowDropdownForMaxRange()
        {
            using PopupMenu menu = new PopupMenu(this.RequireContext(), this.textRangeMin, GravityFlags.End);
            Range range = this.viewModel.SymbolCountRange;

            // We allow selection from the currently selected minimum symbol count until the maximum
            // allowed by the symbology.
            int minAllowedSymbolCount = this.viewModel.CurrentMinActiveSymbolCount;
            int maxAllowedSymbolCount = range.Maximum;
            int step = range.Step;
            
            for (int i = minAllowedSymbolCount; i <= maxAllowedSymbolCount; i += step)
            {
                menu.Menu.Add(0, i, i, "" + i);
            }

            menu.MenuItemClick += async (object setnder, PopupMenu.MenuItemClickEventArgs args) =>
            {
                int selectedSymbolCount = args.Item.ItemId;
                await this.viewModel.SetCurrentMaxActiveSymbolCountAsync(selectedSymbolCount);
                this.UpdateRangeValues();
            };
            
            menu.Show();
        }

        private void RefreshSymbologyAdapterData()
        {
            this.adapter.UpdateData(this.viewModel.GetItems());
        }
    }
}