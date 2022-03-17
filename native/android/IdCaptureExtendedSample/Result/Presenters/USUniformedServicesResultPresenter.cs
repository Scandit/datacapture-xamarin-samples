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
    public class UsUniformedServicesResultPresenter : IResultPresenter
    {
        public IList<ResultEntry> Rows { get; private set; }

        public UsUniformedServicesResultPresenter(CapturedId capturedId)
        {
            if (capturedId == null)
            {
                throw new ArgumentNullException(nameof(capturedId));
            }

            if (capturedId.CapturedResultType != CapturedResultType.UsUniformedServicesBarcodeResult)
            {
                throw new ArgumentException("Unexpected null UsUniformedServicesBarcodeResult");
            }

            this.Rows = capturedId.GetCommonRows()
                                  .Concat(GetUsUniformedServicesBarcodeRows(capturedId.UsUniformedServicesBarcode))
                                  .ToList();
        }

        private IList<ResultEntry> GetUsUniformedServicesBarcodeRows(UsUniformedServicesBarcodeResult usUniformedServicesResult)
        {
            var usUniformedServicesBarcodeRows = new[] {
                new ResultEntry(value: $"({usUniformedServicesResult.Version})", title: "Version"),
                new ResultEntry(value: usUniformedServicesResult.SponsorFlag, title: "Sponsor Flag"),
                new ResultEntry(value: $"({usUniformedServicesResult.PersonDesignatorDocument})", title: "Person Designator Document"),
                new ResultEntry(value: $"({usUniformedServicesResult.FamilySequenceNumber})",  title: "Family Sequence Number"),
                new ResultEntry(value: $"({usUniformedServicesResult.DeersDependentSuffixCode})", title: "Deers Dependent Suffic Code"),
                new ResultEntry(value: usUniformedServicesResult.DeersDependentSuffixDescription, title: "Deers Dependent Suffix Description"),
                new ResultEntry(value: $"({usUniformedServicesResult.Height})", title: "Height"),
                new ResultEntry(value: $"({usUniformedServicesResult.Weight})", title: "Weight"),
                new ResultEntry(value: usUniformedServicesResult.HairColor, title: "Hair Color"),
                new ResultEntry(value: usUniformedServicesResult.EyeColor, title: "Eye Color"),
                new ResultEntry(value: usUniformedServicesResult.DirectCareFlagCode, title: "Direct Care Flag Code"),
                new ResultEntry(value: usUniformedServicesResult.DirectCareFlagDescription, title: "Direct Care Flag Descriptions"),
                new ResultEntry(value: usUniformedServicesResult.CivilianHealthCareFlagCode, title: "Civilian Health Care Flag Code"),
                new ResultEntry(value: usUniformedServicesResult.CivilianHealthCareFlagDescription, title: "Civilian Health Care Flag Description"),
                new ResultEntry(value: usUniformedServicesResult.CommissaryFlagCode, title: "Commissary Flag Code"),
                new ResultEntry(value: usUniformedServicesResult.CommissaryFlagDescription, title: "Commissary Flag Description"),
                new ResultEntry(value: usUniformedServicesResult.MwrFlagCode, title: "MWR Flag Code"),
                new ResultEntry(value: usUniformedServicesResult.MwrFlagDescription, title: "MWR Flag Description"),
                new ResultEntry(value: usUniformedServicesResult.ExchangeFlagCode, title: "Exchange Flag Code"),
                new ResultEntry(value: usUniformedServicesResult.ExchangeFlagDescription, title: "Exchange Flag Description"),
                new ResultEntry(value: usUniformedServicesResult.ChampusEffectiveDate?.Date.ToString(), title: "Champus Effective Date"),
                new ResultEntry(value: usUniformedServicesResult.ChampusExpiryDate?.Date.ToString(), title: "Champus Expiry Date"),
                new ResultEntry(value: usUniformedServicesResult.FormNumber, title: "Form Number"),
                new ResultEntry(value: usUniformedServicesResult.SecurityCode, title: "Security Code"),
                new ResultEntry(value: usUniformedServicesResult.ServiceCode, title: "Service Code"),
                new ResultEntry(value: usUniformedServicesResult.StatusCode, title: "Status Code"),
                new ResultEntry(value: usUniformedServicesResult.StatusCodeDescription, title: "Status Code Description"),
                new ResultEntry(value: usUniformedServicesResult.BranchOfService, title: "Branch of Service"),
                new ResultEntry(value: usUniformedServicesResult.Rank, title: "Rank"),
                new ResultEntry(value: usUniformedServicesResult.PayGrade, title: "Pay Grade"),
                new ResultEntry(value: usUniformedServicesResult.SponsorName, title: "Sponsor Name"),
                new ResultEntry(value: usUniformedServicesResult.SponsorPersonDesignatorIdentifier?.ToString(), title: "Sponsor Person Designator Identifier"),
                new ResultEntry(value: usUniformedServicesResult.RelationshipCode, title: "Relationship Code"),
                new ResultEntry(value: usUniformedServicesResult.GenevaConventionCategory, title: "Geneva Convention Category"),
                new ResultEntry(value: usUniformedServicesResult.RelationshipDescription, title: "Relationship Description"),
                new ResultEntry(value: usUniformedServicesResult.BloodType, title: "Blood Type")
            };

            return usUniformedServicesBarcodeRows;
        }
    }
}
