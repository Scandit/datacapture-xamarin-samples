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
using System.Linq;
using Android.Views;
using AndroidX.RecyclerView.Widget;
using BarcodeCaptureSettingsSample.Base;
using BarcodeCaptureSettingsSample.Utils;

namespace BarcodeCaptureSettingsSample.Settings.Views.Overlays
{
    public class OverlayStyleAdapter : RecyclerView.Adapter
    {
        private IList<OverlayStyleEntry> entries;
        private readonly Action<OverlayStyleEntry> callback;

        public OverlayStyleAdapter(OverlayStyleEntry[] entries, Action<OverlayStyleEntry> callback)
        {
            this.entries = entries;
            this.callback = callback;
        }

        public override int ItemCount => this.entries.Count;

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var entry = this.entries[position];
            var viewHolder = holder as TwoTextsAndIconViewHolder;
            string name = entry.style.Name();
            viewHolder.SetFirstTextView($"{name.Substring(0, 1).ToUpper()}{name.Substring(1).ToLower()}");
            viewHolder.SetIcon(entry.enabled ? Resource.Drawable.ic_check : 0);
            viewHolder.ItemView.Click += (_, __) => callback.Invoke(entry);
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var inflater = LayoutInflater.From(parent.Context);
            return new TwoTextsAndIconViewHolder(
                inflater.Inflate(Resource.Layout.two_texts_and_icon, parent, false),
                Resource.Id.text_field,
                Resource.Id.text_field_2
            );
        }

        public void UpdateData(OverlayStyleEntry[] entries)
        {
            var callback = new TypedDiffUtilCallback<OverlayStyleEntry>(this.entries.ToArray(), entries);
            var diffResult = DiffUtil.CalculateDiff(callback);
            this.entries = entries;
            diffResult.DispatchUpdatesTo(this);
        }
    }
}
