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
using Android.Content;
using Android.OS;
using AndroidX.AppCompat.App;
using AndroidX.Fragment.App;

namespace IdCaptureExtendedSample.Scan
{
    public class AlertDialogFragment : DialogFragment
    {
        private static readonly string KeyTitleRes = "title_res";
        private static readonly string KeyMessage = "message";

        private Action clickHandler;

        public static AlertDialogFragment Create(int title, string message, Action completion = null)
        {
            Bundle arguments = new Bundle();
            arguments.PutInt(KeyTitleRes, title);
            arguments.PutString(KeyMessage, message);

            AlertDialogFragment fragment = new AlertDialogFragment
            {
                Arguments = arguments,
                clickHandler = completion
            };

            return fragment;
        }

        public override Android.App.Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            int titleRes = this.Arguments.GetInt(KeyTitleRes);
            string message = this.Arguments.GetString(KeyMessage);

            return new AlertDialog.Builder(this.RequireContext())
                                  .SetTitle(titleRes)
                                  .SetMessage(message)
                                  .SetPositiveButton(Resource.String.result_button_resume, (object sender, DialogClickEventArgs args) =>
                                  {
                                      this.clickHandler?.Invoke();
                                  })
                                  .Create();
        }
    }
}
