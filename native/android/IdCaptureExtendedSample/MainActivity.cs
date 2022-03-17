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
using Android.Views;
using AndroidX.AppCompat.App;
using IdCaptureExtendedSample.Result;
using IdCaptureExtendedSample.Scan;
using Scandit.DataCapture.ID.Data;

namespace IdCaptureExtendedSample
{
    [Activity(Label = "@string/app_name", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        private const string KeyResultAlert = "RESULT_ALERT";

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            this.SetContentView(Resource.Layout.activity_main);

            if (savedInstanceState == null)
            {
                this.SupportFragmentManager
                    .BeginTransaction()
                    .Replace(Resource.Id.scan_fragment_container, new ScanFragment())
                    .Commit();
            }
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item.ItemId == Android.Resource.Id.Home)
            {
                this.OnBackPressed();
                return true;
            }

            return base.OnOptionsItemSelected(item);
        }

        public void ShowAlert(int titleRes, string message, Action completion = null)
        {
            if (this.SupportFragmentManager.FindFragmentByTag(KeyResultAlert) == null)
            {
                AlertDialogFragment.Create(titleRes, message, completion)
                                   .Show(this.SupportFragmentManager, KeyResultAlert);
            }
        }

        public void GoToResultScreen(CapturedId capturedId)
        {
            this.SupportFragmentManager.BeginTransaction()
                                        .Replace(Resource.Id.scan_fragment_container,
                                                 ResultFragment.Create(capturedId))
                                        .AddToBackStack(null)
                                        .Commit();
        }
    }
}
