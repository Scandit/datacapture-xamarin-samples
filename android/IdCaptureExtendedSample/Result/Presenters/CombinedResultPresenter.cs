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

using System.Collections.Generic;
using System.Linq;
using Scandit.DataCapture.ID.Data;
using IdCaptureExtendedSample.Extensions;

namespace IdCaptureExtendedSample.Result.Presenters
{
    public class CombinedResultPresenter : IResultPresenter
    {
        public IList<ResultEntry> Rows { get; private set; }

        public CombinedResultPresenter(CapturedId capturedId)
        {
            this.Rows = capturedId.GetCommonRows();
        }

        public CombinedResultPresenter(CapturedId capturedId, IList<IResultPresenter> presenters)
        {
            this.Rows = capturedId.GetCommonRows()
                                  .Concat(presenters.SelectMany(p => p.Rows))
                                  .ToList();
        }
    }
}
