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
using IdCaptureExtendedSample.Result;
using Scandit.DataCapture.ID.Data;

namespace IdCaptureExtendedSample.Extensions
{
    public static class CapturedIdExtentions
    {
        public static IList<ResultEntry> GetCommonRows(this CapturedId capturedId)
        {
            if (capturedId == null)
            {
                throw new ArgumentNullException(nameof(capturedId));
            }

            return new[] {
                new ResultEntry(value: capturedId.FirstName, title: "Name"),
                new ResultEntry(value: capturedId.LastName, title: "Lastname"),
                new ResultEntry(value: capturedId.FullName, title: "Full Name"),
                new ResultEntry(value: capturedId.Sex, title: "Sex"),
                new ResultEntry(value: capturedId.DateOfBirth?.Date.ToShortDateString(), title: "Date of Birth"),
                new ResultEntry(value: capturedId.Nationality, title: "Nationality"),
                new ResultEntry(value: capturedId.Address, title: "Address"),
                new ResultEntry(value: capturedId.CapturedResultTypes.GetResultTypes(), title: "Captured Result Types"),
                new ResultEntry(value: capturedId.DocumentType.Name(), title: "Document Type"),
                new ResultEntry(value: capturedId.IssuingCountryIso, title: "Issuing Country ISO"),
                new ResultEntry(value: capturedId.IssuingCountry, title: "Issuing Country"),
                new ResultEntry(value: capturedId.DocumentNumber, title: "Document Number"),
                new ResultEntry(value: capturedId.DateOfExpiry?.Date.ToShortDateString(), title: "Date of Expiry"),
                new ResultEntry(value: capturedId.DateOfIssue?.Date.ToShortDateString(), title: "Date of Issue")
            };
        }

        private static string GetResultTypes(this CapturedResultType resultType)
        {
            return string.Join(", ",
                Enum.GetValues(typeof(CapturedResultType))
                    .Cast<Enum>()
                    .Where(m => resultType.HasFlag(m))
                    .Cast<CapturedResultType>()
                    .Select(i => i.ToString()));
        }
    }
}
