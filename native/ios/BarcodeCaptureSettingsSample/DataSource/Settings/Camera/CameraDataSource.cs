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
using BarcodeCaptureSettingsSample.Model;
using Scandit.DataCapture.Core.Source;

using CameraObject = Scandit.DataCapture.Core.Source.Camera;

namespace BarcodeCaptureSettingsSample.DataSource.Settings.Camera
{
    public class CameraDataSource : IDataSource
    {
        public CameraDataSource(IDataSourceListener dataSourceListener)
        {
            this.DataSourceListener = dataSourceListener;
        }

        public IDataSourceListener DataSourceListener { get; }

        public Section[] Sections
        {
            get
            {
                if (CameraObject.GetDefaultCamera() == null)
                {
                    return new[] { new Section(new Row[] { }, "No camera available") };
                }
                return new[] { this.CreatePositionSection(), this.CreateTorchSection(), this.CreateCameraSettingsSection() };
            }
        }

        private Section CreatePositionSection()
        {
            var rows = new List<Row>();

            var worldFacingCamera = CameraObject.GetCamera(CameraPosition.WorldFacing);
            if (worldFacingCamera != null)
            {
                rows.Add(BoolOptionRow.Create(
                    "World Facing",
                    () => SettingsManager.Instance.Camera == worldFacingCamera,
                    _ => SettingsManager.Instance.Camera = worldFacingCamera,
                    this.DataSourceListener
                ));
            }

            var userFacingCamera = CameraObject.GetCamera(CameraPosition.UserFacing);
            if (userFacingCamera != null)
            {
                rows.Add(BoolOptionRow.Create(
                    "User Facing",
                    () => SettingsManager.Instance.Camera == userFacingCamera,
                    _ => SettingsManager.Instance.Camera = userFacingCamera,
                    this.DataSourceListener
                ));
            }

            return new Section(rows.ToArray(), "Camera Position");
        }

        private Section CreateTorchSection()
        {
            return new Section(new[]
            {
                ChoiceRow<TorchStateType>.Create(
                    "Desired Torch State",
                    Enumeration.GetAll<TorchStateType>().ToArray(),
                    () => SettingsManager.Instance.TorchStateType,
                    value => SettingsManager.Instance.TorchStateType = value,
                    this.DataSourceListener
                )
            });
        }

        private Section CreateCameraSettingsSection()
        {
            return new Section(new Row[]
            {
                ChoiceRow<Resolution>.Create(
                    "Preferred Resolution",
                    Enumeration.GetAll<Resolution>().ToArray(),
                    () => Resolution.Create(SettingsManager.Instance.PreferredResolution),
                    value => SettingsManager.Instance.PreferredResolution = value.VideoResolution,
                    this.DataSourceListener
                ),
                SliderRow.Create(
                    "Zoom Factor",
                    () => SettingsManager.Instance.ZoomFactor,
                    value => SettingsManager.Instance.ZoomFactor = value
                ),
                SliderRow.Create(
                    "Zoom Gesture Zoom Factor",
                    () => SettingsManager.Instance.ZoomGestureZoomFactor,
                    value => SettingsManager.Instance.ZoomGestureZoomFactor = value
                ),
                ChoiceRow<FocusGestureStrategyType>.Create(
                    "Focus Gesture Strategy",
                    Enumeration.GetAll<FocusGestureStrategyType>().ToArray(),
                    () => FocusGestureStrategyType.Create(SettingsManager.Instance.FocusGestureStrategy),
                    value => SettingsManager.Instance.FocusGestureStrategy = value.FocusGestureStrategy,
                    this.DataSourceListener
                ),
                ChoiceRow<FocusRangeType>.Create(
                    "Focus Range",
                    Enumeration.GetAll<FocusRangeType>().ToArray(),
                    () => FocusRangeType.Create(SettingsManager.Instance.FocusRange),
                    value => SettingsManager.Instance.FocusRange = value.FocusRange,
                    this.DataSourceListener
                )

            }, "Camera Settings");
        }
    }
}
