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
using Foundation;
using UIKit;

using Scandit.DataCapture.Barcode.Count.Capture;
using Scandit.DataCapture.Barcode.Count.UI;
using Scandit.DataCapture.Barcode.Data;
using Scandit.DataCapture.Core.Capture;

namespace MatrixScanCountSimpleSample
{
    public partial class ViewController : UIViewController, IListViewControllerListener
    {
        // There is a Scandit sample license key set below here.
        // This license key is enabled for sample evaluation only.
        // If you want to build your own application, get your license key by signing up for a trial at https://ssl.scandit.com/dashboard/sign-up?p=test
        public const string SCANDIT_LICENSE_KEY = "AbvELRLKNvXhGsHO0zMIIg85n3IiQdKMA2p5yeVDSOSZSZg/BhX401FXc+2UHPun8Rp2LRpw26tYdgnIJlXiLAtmXfjDZNQzZmrZY2R0QaJaXJC34UtcQE12hEpIYhu+AmjA5cROhJN3CHPoHDns+ho12ibrRAoFrAocoBIwCVzuTRHr0U6pmCKoa/Mn3sNPdINHh97m1X9Al9xjh3VOTNimP6ZjrHLVWEJSOdp2QYOnqn5izP1329PVcZhn8gqlGCRh+LJytbKJYI/KIRbMy3bNOyq5kNnr2IlOqaoXRgYdz2IU+jIWw8Cby9XoSB1zkphiYMmlCUqrDzxLUmTAXF4rSWobiM+OxnoImDqISpunJBQz0a5DSeT5Zf0lwwvXQLX4ghkgXozyYYfYvIKsqxJLZoza8g1BFsJ1i3fb0JYP2Ju209OMN2NTJifAu9ZJjQKGWS76Rmr/jre13jCqGgx5SX9F2lA2ZpF2AEb6rmYYmMtL9CPwWvstM+W295WvscH+gCBccZ9q3rxfIsak6cV2T50/2uBWfJJka6kL9UOjMOG3BOGKx+O+KWT/twwvOC+GcvC8s1qMwGNNM6G+/m7fG5Xtl5wtp3QhpzPJbBHSmlkYbxXQx0SpuWBmvxygyKOi3lUzz3gRzOdykWRXzrhiMAp5bb1y6n6g4O2v2TVgzWWF8vwZ6F60ehYDUq7pbusgT4Fl3fV7fYPgLxMMvXKduMmUlWyGv3CWL9LfvoY/hLl7RxoyUryTMmSfRVBcsKs+MWYJGh1iIvWk1UhOChb9IGI2PzUsHz7+OikuYMjKhR8LZZYalXpPiEVfT66yy75M5DODcjXRoFZU";

        private DataCaptureContext dataCaptureContext;
        private BarcodeCount barcodeCount;
        private BarcodeCountView barcodeCountView;
        private BarcodeCountSettings barcodeCountSettings;
        private bool shouldCameraStandby = true;

        private NSObject enterBackgroundNotificationToken;
        private bool disposed = false;

        public ViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);

            // Switch camera to stanby to stop streaming frames. The camera is stopped asynchronously and will take
            // some time to completely turn off.
            if (this.shouldCameraStandby)
            {
                CameraManager.Instance.StandbyFrameSource();
            }

            // Unsubscribe from barcode count.
            this.barcodeCount.Scanned -= this.BarcodeCountScanned;
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            this.NavigationController?.SetNavigationBarHidden(true, animated: false);

            // Subscribe for barcode count events.
            this.barcodeCount.Scanned += this.BarcodeCountScanned;

            // Make sure that Barcode Count mode is enabled after going back from the list screen
            this.barcodeCount.Enabled = true;

            // Resume camera to start streaming frames.
            CameraManager.Instance.ResumeFrameSource();
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            this.InitializeBarcodeScanning();

