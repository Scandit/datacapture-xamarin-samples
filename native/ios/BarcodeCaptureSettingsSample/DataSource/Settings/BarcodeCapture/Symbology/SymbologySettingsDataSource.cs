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

using System.Collections.Generic;
using System.Linq;
using BarcodeCaptureSettingsSample.DataSource.Other;
using BarcodeCaptureSettingsSample.DataSource.Other.Rows;
using BarcodeCaptureSettingsSample.Extensions;
using Scandit.DataCapture.Barcode.Capture;
using Scandit.DataCapture.Barcode.Data;

namespace BarcodeCaptureSettingsSample.DataSource.Settings.BarcodeCapture.Symbology
{
    public class SymbologySettingsDataSource : IDataSource
    {
        private readonly SymbologySettings symbologySettings;

        private readonly SymbologyDescription symbologyDescription;

        public SymbologySettingsDataSource(IDataSourceListener dataSourceListener, SymbologySettings symbologySettings)
        {
            this.DataSourceListener = dataSourceListener;
            this.symbologySettings = symbologySettings;
            this.symbologyDescription = SymbologyDescription.Create(symbologySettings.Symbology);
        }

        private Section CreateActiveSymbolCountRangeSection()
        {
            int currentMin;
            int currentMax;
            if (!this.symbologySettings.ActiveSymbolCounts.Any())
            {
                currentMin = (int)this.symbologyDescription.ActiveSymbolCountRange.Minimum;
            }
            else
            {
                currentMin = this.symbologySettings.ActiveSymbolCounts.Min();
            }
            if (!this.symbologySettings.ActiveSymbolCounts.Any())
            {
                currentMax = (int)this.symbologyDescription.ActiveSymbolCountRange.Maximum;
            }
            else
            {
                currentMax = this.symbologySettings.ActiveSymbolCounts.Max();
            }

            var minimumRange = Enumerable.Range((int) this.symbologyDescription.ActiveSymbolCountRange.Minimum,
                currentMax - (int) this.symbologyDescription.ActiveSymbolCountRange.Minimum);
            var maximumRange = Enumerable.Range(currentMin,
                (int) this.symbologyDescription.ActiveSymbolCountRange.Maximum - currentMin);
            return new Section(new[]
            {
                ChoiceRow<SymbolCount>.Create(
                    "Minimum",
                    minimumRange.Select(val => new SymbolCount(val)).ToArray(),
                    () => new SymbolCount((int)currentMin),
                    value =>
                    {
                        var newSet = this.symbologySettings.ActiveSymbolCounts.NewSetWithMinimum((short) value.Id);
                        this.symbologySettings.ActiveSymbolCounts = newSet;
                    },
                    this.DataSourceListener
                ),
                ChoiceRow<SymbolCount>.Create(
                    "Maximum",
                    maximumRange.Select(val => new SymbolCount(val)).ToArray(),
                    () => new SymbolCount(currentMax),
                    value =>
                    {
                        var newSet = this.symbologySettings.ActiveSymbolCounts.NewSetWithMaximum((short) value.Id);
                        this.symbologySettings.ActiveSymbolCounts = newSet;
                    },
                    this.DataSourceListener
                )
            }, "Range");
        }

        private Section CreateGeneralSection()
        {
            var enabled = SwitchRow.Create(
                "Enabled",
                () => this.symbologySettings.Enabled,
                enabled => this.symbologySettings.Enabled = enabled
            ) ;
            var rows = new List<Row>() { enabled };
            if (this.symbologyDescription.ColorInvertible)
            {
                rows.Add(SwitchRow.Create(
                    "Color Inverted",
                    () => this.symbologySettings.ColorInvertedEnabled,
                    enabled => this.symbologySettings.ColorInvertedEnabled = enabled
                ));
            }

            return new Section(rows.ToArray());
        }

        public IDataSourceListener DataSourceListener { get; }

        public Section[] Sections
        {
            get
            {
                var sections = new List<Section>() { this.CreateGeneralSection() };

                if (!symbologyDescription.ActiveSymbolCountRange.Fixed)
                {
                    sections.Add(this.CreateActiveSymbolCountRangeSection());
                }

                if (symbologyDescription.SupportedExtensions.Count > 0)
                {
                    var extensionRows = this.symbologyDescription.SupportedExtensions.Select(extension =>
                    {
                        return BoolOptionRow.Create(
                            extension,
                            () => this.symbologySettings.IsExtensionEnabled(extension),
                            enabled => this.symbologySettings.SetExtensionEnabled(extension, enabled),
                            this.DataSourceListener
                        );
                    }).ToArray();
                    sections.Add(new Section(extensionRows, "Extensions"));
                }

                return sections.ToArray();
            }
        }
    }
}
