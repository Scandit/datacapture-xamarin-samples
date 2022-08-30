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
using System.Threading.Tasks;
using Android.OS;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using AndroidX.Lifecycle;
using BarcodeCaptureSettingsSample.Base;

namespace BarcodeCaptureSettingsSample.Settings.BarcodeCapture.Feedback
{
    public class CodeDuplicateFilterSettingsFragment : NavigationFragment
    {
        private CodeDuplicateFilterSettingsViewModel viewModel;
        private EditText codeDuplicateFilter;

        public static CodeDuplicateFilterSettingsFragment Create()
        {
            return new CodeDuplicateFilterSettingsFragment();
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            this.viewModel = ViewModelProviders.Of(this).Get(Java.Lang.Class.FromType(typeof(CodeDuplicateFilterSettingsViewModel))) as CodeDuplicateFilterSettingsViewModel;
        }

        public override View OnCreateView(
                LayoutInflater inflater,
                ViewGroup container,
                Bundle savedInstanceState)
        {
            return inflater.Inflate(Resource.Layout.fragment_code_duplicate_filter_settings, container, false);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);
            this.codeDuplicateFilter = view.FindViewById<EditText>(Resource.Id.edit_code_duplicate_filter);
            this.RefreshCodeDuplicateFilter();
            this.SetupEditTextCodeDuplicateFilter();
        }

        protected override bool ShouldShowBackButton() => true;

        protected override string GetTitle() => this.Context.GetString((int) BarcodeCaptureSettingsType.CodeDuplicateFilter);

        private void SetupEditTextCodeDuplicateFilter()
        {
            this.codeDuplicateFilter.EditorAction += (object sender, TextView.EditorActionEventArgs args) =>
            {
                if (args.ActionId == ImeAction.Done)
                {
                    this.ApplyDuplicateFilterChangeAsync(this.codeDuplicateFilter.Text);
                    this.DismissKeyboard(this.codeDuplicateFilter);
                    this.codeDuplicateFilter.ClearFocus();
                }
            };
        }

        private void RefreshCodeDuplicateFilter()
        {
            string textFormat = this.Context.GetString(Resource.String.time_millis);
            this.codeDuplicateFilter.Text = string.Format(textFormat, this.viewModel.CodeDuplicateFilter.TotalMilliseconds);
        }

        private void ApplyDuplicateFilterChangeAsync(string text)
        {
            if (float.TryParse(text, out float result))
            {
                this.viewModel.CodeDuplicateFilter = TimeSpan.FromMilliseconds(result);
                this.RefreshCodeDuplicateFilter();
            }
            else 
            {
                this.ShowInvalidNumberToast();
            }
        }

        private void ShowInvalidNumberToast()
        {
            Toast.MakeText(this.RequireContext(), Resource.String.number_not_valid, ToastLength.Long).Show();
        }
    }
}
