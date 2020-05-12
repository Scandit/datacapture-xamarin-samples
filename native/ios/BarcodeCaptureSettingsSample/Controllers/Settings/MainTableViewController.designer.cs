// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System.CodeDom.Compiler;

namespace BarcodeCaptureSettingsSample.Controllers.Settings
{
    [Register ("MainTableViewController")]
    partial class MainTableViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel versionLabel { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (versionLabel != null) {
                versionLabel.Dispose ();
                versionLabel = null;
            }
        }
    }
}