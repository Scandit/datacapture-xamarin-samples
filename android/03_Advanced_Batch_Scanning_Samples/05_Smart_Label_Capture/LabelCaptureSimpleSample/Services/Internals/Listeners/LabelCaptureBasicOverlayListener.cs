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

#nullable enable

using Android.Content;
using Android.Graphics;
using Scandit.DataCapture.Core.UI.Style;
using Scandit.DataCapture.Label.Data;
using Scandit.DataCapture.Label.UI.Overlay;

namespace LabelCaptureSimpleSample.Services.Internals.Listeners
{
    internal class LabelCaptureBasicOverlayListener : Java.Lang.Object, ILabelCaptureBasicOverlayListener
    {
        private readonly Brush upcBrush;
        private readonly Brush expiryDateBrush;
        private readonly Brush weightBrush;
        private readonly Brush unitPriceBrush;
        private readonly Brush transparentBrush;

        public LabelCaptureBasicOverlayListener(Context context)
        {
            this.upcBrush = new Brush(
                fillColor: context.GetColor(Resource.Color.upc),
                strokeColor: context.GetColor(Resource.Color.upc),
                strokeWidth: 1f);

            this.expiryDateBrush = new Brush(
                fillColor: context.GetColor(Resource.Color.expiryDate),
                strokeColor: context.GetColor(Resource.Color.expiryDate),
                strokeWidth: 1f);
            this.weightBrush = new Brush(
                fillColor: context.GetColor(Resource.Color.weight),
                strokeColor: context.GetColor(Resource.Color.weight),
                strokeWidth: 1f);

            this.unitPriceBrush = new Brush(
                fillColor: context.GetColor(Resource.Color.unitPrice),
                strokeColor: context.GetColor(Resource.Color.unitPrice),
                strokeWidth: 1f);

            this.transparentBrush = new Brush(
                fillColor: Color.Transparent,
                strokeColor: Color.Transparent,
                strokeWidth: 0f);
        }

        public Brush BrushForField(LabelCaptureBasicOverlay overlay, LabelField field, CapturedLabel label)
        {
            return field.Name switch
            {
                LabelCaptureService.FIELD_BARCODE => this.upcBrush,
                LabelCaptureService.FIELD_EXPIRY_DATE => this.expiryDateBrush,
                LabelCaptureService.FIELD_UNIT_PRICE => this.unitPriceBrush,
                LabelCaptureService.FIELD_WEIGHT => this.weightBrush,
                _ => null!,
            };
        }

        public Brush BrushForLabel(LabelCaptureBasicOverlay overlay, CapturedLabel label)
        {
            return this.transparentBrush;
        }

        public void OnLabelTapped(LabelCaptureBasicOverlay overlay, CapturedLabel label)
        {}
    }
}
