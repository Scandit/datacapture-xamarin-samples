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
using Scandit.DataCapture.Core.Additions;

namespace BarcodeCaptureSettingsSample.Settings.Camera
{
    public class CameraSettingsPositionAdapter : RecyclerView.Adapter
    {
        private readonly Func<CameraSettingsPositionItem, Task> onClickCallback;
        private IList<CameraSettingsPositionItem> cameraPositions;

        public CameraSettingsPositionAdapter(
                IList<CameraSettingsPositionItem> cameraPositions,
                Func<CameraSettingsPositionItem, Task> onClickCallback)
        {
            this.cameraPositions = cameraPositions.RequireNotNull(nameof(cameraPositions));
            this.onClickCallback = onClickCallback;
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            LayoutInflater inflater = LayoutInflater.From(parent.Context);
            return new TwoTextsAndIconViewHolder(inflater.Inflate(Resource.Layout.two_texts_and_icon, parent, false), Resource.Id.text_field, Resource.Id.text_field_2);
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            CameraSettingsPositionItem currentPositionEntry = cameraPositions[position];

            var viewHolder = holder as TwoTextsAndIconViewHolder;
            viewHolder.SetFirstTextView(currentPositionEntry.CameraPosition.Name());
            viewHolder.SetIcon(currentPositionEntry.Enabled ? Resource.Drawable.ic_check : 0);
            viewHolder.Click += async (object sender, EventArgs args) =>
            {
                await this.onClickCallback?.Invoke(currentPositionEntry);
            };
        }

        public override int ItemCount => this.cameraPositions.Count;

        public void UpdateData(IList<CameraSettingsPositionItem> cameraPositions)
        {
            var diff = new TypedDiffUtilCallback<CameraSettingsPositionItem>(this.cameraPositions.ToArray(), cameraPositions.ToArray());
            DiffUtil.DiffResult result = DiffUtil.CalculateDiff(diff);
            this.cameraPositions = cameraPositions;
            result.DispatchUpdatesTo(this);
        }
    }
}