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
    public class UsUniformedServicesResultPresenter : IResultPresenter
    {
        public IList<ICellProvider> Rows { get; private set; }

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

        private IList<ICellProvider> GetUsUniformedServicesBarcodeRows(UsUniformedServicesBarcodeResult usUniformedServicesResult)
        {
            var usUniformedServicesBarcodeRows = new[] {
                new SimpleTextCellProvider(value: $"({usUniformedServicesResult.Version})", title: "Version"),
                new SimpleTextCellProvider(value: usUniformedServicesResult.SponsorFlag, title: "Sponsor Flag"),
                new SimpleTextCellProvider(value: $"({usUniformedServicesResult.PersonDesignatorDocument})", title: "Person Designator Document"),
                new SimpleTextCellProvider(value: $"({usUniformedServicesResult.FamilySequenceNumber})",  title: "Family Sequence Number"),
                new SimpleTextCellProvider(value: $"({usUniformedServicesResult.DeersDependentSuffixCode})", title: "Deers Dependent Suffic Code"),
                new SimpleTextCellProvider(value: usUniformedServicesResult.DeersDependentSuffixDescription, title: "Deers Dependent Suffix Description"),
                new SimpleTextCellProvider(value: $"({usUniformedServicesResult.Height})", title: "Height"),
                new SimpleTextCellProvider(value: $"({usUniformedServicesResult.Weight})", title: "Weight"),
                new SimpleTextCellProvider(value: usUniformedServicesResult.HairColor, title: "Hair Color"),
                new SimpleTextCellProvider(value: usUniformedServicesResult.EyeColor, title: "Eye Color"),
                new SimpleTextCellProvider(value: usUniformedServicesResult.DirectCareFlagCode, title: "Direct Care Flag Code"),
                new SimpleTextCellProvider(value: usUniformedServicesResult.DirectCareFlagDescription, title: "Direct Care Flag Descriptions"),
                new SimpleTextCellProvider(value: usUniformedServicesResult.CivilianHealthCareFlagCode, title: "Civilian Health Care Flag Code"),
                new SimpleTextCellProvider(value: usUniformedServicesResult.CivilianHealthCareFlagDescription, title: "Civilian Health Care Flag Description"),
                new SimpleTextCellProvider(value: usUniformedServicesResult.CommissaryFlagCode, title: "Commissary Flag Code"),
                new SimpleTextCellProvider(value: usUniformedServicesResult.CommissaryFlagDescription, title: "Commissary Flag Description"),
                new SimpleTextCellProvider(value: usUniformedServicesResult.MwrFlagCode, title: "MWR Flag Code"),
                new SimpleTextCellProvider(value: usUniformedServicesResult.MwrFlagDescription, title: "MWR Flag Description"),
                new SimpleTextCellProvider(value: usUniformedServicesResult.ExchangeFlagCode, title: "Exchange Flag Code"),
                new SimpleTextCellProvider(value: usUniformedServicesResult.ExchangeFlagDescription, title: "Exchange Flag Description"),
                new SimpleTextCellProvider(value: usUniformedServicesResult.ChampusEffectiveDate?.Date.ToString(), title: "Champus Effective Date"),
                new SimpleTextCellProvider(value: usUniformedServicesResult.ChampusExpiryDate?.Date.ToString(), title: "Champus Expiry Date"),
                new SimpleTextCellProvider(value: usUniformedServicesResult.FormNumber, title: "Form Number"),
                new SimpleTextCellProvider(value: usUniformedServicesResult.SecurityCode, title: "Security Code"),
                new SimpleTextCellProvider(value: usUniformedServicesResult.ServiceCode, title: "Service Code"),
                new SimpleTextCellProvider(value: usUniformedServicesResult.StatusCode, title: "Status Code"),
                new SimpleTextCellProvider(value: usUniformedServicesResult.StatusCodeDescription, title: "Status Code Description"),
                new SimpleTextCellProvider(value: usUniformedServicesResult.BranchOfService, title: "Branch of Service"),
                new SimpleTextCellProvider(value: usUniformedServicesResult.Rank, title: "Rank"),
                new SimpleTextCellProvider(value: usUniformedServicesResult.PayGrade, title: "Pay Grade"),
                new SimpleTextCellProvider(value: usUniformedServicesResult.SponsorName, title: "Sponsor Name"),
                new SimpleTextCellProvider(value: usUniformedServicesResult.SponsorPersonDesignatorIdentifier?.ToString(), title: "Sponsor Person Designator Identifier"),
                new SimpleTextCellProvider(value: usUniformedServicesResult.RelationshipCode, title: "Relationship Code"),
                new SimpleTextCellProvider(value: usUniformedServicesResult.GenevaConventionCategory, title: "Geneva Convention Category"),
                new SimpleTextCellProvider(value: usUniformedServicesResult.RelationshipDescription, title: "Relationship Description"),
                new SimpleTextCellProvider(value: usUniformedServicesResult.BloodType, title: "Blood Type")
            };

            return usUniformedServicesBarcodeRows;
        }
    }
}
