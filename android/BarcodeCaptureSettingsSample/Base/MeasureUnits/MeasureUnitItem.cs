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
using Scandit.DataCapture.Core.Common.Geometry;

namespace BarcodeCaptureSettingsSample.Base.MeasureUnits
{
    public class MeasureUnitItem : IEquatable<MeasureUnitItem>
    {
        public MeasureUnit MeasureUnit { get; }

        public bool Enabled { get; }

        public MeasureUnitItem(MeasureUnit measureUnit, bool enabled)
        {
            this.MeasureUnit = measureUnit;
            this.Enabled = enabled;
        }

        public bool Equals(MeasureUnitItem other)
        {
            if (other == null)
            {
                return false;
            }

            return this.GetHashCode() == other.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return (obj as MeasureUnitItem) != null && this.Equals((MeasureUnitItem)obj);
        }

        public override int GetHashCode()
        {
            return this.MeasureUnit.GetHashCode() ^ this.Enabled.GetHashCode();
        }
    }
}