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

using System;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.Fragment.App;

namespace LabelCaptureSimpleSample.Views.Fragments
{
    public class LabelCapturedFragment : DialogFragment
    {
        public const string TAG = nameof(LabelCapturedFragment);

        private readonly string? message;
        private TextView messageView = null!;
        private Button continueButton = null!;

        public event EventHandler? OnDismissed;

        private LabelCapturedFragment(string message)
        {
            this.message = message;
        }

        public static LabelCapturedFragment Create(string message)
        {
            return new LabelCapturedFragment(message);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup? container, Bundle? savedInstanceState)
        {
            View? view = inflater.Inflate(Resource.Layout.fragment_label_captured, container);

            if (view == null)
            {
                throw new ArgumentNullException(
                    nameof(view), $"Cannot inflate the layout {nameof(Resource.Layout.fragment_label_captured)}");
            }

            this.messageView = view.FindViewById<TextView>(Resource.Id.message) ??
                throw new ArgumentNullException(nameof(Resource.Id.message));

            this.continueButton = view.FindViewById<Button>(Resource.Id.continue_button) ??
                throw new ArgumentNullException(nameof(Resource.Id.confirm_button));

            return view;
        }

        public override void OnViewCreated(View view, Bundle? savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

            this.messageView.Text = this.message;
            this.continueButton.Click += (object sender, EventArgs args) => this.Dismiss();
        }

        public override void OnStart()
        {
            base.OnStart();

            if (this.Dialog?.Window != null)
            {
                var window = this.Dialog.Window;

                if (window.Attributes != null)
                {
                    window.Attributes.Width = ViewGroup.LayoutParams.MatchParent;
                }

                window.SetGravity(GravityFlags.Center);
                window.SetBackgroundDrawable(new ColorDrawable(Color.Transparent));
            }
        }

        public override void OnDismiss(IDialogInterface dialog)
        {
            base.OnDismiss(dialog);
            this.OnDismissed?.Invoke(this, EventArgs.Empty);
        }
    }
}
