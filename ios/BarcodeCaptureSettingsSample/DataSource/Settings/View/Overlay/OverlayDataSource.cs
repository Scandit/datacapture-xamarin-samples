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
using BarcodeCaptureSettingsSample.Model;
using Scandit.DataCapture.Barcode.UI.Overlay;

namespace BarcodeCaptureSettingsSample.DataSource.Settings.View.Viewfinder
{
    public class OverlayDataSource : IDataSource
    {
        public OverlayDataSource(IDataSourceListener dataSourceListener)
        {
            this.DataSourceListener = dataSourceListener;
            this.Sections = new[]
            {
                new Section(new []
                {
                    BoolOptionRow.Create("Legacy",
                        () => SettingsManager.Instance.Overlay.Style == BarcodeCaptureOverlayStyle.Legacy,
                        _ => SettingsManager.Instance.OverlayStyle = BarcodeCaptureOverlayStyle.Legacy,
                        this.DataSourceListener
                    ),
                    BoolOptionRow.Create("Frame",
                        () => SettingsManager.Instance.Overlay.Style == BarcodeCaptureOverlayStyle.Frame,
                        _ => SettingsManager.Instance.OverlayStyle = BarcodeCaptureOverlayStyle.Frame,
                        this.DataSourceListener
                    )
                }, "Style"),
                new Section(new[]
                {
                    ChoiceRow<NamedBrush>.Create("Brush",
                        Enumeration.GetAll<NamedBrush>().ToArray(),
                        () => NamedBrush.Create(SettingsManager.Instance.Brush),
                        brush => SettingsManager.Instance.Brush = brush.Brush,
                        this.DataSourceListener
                    )
                }),
            };
        }

        public IDataSourceListener DataSourceListener { get; }

        public Section[] Sections { get; }
    }
}
