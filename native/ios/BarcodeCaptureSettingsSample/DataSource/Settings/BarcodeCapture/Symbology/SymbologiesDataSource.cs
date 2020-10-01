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

using System.Linq;
using BarcodeCaptureSettingsSample.DataSource.Other;
using BarcodeCaptureSettingsSample.DataSource.Other.Rows;
using BarcodeCaptureSettingsSample.Extensions;
using BarcodeCaptureSettingsSample.Model;

namespace BarcodeCaptureSettingsSample.DataSource.Settings.BarcodeCapture.Symbology
{
    public class SymbologiesDataSource : IDataSource
    {
        public SymbologiesDataSource(IDataSourceListener dataSourceListener)
        {
            this.DataSourceListener = dataSourceListener;
            this.Sections = new[]
            {
                new Section(new[]
                {
                    ActionRow.Create(
                        "Enable All",
                        tuple =>
                        {
                            SettingsManager.Instance.EnableAllSymbologies();
                            this.DataSourceListener.OnDataChange();
                        }
                    ),
                    ActionRow.Create(
                        "Disable All",
                        tuple =>
                        {
                            SettingsManager.Instance.DisableAllSymbologies();
                            this.DataSourceListener.OnDataChange();
                        }
                    )
                }),
                new Section(SymbologyExtensions.AllValues.Select(symbology =>
                    {
                        return SymbologyRow.Create(
                            () => SettingsManager.Instance.GetSymbologySettings(symbology),
                            _ => SettingsManager.Instance.SymbologySettingsChanged(),
                            this.DataSourceListener
                        );
                    }).ToArray())
            };
        }

        public IDataSourceListener DataSourceListener { get; }

        public Section[] Sections { get; }
    }
}
