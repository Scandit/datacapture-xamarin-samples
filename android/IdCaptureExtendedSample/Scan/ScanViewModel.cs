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
        private bool isScanningBackSide = false;

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
            this.isScanningBackSide = false;
            this.dataCaptureManager.IdCapture?.RemoveListener(this);
            this.dataCaptureManager.ConfigureIdCapture(mode);
            this.dataCaptureManager.IdCapture.AddListener(this);
        }

        public void ScanBackSide(bool scanningBackSide)
        {
            if (!scanningBackSide)
            {
                // If we want to skip scanning the back of the document, we have to call
                // `IdCapture.Reset()` to allow for another front IDs to be scanned.
                this.IdCapture.Reset();
            }

            this.isScanningBackSide = scanningBackSide;
        }

        #region IIdCaptureListener
        public void OnErrorEncountered(IdCapture capture, Throwable error, IdCaptureSession session, IFrameData frameData)
        {
            // Implement to handle an error encountered during the capture process.
            this.listener?.ShowError(GetErrorMessage(error));
        }

        public void OnIdCaptured(IdCapture capture, IdCaptureSession session, IFrameData frameData)
        {
            CapturedId capturedId = session.NewlyCapturedId;

            // Pause the IdCapture to not capture while showing the result.
            this.PauseScanning();

            // Viz documents support multiple sides scanning.
            // In case the back side is supported and not yet captured we inform the user about the feature.
            if (capturedId.Viz != null &&
                capturedId.Viz.BackSideCaptureSupported &&
                capturedId.Viz.CapturedSides == SupportedSides.FrontOnly)
            {
                // Until the back side is scanned, IdCapture will keep reporting the front side.
                // If we are looking for the back side we just resume scanning.
                if (this.isScanningBackSide)
                {
                    this.ResumeScanning();
                }
                else
                {
                    this.listener?.ShowBackOfCardAlert(capturedId);
                }
            }
            else
            {
                // Show the result
                this.listener?.ShowIdCaptured(capturedId);
            }
        }

        public void OnIdLocalized(IdCapture mode, IdCaptureSession session, IFrameData data)
        {
            // Implement to handle a personal identification document or its part localized within
            // a frame. A document or its part is considered localized when it's detected in a frame,
            // but its data is not yet extracted.

            // In this sample we are not interested in this callback.
        }

        public void OnIdRejected(IdCapture mode, IdCaptureSession session, IFrameData data)
        {
            // Implement to handle documents recognized in a frame, but rejected.
            // A document or its part is considered rejected when (a) it's valid, but not enabled in the settings,
            // (b) it's a barcode of a correct symbology or a Machine Readable Zone (MRZ),
            // but the data is encoded in an unexpected/incorrect format.
            this.listener?.ShowIdRejected();
        }

        public void OnIdCaptureTimedOut(IdCapture mode, IdCaptureSession session, IFrameData data)
        {
            // In this sample we are not interested in this callback.
        }

        public void OnObservationStarted(IdCapture idCapture)
        {
            // In this sample we are not interested in this callback.
        }

        public void OnObservationStopped(IdCapture idCapture)
        {
            // In this sample we are not interested in this callback.
        }
        #endregion

        private void SetupRecognition()
        {
            this.dataCaptureManager.InitializeCamera();
        }

        private static string GetErrorMessage(Throwable error)
        {
            if (error is IdCaptureException idCaptureException)
            {
                StringBuilder messageBuilder = new StringBuilder(idCaptureException.GetKind().ToString());

                if (!string.IsNullOrEmpty(idCaptureException.Message))
                {
                    messageBuilder.Append($": {idCaptureException.Message}");
                }

                return messageBuilder.ToString();
            }
            else
            {
                return error.Message;
            }
        }
    }
}
