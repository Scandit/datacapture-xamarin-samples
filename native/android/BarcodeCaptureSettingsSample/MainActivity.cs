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
using Android.Content;
using Android.OS;
using Android.Views;
using AndroidX.AppCompat.App;
using AndroidX.AppCompat.Widget;
using BarcodeCaptureSettingsSample.Scanning;
using BarcodeCaptureSettingsSample.Settings;
using FragmentTransaction = AndroidX.Fragment.App.FragmentTransaction;

namespace BarcodeCaptureSettingsSample
{
    [Activity(MainLauncher = true, Label = "@string/app_name")]
    public class MainActivity : AppCompatActivity
    {
        private const string BackstackTagScanner = "scanner";

        private class OnDoubleTapGestureDetector : GestureDetector, View.IOnTouchListener
        {
            private class GestureListener : SimpleOnGestureListener
            {
                private readonly Action onDoubleTap;

                public GestureListener(Action onDoubleTap)
                {
                    this.onDoubleTap = onDoubleTap;
                }

                public override bool OnDoubleTap(MotionEvent e)
                {
                    this.onDoubleTap?.Invoke();
                    return true;
                }
            }

            public OnDoubleTapGestureDetector(Context context, Action onDoubleTap) :
                base(context, new GestureListener(onDoubleTap))
            { }

            public bool OnTouch(View view, MotionEvent ev)
            {
                return base.OnTouchEvent(ev);
            }
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            this.SetContentView(Resource.Layout.activity_main);

            if (savedInstanceState == null)
            {
                this.SupportFragmentManager.BeginTransaction()
                                           .Replace(Resource.Id.fragment_container, BarcodeScanFragment.Create())
                                           .Commit();
            }

            Toolbar toolbar = this.FindViewById<Toolbar>(Resource.Id.toolbar);
            this.SetSupportActionBar(toolbar);
            toolbar.SetOnTouchListener(new OnDoubleTapGestureDetector(this, onDoubleTap: () =>
            {
                this.PopToScanner();
            }));
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.action_settings:
                    this.GoToSettings();
                    return true;
                case Android.Resource.Id.Home:
                    this.OnBackPressed();
                    return true;
                default:
                    return base.OnOptionsItemSelected(item);
            }
        }

        private void PopToScanner()
        {
            this.SupportFragmentManager
                .PopBackStack(BackstackTagScanner, (int)PopBackStackFlags.Inclusive);
        }

        private void GoToSettings()
        {
            this.SupportFragmentManager.BeginTransaction()
                                       .Replace(Resource.Id.fragment_container, SettingsOverviewFragment.Create())
                                       .SetTransition(FragmentTransaction.TransitFragmentOpen)
                                       .AddToBackStack(BackstackTagScanner)
                                       .Commit();
        }
    }
}