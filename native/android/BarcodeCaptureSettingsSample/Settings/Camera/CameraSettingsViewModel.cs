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
using System.Threading.Tasks;
using AndroidX.Lifecycle;
using Scandit.DataCapture.Core.Source;

namespace BarcodeCaptureSettingsSample.Settings.Camera
{
    public class CameraSettingsViewModel : ViewModel
    {
        private readonly SettingsManager settingsManager = SettingsManager.Instance;
        private readonly CameraPosition[] cameraPositions = new[] { CameraPosition.WorldFacing, CameraPosition.UserFacing };

        public IList<CameraSettingsPositionItem> GetItems()
        {
            return this.cameraPositions
                       .Select(position =>
                           new CameraSettingsPositionItem
                           {
                               CameraPosition = position,
                               Enabled = position == this.settingsManager.CameraPosition
                           })
                       .ToList();
        }

        public CameraPosition CameraPosition => this.settingsManager.CameraPosition;

        public async Task SetCameraPositionAsync(CameraPosition position)
        {
            await this.settingsManager.SetCameraPositionAsync(position);
        }

        public bool TorchEnabled
        {
            get { return this.settingsManager.TorchState == TorchState.On; }
            set { this.settingsManager.SetTorchState(value ? TorchState.On : TorchState.Off); }
        }

        public VideoResolution VideoResolution => this.settingsManager.VideoResolution;

        public async Task SetVideoResolutionAsync(VideoResolution videoResolution)
        {
            await this.settingsManager.SetVideoResolutionAsync(videoResolution);
        }

        public float MaxFrameRate => this.settingsManager.MaxFrameRate;

        public async Task SetMaxFrameRateAsync(float value)
        {
            await this.settingsManager.SetMaxFrameRateAsync(value);
        }

        public float ZoomFactor => this.settingsManager.ZoomFactor;

        public async Task SetZoomFactorAsync(float value)
        {
            await this.settingsManager.SetZoomFactorAsync(value);
        }
    }
}