            this.enterBackgroundNotificationToken = NSNotificationCenter.DefaultCenter.AddObserver(
                UIApplication.DidEnterBackgroundNotification, this.OnEnterBackground);
        }

        protected void InitializeBarcodeScanning()
        {
            // Create data capture context using your license key.
            this.dataCaptureContext = DataCaptureContext.ForLicenseKey(SCANDIT_LICENSE_KEY);

            // Initialize the shared camera manager.
            CameraManager.Instance.Initialize(this.dataCaptureContext);

            // The barcode count process is configured through barcode count settings
            // which are then applied to the barcode count instance that manages barcode count.
            this.barcodeCountSettings = new BarcodeCountSettings();

            // The settings instance initially has all types of barcodes (symbologies) disabled.
            // For the purpose of this sample we enable a very generous set of symbologies.
            // In your own app ensure that you only enable the symbologies that your app requires
            // as every additional enabled symbology has an impact on processing times.
            HashSet<Symbology> symbologies = new HashSet<Symbology>
            {
                Symbology.Ean13Upca,
                Symbology.Ean8,
                Symbology.Upce,
                Symbology.Code39,
                Symbology.Code128
            };
            this.barcodeCountSettings.EnableSymbologies(symbologies);

            // Create barcode count and attach to context.
            this.barcodeCount = BarcodeCount.Create(this.dataCaptureContext, this.barcodeCountSettings);

            // Initialize the shared barcode manager.
            BarcodeManager.Instance.Initialize(this.barcodeCount);

            // To visualize the on-going barcode count process on screen, setup a BarcodeCountView
            // that renders the camera preview. The view must be connected to the data capture context
            // and to the barcode count.
            this.barcodeCountView = BarcodeCountView.Create(
                    this.View.Bounds,
                    this.dataCaptureContext,
                    this.barcodeCount,
                    BarcodeCountViewStyle.Icon
            );

            // Subscribe to BarcodeCountView events.
            this.barcodeCountView.ListButtonTapped += BarcodeCountViewListButtonTapped;
            this.barcodeCountView.ExitButtonTapped += BarcodeCountViewExitButtonTapped;

            // Add the BarcodeCountView to the container.
            this.barcodeCountView.AutoresizingMask = UIViewAutoresizing.FlexibleWidth |
                                                     UIViewAutoresizing.FlexibleHeight;
            this.View.AddSubview(this.barcodeCountView);
        }

        protected override void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    this.enterBackgroundNotificationToken.Dispose();
                }

                this.disposed = true;
            }

            base.Dispose();
        }

        private void ShowList(bool isOrderCompleted)
        {
            this.shouldCameraStandby = false;

            // Get a list of ScannedItem to display
            var scannedItems = BarcodeManager.Instance.GetScanResults().Values.ToList();

            var listController = new ListViewController(
                this,
                scanResults: scannedItems,
                isOrderCompleted: isOrderCompleted);

            // Show the list
            this.NavigationController?.PushViewController(listController, animated: true);
        }

        private void ResetSession()
        {
            BarcodeManager.Instance.Reset();
            this.barcodeCount.ClearAdditionalBarcodes();
            this.barcodeCount.Reset();
        }

        private void OnEnterBackground(NSNotification notification)
        {
            BarcodeManager.Instance.SaveCurrentBarcodesAsAdditionalBarcodes();
        }

        private void BarcodeCountScanned(object sender, BarcodeCountEventArgs args)
        {
            BarcodeManager.Instance.UpdateWithSession(args.Session);
        }

        private void BarcodeCountViewExitButtonTapped(object sender, ExitButtonTappedEventArgs args)
        {
            // The order is completed
            this.ShowList(isOrderCompleted: true);
        }

        private void BarcodeCountViewListButtonTapped(object sender, ListButtonTappedEventArgs args)
        {
            // Show the current progress but the order is not completed
            this.ShowList(isOrderCompleted: false);
        }

        public void ResumeScanning()
        {
            this.shouldCameraStandby = true;
            this.NavigationController?.PopViewController(animated: true);
        }

        public void RestartScanning()
        {
            this.ResetSession();
            this.shouldCameraStandby = true;
            this.NavigationController?.PopViewController(animated: true);
        }
    }
}
