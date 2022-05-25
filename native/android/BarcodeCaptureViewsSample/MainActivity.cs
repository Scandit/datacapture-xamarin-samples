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
using Android.App;
using Android.OS;
using AndroidX.AppCompat.App;
using AndroidX.AppCompat.Widget;
using BarcodeCaptureViewsSample.Modes.Activity;
using BarcodeCaptureViewsSample.Modes.Fragment;
using BarcodeCaptureViewsSample.Modes.SplitView;
using Scandit.DataCapture.Core.Capture;
using TextView = Android.Widget.TextView;

namespace BarcodeCaptureViewsSample
{
    [Activity(Label = "@string/app_name", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            this.SetContentView(Resource.Layout.activity_main);

            Toolbar toolbar = this.FindViewById<Toolbar>(Resource.Id.toolbar);
            this.SetSupportActionBar(toolbar);
            this.SetTitle(Resource.String.app_title);

            TextView textSdkVersion = this.FindViewById<TextView>(Resource.Id.text_sdk_version);
            string sdkFormat = this.GetString(Resource.String.sdk_version);
            textSdkVersion.Text = string.Format(sdkFormat, DataCaptureVersion.Version);

            TextView textFullscreenFragment = this.FindViewById<TextView>(Resource.Id.text_fullscreen);
            TextView textSplitView = this.FindViewById<TextView>(Resource.Id.text_split_view);
            TextView textPickerActivity = this.FindViewById<TextView>(Resource.Id.text_picker_activity);

            textFullscreenFragment.Click += (object sender, EventArgs e) =>
            {
                this.StartActivity(FullscreenScanFragmentContainerActivity.GetIntent(this));
            };

            textSplitView.Click += (object sender, EventArgs e) =>
            {
                this.StartActivity(SplitViewScanActivity.GetIntent(this));
            };

            textPickerActivity.Click += (object sender, EventArgs e) =>
            {
                this.StartActivity(FullscreenScanActivity.GetIntent(this));
            };
        }
    }
}
