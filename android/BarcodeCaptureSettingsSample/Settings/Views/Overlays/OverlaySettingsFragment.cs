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
using AndroidX.RecyclerView.Widget;
using BarcodeCaptureSettingsSample.Base;

namespace BarcodeCaptureSettingsSample.Settings.Views.Overlays
{
    public class OverlaySettingsFragment : NavigationFragment
    {
        private OverlaySettingsViewModel viewModel;
        private View containerBrush;
        private TextView textBrush;
        private RecyclerView recyclerOverlayStyles;
        private OverlayStyleAdapter adapter;

        public static OverlaySettingsFragment Create()
        {
            return new OverlaySettingsFragment();
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            this.viewModel = ViewModelProviders.Of(this)
                                               .Get(Java.Lang.Class.FromType(typeof(OverlaySettingsViewModel))) as OverlaySettingsViewModel;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return inflater.Inflate(Resource.Layout.fragment_overlay_settings, container, false);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);
            this.textBrush = view.FindViewById<TextView>(Resource.Id.text_brush);
            this.containerBrush = view.FindViewById<View>(Resource.Id.container_brush);
            this.SetupBrush();
            this.RefreshBrushData();

            this.recyclerOverlayStyles = view.FindViewById<RecyclerView>(Resource.Id.recycler_overlay_styles);
            this.SetupRecyclerStyles();
        }

        protected override bool ShouldShowBackButton() => true;

        protected override string GetTitle() => this.Context.GetString(Resource.String.overlay);

        private void SetupBrush()
        {
            this.containerBrush.Click += (object sender, EventArgs args) =>
            {
                this.BuildAndShowBrushMenu();
            };
        }

        private void RefreshBrushData()
        {
            this.textBrush.Text = this.Context.GetString(this.viewModel.CurrentBrush.DisplayNameResourceId);
        }

        private void BuildAndShowBrushMenu()
        {
            using PopupMenu popup = new PopupMenu(this.RequireContext(), this.containerBrush, GravityFlags.End);

            for (int i = 0; i < this.viewModel.AvailableBrushes.Count; i++)
            {
                OverlaySettingsBrush brush = this.viewModel.AvailableBrushes[i];
                popup.Menu.Add(0, i, i, brush.DisplayNameResourceId);
            }

            popup.MenuItemClick += (object sender, PopupMenu.MenuItemClickEventArgs args) =>
            {
                OverlaySettingsBrush selectedBrush = this.viewModel.AvailableBrushes[args.Item.ItemId];
                this.viewModel.CurrentBrush = selectedBrush;
                this.RefreshBrushData();
            };

            popup.Show();
        }

        private void SetupRecyclerStyles()
        {
            this.adapter = new OverlayStyleAdapter(viewModel.Entries, (entry) =>
            {
                this.viewModel.CurrentStyle = entry;
                this.RefreshStyleData();
                this.RefreshBrushData();
            });
            this.recyclerOverlayStyles.SetAdapter(this.adapter);
        }

        private void RefreshStyleData()
        {
            this.adapter.UpdateData(viewModel.Entries);
        }
    }
}
