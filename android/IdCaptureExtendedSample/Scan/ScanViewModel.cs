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

using AndroidX.Lifecycle;
using IdCaptureExtendedSample.Data;
using Java.Lang;

using Scandit.DataCapture.Core.Capture;
using Scandit.DataCapture.Core.Data;
using Scandit.DataCapture.Core.Source;
using Scandit.DataCapture.ID.Capture;
using Scandit.DataCapture.ID.Data;

namespace IdCaptureExtendedSample.Scan
{
    public class ScanViewModel : ViewModel, IIdCaptureListener
    {
        private readonly DataCaptureManager dataCaptureManager = DataCaptureManager.Instance;
        private IScanViewModelListener listener;

        public DataCaptureContext DataCaptureContext => this.dataCaptureManager.DataCaptureContext;
        public IdCapture IdCapture => this.dataCaptureManager.IdCapture;
        public Camera Camera => this.dataCaptureManager.Camera;

        public ScanViewModel()
        {
            this.SetupRecognition();
        }

        public void SetListener(IScanViewModelListener listener)
        {
            this.listener = listener;
        }

        public void ResumeScanning()
        {
            this.IdCapture.Enabled = true;
        }

        public void PauseScanning()
        {
            this.IdCapture.Enabled = false;
        }

        public void StartFrameSource()
        {
            this.Camera?.SwitchToDesiredStateAsync(FrameSourceState.On);
        }

        public void StopFrameSource()
        {
            this.Camera?.SwitchToDesiredStateAsync(FrameSourceState.Off);
        }

        public void OnModeSelected(Mode mode)
        {
            this.dataCaptureManager.IdCapture?.RemoveListener(this);
            this.dataCaptureManager.ConfigureIdCapture(mode);
            this.dataCaptureManager.IdCapture.AddListener(this);
        }

        #region IIdCaptureListener

        public void OnIdCaptured(IdCapture capture, CapturedId capturedId)
        {
            // Pause the IdCapture to not capture while showing the result.
            this.PauseScanning();

            // Show the result
            this.listener?.ShowIdCaptured(capturedId);
        }

        public void OnIdRejected(IdCapture mode, CapturedId capturedId, RejectionReason reason)
        {
            // Implement to handle documents recognized in a frame, but rejected.
            // A document or its part is considered rejected when (a) it's valid, but not enabled in the settings,
            // (b) it's a barcode of a correct symbology or a Machine Readable Zone (MRZ),
            // but the data is encoded in an unexpected/incorrect format.
            this.listener?.ShowIdRejected();
        }
        #endregion

        private void SetupRecognition()
        {
            this.dataCaptureManager.InitializeCamera();
        }
    }
}
