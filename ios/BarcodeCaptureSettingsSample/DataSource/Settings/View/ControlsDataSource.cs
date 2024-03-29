﻿/*
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

namespace BarcodeCaptureSettingsSample.DataSource.Settings.View
{
    public class ControlsDataSource : IDataSource
    {
        public ControlsDataSource(IDataSourceListener dataSourceListener)
        {
            this.DataSourceListener = dataSourceListener;
            this.Sections = new[]
            {
                new Section(new[]
                {
                    SwitchRow.Create(
                        "Torch Button",
                        () => SettingsManager.Instance.TorchSwitchShown,
                        value => SettingsManager.Instance.TorchSwitchShown = value
                    ),
                    SwitchRow.Create(
                        "Zoom Switch Button",
                        () => SettingsManager.Instance.IsZoomSwitchButtonEnabled,
                        value => SettingsManager.Instance.IsZoomSwitchButtonEnabled = value
                    )
                })
            };
        }

        public IDataSourceListener DataSourceListener { get; }

        public Section[] Sections { get; }
    }
}
