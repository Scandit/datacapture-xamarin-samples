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

namespace BarcodeCaptureSettingsSample.Settings.Views
{
    public class ViewSettingsAdapter : RecyclerView.Adapter
    {
        private readonly IList<ViewSettingsItem> items;
        private readonly Action<ViewSettingsItem> onClickCallback;

        public ViewSettingsAdapter(IList<ViewSettingsItem> items, Action<ViewSettingsItem> onClickCallback)
        {
            this.items = items;
            this.onClickCallback = onClickCallback;
        }

        public override int ItemCount => items.Count;

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            LayoutInflater inflater = LayoutInflater.From(parent.Context);
            return new TwoTextsAndIconViewHolder(
                inflater.Inflate(Resource.Layout.two_texts_and_icon, parent, false), 
                Resource.Id.text_field, 
                Resource.Id.text_field_2);
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            if (this.items.Count <= position)
            {
                return;
            }

            ViewSettingsItem currentEntry = this.items[position];

            var viewHolder = holder as TwoTextsAndIconViewHolder;

            viewHolder.SetFirstTextView(currentEntry.DisplayNameResourceId);
            viewHolder.Click += (object sender, EventArgs args) => this.onClickCallback?.Invoke(currentEntry);
        }
    }
}
