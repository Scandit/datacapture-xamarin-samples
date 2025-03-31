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
using System.Text;
using CoreFoundation;
using Foundation;
using Scandit.DataCapture.Core.Data;
using Scandit.DataCapture.Core.Source;
using Scandit.DataCapture.Core.UI;
using Scandit.DataCapture.ID.Capture;
using Scandit.DataCapture.ID.Data;
using Scandit.DataCapture.ID.UI;
using Scandit.DataCapture.ID.UI.Overlay;
using UIKit;

namespace IdCaptureSimpleSample
{
    public partial class ViewController : UIViewController, IIdCaptureListener
    {
        private DataCaptureView dataCaptureView;
        private IdCaptureOverlay captureOverlay;

        public ViewController(IntPtr handle) : base(handle)
        {}

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            DataCaptureManager.Instance.InitializeAndStartScanning();

            // To visualize the on-going capturing process on screen, setup a data capture view
            // that renders the camera preview. The view must be connected to the data capture context.
            this.dataCaptureView = DataCaptureView.Create(DataCaptureManager.Instance.DataCaptureContext, this.View.Bounds);
            this.dataCaptureView.AutoresizingMask = UIViewAutoresizing.FlexibleHeight |
                                                    UIViewAutoresizing.FlexibleWidth;

            this.captureOverlay = IdCaptureOverlay.Create(DataCaptureManager.Instance.IdCapture, this.dataCaptureView);
            this.captureOverlay.IdLayoutStyle = IdLayoutStyle.Square;

            this.View.AddSubview(this.dataCaptureView);
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            // Start listening on IdCapture events.
            DataCaptureManager.Instance.IdCapture.AddListener(this);
            DataCaptureManager.Instance.IdCapture.Enabled = true;

            // Switch the camera on. The camera frames will be sent to TextCapture for processing.
            // Additionally the preview will appear on the screen. The camera is started asynchronously,
            // and you may notice a small delay before the preview appears.
            DataCaptureManager.Instance.Camera.SwitchToDesiredStateAsync(FrameSourceState.On);
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);

            DataCaptureManager.Instance.IdCapture.RemoveListener(this);
            DataCaptureManager.Instance.IdCapture.Enabled = false;

            // Switch the camera off to stop streaming frames. The camera is stopped asynchronously.
            DataCaptureManager.Instance.Camera.SwitchToDesiredStateAsync(FrameSourceState.Off);
        }

        public void OnIdCaptured(IdCapture idCapture, CapturedId capturedId)
        {
            // Pause the idCapture to not capture while showing the result.
            idCapture.Enabled = false;

            string message = GetDescriptionForCapturedId(capturedId);

            this.ShowResult(message);
        }


        public void OnIdRejected(IdCapture idCapture, CapturedId capturedId, RejectionReason reason)
        {
            String message = reason == RejectionReason.NotAcceptedDocumentType ?
                "Document not supported. Try scanning another document." : 
                $"Document capture was rejected. Reason={reason}.";

            // Pause the idCapture to not capture while showing the result.
            idCapture.Enabled = false;
            this.ShowResult(message);
        }

        public void ShowResult(string result)
        {
            DispatchQueue.MainQueue.DispatchAsync(() => {
                UIAlertController alert = UIAlertController.Create(result, message: null, preferredStyle: UIAlertControllerStyle.Alert);
                var action = UIAlertAction.Create("OK", UIAlertActionStyle.Default, (UIAlertAction) => {
                    DataCaptureManager.Instance.IdCapture.Enabled = true;
                });
                alert.AddAction(action);
                this.PresentViewController(alert, animated: true, completionHandler: () => { });
            });
        }

        private static string GetDescriptionForCapturedId(CapturedId result)
        {
            var builder = new StringBuilder();
            AppendDescriptionForCapturedId(result, builder);

            return builder.ToString();
        }
        
        private static void AppendDescriptionForCapturedId(CapturedId result, StringBuilder builder)
        {
            AppendField(builder, "Full Name: ", result.FullName);
            AppendField(builder, "Date of Birth: ", result.DateOfBirth);
            AppendField(builder, "Date of Expiry: ", result.DateOfExpiry);
            AppendField(builder, "Document Number: ", result.DocumentNumber);
            AppendField(builder, "Nationality: ", result.Nationality);
        }

        private static void AppendField(StringBuilder builder, string name, string value)
        {
            builder.Append(name);

            if (string.IsNullOrEmpty(value))
            {
                builder.Append("<empty>");
            }
            else
            {
                builder.Append(value);
            }

            builder.Append(System.Environment.NewLine);
        }

        private static void AppendField(StringBuilder builder, string name, DateResult value)
        {
            builder.Append(name);

            if (value == null)
            {
                builder.Append("<empty>");
            }
            else
            {
                builder.Append(value.Date.ToString());
            }

            builder.Append(System.Environment.NewLine);
        }
    }
}
