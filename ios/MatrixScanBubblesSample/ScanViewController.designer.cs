// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System.CodeDom.Compiler;

namespace MatrixScanBubblesSample
{
    [Register ("ScanViewController")]
    partial class ScanViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton freezeButton { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (freezeButton != null) {
                freezeButton.Dispose ();
                freezeButton = null;
            }
        }
    }
}