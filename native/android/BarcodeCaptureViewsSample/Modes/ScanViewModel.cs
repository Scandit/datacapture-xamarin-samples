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
using System.Linq;
using System.Threading.Tasks;
using Android.OS;
using AndroidX.Lifecycle;
using BarcodeCaptureViewsSample.Model;
using Scandit.DataCapture.Barcode.Capture;
using Scandit.DataCapture.Barcode.Data;
using Scandit.DataCapture.Core.Capture;
using Scandit.DataCapture.Core.Source;

namespace BarcodeCaptureViewsSample.Modes
{
    public class ScanViewModel : ViewModel
    {
        protected readonly Handler mainHandler = new Handler(Looper.MainLooper);
        protected readonly DataCaptureManager dataCaptureManager = DataCaptureManager.Instance;

        public event EventHandler<Barcode> BarcodeScanned;

        public ScanViewModel()
        {
            this.dataCaptureManager.InitializeCamera();
            this.dataCaptureManager.InitializeBarcodeCapture();
            this.dataCaptureManager.BarcodeCapture.BarcodeScanned += OnBarcodeScanned;
        }

        public DataCaptureContext DataCaptureContext => this.dataCaptureManager.DataCaptureContext;
        public BarcodeCapture BarcodeCapture => this.dataCaptureManager.BarcodeCapture;
        public BarcodeCaptureSettings BarcodeCaptureSettings => this.dataCaptureManager.BarcodeCaptureSettings;

        public virtual void ResumeScanning()
        {
            this.dataCaptureManager.BarcodeCapture.Enabled = true;
        }

        public virtual void PauseScanning()
        {
            this.dataCaptureManager.BarcodeCapture.Enabled = false;
        }

        public Task StartFrameSourceAsync()
        {
            return this.dataCaptureManager.Camera?.SwitchToDesiredStateAsync(FrameSourceState.On);
        }

        public Task StopFrameSourceAsync()
        {
            return this.dataCaptureManager.Camera?.SwitchToDesiredStateAsync(FrameSourceState.Off);
        }

        protected virtual void OnBarcodeScanned(object sender, BarcodeCaptureEventArgs args)
        {
            Barcode firstBarcode = args.Session.NewlyRecognizedBarcodes.FirstOrDefault();

            if (firstBarcode != null)
            {
                // Stop recognizing barcodes for as long as we are displaying the result.
                // There won't be any new results until the capture mode is enabled again.
                // Note that disabling the capture mode does not stop the camera, the camera
                // continues to stream frames until it is turned off.
                this.PauseScanning();

                // This method is invoked on a non-UI thread, so in order to perform UI work,
                // we have to switch to the main thread.
                this.mainHandler.Post(() => this.BarcodeScanned?.Invoke(this, firstBarcode));
            }
        }
    }
}
