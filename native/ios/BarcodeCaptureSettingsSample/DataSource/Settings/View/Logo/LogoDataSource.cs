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

namespace BarcodeCaptureSettingsSample.DataSource.Settings.View.Logo
{
    public class LogoDataSource : IDataSource
    {
        public LogoDataSource(IDataSourceListener dataSourceListener)
        {
            this.DataSourceListener = dataSourceListener;
            this.Sections = new[]
                {
                    new Section(new []
                    {
                        ChoiceRow<NamedAnchor>.Create(
                            "Anchor",
                            Enumeration.GetAll<NamedAnchor>().ToArray(),
                            () => SettingsManager.Instance.LogoAnchor,
                            anchor => SettingsManager.Instance.LogoAnchor = anchor,
                            this.DataSourceListener
                        )
                    }),
                    new Section(new[]
                    {
                        FloatWithUnitRow.Create(
                            "X",
                            () => SettingsManager.Instance.LogoOffset.X,
                            value => SettingsManager.Instance.LogoOffset = SettingsManager.Instance.LogoOffset.NewWithX(value),
                            this.DataSourceListener
                        ),
                        FloatWithUnitRow.Create(
                            "Y",
                            () => SettingsManager.Instance.LogoOffset.Y,
                            value => SettingsManager.Instance.LogoOffset = SettingsManager.Instance.LogoOffset.NewWithY(value),
                            this.DataSourceListener
                        )
                    }, "Offset")
                };
        }

        public IDataSourceListener DataSourceListener { get; }

        public Section[] Sections { get; }
    }
}
