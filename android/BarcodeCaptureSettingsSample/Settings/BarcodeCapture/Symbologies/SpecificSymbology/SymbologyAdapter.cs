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
using System.Threading.Tasks;
using Android.Views;
using AndroidX.RecyclerView.Widget;
using BarcodeCaptureSettingsSample.Base;
using BarcodeCaptureSettingsSample.Utils;

namespace BarcodeCaptureSettingsSample.Settings.BarcodeCapture.Symbologies.SpecificSymbology
{ 
    public class SymbologyAdapter : RecyclerView.Adapter
    {
        private readonly Func<string, Task> onClickCallback;
        private IList<SymbologyItem> symbologyItems;

        private class SymbologyAdapterViewHolder : TwoTextsAndIconViewHolder
        {
            public SymbologyAdapterViewHolder(View itemView) : base (itemView, Resource.Id.text_field, Resource.Id.text_field_2)
            { }

            public void Bind(SymbologyItem item)
            {
                item.RequireNotNull(nameof(item));

                this.SetFirstTextView(item.Name);
                this.SetIcon(item.Enabled ? Resource.Drawable.ic_check : 0);
            }
        }

        public SymbologyAdapter(IList<SymbologyItem> symbologyItems, Func<string, Task> onClickCallback)
        {
            this.symbologyItems = symbologyItems.RequireNotNull(nameof(symbologyItems));
            this.onClickCallback = onClickCallback;
        }

        public override int ItemCount => symbologyItems.Count;

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            LayoutInflater inflater = LayoutInflater.From(parent.Context);
            return new SymbologyAdapterViewHolder(inflater.Inflate(Resource.Layout.two_texts_and_icon, parent, false));
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            if (this.symbologyItems.Count <= position)
            {
                return;
            }

            SymbologyItem currentItem = this.symbologyItems[position];

            var viewHolder = holder as SymbologyAdapterViewHolder;
            viewHolder.Bind(currentItem);
            viewHolder.Click += async (object sender, EventArgs args) => await this.onClickCallback?.Invoke(currentItem.Name);
        }

        public void UpdateData(IList<SymbologyItem> items)
        {
            if (items == null)
            {
                return;
            }

            DiffUtil.DiffResult result = DiffUtil.CalculateDiff(
                new TypedDiffUtilCallback<SymbologyItem>(this.symbologyItems.ToArray(), items.ToArray()));
            this.symbologyItems = items;
            result.DispatchUpdatesTo(this);
        }
    }
}