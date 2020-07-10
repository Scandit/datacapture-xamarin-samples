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
using System.Threading.Tasks;
using Android.Views;
using AndroidX.RecyclerView.Widget;
using BarcodeCaptureSettingsSample.Utils;

namespace BarcodeCaptureSettingsSample.Base.MeasureUnits
{
    public class MeasureUnitAdapter : RecyclerView.Adapter
    {
        private readonly Func<MeasureUnitItem, Task> onClickCallback;
        private MeasureUnitItem[] measureUnits;

        public MeasureUnitAdapter(MeasureUnitItem[] measureUnits, Func<MeasureUnitItem, Task> onClickCallback)
        {
            this.measureUnits = measureUnits.RequireNotNull(nameof(measureUnits));
            this.onClickCallback = onClickCallback;
        }
        
        public override int ItemCount => this.measureUnits.Length;

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            LayoutInflater inflater = LayoutInflater.From(parent.Context);

            return new MeasureUnitViewHolder(inflater.Inflate(Resource.Layout.two_texts_and_icon, parent, false));
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            holder.RequireCondition((RecyclerView.ViewHolder vh) => vh is MeasureUnitViewHolder, nameof(holder), "Runtime error");

            if (this.measureUnits.Length <= position)
            {
                return;
            }

            var viewHolder = holder as MeasureUnitViewHolder;

            MeasureUnitItem currentEntry = this.measureUnits[position];
            viewHolder.Bind(currentEntry);
            viewHolder.Click += async (object sender, EventArgs args) => await this.onClickCallback?.Invoke(currentEntry);
        }

        public void UpdateData(MeasureUnitItem[] measureUnits)
        {
            if (measureUnits == null)
            {
                return;
            }

            DiffUtil.DiffResult result = DiffUtil.CalculateDiff(new TypedDiffUtilCallback<MeasureUnitItem>(this.measureUnits, measureUnits));
            this.measureUnits = measureUnits;
            result.DispatchUpdatesTo(this);
        }
    }
}