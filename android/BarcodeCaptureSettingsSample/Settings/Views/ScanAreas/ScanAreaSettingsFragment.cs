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
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.Lifecycle;
using BarcodeCaptureSettingsSample.Base;
using BarcodeCaptureSettingsSample.Settings.Views.ScanAreas.Bottom;
using BarcodeCaptureSettingsSample.Settings.Views.ScanAreas.Left;
using BarcodeCaptureSettingsSample.Settings.Views.ScanAreas.Right;
using BarcodeCaptureSettingsSample.Settings.Views.ScanAreas.Top;
using BarcodeCaptureSettingsSample.Utils;

namespace BarcodeCaptureSettingsSample.Settings.Views.ScanAreas
{
    public class ScanAreaSettingsFragment : NavigationFragment
    {
        private ScanAreaSettingsViewModel viewModel;
        private View containerTopMargin, containerRightMargin, containerBottomMargin, containerLeftMargin;
        private TextView textTopMargin, textRightMargin, textBottomMargin, textLeftMargin;
        private Switch switchShowGuides;

        public static ScanAreaSettingsFragment Create()
        {
            return new ScanAreaSettingsFragment();
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            this.viewModel = ViewModelProviders.Of(this)
                                               .Get(Java.Lang.Class.FromType(typeof(ScanAreaSettingsViewModel))) as ScanAreaSettingsViewModel;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return inflater.Inflate(Resource.Layout.fragment_scan_area_settings, container, false);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);
            this.switchShowGuides = view.FindViewById<Switch>(Resource.Id.switch_enable_guides);
            this.RefreshSwitchGuidesData();
            this.SetupSwitchGuides();

            this.containerTopMargin = view.FindViewById<View>(Resource.Id.container_top);
            this.textTopMargin = view.FindViewById<TextView>(Resource.Id.text_top_margin);
            this.RefreshTextTopData();
            this.SetupTopMarginData();

            this.containerRightMargin = view.FindViewById<View>(Resource.Id.container_right);
            this.textRightMargin = view.FindViewById<TextView>(Resource.Id.text_right_margin);
            this.RefreshTextRightData();
            this.SetupRightMarginData();

            this.containerBottomMargin = view.FindViewById<View>(Resource.Id.container_bottom);
            this.textBottomMargin = view.FindViewById<TextView>(Resource.Id.text_bottom_margin);
            this.RefreshTextBottomData();
            this.SetupBottomMarginData();

            this.containerLeftMargin = view.FindViewById<View>(Resource.Id.container_left);
            this.textLeftMargin = view.FindViewById<TextView>(Resource.Id.text_left_margin);
            this.RefreshTextLeftData();
            this.SetupLeftMarginData();
        }

        private void SetupSwitchGuides()
        {
            this.switchShowGuides.CheckedChange += (object sender, CompoundButton.CheckedChangeEventArgs args) =>
            {
                this.viewModel.ShowGuides = args.IsChecked;
            };
        }

        private void SetupTopMarginData()
        {
            this.containerTopMargin.Click += (object sender, EventArgs args) =>
            {
                this.MoveToFragment(ScanAreaTopMarginFragment.Create(), true, null);
            };
        }

        private void SetupRightMarginData()
        {
            this.containerRightMargin.Click += (object sender, EventArgs args) =>
            {
                this.MoveToFragment(ScanAreaRightMarginFragment.Create(), true, null);
            };
        }

        private void SetupBottomMarginData()
        {
            this.containerBottomMargin.Click += (object sender, EventArgs args) =>
            {
                this.MoveToFragment(ScanAreaBottomMarginFragment.Create(), true, null);
            };
        }

        private void SetupLeftMarginData()
        {
            this.containerLeftMargin.Click += (object sender, EventArgs args) =>
            {
                this.MoveToFragment(ScanAreaLeftMarginFragment.Create(), true, null);
            };
        }

        private void RefreshSwitchGuidesData()
        {
            this.switchShowGuides.Checked = this.viewModel.ShowGuides;
        }

        private void RefreshTextTopData()
        {
            this.textTopMargin.Text = this.viewModel.TopMargin.GetStringWithUnit(this.Context);
        }

        private void RefreshTextRightData()
        {
            this.textRightMargin.Text = this.viewModel.RightMargin.GetStringWithUnit(this.Context);
        }

        private void RefreshTextBottomData()
        {
            this.textBottomMargin.Text = this.viewModel.BottomMargin.GetStringWithUnit(this.Context);
        }

        private void RefreshTextLeftData()
        {
            this.textLeftMargin.Text = this.viewModel.LeftMargin.GetStringWithUnit(this.Context);
        }

        protected override bool ShouldShowBackButton() => true;

        protected override string GetTitle() => this.Context.GetString((int)ViewSettingsType.ScanArea);
    }
}