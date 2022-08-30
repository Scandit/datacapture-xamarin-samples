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

using BarcodeCaptureSettingsSample.DataSource.Other;
using Scandit.DataCapture.Core.Source;

namespace BarcodeCaptureSettingsSample.DataSource.Settings.Camera
{
    public class FocusRangeType : Enumeration
    {
        public static readonly FocusRangeType Far = new FocusRangeType(FocusRange.Far);
        public static readonly FocusRangeType Near = new FocusRangeType(FocusRange.Near);
        public static readonly FocusRangeType Full = new FocusRangeType(FocusRange.Full);

        public FocusRange FocusRange { get; }

        public FocusRangeType(FocusRange focusRange) : base((int)focusRange, focusRange.ToString())
        {
            this.FocusRange = focusRange;
        }

        public static FocusRangeType Create(FocusRange focusRange)
        {
            return focusRange switch
            {
                FocusRange.Far => Far,
                FocusRange.Near => Near,
                FocusRange.Full => Full,
                _ => Full,
            };
        }
    }
}
