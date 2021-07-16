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

using Android.App;
using Android.OS;
using Android.Views;
using AndroidX.AppCompat.Widget;
using Java.Lang;
using Scandit.DataCapture.Core.Data;
using Scandit.DataCapture.Core.Source;
using Scandit.DataCapture.Core.UI;
using Scandit.DataCapture.ID.Capture;
using Scandit.DataCapture.ID.Data;
using Scandit.DataCapture.ID.UI;
using Scandit.DataCapture.ID.UI.Overlay;

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

        public void OnIdCaptured(IdCapture mode, IdCaptureSession session, IFrameData data)
        {
            CapturedId capturedId = session.NewlyCapturedId;

            // This callback may be executed on an arbitrary thread. We post to switch back
            // to the main thread.
            this.view.Post(() =>
            {
                string message = string.Empty;

                // The recognized fields of the captured Id can vary based on the capturedResultType.
                if (capturedId.CapturedResultType == CapturedResultType.AamvaBarcodeResult)
                {
                    message = GetDescriptionForAamvaBarcodeResult(capturedId);
                }
                else if (capturedId.CapturedResultType == CapturedResultType.VizResult)
                { 
                    message = GetDescriptionForViz(capturedId);    
                }
                else
                {
                    message = GetDescriptionForCapturedId(capturedId);
                }

                this.ShowAlert(Resource.String.captured_id_title, message);
            });
        }

        public void OnErrorEncountered(IdCapture mode, Throwable error, IdCaptureSession session, IFrameData data)
        {
            // Implement to handle an error encountered during the capture process.
            // This callback may be executed on an arbitrary thread.
            this.view.Post(() =>
            {
                this.ShowAlert(Resource.String.error_title, GetErrorMessage(error));
            });
        }

        public void OnObservationStarted(IdCapture mode)
        {
            // In this sample we are not interested in this callback.
        }

        public void OnObservationStopped(IdCapture mode)
        {
            // In this sample we are not interested in this callback.
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

        private static string GetErrorMessage(Throwable error)
        {
            if (error is IdCaptureException idCaptureException)
            {
                StringBuilder messageBuilder = new StringBuilder(idCaptureException.GetKind().ToString());

                if (!string.IsNullOrEmpty(idCaptureException.Message))
                {
                    messageBuilder.Append($": {idCaptureException.Message}");
                }

                return messageBuilder.ToString();
            }
            else
            {
                return error.Message;
            }
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

        private static string GetDescriptionForAamvaBarcodeResult(CapturedId result)
        {
            StringBuilder builder = new StringBuilder();

            AppendDescriptionForCapturedId(result, builder);

            AamvaBarcodeResult aamvaBarcode = result.AamvaBarcode;

            AppendField(builder, "AAMVA Version: ", aamvaBarcode.AamvaVersion);
            AppendField(builder, "Jurisdiction Version: ", aamvaBarcode.JurisdictionVersion);
            AppendField(builder, "IIN: ", aamvaBarcode.IIN);
            AppendField(builder, "Issuing Jurisdiction: ", aamvaBarcode.IssuingJurisdiction);
            AppendField(builder, "Issuing Jurisdiction ISO: ", aamvaBarcode.IssuingJurisdictionIso);
            AppendField(builder, "Eye Color: ", aamvaBarcode.EyeColor);
            AppendField(builder, "Hair Color: ", aamvaBarcode.HairColor);
            AppendField(builder, "Height (inch): ", aamvaBarcode.HeightInch);
            AppendField(builder, "Height (cm): ", aamvaBarcode.HeightCm);
            AppendField(builder, "Weight (lbs): ", aamvaBarcode.WeightLbs);
            AppendField(builder, "Weight (kg): ", aamvaBarcode.WeightKg);
            AppendField(builder, "Place Of Birth: ", aamvaBarcode.PlaceOfBirth);
            AppendField(builder, "Race: ", aamvaBarcode.Race);
            AppendField(builder, "Document Discriminator Number: ", aamvaBarcode.DocumentDiscriminatorNumber);
            AppendField(builder, "Vehicle Class: ", aamvaBarcode.VehicleClass);
            AppendField(builder, "Restrictions Code: ", aamvaBarcode.RestrictionsCode);
            AppendField(builder, "Endorsements Code: ", aamvaBarcode.EndorsementsCode);
            AppendField(builder, "Card Revision Date: ", aamvaBarcode.CardRevisionDate);
            AppendField(builder, "Middle Name: ", aamvaBarcode.MiddleName);
            AppendField(builder, "Driver Name Suffix: ", aamvaBarcode.DriverNameSuffix);
            AppendField(builder, "Driver Name Prefix: ", aamvaBarcode.DriverNamePrefix);
            AppendField(builder, "Last Name Truncation: ", aamvaBarcode.LastNameTruncation);
            AppendField(builder, "First Name Truncation: ", aamvaBarcode.FirstNameTruncation);
            AppendField(builder, "Middle Name Truncation: ", aamvaBarcode.MiddleNameTruncation);
            AppendField(builder, "Alias Family Name: ", aamvaBarcode.AliasFamilyName);
            AppendField(builder, "Alias Given Name: ", aamvaBarcode.AliasGivenName);
            AppendField(builder, "Alias Suffix Name: ", aamvaBarcode.AliasSuffixName);

            return builder.ToString();
        }

        private static string GetDescriptionForViz(CapturedId result)
        {
            StringBuilder builder = new StringBuilder();

            AppendDescriptionForCapturedId(result, builder);

            VizResult viz = result.Viz;

            AppendField(builder, "Issuing Authority: ", viz.IssuingAuthority);
            AppendField(builder, "Issuing Jurisdiction: ", viz.IssuingJurisdiction);
            AppendField(builder, "Issuing Jurisdiction ISO: ", viz.IssuingJurisdictionIso);
            AppendField(builder, "Additional Name Information: ", viz.AdditionalNameInformation);
            AppendField(builder, "Additional Address Information: ", viz.AdditionalAddressInformation);
            AppendField(builder, "Place of Birth: ", viz.PlaceOfBirth);
            AppendField(builder, "Race: ", viz.Race);
            AppendField(builder, "Religion: ", viz.Religion);
            AppendField(builder, "Profession: ", viz.Profession);
            AppendField(builder, "Marital Status: ", viz.MaritalStatus);
            AppendField(builder, "Residential Status: ", viz.ResidentialStatus);
            AppendField(builder, "Employer: ", viz.Employer);
            AppendField(builder, "Personal Id Number: ", viz.PersonalIdNumber);
            AppendField(builder, "Document Additional Number: ", viz.DocumentAdditionalNumber);

            return builder.ToString();
        }

        private static void AppendDescriptionForCapturedId(CapturedId result, StringBuilder builder)
        {
            AppendField(builder, "Result Type: ", result.CapturedResultType.ToString());
            AppendField(builder, "Document Type: ", result.DocumentType.ToString());
            AppendField(builder, "First Name: ", result.FirstName);
            AppendField(builder, "Last Name: ", result.LastName);
            AppendField(builder, "Full Name: ", result.FullName);
            AppendField(builder, "Sex: ", result.Sex);
            AppendField(builder, "Date of Birth: ", result.DateOfBirth);
            AppendField(builder, "Nationality: ", result.Nationality);
            AppendField(builder, "Address: ", result.Address);
            AppendField(builder, "Issuing Country ISO: ", result.IssuingCountryIso);
            AppendField(builder, "Issuing Country: ", result.IssuingCountry);
            AppendField(builder, "Document Number: ", result.DocumentNumber);
            AppendField(builder, "Date of Expiry: ", result.DateOfExpiry);
            AppendField(builder, "Date of Issue: ", result.DateOfIssue);
        }

        private static void AppendField(StringBuilder builder, string name, int value)
        {
            builder.Append(name)
                   .Append(value.ToString())
                   .Append(System.Environment.NewLine);
        }

        private static void AppendField(StringBuilder builder, string name, int? value)
        {
            builder.Append(name);

            if (!value.HasValue)
            {
                builder.Append("<empty>");
            }
            else
            {
                builder.Append(value.Value);
            }

            builder.Append(System.Environment.NewLine);
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
