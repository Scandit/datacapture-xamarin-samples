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

using System.Text;
using Android;
using Android.App;
using Android.OS;
using Android.Views;
using AndroidX.AppCompat.Widget;
using Scandit.DataCapture.Core.Source;
using Scandit.DataCapture.Core.UI;
using Scandit.DataCapture.ID.Capture;
using Scandit.DataCapture.ID.Data;
using Scandit.DataCapture.ID.UI;
using Scandit.DataCapture.ID.UI.Overlay;
using String = System.String;

namespace IdCaptureSimpleSample
{
    [Activity(MainLauncher = true, Label = "@string/app_name")]
    public class IdCaptureActivity : CameraPermissionActivity, IIdCaptureListener, AlertDialogFragment.ICallbacks
    {
        private static readonly string ResultFragmentTag = "result_fragment";

        private DataCaptureView view;
        private IdCaptureOverlay overlay;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            this.SetContentView(Resource.Layout.activity_main);

            Toolbar toolbar = this.FindViewById<Toolbar>(Resource.Id.toolbar);
            this.SetSupportActionBar(toolbar);
            this.SupportActionBar.SetDisplayHomeAsUpEnabled(true);

            ViewGroup container = this.FindViewById<ViewGroup>(Resource.Id.data_capture_view_container);

            // Create a new DataCaptureView and fill the screen with it. DataCaptureView will show
            // the camera preview on the screen. Pass your DataCaptureContext to the DataCaptureView.
            this.view = DataCaptureView.Create(this, DataCaptureManager.Instance.DataCaptureContext);
            container.AddView(view, new Android.Widget.FrameLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent));

            this.overlay = IdCaptureOverlay.Create(DataCaptureManager.Instance.IdCapture, this.view);
            this.overlay.IdLayoutStyle = IdLayoutStyle.Square;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    this.OnBackPressed();
                    return true;
                default:
                    return base.OnOptionsItemSelected(item);
            }
        }

        public void OnIdCaptured(IdCapture mode, CapturedId capturedId)
        {
            // This callback may be executed on an arbitrary thread. We post to switch back
            // to the main thread.
            this.view.Post(() =>
            {
                string message = GetDescriptionForCapturedId(capturedId);
                this.ShowAlert(Resource.String.captured_id_title, message);
            });
        }

        public void OnIdRejected(IdCapture mode, CapturedId capturedId, RejectionReason reason)
        {
            String message = reason == RejectionReason.NotAcceptedDocumentType ?
                "Document not supported. Try scanning another document." : 
                $"Document capture was rejected. Reason={reason}.";

            // This callback may be executed on an arbitrary thread.
            this.view.Post(() =>
            {
                this.ShowAlert(Resource.String.captured_id_title, message);
            });
        }

        protected override void OnResumeFragments()
        {
            base.OnResumeFragments();

            // Check for camera permission and request it, if it hasn't yet been granted.
            // Once we have the permission the onCameraPermissionGranted() method will be called.
            this.RequestCameraPermission();
        }

        protected override void OnCameraPermissionGranted()
        {
            // Start listening on IdCapture events.
            DataCaptureManager.Instance.IdCapture.AddListener(this);

            // Switch the camera on. The camera frames will be sent to TextCapture for processing.
            // Additionally the preview will appear on the screen. The camera is started asynchronously,
            // and you may notice a small delay before the preview appears.
            DataCaptureManager.Instance.Camera.SwitchToDesiredStateAsync(FrameSourceState.On);
        }

        protected override void OnPause()
        {
            base.OnPause();

            // Switch the camera off to stop streaming frames. The camera is stopped asynchronously.
            DataCaptureManager.Instance.Camera.SwitchToDesiredStateAsync(FrameSourceState.Off);
            DataCaptureManager.Instance.IdCapture.RemoveListener(this);
        }

        private void ShowAlert(int titleRes, string message)
        {
            // Show the result fragment only if we are not displaying one at the moment.
            if (this.SupportFragmentManager.FindFragmentByTag(ResultFragmentTag) == null)
            {
                AlertDialogFragment.Create(titleRes, message)
                                   .Show(this.SupportFragmentManager, ResultFragmentTag);

                // Don't capture unnecessarily when the result is displayed.
                DataCaptureManager.Instance.IdCapture.Enabled = false;
            }
        }

        public void OnResultDismissed()
        {
            // Enable capture again, after the result dialog is dismissed.
            DataCaptureManager.Instance.IdCapture.Enabled = true;
        }

        private static string GetDescriptionForCapturedId(CapturedId result)
        {
            StringBuilder builder = new StringBuilder();
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
