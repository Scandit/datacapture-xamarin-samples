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
using BarcodeCaptureSettingsSample.Settings.BarcodeCapture.Location.LocationTypes;
using BarcodeCaptureSettingsSample.Utils;

namespace BarcodeCaptureSettingsSample.Settings.BarcodeCapture.Location
{
    public class LocationTypeAdapter : RecyclerView.Adapter
    {
        private readonly Func<LocationType, Task> onClickCallback;
        private IList<LocationType> locationTypes;

        public LocationTypeAdapter(IList<LocationType> locationTypes, Func<LocationType, Task> onClickCallback)
        {
            this.locationTypes = locationTypes;
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
            LocationType currentEntry = locationTypes[position];

            var viewHolder = holder as TwoTextsAndIconViewHolder;

            viewHolder.SetFirstTextView(currentEntry.DisplayNameResourceId);
            viewHolder.SetIcon(currentEntry.Enabled ? Resource.Drawable.ic_check : 0);
            viewHolder.ItemView.Click += async (object sender, EventArgs args) =>
            {
                await this.onClickCallback?.Invoke(currentEntry);
            };
        }

        public override int ItemCount => this.locationTypes.Count;

        public void UpdateData(IList<LocationType> locationTypes)
        {
            DiffUtil.DiffResult result = DiffUtil.CalculateDiff(
                    new TypedDiffUtilCallback<LocationType>(this.locationTypes.ToArray(), locationTypes.ToArray()));
            this.locationTypes = locationTypes;
            result.DispatchUpdatesTo(this);
        }
    }
}
