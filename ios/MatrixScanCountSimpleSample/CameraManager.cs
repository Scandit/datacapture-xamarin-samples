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
using System.Threading;
using Scandit.DataCapture.Barcode.Count.Capture;
using Scandit.DataCapture.Core.Capture;
using Scandit.DataCapture.Core.Source;

namespace MatrixScanCountSimpleSample
{
    public sealed class CameraManager
    {
        private static readonly Lazy<CameraManager> instance = new Lazy<CameraManager>(
            () => new CameraManager(), LazyThreadSafetyMode.PublicationOnly);
        private Camera camera;

        public static CameraManager Instance => instance.Value;

        private CameraManager()
		{ }

        public void Initialize(DataCaptureContext dataCaptureContext)
        {
            // Use the default camera and set it as the frame source of the context.
            // The camera is off by default and must be turned on to start streaming frames to the data
            // capture context for recognition.
            // See resumeFrameSource and pauseFrameSource below.
            this.camera = Camera.GetDefaultCamera();

            if (camera != null)
            {
                // Use the recommended camera settings for the BarcodeCount mode.
                this.camera.ApplySettingsAsync(BarcodeCount.RecommendedCameraSettings);

                dataCaptureContext.SetFrameSourceAsync(camera);
            }
            else
            {
                throw new SystemException("Sample depends on a camera, which failed to initialize.");
            }
        }

        public void PauseFrameSource()
        {
            // Switch camera off to stop streaming frames.
            // The camera is stopped asynchronously and will take some time to completely turn off.
            this.camera.SwitchToDesiredStateAsync(FrameSourceState.Off);
        }

        public void StandbyFrameSource()
        {
            // Switch camera to stanby to stop streaming frames.
            // The camera is stopped asynchronously and will take some time to completely turn off.
            this.camera.SwitchToDesiredStateAsync(FrameSourceState.Standby);
        }

        public void ResumeFrameSource()
        {
            // Switch camera on to start streaming frames.
            // The camera is started asynchronously and will take some time to completely turn on.
            this.camera.SwitchToDesiredStateAsync(FrameSourceState.On);
        }
    }
}
