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
using System.Threading;
using MatrixScanCountSimpleSample.Data;
using Scandit.DataCapture.Barcode.Count.Capture;
using Scandit.DataCapture.Barcode.Data;
using Scandit.DataCapture.Barcode.Tracking.Data;

namespace MatrixScanCountSimpleSample
{
    public sealed class BarcodeManager
	{
        private static readonly Lazy<BarcodeManager> instance = new Lazy<BarcodeManager>(() => new BarcodeManager(), LazyThreadSafetyMode.PublicationOnly);
        private List<TrackedBarcode> scannedBarcodes;
        private List<Barcode> additionalBarcodes;
        private BarcodeCount barcodeCount;

        public static BarcodeManager Instance => instance.Value;

        private BarcodeManager()
		{}

        public void Initialize(BarcodeCount barcodeCount)
        {
            this.barcodeCount = barcodeCount ?? throw new ArgumentNullException(nameof(barcodeCount));
        }

        public void UpdateWithSession(BarcodeCountSession session)
        {
            // Update lists of barcodes with the contents of the current session.
            this.scannedBarcodes = session.RecognizedBarcodes.Values.ToList();
            this.additionalBarcodes = session.AdditionalBarcodes.ToList();
        }

        public void SaveCurrentBarcodesAsAdditionalBarcodes()
        {
            if (this.scannedBarcodes != null && this.scannedBarcodes.Any())
            {
                // Save any scanned barcodes as additional barcodes, so they're still scanned
                // after coming back from background.
                List<Barcode> barcodesToSave = new List<Barcode>();

                foreach (TrackedBarcode barcode in this.scannedBarcodes)
                {
                    barcodesToSave.Add(barcode.Barcode);
                }

                barcodesToSave.AddRange(additionalBarcodes);
                this.barcodeCount.SetAdditionalBarcodes(barcodesToSave);
            }
        }

        public IDictionary<string, ScanItem> GetScanResults()
        {
            // Create a map of barcodes to be passed to the scan results screen.
            Dictionary<string, ScanItem> scanResults = new Dictionary<string, ScanItem>();

            void copyToResult(Barcode barcode)
            {
                using SymbologyDescription description = SymbologyDescription.Create(barcode.Symbology);

                if (scanResults.ContainsKey(barcode.Data))
                {
                    scanResults[barcode.Data].IncreaseQuantity();
                }
                else
                {
                    scanResults.Add(
                        barcode.Data,
                        new ScanItem(
                            barcode.Data,
                            description.ReadableName));
                }
            }

            if (this.scannedBarcodes != null && this.scannedBarcodes.Any())
            {
                // Add the inner Barcode objects of each scanned TrackedBarcode to the results map.
                foreach (TrackedBarcode trackedBarcode in this.scannedBarcodes)
                {
                    copyToResult(trackedBarcode.Barcode);
                }
            }

            if (this.additionalBarcodes != null && this.additionalBarcodes.Any())
            {
                // Add the previously saved Barcode objects to the results map.
                foreach (Barcode barcode in this.additionalBarcodes)
                {
                    copyToResult(barcode);
                }
            }

            return scanResults;
        }

        public void Reset()
        {
            // Reset the barcodes lists.
            this.scannedBarcodes?.Clear();
            this.additionalBarcodes?.Clear();
        }
    }
}
