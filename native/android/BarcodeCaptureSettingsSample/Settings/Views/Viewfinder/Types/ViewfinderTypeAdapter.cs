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

namespace BarcodeCaptureSettingsSample.Settings.Views.Viewfinder.Types
{
    public class ViewfinderTypeAdapter : RecyclerView.Adapter
    {
        private readonly Action<ViewfinderType> onClickCallback;
        private IList<ViewfinderType> types;

        public ViewfinderTypeAdapter(ViewfinderType[] types, Action<ViewfinderType> onClickCallback)
        {
            this.types = types;
            this.onClickCallback = onClickCallback;
        }

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
            ViewfinderType currentType = types[position];

            var viewHolder = holder as TwoTextsAndIconViewHolder;
            viewHolder.SetFirstTextView(currentType.DisplayNameResourceId);
            viewHolder.SetIcon(currentType.Enabled ? Resource.Drawable.ic_check : 0);
            viewHolder.ItemView.Click += (object senver, EventArgs args) =>
            {
                this.onClickCallback?.Invoke(currentType);
            };
        }

        public override int ItemCount => this.types.Count;

        public void UpdateData(IList<ViewfinderType> types)
        {
            DiffUtil.DiffResult result = DiffUtil.CalculateDiff(
                    new TypedDiffUtilCallback<ViewfinderType>(this.types.ToArray(), types.ToArray())
            );
            this.types = types;
            result.DispatchUpdatesTo(this);
        }
    }
}
