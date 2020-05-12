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
using BarcodeCaptureSettingsSample.Utils;

namespace BarcodeCaptureSettingsSample.Settings.Views.PointOfInterests
{
    public class PointOfInterestSettingsFragment : NavigationFragment
    {
        private PointOfInterestSettingsViewModel viewModel;

        private View containerX, containerY;
        private TextView textX, textY;

        public static PointOfInterestSettingsFragment Create()
        {
            return new PointOfInterestSettingsFragment();
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            this.viewModel = ViewModelProviders.Of(this)
                                               .Get(Java.Lang.Class.FromType(typeof(PointOfInterestSettingsViewModel))) as PointOfInterestSettingsViewModel;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return inflater.Inflate(Resource.Layout.fragment_point_of_interest_settings, container, false);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);
            this.containerX = view.FindViewById<View>(Resource.Id.container_x);
            this.textX = view.FindViewById<TextView>(Resource.Id.text_x);
            this.RefreshXData();
            this.SetupX();

            this.containerY = view.FindViewById<View>(Resource.Id.container_y);
            this.textY = view.FindViewById<TextView>(Resource.Id.text_y);
            this.RefreshYData();
            this.SetupY();
        }

        protected override bool ShouldShowBackButton() => true;

        protected override string GetTitle() => this.Context.GetString((int)ViewSettingsType.PointOfInterest);

        private void SetupX()
        {
            this.containerX.Click += (object sender, EventArgs args) =>
            {
                this.MoveToFragment(PointOfInterestXFragment.Create(), true, null);
            };
        }

        private void SetupY()
        {
            this.containerY.Click += (object sender, EventArgs args) =>
            {
                this.MoveToFragment(PointOfInterestYFragment.Create(), true, null);
            };
        }

        private void RefreshXData()
        {
            this.textX.Text = this.viewModel.PointOfInterestX.GetStringWithUnit(this.Context);
        }

        private void RefreshYData()
        {
            this.textY.Text = this.viewModel.PointOfInterestY.GetStringWithUnit(this.Context);
        }
    }
}