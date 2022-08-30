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

using Android.App;
using Android.Content;
using Android.OS;
using AndroidX.AppCompat.App;

namespace BarcodeCaptureViewsSample.Modes.Fragment
{
    [Activity(Label = "@string/full_screen_fragment", Theme = "@style/Theme.AppCompat.Light", ParentActivity = typeof(MainActivity))]
    public class FullscreenScanFragmentContainerActivity : AppCompatActivity
    {
        public static Intent GetIntent(Context context)
        {
            return new Intent(context, typeof(FullscreenScanFragmentContainerActivity));
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            this.SetContentView(Resource.Layout.fragment_container_activity);
            this.SupportActionBar.SetDisplayHomeAsUpEnabled(true);

            if (savedInstanceState == null)
            {
                this.SupportFragmentManager.BeginTransaction()
                                           .Replace(Resource.Id.fragment_container, FullscreenScanFragment.Create(), null)
                                           .Commit();
            }
        }
    }
}
