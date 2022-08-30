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
using BarcodeCaptureSettingsSample.DataSource.Other;
using BarcodeCaptureSettingsSample.DataSource.Other.Rows;
using BarcodeCaptureSettingsSample.Model;
using Scandit.DataCapture.Barcode.Data;

namespace BarcodeCaptureSettingsSample.DataSource.Settings.BarcodeCapture
{
    public class CompositeTypesDataSource : IDataSource
    {
        public IDataSourceListener DataSourceListener { get; }

        public Section[] Sections { get; }

        public CompositeTypesDataSource(IDataSourceListener dataSourceListener)
        {
            this.DataSourceListener = dataSourceListener;

            List<Row> options = new List<Row>();
            foreach (CompositeType type in Enum.GetValues(typeof(CompositeType)))
            {
                options.Add(BoolOptionRow.Create(
                    title: type.ToString(),
                    getter: () => SettingsManager.Instance.EnabledCompositeTypes.HasFlag(type),
                    setter: selected =>
                    {
                        CompositeType compositeTypes = SettingsManager.Instance.EnabledCompositeTypes;
                        if (selected)
                        {
                            compositeTypes |= type;
                        }
                        else
                        {
                            compositeTypes &= ~type;
                        }
                        SettingsManager.Instance.EnabledCompositeTypes = compositeTypes;
                    },
                    this.DataSourceListener));
            }

            this.Sections = new[] { new Section(options.ToArray(), "Type") };
        }
    }
}
