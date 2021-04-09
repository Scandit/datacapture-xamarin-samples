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
using AndroidX.Lifecycle;
using Scandit.DataCapture.Barcode.Capture;
using Scandit.DataCapture.Barcode.Data;
using Scandit.DataCapture.Core.Data;
using Scandit.DataCapture.Core.Source;

namespace BarcodeCaptureSettingsSample.Scanning
{
    public class BarcodeScanViewModel : ViewModel, IBarcodeCaptureListener 
    {
        private IBarcodeScanViewModelListener listener = null;

        public BarcodeScanViewModel()
        {
            this.SettingsManager.BarcodeCapture.AddListener(this);
        }

        public SettingsManager SettingsManager => SettingsManager.Instance;

        #region IBarcodeCaptureListener
        public void OnBarcodeScanned(BarcodeCapture barcodeCapture, BarcodeCaptureSession session, IFrameData frameData)
        {
            if (session.NewlyRecognizedBarcodes.Any())
            {
                if (!this.ContinuousScanningEnabled)
                {
                    this.PauseScanning();
                }

                Barcode barcode = session.NewlyRecognizedBarcodes[0];
                using SymbologyDescription description = SymbologyDescription.Create(barcode.Symbology);
                this.listener?.ShowDialog(description.ReadableName, barcode.Data, barcode.AddOnData, barcode.SymbolCount);
            }
        }

        public void OnObservationStarted(BarcodeCapture barcodeCapture)
        { }

        public void OnObservationStopped(BarcodeCapture barcodeCapture)
        { }

        public void OnSessionUpdated(BarcodeCapture barcodeCapture, BarcodeCaptureSession session, IFrameData frameData)
        { }
        #endregion

        public void StartFrameSource() => this.SettingsManager.CurrentCamera?.SwitchToDesiredStateAsync(FrameSourceState.On);

        public void StopFrameSource() => this.SettingsManager.CurrentCamera?.SwitchToDesiredStateAsync(FrameSourceState.Off);

        public void ResumeScanning() => this.SettingsManager.BarcodeCapture.Enabled = true;

        public void PauseScanning() => this.SettingsManager.BarcodeCapture.Enabled = false;

        public void SetListener(IBarcodeScanViewModelListener listener) => this.listener = listener;

        public bool ContinuousScanningEnabled => this.SettingsManager.ContinuousScanningEnabled;
    }
}
