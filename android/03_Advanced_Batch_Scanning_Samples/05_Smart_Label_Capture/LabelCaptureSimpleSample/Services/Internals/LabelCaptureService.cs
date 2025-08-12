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

using System;
using Android.Content;
using Java.Util;
using LabelCaptureSimpleSample.Services.Internals.Listeners;
using Scandit.DataCapture.Barcode.Data;
using Scandit.DataCapture.Core.Capture;
using Scandit.DataCapture.Label.Capture;
using Scandit.DataCapture.Label.Data;
using Scandit.DataCapture.Label.UI.Overlay;
using Scandit.DataCapture.Label.UI.Overlay.Validation;

namespace LabelCaptureSimpleSample.Services.Internals
{
    internal class LabelCaptureService : ILabelCaptureService
    {
        public const string FIELD_BARCODE = "barcode";
        public const string FIELD_UNIT_PRICE = "unit_price";
        public const string FIELD_WEIGHT = "weight";
        public const string FIELD_EXPIRY_DATE = "expiry_date";
        public const string LABEL_WEIGHT_PRICE = "weighted_item";

        private LabelCapture? labelCapture;

        public void Initialize(DataCaptureContext dataCaptureContext)
        {
            var labelCaptureSettings = this.BuildLabelCaptureSettings();

            this.labelCapture = LabelCapture.Create(dataCaptureContext, labelCaptureSettings);
        }

        public LabelCaptureBasicOverlay BuildOverlay(Context context)
        {
            var overlay = LabelCaptureBasicOverlay.Create(this.labelCapture, view: null);
            overlay.Listener = new LabelCaptureBasicOverlayListener(context);

            return overlay;
        }

        public LabelCaptureValidationFlowOverlay BuildValidationFlowOverlay(Context context, Action<string> onLabelScanned)
        {
            // Customize the validation flow by changing labels.
            var settings = LabelCaptureValidationFlowSettings.Create();

            var overlay = LabelCaptureValidationFlowOverlay.Create(context, this.labelCapture, null);
            overlay.Listener = new LabelCaptureValidationFlowListener(onLabelScanned);
            overlay.ApplySettings(settings);

            return overlay;
        }

        public void Disable()
        {
            if (this.labelCapture != null)
            {
                this.labelCapture.Enabled = false;
            }
        }

        public void Enable()
        {
            if (this.labelCapture != null)
            {
                this.labelCapture.Enabled = true;
            }
        }

        private LabelCaptureSettings BuildLabelCaptureSettings()
        {
            // Demonstrates LabelCaptureSettings by configuring a label 
            // containing barcode, date, price, and weight fields.
            var settingsBuilder = LabelCaptureSettings.Builder();
            var labelBuilder = settingsBuilder.LabelDefinitionBuilder;

            labelBuilder.AddCustomBarcode(
                new CustomBarcode.CustomBarcodeBuilder
                {
                    Optional = false,
                    Symbologies = new ArrayList(new Symbology[]
                    {
                        Symbology.Ean13Upca,
                        Symbology.Gs1DatabarExpanded,
                        Symbology.Code128
                    })
                }.Build(FIELD_BARCODE));

            ExpiryDateText expiryDateText = new ExpiryDateText.ExpiryDateTextBuilder()
            {
                Optional = true
            }.Build(FIELD_EXPIRY_DATE);
            expiryDateText.LabelDateFormat =
                new LabelDateFormat(LabelDateComponentFormat.Mdy, acceptPartialDates: false);

            labelBuilder.AddExpiryDateText(expiryDateText);

            labelBuilder.AddUnitPriceText(
                new UnitPriceText.UnitPriceTextBuilder
                {
                    Optional = true
                }.Build(FIELD_UNIT_PRICE));

            labelBuilder.AddWeightText(
                new WeightText.WeightTextBuilder
                {
                    Optional = true
                }.Build(FIELD_WEIGHT));

            settingsBuilder.AddLabel(labelBuilder.Build(LABEL_WEIGHT_PRICE));

            var settings = settingsBuilder.Build();
            return settings;
        }
    }
}
