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
using Scandit.DataCapture.ID.Data;

namespace IdCaptureExtendedSample.Result.Presenters
{
    public class CommonResultPresenter : IResultPresenter
    {
        public IList<ResultEntry> Rows { get; private set; }

        public CommonResultPresenter(CapturedId capturedId)
        {
            if (capturedId == null)
            {
                throw new ArgumentNullException(nameof(capturedId));
            }

            this.Rows = GetRows(capturedId);
        }

        private IList<ResultEntry> GetRows(CapturedId result)
        {
            var rows = new[] {
                new ResultEntry(value: result.FullName, title: "Full Name"),
                new ResultEntry(value: result.DateOfBirth?.UtcDate.ToShortDateString(), title: "Date of Birth"),
                new ResultEntry(value: result.DateOfExpiry?.UtcDate.ToShortDateString(), title: "Date of Expiry"),
                new ResultEntry(value: result.DocumentNumber, title: "Document Number"),
                new ResultEntry(value: result.Nationality, title: "Nationality")
            };

            return rows;
        }
    }
}
