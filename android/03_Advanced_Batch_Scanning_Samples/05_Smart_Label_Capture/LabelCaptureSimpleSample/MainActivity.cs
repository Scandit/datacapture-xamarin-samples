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

#nullable enable

using Android.App;
using Android.OS;
using LabelCaptureSimpleSample.ViewModels;
using LabelCaptureSimpleSample.Views.Fragments;
using Android.Content.PM;

namespace LabelCaptureSimpleSample
{
    [Activity(MainLauncher = true, ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : CameraPermissionActivity
    {
        private readonly ScanViewModel scanViewModel = new ScanViewModel();

        // Enter your Scandit License key here.
        // Your Scandit License key is available via your Scandit SDK web account.
        public static string SCANDIT_LICENSE_KEY = "-- ENTER YOUR SCANDIT LICENSE KEY HERE --";

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            this.SetContentView(Resource.Layout.activity_main);

            if (savedInstanceState == null)
            {
                this.SupportFragmentManager
                    .BeginTransaction()
                    .Replace(Resource.Id.container, ScanFragment.Create(this.scanViewModel))
                    .AddToBackStack(null)
                    .Commit();
            }
        }

        protected override void OnResume()
        {
            base.OnResume();

            // Check for camera permission and request it, if it hasn't yet been granted.
            // Once we have the permission the onCameraPermissionGranted() method will be called.
            this.RequestCameraPermission();
        }

        public override void OnBackPressed()
        {
            if (this.SupportFragmentManager.BackStackEntryCount <= 1)
            {
                this.Finish();
            }
            else
            {
                base.OnBackPressed();
            }
        }

        protected override void OnCameraPermissionGranted()
        {
            this.scanViewModel.HasCameraPermission = true;
        }
    }
}
