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
using System.Linq;
using CoreFoundation;
using Foundation;
using Scandit.DataCapture.Barcode.Data;
using Scandit.DataCapture.Barcode.Tracking.Capture;
using Scandit.DataCapture.Barcode.Tracking.UI.Overlay;
using Scandit.DataCapture.Core.Capture;
using Scandit.DataCapture.Core.Data;
using Scandit.DataCapture.Core.Source;
using Scandit.DataCapture.Core.UI;
using UIKit;

namespace MatrixScanSimpleSample
{
    public partial class ViewController : UIViewController, IBarcodeTrackingListener
    {
        // There is a Scandit sample license key set below here.
        // This license key is enabled for sample evaluation only.
        // If you want to build your own application, get your license key by signing up for a trial at https://ssl.scandit.com/dashboard/sign-up?p=test
        public const string SCANDIT_LICENSE_KEY = "AfUkdmKlRiP5FdlOFQnOhu4V3j5LFKttPGTWXFd7CkuRaTAstDqq78RrBm2ZG9LRu1T8CNgP6oLScGrUoEwfmP1TUXonIGCl2g9Fo5NYtmK/aEV8FX/YcdRKfWS5bJrTcWGDHdcsJxT6Me5C3RMdWZkdqeR5GEjDzT6dO4ZPWOBbNLjpkgZ0/MjtYQPKqSV+bSZC7+ekFaXovSKWfXV89BXtta/6sZHFJOMKxyvzh6zw5yA+NDR67OXoWKCrrNq4AOuBlt1ZelIHCqjQgTy/SZG110eJr5e4pth38Bx0fXE8FGX92BoxwJr1EG+P5CEJF8EFMy2zf87aJQYuzHmg0nM7czcNqLUd9F23uxntZYjKlwgWmmSzev/ozaumEvbW9RVW1bUQmV8pQ1SWILBuzQPeAw8iWOWgnTH18tH7cT+fUJumvM2rn7LWx9JYLAKBKRuwe2sDh3l5eqobZKdarIRsKVgXa4pw+gkYKuplzTo+Bzh70rbmtgq3IJ8hSpdoZITzfUQSwXkrgdQa5Cmrpxz9gXManBRt01h3eFXG7znZU9w0+uzzV/b5e6MQcPncODrCQOq0kfEBYgRoLAwVCOKnxyWQkqRbUpsTN2wy2MTg10flYhR/zf1eXdiUjgPUhWj8LtmgxJELYky7uMu46abfCkAw73e+12iJmlf9/tmTFk34La9ZQiF/BYps5h327ZW8qobay+Esx1i9dsaFKYt/nCN8jZdUYD/df+/vApyK4PMbph9EPRe5u0alg8BqpEExnkQsy1W7r85yngO/rxSXsY6rTMoTXb/87ul8uQnsrD41ZLtFdzo0OlbNTeNOI1mJz/E6/SOLbRRK";

        private DataCaptureContext dataCaptureContext;
        private Camera camera;
        private BarcodeTracking barcodeTracking;

        private HashSet<ScanResult> scanResults = new HashSet<ScanResult>();

        public ViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
            // First, disable barcode tracking to stop processing frames.
            this.barcodeTracking.Enabled = false;
            // Switch the camera off to stop streaming frames. The camera is stopped asynchronously.
            this.camera?.SwitchToDesiredStateAsync(FrameSourceState.Off);
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            // Remove the scanned barcodes everytime the barcode tracking starts.
            this.scanResults?.Clear();

            // First, enable barcode tracking to resume processing frames.
            this.barcodeTracking.Enabled = true;
            // Switch camera on to start streaming frames. The camera is started asynchronously and will take some time to
            // completely turn on.
            this.camera?.SwitchToDesiredStateAsync(FrameSourceState.On);
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            this.InitializeAndStartBarcodeScanning();
        }

