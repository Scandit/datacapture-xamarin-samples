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
using Foundation;

namespace BarcodeCaptureSettingsSample.Extensions
{
    public class NumberFormatter
    {
        public static readonly NumberFormatter Instance = new NumberFormatter();

        private readonly NumberFormatInfo formatInfo;

        private NumberFormatter()
        {
            var cultureInfo = CultureInfoExtensions.LocaleAwareCultureInfo;
            this.formatInfo = cultureInfo.NumberFormat;
        }

        public string FormatNFloat(nfloat number, int decimalPlaces = 2)
        {
            return number.ToString($"F{decimalPlaces}", this.formatInfo);
        }

        public string FormatTimeSpanToSeconds(TimeSpan timeSpan)
        {
            return timeSpan.TotalSeconds.ToString(this.formatInfo);
        }

        public nfloat ParseNFloat(string raw)
        {
            try
            {
                return nfloat.Parse(raw, this.formatInfo);
            }
            catch (Exception)
            {
                return 0;
            }
        }
    }
}
