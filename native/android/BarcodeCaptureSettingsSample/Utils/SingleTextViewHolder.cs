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
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;

namespace BarcodeCaptureSettingsSample.Utils
{
    public class SingleTextViewHolder : RecyclerView.ViewHolder
    {
        private readonly TextView textView;

        public SingleTextViewHolder(View itemView, int resourceId) : base(itemView)
        {
            this.textView = itemView.FindViewById<TextView>(resourceId);
        }

        public event EventHandler Click
        {
            add { this.ItemView.Click += value; }
            remove { this.ItemView.Click -= value; }
        }

        public void SetFirstTextView(int textResourceId)
        {
            this.textView?.SetText(textResourceId);
        }

        public void SetFirstTextView(string text)
        {
            this.textView?.SetText(text, TextView.BufferType.Normal);
        }
    }
}