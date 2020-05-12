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
using System.Collections.Generic;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.Lifecycle;
using BarcodeCaptureSettingsSample.Base;
using BarcodeCaptureSettingsSample.Settings.Views.Logo.OffsetX;
using BarcodeCaptureSettingsSample.Settings.Views.Logo.OffsetY;
using BarcodeCaptureSettingsSample.Utils;
using Scandit.DataCapture.Core.Common.Geometry;

namespace BarcodeCaptureSettingsSample.Settings.Views.Logo
{
    public class LogoSettingsFragment : NavigationFragment
    {
        private LogoSettingsViewModel viewModel;

        private View containerAnchor, containerOffsetX, containerOffsetY;
        private TextView textAnchor, textOffsetX, textOffsetY;

        public static LogoSettingsFragment Create()
        {
            return new LogoSettingsFragment();
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            this.viewModel = ViewModelProviders.Of(this)
                                               .Get(Java.Lang.Class.FromType(typeof(LogoSettingsViewModel))) as LogoSettingsViewModel;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return inflater.Inflate(Resource.Layout.fragment_logo_settings, container, false);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);
            this.containerAnchor = view.FindViewById<View>(Resource.Id.container_anchor);
            this.containerOffsetX = view.FindViewById<View>(Resource.Id.container_offset_x);
            this.containerOffsetY = view.FindViewById<View>(Resource.Id.container_offset_y);

            this.textAnchor = view.FindViewById<TextView>(Resource.Id.text_anchor);
            this.textOffsetX = view.FindViewById<TextView>(Resource.Id.text_offset_x);
            this.textOffsetY = view.FindViewById<TextView>(Resource.Id.text_offset_y);

            this.SetupAnchor();
            this.RefreshAnchorData();
            this.SetupOffsetX();
            this.RefreshOffsetXData();
            this.SetupOffsetY();
            this.RefreshOffsetYData();
        }

        protected override bool ShouldShowBackButton() => true;

        protected override string GetTitle() => this.Context.GetString((int)ViewSettingsType.Logo);

        private void SetupAnchor()
        {
            this.containerAnchor.Click += (object sender, EventArgs args) =>
            {
                this.BuildAndShowAnchorMenu();
            };
        }

        private void SetupOffsetX()
        {
            this.containerOffsetX.Click += (object sender, EventArgs args) =>
            {
                this.MoveToFragment(OffsetXFragment.Create(), true, null);
            };
        }

        private void SetupOffsetY()
        {
            this.containerOffsetY.Click += (object sender, EventArgs args) =>
            {
                this.MoveToFragment(OffsetYFragment.Create(), true, null);
            };
        }

        private void RefreshAnchorData()
        {
            this.textAnchor.Text = this.viewModel.CurrentAnchor.Name();
        }

        private void RefreshOffsetXData()
        {
            this.textOffsetX.Text = this.viewModel.CurrentAnchorXOffset.GetStringWithUnit(this.Context);
        }

        private void RefreshOffsetYData()
        {
            this.textOffsetY.Text = this.viewModel.CurrentAnchorYOffset.GetStringWithUnit(this.Context);
        }

        private void BuildAndShowAnchorMenu()
        {
            using PopupMenu menu = new PopupMenu(this.RequireContext(), this.containerAnchor, GravityFlags.End);

            IList<Anchor> anchors = LogoSettingsViewModel.GetItems();

            for (int i = 0; i < anchors.Count; i++)
            {
                Anchor anchor = anchors[i];
                menu.Menu.Add(0, i, i, anchor.ToString());
            }

            menu.MenuItemClick += (object sender, PopupMenu.MenuItemClickEventArgs args) =>
            {
                int selectedAnchor = args.Item.ItemId;
                this.viewModel.CurrentAnchor = anchors[selectedAnchor];
                this.RefreshAnchorData();
            };

            menu.Show();
        }
    }
}