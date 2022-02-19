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
using BarcodeCaptureSettingsSample.Model;
using CoreFoundation;
using Scandit.DataCapture.Barcode.Capture;
using Scandit.DataCapture.Barcode.Data;
using Scandit.DataCapture.Core.Capture;
using Scandit.DataCapture.Core.Data;
using Scandit.DataCapture.Core.Source;
using Scandit.DataCapture.Core.UI;
using UIKit;

namespace BarcodeCaptureSettingsSample.Controllers
{
    public partial class ScanViewController : UIViewController, IBarcodeCaptureListener
    {
        private const int ShownDurationInContinuousModeInMilliSecs = 500;
        private Timer continuousResultTimer;

        public ScanViewController(IntPtr handle) : base(handle) { }

        public DataCaptureContext Context => SettingsManager.Instance.Context;

        public Camera Camera => SettingsManager.Instance.Camera;

        public BarcodeCapture BarcodeCapture => SettingsManager.Instance.BarcodeCapture;

        private DataCaptureView dataCaptureView;
        private UIView continuousResultView;
        private UILabel continuousText;
        private NSLayoutConstraint continuousAnchor;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            this.SetupRecognition();
            this.CreateResultViewForContinuousScan();

            if (!UIDevice.CurrentDevice.CheckSystemVersion(11, 0))
            {
                this.EdgesForExtendedLayout = UIRectEdge.None;
            }

            this.continuousResultTimer = new Timer(ShownDurationInContinuousModeInMilliSecs)
            {
                AutoReset = false,
                Enabled = false
            };
            this.continuousResultTimer.Elapsed += (object sender, ElapsedEventArgs e) =>
            {
                if (SettingsManager.Instance.ContinuousModeEnabled)
                {
                    DispatchQueue.MainQueue.DispatchAsync(() => this.continuousResultView.Hidden = true);
                }
            };
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            // Switch camera on to start streaming frames.
            // The camera is started asynchronously and will take some time to
            // completely turn on.
            this.BarcodeCapture.Enabled = true;
            this.Camera?.SwitchToDesiredStateAsync(FrameSourceState.On);
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
            // Switch camera off to stop streaming frames. The camera is
            // stopped asynchronously and will take some time to
            // completely turn off. Until it is completely stopped,
            // it is still possible to receive further results, hence
            // it's a good idea to first disable barcode tracking as well.
            this.BarcodeCapture.Enabled = false;
            this.Camera?.SwitchToDesiredStateAsync(FrameSourceState.Off);
        }

        public override void ViewDidLayoutSubviews()
        {
            base.ViewDidLayoutSubviews();

            this.View.RemoveConstraint(this.continuousAnchor);
            var height = this.NavigationController.NavigationBar.Frame.Height + UIApplication.SharedApplication.StatusBarFrame.Height;
            this.continuousAnchor = this.continuousResultView.TopAnchor.ConstraintEqualTo(this.View.TopAnchor, height);
            this.View.AddConstraint(this.continuousAnchor);
            this.View.LayoutIfNeeded();
        }

        private void SetupRecognition()
        {
            // Register this as a listener to get informed whenever a
            // new barcode got recognized.
            this.BarcodeCapture.AddListener(this);

            // To visualize the on-going barcode capturing process on screen,
            // setup a data capture view that renders the camera preview.
            // The view must be connected to the data capture context.
            this.dataCaptureView = DataCaptureView.Create(Context, View.Bounds);
            this.dataCaptureView.AutoresizingMask = UIViewAutoresizing.FlexibleWidth |
                                                    UIViewAutoresizing.FlexibleHeight;
            this.View.AddSubview(this.dataCaptureView);
            this.View.SendSubviewToBack(this.dataCaptureView);
            SettingsManager.Instance.CaptureView = this.dataCaptureView;
        }

        private void ShowResult(IList<Barcode> barcodes, Action completion)
        {
            DispatchQueue.MainQueue.DispatchAsync(() =>
            {
                var result = new ScanResult(barcodes);

                if (!SettingsManager.Instance.ContinuousModeEnabled)
                {
                    var alert = UIAlertController.Create("Scan Results", result.Text, UIAlertControllerStyle.Alert);
                    alert.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Default, action =>
                    {
                        DismissViewController(true, null);
                        completion.Invoke();
                    }));
                    this.PresentViewController(alert, true, completionHandler: null);
                }
                else
                {
                    this.ShowViewForContinuousScanning(result.Text);
                }
            });
        }

        private void ShowViewForContinuousScanning(string text)
        {
            this.continuousResultTimer.Stop();
            this.continuousResultTimer.Start();

            this.continuousText.Text = text;
            this.continuousResultView.Hidden = false;
        }

        #region IBarcodeCaptureListener
        public void OnBarcodeScanned(BarcodeCapture barcodeCapture, BarcodeCaptureSession session, IFrameData frameData)
        {
            if (!SettingsManager.Instance.ContinuousModeEnabled)
            {
                // Stop recognizing barcodes for as long as we are
                // displaying the result. There won't be any new results until
                // the capture mode is enabled again.
                // Note that disabling the capture mode does not stop
                // the camera, the camera continues to stream frames
                // until it is turned off.
                this.BarcodeCapture.Enabled = false;
            }

            this.ShowResult(session.NewlyRecognizedBarcodes, completion: () =>
            {
                if (!SettingsManager.Instance.ContinuousModeEnabled)
                {
                    // Enable recognizing barcodes when the result
                    // is not shown anymore.
                    this.BarcodeCapture.Enabled = true;
                }
            });

            frameData.Dispose();
        }

        public void OnSessionUpdated(BarcodeCapture barcodeCapture, BarcodeCaptureSession session, IFrameData frameData)
        {
            frameData.Dispose();
        }

        public void OnObservationStarted(BarcodeCapture barcodeCapture) { }

        public void OnObservationStopped(BarcodeCapture barcodeCapture) { }
        #endregion

        private void CreateResultViewForContinuousScan()
        {
            this.continuousResultView = new UIView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                BackgroundColor = UIColor.White,
                Hidden = true
            };

            this.continuousText = new UILabel
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                TextAlignment = UITextAlignment.Left,
                TextColor = UIColor.Black,
                Lines = 0
            };

            this.continuousResultView.AddSubview(this.continuousText);
            this.continuousResultView.AddConstraints(new[]
            {
                this.continuousText.LeadingAnchor.ConstraintEqualTo(this.continuousResultView.LeadingAnchor, 15),
                this.continuousText.TopAnchor.ConstraintEqualTo(this.continuousResultView.TopAnchor, 15),
                this.continuousText.TrailingAnchor.ConstraintEqualTo(this.continuousResultView.TrailingAnchor, -15),
                this.continuousText.BottomAnchor.ConstraintEqualTo(this.continuousResultView.BottomAnchor, -15)
            });
            
            this.View.AddSubview(this.continuousResultView);
            var height = this.NavigationController.NavigationBar.Frame.Height + UIApplication.SharedApplication.StatusBarFrame.Height;
            this.continuousAnchor = this.continuousResultView.TopAnchor.ConstraintEqualTo(this.View.TopAnchor, height);
            this.View.AddConstraints(new[]
            {
                this.continuousResultView.LeadingAnchor.ConstraintEqualTo(this.View.LeadingAnchor),
                this.continuousAnchor,
                this.continuousResultView.TrailingAnchor.ConstraintEqualTo(this.View.TrailingAnchor),
            });
        }
    }
}
