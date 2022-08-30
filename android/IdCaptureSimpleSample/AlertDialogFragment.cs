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

namespace IdCaptureSimpleSample
{
    /*
     * A fragment that displays an AlertDialog with the recognized document.
     */
    public class AlertDialogFragment : DialogFragment
    {
        private static readonly string KeyTitleRes = "title_res";
        private static readonly string KeyMessage = "message";

        private ICallbacks callbacks;
    
        public override void OnAttach(Context context)
        {
            base.OnAttach(context);

            if (this.Host is ICallbacks callbacks)
            {
                this.callbacks = callbacks;
            }
            else
            {
                throw new InvalidCastException("Parent fragment doesn't implement Callbacks!");
            }
        }

        public static AlertDialogFragment Create(int title, string message)
        {
            Bundle arguments = new Bundle();
            arguments.PutInt(KeyTitleRes, title);
            arguments.PutString(KeyMessage, message);

            AlertDialogFragment fragment = new AlertDialogFragment
            {
                Arguments = arguments
            };

            return fragment;
        }

        public override Android.App.Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            int titleRes = this.Arguments.GetInt(KeyTitleRes);
            string message = this.Arguments.GetString(KeyMessage);

            IDialogInterfaceOnClickListener onClickListener = null;

            return new AlertDialog.Builder(this.RequireContext())
                                  .SetTitle(titleRes)
                                  .SetMessage(message)
                                  .SetPositiveButton(Resource.String.result_button_resume, onClickListener)
                                  .Create();
        }

        public override void OnDismiss(IDialogInterface dialog)
        {
            base.OnDismiss(dialog);

            this.callbacks.OnResultDismissed();
        }

        public interface ICallbacks
        {
            void OnResultDismissed();
        }
    }
}
