﻿/*
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
using System.Collections.Generic;
using Scandit.DataCapture.Core.Capture;
using Scandit.DataCapture.Core.Source;
using Scandit.DataCapture.Barcode.Capture;
using Scandit.DataCapture.Barcode.Data;

namespace BarcodeCaptureViewsSample.Model
{
    public class DataCaptureManager
    {
        // Enter your Scandit License key here.
        // Your Scandit License key is available via your Scandit SDK web account.
        private static readonly string SCANDIT_LICENSE_KEY = "-- ENTER YOUR SCANDIT LICENSE KEY HERE --";

        private static readonly Lazy<DataCaptureManager> instance = new Lazy<DataCaptureManager>(() => new DataCaptureManager(), LazyThreadSafetyMode.PublicationOnly);

        public DataCaptureContext DataCaptureContext { get; private set; }
        public Camera Camera { get; private set; }
        public BarcodeCapture BarcodeCapture { get; private set; }
        public BarcodeCaptureSettings BarcodeCaptureSettings { get; private set; }

        public static DataCaptureManager Instance
        {
            get { return instance.Value; }
        }

        private DataCaptureManager()
        {
            // Create DataCaptureContext using your license key.
            this.DataCaptureContext = DataCaptureContext.ForLicenseKey(SCANDIT_LICENSE_KEY);
        }

        public void InitializeCamera()
        {
            // Set the device's default camera as DataCaptureContext's FrameSource. DataCaptureContext
            // passes the frames from it's FrameSource to the added modes to perform capture.
            //
            // Since we are going to perform BarcodeCapture in this sample, we initiate the camera with
            // the recommended settings for this mode.
            this.Camera = Camera.GetDefaultCamera();

            if (this.Camera != null)
            {
                // Use the settings recommended by BarcodeCapture.
                this.Camera.ApplySettingsAsync(BarcodeCapture.RecommendedCameraSettings);
                this.DataCaptureContext.SetFrameSourceAsync(this.Camera);
            }
        }

        public void InitializeBarcodeCapture()
        {
            this.DataCaptureContext.RemoveAllModes();

            // The barcode capturing process is configured through barcode capture settings
            // which are then applied to the barcode capture instance that manages barcode recognition.
            this.BarcodeCaptureSettings = BarcodeCaptureSettings.Create();

            // The settings instance initially has all types of barcodes (symbologies) disabled.
            // For the purpose of this sample we enable a very generous set of symbologies.
            // In your own app ensure that you only enable the symbologies that your app requires as
            // every additional enabled symbology has an impact on processing times.
            HashSet<Symbology> symbologies = new HashSet<Symbology>
            {
                Symbology.Ean13Upca,
                Symbology.Ean8,
                Symbology.Upce,
                Symbology.Qr,
                Symbology.DataMatrix,
                Symbology.Code39,
                Symbology.Code128,
                Symbology.InterleavedTwoOfFive
            };

            this.BarcodeCaptureSettings.EnableSymbologies(symbologies);

            // Create new barcode capture mode with the settings from above.
            this.BarcodeCapture = BarcodeCapture.Create(this.DataCaptureContext, this.BarcodeCaptureSettings);
        }
    }
}
