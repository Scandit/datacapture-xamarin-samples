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

        public void OnIdCaptured(IdCapture idCapture, IdCaptureSession session, IFrameData frameData)
        {
            var capturedId = session.NewlyCapturedId;
            try
            {
                // Pause the idCapture to not capture while showing the result.
                idCapture.Enabled = false;

                string message;

                switch (capturedId.CapturedResultType)
                {
                    // The recognized fields of the captured Id can vary based on the capturedResultType.
                    case CapturedResultType.MrzResult:
                        message = GetDescriptionForMrzResult(capturedId);
                        break;
                    case CapturedResultType.AamvaBarcodeResult:
                        message = GetDescriptionForAamvaBarcodeResult(capturedId);
                        break;
                    case CapturedResultType.VizResult:
                        message = GetDescriptionForViz(capturedId);
                        break;
                    case CapturedResultType.UsUniformedServicesBarcodeResult:
                        message = GetDescriptionForUsUniformedServicesBarcodeResult(capturedId);
                        break;
                    case CapturedResultType.ColombiaIdBarcodeResult:
                        message = GetDescriptionForColombianIdResult(capturedId);
                        break;
                    case CapturedResultType.ArgentinaIdBarcodeResult:
                        message = GetDescriptionForArgentinianIdResult(capturedId);
                        break;
                    case CapturedResultType.SouthAfricaIdBarcodeResult:
                        message = GetDescriptionForSouthAfricanIdResult(capturedId);
                        break;
                    case CapturedResultType.SouthAfricaDlBarcodeResult:
                        message = GetDescriptionForSouthAfricaDlResult(capturedId);
                        break;
                    default:
                        message = GetDescriptionForCapturedId(capturedId);
                        break;
                }

                this.ShowResult(message);
            }
            finally
            {
                // Dispose the frame when you have finished processing it. If the frame is not properly disposed,
                // different issues could arise, e.g. a frozen, non-responsive, or "severely stuttering" video feed.
                frameData.Dispose();
            }
        }

        public void OnErrorEncountered(IdCapture idCapture, NSError error, IdCaptureSession session, IFrameData frameData)
        {
            try
            {
                // Implement to handle an error encountered during the capture process.
                // The error message can be retrieved from the Error localizedDescription.
                this.ShowResult(GetErrorMessage(error));
            }
            finally
            {
                // Dispose the frame when you have finished processing it. If the frame is not properly disposed,
                // different issues could arise, e.g. a frozen, non-responsive, or "severely stuttering" video feed.
                frameData.Dispose();
            }
        }

        public void OnIdLocalized(IdCapture capture, IdCaptureSession session, IFrameData frameData)
        {
            // In this sample we are not interested in this callback.

            // Dispose the frame when you have finished processing it. If the frame is not properly disposed,
            // different issues could arise, e.g. a frozen, non-responsive, or "severely stuttering" video feed.
            frameData.Dispose();
        }

        public void OnIdRejected(IdCapture idCapture, IdCaptureSession session, IFrameData frameData)
        {
            // Implement to handle documents recognized in a frame, but rejected.
            // A document or its part is considered rejected when (a) it's valid, but not enabled in the settings,
            // (b) it's a barcode of a correct symbology or a Machine Readable Zone (MRZ),
            // but the data is encoded in an unexpected/incorrect format.

            // Pause the idCapture to not capture while showing the result.
            idCapture.Enabled = false;
            this.ShowResult("Document not supported");

            // Dispose the frame when you have finished processing it. If the frame is not properly disposed,
            // different issues could arise, e.g. a frozen, non-responsive, or "severely stuttering" video feed.
            frameData.Dispose();
        }

        public void OnObservationStarted(IdCapture idCapture)
        {
            // In this sample we are not interested in this callback.
        }

        public void OnObservationStopped(IdCapture idCapture)
        {
            // In this sample we are not interested in this callback.
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

        private static string GetErrorMessage(NSError error)
        {
            int errCode = (int)error.Code;
            StringBuilder messageBuilder = new StringBuilder(((IdCaptureErrorCode)errCode).ToString());

            if (!string.IsNullOrEmpty(error.LocalizedDescription))
            {
                messageBuilder.Append($": {error.LocalizedDescription}");
            }

            return messageBuilder.ToString();
        }

        private static string GetDescriptionForCapturedId(CapturedId result)
        {
            var builder = new StringBuilder();
            AppendDescriptionForCapturedId(result, builder);

            return builder.ToString();
        }
        
        private static string GetDescriptionForSouthAfricaDlResult(CapturedId capturedId)
        {
            var builder = new StringBuilder();
            AppendDescriptionForCapturedId(capturedId, builder);
            var southAfricanDrivingLicense = capturedId.SouthAfricaDlBarcode;
            AppendField(builder, "Version: ", southAfricanDrivingLicense?.Version.ToString());
            AppendField(builder, "Document copy: ", southAfricanDrivingLicense?.DocumentCopy.ToString());
            AppendField(builder, "Personal ID number: ", southAfricanDrivingLicense?.PersonalIdNumber);
            AppendField(builder, "Personal ID number type: ", southAfricanDrivingLicense?.PersonalIdNumberType);
            return builder.ToString();
        }
        
        private static string GetDescriptionForSouthAfricanIdResult(CapturedId capturedId)
        {
            var builder = new StringBuilder();
            AppendDescriptionForCapturedId(capturedId, builder);
            var southAfricanId = capturedId.SouthAfricaIdBarcode;
            AppendField(builder, "Citizenship status: ", southAfricanId?.CitizenshipStatus);
            AppendField(builder, "Country of birth: ", southAfricanId?.CountryOfBirth);
            AppendField(builder, "Personal ID number: ", southAfricanId?.PersonalIdNumber);
            AppendField(builder, "Country of birth ISO: ", southAfricanId?.CountryOfBirthIso);
            return builder.ToString();
        }
        
        private static string GetDescriptionForArgentinianIdResult(CapturedId capturedId)
        {
            var builder = new StringBuilder();
            AppendDescriptionForCapturedId(capturedId, builder);
            var argentinianId = capturedId.ArgentinaIdBarcode;
            AppendField(builder, "Document copy: ", argentinianId?.DocumentCopy);
            AppendField(builder, "Personal ID number: ", argentinianId?.PersonalIdNumber);
            return builder.ToString();
        }

        private static string GetDescriptionForColombianIdResult(CapturedId capturedId)
        {
            var builder = new StringBuilder();
            AppendDescriptionForCapturedId(capturedId, builder);
            AppendField(builder, "Blood type: ", capturedId.ColombiaIdBarcode?.BloodType);
            return builder.ToString();
        }

        private static string GetDescriptionForMrzResult(CapturedId result)
        {
            StringBuilder builder = new StringBuilder();
            AppendDescriptionForCapturedId(result, builder);

            MrzResult mrzResult = result.Mrz;

            AppendField(builder, "Document Code: ", mrzResult.DocumentCode);
            AppendField(builder, "Names Are Truncated: ", mrzResult.NamesAreTruncated ? "Yes" : "No");
            AppendField(builder, "Optional: ", mrzResult.Optional ?? "<empty>");
            AppendField(builder, "Optional 1: ", mrzResult.Optional1 ?? "<empty>");

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

        private static string GetDescriptionForUsUniformedServicesBarcodeResult(CapturedId result)
        {
            StringBuilder builder = new StringBuilder();

            AppendDescriptionForCapturedId(result, builder);

            UsUniformedServicesBarcodeResult ususBarcodeResult = result.UsUniformedServicesBarcode;

            AppendField(builder, "Version: ", ususBarcodeResult.Version);
            AppendField(builder, "Sponsor Flag: ", ususBarcodeResult.SponsorFlag);
            AppendField(builder, "Person Designator Document: ", ususBarcodeResult.PersonDesignatorDocument);
            AppendField(builder, "Family Sequence Number: ", ususBarcodeResult.FamilySequenceNumber);
            AppendField(builder, "Deers Dependent Suffix Code: ", ususBarcodeResult.DeersDependentSuffixCode);
            AppendField(builder, "Deers Dependent Suffix Description: ", ususBarcodeResult.DeersDependentSuffixDescription);
            AppendField(builder, "Height: ", ususBarcodeResult.Height);
            AppendField(builder, "Weight: ", ususBarcodeResult.Weight);
            AppendField(builder, "Hair Color: ", ususBarcodeResult.HairColor);
            AppendField(builder, "Eye Color: ", ususBarcodeResult.EyeColor);
            AppendField(builder, "Direct Care Flag Code: ", ususBarcodeResult.DirectCareFlagCode);
            AppendField(builder, "Direct Care Flag Description: ", ususBarcodeResult.DirectCareFlagDescription);
            AppendField(builder, "Civilian Health Care Flag Code: ", ususBarcodeResult.CivilianHealthCareFlagCode);
            AppendField(builder, "Civilian Health Care Flag Description: ", ususBarcodeResult.CivilianHealthCareFlagDescription);
            AppendField(builder, "Commissary Flag Code: ", ususBarcodeResult.CommissaryFlagCode);
            AppendField(builder, "Commissary Flag Description: ", ususBarcodeResult.CommissaryFlagDescription);
            AppendField(builder, "MWR Flag Code: ", ususBarcodeResult.MwrFlagCode);
            AppendField(builder, "MWR Flag Description: ", ususBarcodeResult.MwrFlagDescription);
            AppendField(builder, "Exchange Flag Code: ", ususBarcodeResult.ExchangeFlagCode);
            AppendField(builder, "Exchange Flag Description: ", ususBarcodeResult.ExchangeFlagDescription);
            AppendField(builder, "Champus Effective Date: ", ususBarcodeResult.ChampusEffectiveDate?.ToString() ?? "<empty>");
            AppendField(builder, "Champus Expiry Date: ", ususBarcodeResult.ChampusExpiryDate?.ToString() ?? "<empty>");
            AppendField(builder, "Form Number: ", ususBarcodeResult.FormNumber);
            AppendField(builder, "Security Code: ", ususBarcodeResult.SecurityCode);
            AppendField(builder, "Service Code: ", ususBarcodeResult.ServiceCode);
            AppendField(builder, "Status Code: ", ususBarcodeResult.StatusCode);
            AppendField(builder, "Status Code Description: ", ususBarcodeResult.StatusCodeDescription);
            AppendField(builder, "Branch Of Service: ", ususBarcodeResult.BranchOfService);
            AppendField(builder, "Rank: ", ususBarcodeResult.Rank);
            AppendField(builder, "Pay Grade: ", ususBarcodeResult.PayGrade ?? "<empty>");
            AppendField(builder, "Sponsor Name: ", ususBarcodeResult.SponsorName ?? "<empty>");
            AppendField(builder, "Sponsor Person Designator Identifier: ", ususBarcodeResult.SponsorPersonDesignatorIdentifier);
            AppendField(builder, "Relationship Code: ", ususBarcodeResult.RelationshipCode ?? "<empty>");
            AppendField(builder, "Relationship Description: ", ususBarcodeResult.RelationshipDescription ?? "<empty>");
            AppendField(builder, "Geneva Convention Category: ", ususBarcodeResult.GenevaConventionCategory ?? "<empty>");
            AppendField(builder, "Blood Type: ", ususBarcodeResult.BloodType ?? "<empty>");

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

        private static void AppendField(StringBuilder builder, string name, nint value)
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