        public override void PrepareForSegue(UIStoryboardSegue segue, NSObject sender)
        {
            base.PrepareForSegue(segue, sender);

            if (segue.DestinationViewController is ResultsViewController resultViewController)
            {
                resultViewController.Items = this.scanResults.ToList();
            }
        }

        #region IBarcodeTrackingListener

        public void OnSessionUpdated(BarcodeTracking barcodeTracking, BarcodeTrackingSession session, IFrameData frameData)
        {
            DispatchQueue.MainQueue.DispatchAsync(() =>
            {
                this.scanResults = session.TrackedBarcodes?.Values.Select(v => new ScanResult
                {
                    Data = v.Barcode.Data,
                    Symbology = v.Barcode.Symbology.ReadableName()
                }).ToHashSet();
            });

            // Dispose the frame when you have finished processing it. If the frame is not properly disposed,
            // different issues could arise, e.g. a frozen, non-responsive, or "severely stuttering" video feed.
            frameData.Dispose();
        }

        public void OnObservationStarted(BarcodeTracking barcodeTracking)
        {
        }

        public void OnObservationStopped(BarcodeTracking barcodeTracking)
        {
        }

        #endregion

        protected void InitializeAndStartBarcodeScanning()
        {
            // Create data capture context using your license key.
            this.dataCaptureContext = DataCaptureContext.ForLicenseKey(SCANDIT_LICENSE_KEY);

            // Use the default camera and set it as the frame source of the context.
            // The camera is off by default and must be turned on to start streaming frames to the data
            // capture context for recognition.
            // See resumeFrameSource and pauseFrameSource below.
            this.camera = Camera.GetDefaultCamera();
            if (this.camera != null)
            {
                // Use the settings recommended by barcode capture.
                this.dataCaptureContext.SetFrameSourceAsync(this.camera);
            }

            // Use the recommended camera settings for the BarcodeTracking mode as default settings.
            // The preferred resolution is automatically chosen, which currently defaults to HD on all devices.
            CameraSettings cameraSettings = BarcodeTracking.RecommendedCameraSettings;

            // Setting the preferred resolution to full HD helps to get a better decode range.
            cameraSettings.PreferredResolution = VideoResolution.FullHd;
            camera?.ApplySettingsAsync(cameraSettings);
            BarcodeTrackingSettings barcodeTrackingSettings = BarcodeTrackingSettings.Create();

            // The settings instance initially has all types of barcodes (symbologies) disabled.
            // For the purpose of this sample we enable a very generous set of symbologies.
            // In your own app ensure that you only enable the symbologies that your app requires as
            // every additional enabled symbology has an impact on processing times.
            HashSet<Symbology> symbologies = new HashSet<Symbology>()
            {
                Symbology.Ean13Upca,
                Symbology.Ean8,
                Symbology.Upce,
                Symbology.Code39,
                Symbology.Code128
            };

            barcodeTrackingSettings.EnableSymbologies(symbologies);

            // Create new barcode tracking mode with the settings from above.
            this.barcodeTracking = BarcodeTracking.Create(this.dataCaptureContext, barcodeTrackingSettings);

            // Register self as a listener to get informed whenever a new barcode got recognized.
            this.barcodeTracking.AddListener(this);

            // To visualize the on-going barcode tracking process on screen, setup a data capture view
            // that renders the camera preview. The view must be connected to the data capture context.
            var dataCaptureView = DataCaptureView.Create(this.dataCaptureContext, this.View.Bounds);
            dataCaptureView.AutoresizingMask = UIViewAutoresizing.FlexibleHeight |
                                               UIViewAutoresizing.FlexibleWidth;
            this.View.AddSubview(dataCaptureView);
            this.View.SendSubviewToBack(dataCaptureView);
            BarcodeTrackingBasicOverlay.Create(this.barcodeTracking, dataCaptureView, BarcodeTrackingBasicOverlayStyle.Frame);
        }
    }
}
