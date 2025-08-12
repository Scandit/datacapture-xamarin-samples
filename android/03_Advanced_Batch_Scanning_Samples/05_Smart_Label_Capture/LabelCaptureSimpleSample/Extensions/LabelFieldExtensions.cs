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

using LabelCaptureSimpleSample.Data;
using System.Collections.Generic;
using System.Linq;
using Scandit.DataCapture.Label.Data;

namespace LabelCaptureSimpleSample.Extensions
{
    public static class LabelFieldExtensions
    {
        public static ScannedResult ToScannedResult(this IEnumerable<LabelField> fields)
        {
            var fieldDictionary = fields
                .Where(field =>
                {
                    var data = field.Barcode?.Data ?? field.Text;
                    return !string.IsNullOrWhiteSpace(data);
                })
                .ToDictionary(
                    field => field.Name,
                    field => field.Barcode?.Data ?? field.Text ?? string.Empty
                );

            return new ScannedResult(fieldDictionary);
        }
    }
}
