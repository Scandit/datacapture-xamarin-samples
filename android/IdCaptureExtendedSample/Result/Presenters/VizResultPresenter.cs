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
using Scandit.DataCapture.ID.Data;

namespace IdCaptureExtendedSample.Result.Presenters
{
    public class VizResultPresenter : IResultPresenter
    {
        public IList<ResultEntry> Rows { get; private set; }

        public VizResultPresenter(CapturedId capturedId)
        {
            if (capturedId == null)
            {
                throw new ArgumentNullException(nameof(capturedId));
            }

            if (!capturedId.CapturedResultTypes.HasFlag(CapturedResultType.VizResult))
            {
                throw new ArgumentException("Unexpected null VizResult");
            }

            this.Rows = this.GetMrzRows(capturedId).ToList();
        }

        private IList<ResultEntry> GetMrzRows(CapturedId capturedId)
        {
            var vizResult = capturedId.Viz;

            ResultEntry[] vizRows = new ResultEntry[] {
                new ResultEntry(value: vizResult.AdditionalNameInformation, title: "Additional Name Information"),
                new ResultEntry(value: vizResult.AdditionalAddressInformation, title: "Additional Address Information"),
                new ResultEntry(value: vizResult.PlaceOfBirth, title: "Place of Birth"),
                new ResultEntry(value: vizResult.Race, title: "Race"),
                new ResultEntry(value: vizResult.Religion, title: "Religion"),
                new ResultEntry(value: vizResult.Profession, title: "Profession"),
                new ResultEntry(value: vizResult.MaritalStatus, title: "Marital Status"),
                new ResultEntry(value: vizResult.ResidentialStatus, title: "Residential Status"),
                new ResultEntry(value: vizResult.Employer, title: "Employer"),
                new ResultEntry(value: vizResult.PersonalIdNumber, title: "Personal ID Number"),
                new ResultEntry(value: vizResult.DocumentAdditionalNumber, title: "Document Additional Number"),
                new ResultEntry(value: vizResult.IssuingJurisdiction, title: "Issuing Jurisdiction"),
                new ResultEntry(value: vizResult.IssuingJurisdictionIso, title: "Issuing Jurisdiction ISO"),
                new ResultEntry(value: vizResult.IssuingAuthority, title: "Issuing Authority"),
                new ResultEntry(value: vizResult.CapturedSides.Name(), title: "Captured Sides"),
                new ResultEntry(value: vizResult.BackSideCaptureSupported ? "Yes" : "No", title: "Backside Supported")
            };

            return vizRows;
        }
    }
}
