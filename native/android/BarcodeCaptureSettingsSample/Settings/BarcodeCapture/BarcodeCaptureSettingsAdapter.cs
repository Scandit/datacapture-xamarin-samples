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
using System.Collections.Generic;
using Android.Views;
using AndroidX.RecyclerView.Widget;
using BarcodeCaptureSettingsSample.Utils;

namespace BarcodeCaptureSettingsSample.Settings.BarcodeCapture
{
    public class BarcodeCaptureSettingsAdapter : RecyclerView.Adapter
    {
        private readonly Action<BarcodeCaptureSettingsItem> onClickCallback;
        private readonly IList<BarcodeCaptureSettingsItem> items;

        public override int ItemCount => this.items.Count;

        public BarcodeCaptureSettingsAdapter(IList<BarcodeCaptureSettingsItem> items, Action<BarcodeCaptureSettingsItem> onClickCallback)
        {
            this.items = items;
            this.onClickCallback = onClickCallback;
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            LayoutInflater inflater = LayoutInflater.From(parent.Context);
            return new SingleTextViewHolder(inflater.Inflate(Resource.Layout.single_text_layout, parent, false), Resource.Id.text_field);
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            if (this.items.Count <= position)
            {
                return;
            }

            BarcodeCaptureSettingsItem currentEntry = this.items[position];

            var viewHolder = holder as SingleTextViewHolder;
            viewHolder.SetFirstTextView(currentEntry.DisplayNameResourceId);
            viewHolder.Click += (object sender, EventArgs args) => this.onClickCallback?.Invoke(currentEntry);
        }
    }
}
