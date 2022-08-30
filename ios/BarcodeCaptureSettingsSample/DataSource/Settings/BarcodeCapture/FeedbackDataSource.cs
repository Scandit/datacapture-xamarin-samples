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
using Scandit.DataCapture.Core.Common.Feedback;

namespace BarcodeCaptureSettingsSample.DataSource.Settings.BarcodeCapture
{
    public class FeedbackDataSource : IDataSource
    {
        public FeedbackDataSource(IDataSourceListener dataSourceListener)
        {
            this.DataSourceListener = dataSourceListener;
            this.Sections = new[]
            {
                new Section(new Row[]
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
                    ChoiceRow<VibrationType>.Create(
                        "Vibration",
                        Enumeration.GetAll<VibrationType>().ToArray(),
                        () => VibrationType.Create(SettingsManager.Instance.Vibration),
                        type => {
                            SettingsManager.Instance.Vibration = type.Vibration;
                            var feedback = new Feedback(
                                SettingsManager.Instance.Vibration,
                                SettingsManager.Instance.Feedback.Sound);
                            SettingsManager.Instance.Feedback = feedback;
                        },
                        this.DataSourceListener
                    )
                })
            };
        }

        public IDataSourceListener DataSourceListener { get; }

        public Section[] Sections { get; }
    }
}
