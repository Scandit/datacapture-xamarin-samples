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
using System.Linq;
using IdCaptureExtendedSample.Extensions;
using IdCaptureExtendedSample.Result.CellProviders;
using Scandit.DataCapture.ID.Data;

namespace IdCaptureExtendedSample.Result.Presenters
{
    public class SouthAfricaDlResultPresenter : IResultPresenter
    {
        public IList<ICellProvider> Rows { get; private set; }

        public SouthAfricaDlResultPresenter(CapturedId capturedId)
        {
            if (capturedId == null)
            {
                throw new ArgumentNullException(nameof(capturedId));
            }

            if (capturedId.CapturedResultType != CapturedResultType.SouthAfricaDlBarcodeResult)
            {
                throw new ArgumentException("Unexpected null SouthAfricaDlBarcodeResult");
            }

            this.Rows = capturedId.GetCommonRows()
                                  .Concat(GetSouthAfricaDlBarcodeRows(capturedId.SouthAfricaDlBarcode))
                                  .ToList();
        }

        private IList<ICellProvider> GetSouthAfricaDlBarcodeRows(SouthAfricaDlBarcodeResult southAfricaDlBarcodeResult)
        {
            var southAfricaDlRows = new[] {
                new SimpleTextCellProvider(value: $"({southAfricaDlBarcodeResult.Version})", title: "Version"),
                new SimpleTextCellProvider(value: southAfricaDlBarcodeResult.LicenseCountryOfIssue, title: "License Country of Issue"),
                new SimpleTextCellProvider(value: southAfricaDlBarcodeResult.PersonalIdNumber, title: "Personal Id Number"),
                new SimpleTextCellProvider(value: southAfricaDlBarcodeResult.PersonalIdNumberType, title: "Personal Id Number Type"),
                new SimpleTextCellProvider(value: $"({southAfricaDlBarcodeResult.DocumentCopy})", title: "Document Copy"),
                new SimpleTextCellProvider(value: string.Join(" ", southAfricaDlBarcodeResult.DriverRestrictionCodes), title: "Driver Restriction Codes")
            };

            if (southAfricaDlBarcodeResult.ProfessionalDrivingPermit != null)
            {
                var professionalDrivingPermit = southAfricaDlBarcodeResult.ProfessionalDrivingPermit;
                southAfricaDlRows.Append(
                    new SimpleTextCellProvider(value: string.Join(" ", professionalDrivingPermit.Codes),
                                               title: "Professional Driving Permit - Codes"));
                southAfricaDlRows.Append(
                    new SimpleTextCellProvider(value: professionalDrivingPermit.DateOfExpiry.Date.ToString(),
                                               title: "Professional Driving Permit - Date of Expiry"));
            }

            foreach(var vehicleRestriction in southAfricaDlBarcodeResult.VehicleRestrictions)
            {
                southAfricaDlRows.Append(
                    new SimpleTextCellProvider(value: vehicleRestriction.Code,
                                           title: "Vehicle Restriction - Vehicle Code"));
                southAfricaDlRows.Append(
                    new SimpleTextCellProvider(value: vehicleRestriction.Restriction,
                                           title: "Vehicle Restriction - Vehicle Restriction"));
                southAfricaDlRows.Append(
                    new SimpleTextCellProvider(value: vehicleRestriction.DateOfIssue.Date.ToString(),
                                           title: "Vehicle Restriction - Date of Issue"));
            }

            return southAfricaDlRows;
        }
    }
}
