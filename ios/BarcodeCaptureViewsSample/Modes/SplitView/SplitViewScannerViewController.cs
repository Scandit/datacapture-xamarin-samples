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
using System.Timers;
using CoreFoundation;
using Scandit.DataCapture.Barcode.Capture;
using Scandit.DataCapture.Barcode.Data;
using Scandit.DataCapture.Barcode.UI.Overlay;
using Scandit.DataCapture.Core.Area;
using Scandit.DataCapture.Core.Capture;
using Scandit.DataCapture.Core.Common.Geometry;
using Scandit.DataCapture.Core.Source;
using Scandit.DataCapture.Core.UI;
using Scandit.DataCapture.Core.UI.Viewfinder;
using UIKit;

namespace BarcodeCaptureViewsSample.Modes.SplitView
{
    public partial class SplitViewScannerViewController : UIViewController
    {
        private DataCaptureView dataCaptureView;
        private DataCaptureContext dataCaptureContext;
        private Camera camera;
        private BarcodeCapture barcodeCapture;
        private BarcodeCaptureOverlay barcodeCaptureOverlay;

        private bool isLicenseValid = false;
        private Timer scannerTimeoutTimer;

        public event EventHandler<BarcodeCaptureEventArgs> BarcodeScanned
        {
            add { this.barcodeCapture.BarcodeScanned += value; }
            remove { this.barcodeCapture.BarcodeScanned -= value; }
        }

        public SplitViewScannerViewController(IntPtr handle) : base(handle)
        { }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            this.InitializeBarcodeScanning();

            this.scannerTimeoutTimer = new Timer()
            {
                AutoReset = false,
                Interval = TimeSpan.FromSeconds(10).TotalMilliseconds,
                Enabled = false
            };
            this.scannerTimeoutTimer.Elapsed += PauseScanning;
            this.TapToContinueLabel.AddGestureRecognizer(new UITapGestureRecognizer(ContinueScanning));
        }

        private void ContinueScanning()
        {
            DispatchQueue.MainQueue.DispatchAsync(() =>
            {
                this.TapToContinueLabel.Hidden = true;
            });
            this.barcodeCapture.Enabled = true;
            this.scannerTimeoutTimer.Start();
        }

        private void PauseScanning(object sender, ElapsedEventArgs e)
        {
            this.barcodeCapture.Enabled = false;
            DispatchQueue.MainQueue.DispatchAsync(() => {
                this.TapToContinueLabel.Hidden = false;
            });
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            // Switch camera on to start streaming frames. The camera is started asynchronously and will take some time to
            // completely turn on.
            this.barcodeCapture.Enabled = true;
            this.camera?.SwitchToDesiredStateAsync(FrameSourceState.On);

            if (this.isLicenseValid)
            {
                this.scannerTimeoutTimer.Start();
            }
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);

            // Switch camera off to stop streaming frames. The camera is stopped asynchronously and will take some time to
            // completely turn off. Until it is completely stopped, it is still possible to receive further results, hence
            // it's a good idea to first disable barcode tracking as well.
            this.barcodeCapture.Enabled = false;
            this.camera?.SwitchToDesiredStateAsync(FrameSourceState.Off);
        }

        protected void InitializeBarcodeScanning()
        {
            // Create data capture context using your license key.
            this.dataCaptureContext = DataCaptureContext.ForLicenseKey(Application.SCANDIT_LICENSE_KEY);

            this.dataCaptureContext.StatusChanged += this.DataCaptureContextStatusChanged;

            // Use the default camera and set it as the frame source of the context.
            // The camera is off by default and must be turned on to start streaming frames to the data
            // capture context for recognition.
            this.camera = Camera.GetDefaultCamera();

            if (this.camera != null)
            {
                // Use the settings recommended by barcode capture.
                this.camera.ApplySettingsAsync(BarcodeCapture.RecommendedCameraSettings);
                this.dataCaptureContext.SetFrameSourceAsync(this.camera);
            }

            BarcodeCaptureSettings barcodeCaptureSettings = BarcodeCaptureSettings.Create();

            // The settings instance initially has all types of barcodes (symbologies) disabled.
            // For the purpose of this sample we enable a very generous set of symbologies.
            // In your own app ensure that you only enable the symbologies that your app requires as
            // every additional enabled symbology has an impact on processing times.
            HashSet<Symbology> symbologies = new HashSet<Symbology>()
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

            barcodeCaptureSettings.EnableSymbologies(symbologies);

            // Setting the code duplicate filter to one means that the scanner won't report the same code as recognized
            // for one second once it's recognized.
            barcodeCaptureSettings.CodeDuplicateFilter = TimeSpan.FromMilliseconds(1000);

            // By setting the radius to zero, the barcode's frame has to contain the point of interest.
            // The point of interest is at the center of the data capture view by default, as in this case.
            barcodeCaptureSettings.LocationSelection = RadiusLocationSelection.Create(new FloatWithUnit(0f, MeasureUnit.Fraction));

            // Create new barcode capture mode with the settings from above.
            this.barcodeCapture = BarcodeCapture.Create(this.dataCaptureContext, barcodeCaptureSettings);
            this.barcodeCapture.BarcodeScanned += BarcodeCaptureBarcodeScanned;

            // To visualize the on-going barcode capturing process on screen, setup a data capture view
            // that renders the camera preview. The view must be connected to the data capture context.
            this.dataCaptureView = DataCaptureView.Create(this.dataCaptureContext, this.View.Bounds);
            this.dataCaptureView.AutoresizingMask = UIViewAutoresizing.FlexibleHeight |
                                                    UIViewAutoresizing.FlexibleWidth;

            this.barcodeCaptureOverlay = BarcodeCaptureOverlay.Create(this.barcodeCapture, this.dataCaptureView, BarcodeCaptureOverlayStyle.Frame);
            // Add the laser line viewfinder to the overlay.
            var viewFinder = LaserlineViewfinder.Create(LaserlineViewfinderStyle.Animated);
            viewFinder.Width = new FloatWithUnit(value: 0.9f, unit: MeasureUnit.Fraction);
            barcodeCaptureOverlay.Viewfinder = viewFinder;

            // We are resizing the capture view to not to take the whole screen,
            // but just fill it's parent, both horizontally and vertically.
            this.View.AddSubview(this.dataCaptureView);
            this.dataCaptureView.TranslatesAutoresizingMaskIntoConstraints = false;
            this.View.AddConstraints(new[] {
                this.dataCaptureView.LeadingAnchor.ConstraintEqualTo(this.View.LeadingAnchor),
                this.dataCaptureView.TopAnchor.ConstraintEqualTo(this.View.TopAnchor),
                this.dataCaptureView.TrailingAnchor.ConstraintEqualTo(this.View.TrailingAnchor),
                this.dataCaptureView.BottomAnchor.ConstraintEqualTo(this.View.BottomAnchor)
            });
            this.View.SendSubviewToBack(this.dataCaptureView);
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

        private void BarcodeCaptureBarcodeScanned(object sender, BarcodeCaptureEventArgs args)
        {
            this.scannerTimeoutTimer.Stop();
            this.scannerTimeoutTimer.Start();
        }
    }
}

