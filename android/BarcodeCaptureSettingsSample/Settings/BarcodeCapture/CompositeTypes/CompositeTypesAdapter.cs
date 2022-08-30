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
using BarcodeCaptureSettingsSample.Settings.BarcodeCapture.CompositeTypes.Type;
using BarcodeCaptureSettingsSample.Utils;

namespace BarcodeCaptureSettingsSample.Settings.BarcodeCapture.CompositeTypes
{
    public class CompositeTypesAdapter : RecyclerView.Adapter
    {
        private List<CompositeTypeItem> types;
        private readonly Action<CompositeTypeItem> onClickCallback;

        public CompositeTypesAdapter(IList<CompositeTypeItem> types, Action<CompositeTypeItem> onClickCallback)
        {
            this.types = types.ToList();
            this.onClickCallback = onClickCallback;
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            LayoutInflater inflater = LayoutInflater.From(parent.Context);
            return new TwoTextsAndIconViewHolder(inflater.Inflate(Resource.Layout.two_texts_and_icon, parent, false), Resource.Id.text_field, Resource.Id.text_field_2);
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            if (this.types.Count <= position)
            {
                return;
            }

            CompositeTypeItem currentEntry = this.types[position];

            var viewHolder = holder as TwoTextsAndIconViewHolder;
            viewHolder.SetFirstTextView(currentEntry.DisplayNameResourceId);
            viewHolder.SetIcon(currentEntry.Enabled ? Resource.Drawable.ic_check : 0);
            viewHolder.ItemView.SetOnClickListener(new ItemViewOnClickListener(this.onClickCallback, currentEntry));
        }

        public override int ItemCount => this.types.Count;

        public void UpdateData(IList<CompositeTypeItem> types)
        {
            DiffUtil.DiffResult result = DiffUtil.CalculateDiff(
                    new TypedDiffUtilCallback<CompositeTypeItem>(this.types.ToArray(), types.ToArray()));
            this.types = types.ToList();
            result.DispatchUpdatesTo(this);
        }

        private class ItemViewOnClickListener : Java.Lang.Object, View.IOnClickListener
        {
            private readonly Action<CompositeTypeItem> onClickCallback;
            private readonly CompositeTypeItem currentItem;

            public ItemViewOnClickListener(Action<CompositeTypeItem> onClickCallback, CompositeTypeItem typeItem)
            {
                this.onClickCallback = onClickCallback;
                this.currentItem = typeItem;
            }

            public void OnClick(View v)
            {
                this.onClickCallback?.Invoke(this.currentItem);
            }
        }
    }
}
