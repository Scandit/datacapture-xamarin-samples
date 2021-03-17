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

using System.Timers;
using Scandit.DataCapture.Core.Common.Geometry;

namespace BarcodeCaptureSettingsSample.Utils
{
    public static class Extensions
    {
        public static string GetStringWithUnit(this FloatWithUnit floatWithUnit, Android.Content.Context context)
        {
            string textFormat = context?.GetString(Resource.String.size_with_unit);
            return string.IsNullOrEmpty(textFormat) ? string.Empty : string.Format(textFormat, floatWithUnit.Value, floatWithUnit.Unit.Name());
        }

        public static string GetStringWithoutUnit(this FloatWithUnit floatWithUnit, Android.Content.Context context)
        {
            var textFormat = context?.GetString(Resource.String.size_no_unit);
            return string.IsNullOrEmpty(textFormat) ? string.Empty : string.Format(textFormat, floatWithUnit.Value);
        }

        public static void Reset(this Timer timer)
        {
            timer.Stop();
            timer.Start();
        }
    }
}