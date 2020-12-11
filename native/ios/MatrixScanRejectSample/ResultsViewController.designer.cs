//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace MatrixScanRejectSample
{
    [Register("ResultsViewController")]
    partial class ResultsViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton scanAgainButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITableView tableView { get; set; }

        [Action ("ScanAgainButton_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void ScanAgainButton_TouchUpInside (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (scanAgainButton != null) {
                scanAgainButton.Dispose ();
                scanAgainButton = null;
            }

            if (tableView != null) {
                tableView.Dispose ();
                tableView = null;
            }
        }
    }
}
