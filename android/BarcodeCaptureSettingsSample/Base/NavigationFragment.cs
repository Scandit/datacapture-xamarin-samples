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
using Android.Util;
using Android.Views;
using Android.Views.InputMethods;
using AndroidX.AppCompat.App;
using AndroidX.Fragment.App;

namespace BarcodeCaptureSettingsSample.Base
{
    public abstract class NavigationFragment : Fragment
    {
        public override void OnResume()
        {
            base.OnResume();
            this.SetupActionBar();
        }

        private void SetupActionBar()
        {
            AppCompatActivity activity = (AppCompatActivity) this.RequireActivity();

            activity.SupportActionBar.SetDisplayHomeAsUpEnabled(this.ShouldShowBackButton());
            activity.SupportActionBar.Title = this.GetTitle();
        }

        protected void MoveToFragment(Fragment fragment, bool addToBackStack, string tag)
        {
            FragmentTransaction transaction = this.RequireActivity()
                                                  .SupportFragmentManager
                                                  .BeginTransaction()
                                                  .Replace(Resource.Id.fragment_container, fragment)
                                                  .SetTransition(FragmentTransaction.TransitFragmentOpen);

            if (addToBackStack)
            {
                transaction.AddToBackStack(tag);
            }

            transaction.Commit();
        }

        protected void DismissKeyboard(View focusedView)
        {
            try
            {
                InputMethodManager imm = (InputMethodManager) this.RequireContext()
                                                                  .GetSystemService(Android.Content.Context.InputMethodService);
                imm.HideSoftInputFromWindow(focusedView.WindowToken, 0);
            }
            catch (Exception)
            {
                Log.Debug(this.GetType().Name, "Error closing the keyboard");
            }
        }

        protected abstract bool ShouldShowBackButton();

        protected abstract string GetTitle();
    }
}
