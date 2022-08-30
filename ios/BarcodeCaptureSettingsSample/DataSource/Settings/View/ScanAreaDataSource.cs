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

using BarcodeCaptureSettingsSample.DataSource.Other;
using BarcodeCaptureSettingsSample.DataSource.Other.Rows;
using BarcodeCaptureSettingsSample.Extensions;
using BarcodeCaptureSettingsSample.Model;

namespace BarcodeCaptureSettingsSample.DataSource.Settings.View
{
    public class ScanAreaDataSource : IDataSource
    {
        public ScanAreaDataSource(IDataSourceListener dataSourceListener)
        {
            this.DataSourceListener = dataSourceListener;

            this.Sections = new[]
            {
                new Section(new []
                {
                    FloatWithUnitRow.Create(
                        "Top",
                        () => SettingsManager.Instance.ScanAreaMargins.Top,
                        value =>
                        {
                            SettingsManager.Instance.ScanAreaMargins =
                                SettingsManager.Instance.ScanAreaMargins.NewWithTop(value);
                        },
                        this.DataSourceListener
                    ),
                    FloatWithUnitRow.Create(
                        "Right",
                        () => SettingsManager.Instance.ScanAreaMargins.Right,
                        value =>
                        {
                            SettingsManager.Instance.ScanAreaMargins = SettingsManager.Instance.ScanAreaMargins.NewWithRight(value);
                        },
                        this.DataSourceListener
                    ),
                    FloatWithUnitRow.Create(
                        "Bottom",
                        () => SettingsManager.Instance.ScanAreaMargins.Bottom,
                        value =>
                        {
                            SettingsManager.Instance.ScanAreaMargins = SettingsManager.Instance.ScanAreaMargins.NewWithBottom(value);
                        },
                        this.DataSourceListener
                    ),
                    FloatWithUnitRow.Create(
                        "Left",
                        () => SettingsManager.Instance.ScanAreaMargins.Left,
                        value =>
                        {
                            SettingsManager.Instance.ScanAreaMargins = SettingsManager.Instance.ScanAreaMargins.NewWithLeft(value);
                        },
                        this.DataSourceListener
                    )
                }, "Margins"),
                new Section(new Row[]
                {
                    SwitchRow.Create(
                        "Should Show Scan Area Guides",
                        () => SettingsManager.Instance.ShouldShowScanAreaGuides,
                        value => SettingsManager.Instance.ShouldShowScanAreaGuides = value
                    )
                })
            };
        }

        public IDataSourceListener DataSourceListener { get; }

        public Section[] Sections { get; }
    }
}
