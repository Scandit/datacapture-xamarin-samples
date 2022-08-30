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
using BarcodeCaptureViewsSample.Models;
using CoreFoundation;
using Scandit.DataCapture.Barcode.Capture;
using Scandit.DataCapture.Barcode.Data;
using UIKit;

namespace BarcodeCaptureViewsSample.Modes.SplitView
{
    public partial class SplitViewModeViewController : UIViewController
    {
        private SplitViewTableController tableViewController;
        private SplitViewScannerViewController scannerViewController;

        public SplitViewModeViewController(IntPtr handle) : base(handle)
        { }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            this.NavigationItem.RightBarButtonItem = new UIBarButtonItem(title: "Clear",
                                                                         style: UIBarButtonItemStyle.Plain,
                                                                         handler: ClearTableView);
            this.tableViewController = new SplitViewTableController();
            this.scannerViewController = this.ChildViewControllers
                .Where(c => c is SplitViewScannerViewController)
                .Cast<SplitViewScannerViewController>()
                .First();
            this.scannerViewController.BarcodeScanned += BarcodeScannedHandler;
            this.SetupTableView();
        }

        private void ClearTableView(object sender, EventArgs args)
        {
            this.tableViewController.Clear();
        }

        private void BarcodeScannedHandler(object sender, BarcodeCaptureEventArgs args)
        {
            var barcode = args.Session?.NewlyRecognizedBarcodes.FirstOrDefault();

            if (barcode == null)
            {
                return;
            }

            // Get the human readable name of the symbology and assemble the result to be shown.
            string symbology = SymbologyDescription.Create(barcode.Symbology).ReadableName;

            DispatchQueue.MainQueue.DispatchAsync(() =>
            {
                this.tableViewController.Add(new ScanResult(symbology, barcode.Data));
            });
        }

        private void SetupTableView()
        {
            AddChildViewController(this.tableViewController);
            this.View.AddSubview(this.tableViewController.View);
            this.tableViewController.DidMoveToParentViewController(this);
            this.tableViewController.View.TranslatesAutoresizingMaskIntoConstraints = false;
            this.View.AddConstraints(new[]
            {
                this.tableViewController.View.LeadingAnchor.ConstraintEqualTo(this.View.LeadingAnchor),
                this.tableViewController.View.TrailingAnchor.ConstraintEqualTo(this.View.TrailingAnchor),
                this.tableViewController.View.TopAnchor.ConstraintEqualTo(this.scannerViewController.View.BottomAnchor),
                this.tableViewController.View.BottomAnchor.ConstraintEqualTo(View.BottomAnchor)
            });
        }
    }
}
