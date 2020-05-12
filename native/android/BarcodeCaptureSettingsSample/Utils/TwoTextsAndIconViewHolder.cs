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

using Android.Graphics.Drawables;
using Android.Views;
using Android.Widget;

namespace BarcodeCaptureSettingsSample.Utils
{
    public class TwoTextsAndIconViewHolder : SingleTextViewHolder
    {
        private readonly TextView secondTextView;

        public TwoTextsAndIconViewHolder(View itemView, int resourceId, int secondTextResourceId) : base(itemView, resourceId)
        {
            this.secondTextView = itemView.FindViewById<TextView>(secondTextResourceId);
        }

        public void SetSecondTextViewText(string secondText)
        {
            this.secondTextView?.SetText(secondText, TextView.BufferType.Normal);
        }

        public void SetSecondTextViewText(int secondTextRes)
        {
            this.secondTextView?.SetText(secondTextRes);
        }

        public void SetIcon(Drawable icon)
        {
            this.secondTextView?.SetCompoundDrawablesWithIntrinsicBounds(null, null, icon, null);
        }

        public void SetIcon(int iconRes)
        {
            this.secondTextView?.SetCompoundDrawablesWithIntrinsicBounds(0, 0, iconRes, 0);
        }
    }
}