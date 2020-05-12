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
using System.Globalization;

namespace BarcodeCaptureSettingsSample.Extensions
{
    public class NumberFormatter
    {
        public static readonly NumberFormatter Instance = new NumberFormatter();

        private readonly NumberFormatInfo formatInfo;

        private NumberFormatter()
        {
            this.formatInfo = new NumberFormatInfo()
            {
                NumberDecimalSeparator = ","
            };
        }

        public string FormatNFloat(nfloat number, int decimalPlaces = 2)
        {
            return number.ToString($"F{decimalPlaces}", this.formatInfo);
        }

        public nfloat ParseNFloat(string raw)
        {
            return nfloat.Parse(raw, this.formatInfo);
        }
    }
}
