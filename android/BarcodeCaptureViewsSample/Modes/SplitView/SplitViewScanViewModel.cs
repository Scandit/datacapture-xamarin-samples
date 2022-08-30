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
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using Scandit.DataCapture.Barcode.Capture;
using Scandit.DataCapture.Barcode.Data;
using Scandit.DataCapture.Core.Area;
using Scandit.DataCapture.Core.Capture;
using Scandit.DataCapture.Core.Common.Geometry;

namespace BarcodeCaptureViewsSample.Modes.SplitView
{
    public class SplitViewScanViewModel : ScanViewModel
    {
        private readonly Timer scannerTimeoutTimer;
        private bool isLicenseValid = false;

        public IList<Barcode> Barcodes { get; } = new List<Barcode>();

        public event EventHandler<int> BarcodesCleared;
        public event EventHandler<int> BarcodeInserted;
        public event EventHandler ScannerPaused;

        public SplitViewScanViewModel()
        {
            this.scannerTimeoutTimer = new Timer()
            {
                AutoReset = false,
                Interval = TimeSpan.FromSeconds(10).TotalMilliseconds,
                Enabled = false
            };
            this.scannerTimeoutTimer.Elapsed += this.ScannerTimeoutTimerElapsed;
            this.DataCaptureContext.StatusChanged += this.DataCaptureContextStatusChanged;

            // Setting the code duplicate filter to one means that the scanner won't report the same code as recognized
            // for one second once it's recognized.
            this.BarcodeCaptureSettings.CodeDuplicateFilter = TimeSpan.FromMilliseconds(1000);

            // By setting the radius to zero, the barcode's frame has to contain the point of interest.
            // The point of interest is at the center of the data capture view by default, as in this case.
            this.BarcodeCaptureSettings.LocationSelection = RadiusLocationSelection.Create(new FloatWithUnit(0f, MeasureUnit.Fraction));

            this.BarcodeCapture.ApplySettingsAsync(this.BarcodeCaptureSettings);
        }

        public override void ResumeScanning()
        {
            base.ResumeScanning();

            if (this.isLicenseValid)
            {
                this.scannerTimeoutTimer.Stop();
                this.scannerTimeoutTimer.Start();
            }
        }

        public void ClearResults()
        {
            var clearedItems = this.Barcodes.Count;

            this.Barcodes.Clear();
            this.BarcodesCleared?.Invoke(this, clearedItems);
        }

        protected override void OnBarcodeScanned(object sender, BarcodeCaptureEventArgs args)
        {
            Barcode firstBarcode = args.Session.NewlyRecognizedBarcodes.FirstOrDefault();

            if (firstBarcode != null)
            {
                this.scannerTimeoutTimer.Stop();
                this.scannerTimeoutTimer.Start();

                this.Barcodes.Add(firstBarcode);

                // This method is invoked on a non-UI thread, so in order to perform UI work,
                // we have to switch to the main thread.
                this.mainHandler.Post(() => this.BarcodeInserted?.Invoke(this, this.Barcodes.Count - 1));
            }
        }

        private void DataCaptureContextStatusChanged(object sender, StatusChangedEventArgs args)
        {
            if (this.isLicenseValid == args.Status.Valid)
            {
                // No further action required.
                return;
            }

            this.scannerTimeoutTimer.Stop();

            // We want to start the countdown to show the tap-to-continue overlay only if the license is valid.
            if (args.Status.Valid)
            {
                this.scannerTimeoutTimer.Start();
            }

            this.isLicenseValid = args.Status.Valid;
        }

        private void ScannerTimeoutTimerElapsed(object sender, ElapsedEventArgs args)
        {
            this.PauseScanning();
            this.mainHandler.Post(() => this.ScannerPaused?.Invoke(this, EventArgs.Empty));
        }
    }
}
