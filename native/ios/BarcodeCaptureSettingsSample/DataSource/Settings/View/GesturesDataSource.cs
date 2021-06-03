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
using BarcodeCaptureSettingsSample.Model;
using Scandit.DataCapture.Core.UI.Gestures;

namespace BarcodeCaptureSettingsSample.DataSource.Settings.View
{
    public class GesturesDataSource : IDataSource
    {
        public GesturesDataSource(IDataSourceListener dataSourceListener)
        {
            this.DataSourceListener = dataSourceListener;
            this.Sections = new[]
            {
                new Section(new[]
                {
                    SwitchRow.Create(
                        "Tap to Focus",
                        () => SettingsManager.Instance.TapToFocus != null,
                        enabled => SettingsManager.Instance.TapToFocus = enabled ? TapToFocus.Create() : null
                    ),
                    SwitchRow.Create(
                        "Swipe to Zoom",
                        () => SettingsManager.Instance.SwipeToZoom != null,
                        enabled => SettingsManager.Instance.SwipeToZoom = enabled ? SwipeToZoom.Create(): null
                    )
                })
            };
        }

        public IDataSourceListener DataSourceListener { get; }

        public Section[] Sections { get; }
    }
}
