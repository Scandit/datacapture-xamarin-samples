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
using IdCaptureExtendedSample.Result.CellProviders;
using Scandit.DataCapture.ID.Data;

namespace IdCaptureExtendedSample.Extensions
{
    public static class CapturedIdExtentions
    {
        public static IList<ICellProvider> GetCommonRows(this CapturedId capturedId)
        {
            if (capturedId == null)
            {
                throw new ArgumentNullException(nameof(capturedId));
            }

            return new[] {
                new SimpleTextCellProvider(value: capturedId.FirstName, title: "Name"),
                new SimpleTextCellProvider(value: capturedId.LastName, title: "Lastname"),
                new SimpleTextCellProvider(value: capturedId.FullName, title: "Full Name"),
                new SimpleTextCellProvider(value: capturedId.Sex, title: "Sex"),
                new SimpleTextCellProvider(value: capturedId.DateOfBirth?.Date.ToShortDateString(), title: "Date of Birth"),
                new SimpleTextCellProvider(value: capturedId.Nationality, title: "Nationality"),
                new SimpleTextCellProvider(value: capturedId.Address, title: "Address"),
                new SimpleTextCellProvider(value: capturedId.CapturedResultType.GetName(), title: "Captured Result Type"),
                new SimpleTextCellProvider(value: capturedId.DocumentType.GetName(), title: "Document Type"),
                new SimpleTextCellProvider(value: capturedId.IssuingCountryIso, title: "Issuing Country ISO"),
                new SimpleTextCellProvider(value: capturedId.IssuingCountry, title: "Issuing Country"),
                new SimpleTextCellProvider(value: capturedId.DocumentNumber, title: "Document Number"),
                new SimpleTextCellProvider(value: capturedId.DateOfExpiry?.Date.ToShortDateString(), title: "Date of Expiry"),
                new SimpleTextCellProvider(value: capturedId.DateOfIssue?.Date.ToShortDateString(), title: "Date of Issue")
            };
        }
    }
}
