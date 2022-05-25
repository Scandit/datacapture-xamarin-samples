// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace BarcodeCaptureViewsSample.Modes.SplitView
{
	[Register ("SplitViewScannerViewController")]
	partial class SplitViewScannerViewController
	{
		[Outlet]
		UIKit.UILabel TapToContinueLabel { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (TapToContinueLabel != null) {
				TapToContinueLabel.Dispose ();
				TapToContinueLabel = null;
			}
		}
	}
}
