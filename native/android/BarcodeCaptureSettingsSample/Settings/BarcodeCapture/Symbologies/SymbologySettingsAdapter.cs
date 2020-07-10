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
using Scandit.DataCapture.Barcode.Data;

namespace BarcodeCaptureSettingsSample.Settings.BarcodeCapture.Symbologies
{
    public class SymbologySettingsAdapter : RecyclerView.Adapter
    {
        private readonly Action<SymbologyDescription> onClickCallback;
        private IList<SymbologySettingsItem> symbologyDescriptions;

        public SymbologySettingsAdapter(IList<SymbologySettingsItem> symbologyDescriptions, Action<SymbologyDescription> onClickCallback)
        {
            this.symbologyDescriptions = symbologyDescriptions.RequireNotNull(nameof(symbologyDescriptions));
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
            SymbologySettingsItem currentItem = this.symbologyDescriptions[position];

            var viewHolder = holder as TwoTextsAndIconViewHolder;
            viewHolder.SetFirstTextView(currentItem.SymbologyDescription.ReadableName);
            viewHolder.SetSecondTextViewText(currentItem.Enabled ? "On" : "Off");

            viewHolder.ItemView.Click += (object sender, EventArgs args) =>
            {
                this.onClickCallback?.Invoke(currentItem.SymbologyDescription);
            };
        }

        public override int ItemCount => this.symbologyDescriptions.Count;

        public void UpdateData(IList<SymbologySettingsItem> symbologyDescriptions)
        {
            DiffUtil.DiffResult result = DiffUtil.CalculateDiff(
                    new TypedDiffUtilCallback<SymbologySettingsItem>(this.symbologyDescriptions.ToArray(), symbologyDescriptions.ToArray()));
            this.symbologyDescriptions = symbologyDescriptions;
            result.DispatchUpdatesTo(this);
        }
    }
}
