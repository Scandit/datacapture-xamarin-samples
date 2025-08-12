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

#nullable enable

using System;
using Scandit.DataCapture.Core.Capture;
using Scandit.DataCapture.Core.Source;
using Scandit.DataCapture.Label.Capture;

namespace LabelCaptureSimpleSample.Services.Internals
{
    internal sealed class CameraService : ICameraService
    {
        private Camera? _camera;

        public CameraSettings CameraSettings { get; } = LabelCapture.RecommendedCameraSettings;

        public void Initialize(DataCaptureContext dataCaptureContext)
        {
            if (dataCaptureContext == null)
            {
                throw new ArgumentNullException(nameof(dataCaptureContext));
            }

            // Use the default camera and set it as the frame source of the context.
            // The camera is off by default and must be turned on to start streaming frames to the data
            // capture context for recognition.
            // See ResumeFrameSource and PauseFrameSource below.
            _camera = Camera.GetDefaultCamera(CameraSettings);

            if (_camera != null)
            {
                dataCaptureContext.SetFrameSourceAsync(_camera);
            }
        }

        public void PauseFrameSource()
        {
            // Switch camera off to stop streaming frames.
            // The camera is stopped asynchronously and will take some time to completely turn off.
            _camera?.SwitchToDesiredStateAsync(FrameSourceState.Off);
        }

        public void ResumeFrameSource()
        {
            // Switch camera on to start streaming frames.
            // The camera is started asynchronously and will take some time to completely turn on.
            _camera?.SwitchToDesiredStateAsync(FrameSourceState.On);
        }
    }
}
