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
using System.Collections.Generic;
using System.Threading;
using Scandit.DataCapture.Core.Capture;
using Scandit.DataCapture.Core.Source;
using Scandit.DataCapture.ID.Capture;
using Scandit.DataCapture.ID.Data;

namespace IdCaptureSimpleSample
{
    public class DataCaptureManager
    {
        private static readonly string SCANDIT_LICENSE_KEY = "-- ENTER YOUR SCANDIT LICENSE KEY HERE --";

        private static readonly Lazy<DataCaptureManager> instance = new Lazy<DataCaptureManager>(() => new DataCaptureManager(), LazyThreadSafetyMode.PublicationOnly);

        public DataCaptureContext DataCaptureContext { get; private set; }
        public Camera Camera { get; private set; }
        public IdCapture IdCapture { get; private set; }

        public static DataCaptureManager Instance
        {
            get { return instance.Value; }
        }

        private DataCaptureManager()
        {
            // Create DataCaptureContext using your license key.
            this.DataCaptureContext = DataCaptureContext.ForLicenseKey(SCANDIT_LICENSE_KEY);
        }

        public void InitializeAndStartScanning()
        {
            this.InitCamera();
            this.InitIdCapture();
        }

        private void InitCamera()
        {
            // Set the device's default camera as DataCaptureContext's FrameSource. DataCaptureContext
            // passes the frames from it's FrameSource to the added modes to perform capture.
            //
            // Since we are going to perform IdCapture in this sample, we initiate the camera with
            // the recommended settings for this mode.
            this.Camera = Camera.GetDefaultCamera();

            if (this.Camera != null)
            {
                // Use the settings recommended by barcode capture.
                this.Camera.ApplySettingsAsync(IdCapture.RecommendedCameraSettings);
                this.DataCaptureContext.SetFrameSourceAsync(this.Camera);
            }
        }

        private void InitIdCapture()
        {
            // Create a mode responsible for recognizing documents. This mode is automatically added
            // to the passed DataCaptureContext.
            var settings = new IdCaptureSettings
            {
                AcceptedDocuments = new List<IdCaptureDocument> {
                    new IdCard(IdCaptureRegion.Any),
                    new DriverLicense(IdCaptureRegion.Any),
                    new Passport(IdCaptureRegion.Any)
                },
                ScannerType = new FullDocumentScanner()
            };

            this.IdCapture = IdCapture.Create(this.DataCaptureContext, settings);
        }
    }
}
