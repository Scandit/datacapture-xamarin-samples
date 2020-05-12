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
using Scandit.DataCapture.Core.Common.Feedback;

namespace BarcodeCaptureSettingsSample.DataSource.Settings.BarcodeCapture
{
    public class FeedbackDataSource : IDataSource
    {
        public FeedbackDataSource(IDataSourceListener dataSource)
        {
            this.DataSourceListener = DataSourceListener;
            this.Sections = new[]
            {
                new Section(new[]
                {
                    SwitchRow.Create(
                        "Sound",
                        () => SettingsManager.Instance.Feedback.Sound != null,
                        value =>
                        {
                            Sound sound = value ? Sound.DefaultSound : null;
                            var feedback = new Feedback(SettingsManager.Instance.Feedback.Vibration, sound);
                            SettingsManager.Instance.Feedback = feedback;
                        }
                    ),
                    SwitchRow.Create(
                        "Vibration",
                        () => SettingsManager.Instance.Feedback.Vibration != null,
                        value =>
                        {
                            Vibration vibration = value ? Vibration.DefaultVibration : null;
                            var feedback = new Feedback(vibration, SettingsManager.Instance.Feedback.Sound);
                            SettingsManager.Instance.Feedback = feedback;
                        }
                    )
                })
            };
        }

        public IDataSourceListener DataSourceListener { get; }

        public Section[] Sections { get; }
    }
}
