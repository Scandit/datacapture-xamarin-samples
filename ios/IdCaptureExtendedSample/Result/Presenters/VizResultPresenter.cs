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
    public class VizResultPresenter : IResultPresenter
    {
        public IList<ICellProvider> Rows { get; private set; }

        public VizResultPresenter(CapturedId capturedId)
        {
            if (capturedId == null)
            {
                throw new ArgumentNullException(nameof(capturedId));
            }

            if (capturedId.CapturedResultType != CapturedResultType.VizResult)
            {
                throw new ArgumentException("Unexpected null VizResult");
            }

            this.Rows = capturedId.GetCommonRows()
                                  .Concat(GetMrzRows(capturedId))
                                  .ToList();
        }

        private IList<ICellProvider> GetMrzRows(CapturedId capturedId)
        {
            var vizResult = capturedId.Viz;

            ICellProvider[] vizRows = new ICellProvider[] {
                new SimpleTextCellProvider(value: vizResult.AdditionalNameInformation, title: "Additional Name Information"),
                new SimpleTextCellProvider(value: vizResult.AdditionalAddressInformation, title: "Additional Address Information"),
                new SimpleTextCellProvider(value: vizResult.PlaceOfBirth, title: "Place of Birth"),
                new SimpleTextCellProvider(value: vizResult.Race, title: "Race"),
                new SimpleTextCellProvider(value: vizResult.Religion, title: "Religion"),
                new SimpleTextCellProvider(value: vizResult.Profession, title: "Profession"),
                new SimpleTextCellProvider(value: vizResult.MaritalStatus, title: "Marital Status"),
                new SimpleTextCellProvider(value: vizResult.ResidentialStatus, title: "Residential Status"),
                new SimpleTextCellProvider(value: vizResult.Employer, title: "Employer"),
                new SimpleTextCellProvider(value: vizResult.PersonalIdNumber, title: "Personal ID Number"),
                new SimpleTextCellProvider(value: vizResult.DocumentAdditionalNumber, title: "Document Additional Number"),
                new SimpleTextCellProvider(value: vizResult.IssuingJurisdiction, title: "Issuing Jurisdiction"),
                new SimpleTextCellProvider(value: vizResult.IssuingJurisdictionIso, title: "Issuing Jurisdiction ISO"),
                new SimpleTextCellProvider(value: vizResult.IssuingAuthority, title: "Issuing Authority"),
                new SimpleTextCellProvider(value: vizResult.CapturedSides.GetName(), title: "Captured Sides"),
                new SimpleTextCellProvider(value: vizResult.BackSideCaptureSupported ? "Yes" : "No", title: "Backside Supported"),
                new ImageCellProvider(image: capturedId.GetImageBitmapForType(IdImageType.Face), title: "Face Image"),
                new ImageCellProvider(image: capturedId.GetImageBitmapForType(IdImageType.IdFront), title: "Front Image"),
                new ImageCellProvider(image: capturedId.GetImageBitmapForType(IdImageType.IdBack), title: "Back Image")
            };

            return vizRows;
        }
    }
}
