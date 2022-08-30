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
using Scandit.DataCapture.Core.Area;

namespace BarcodeCaptureSettingsSample.Settings.BarcodeCapture.Location.LocationTypes
{
    public abstract class LocationType : IEquatable<LocationType>
    {
        public int DisplayNameResourceId { get; }

        public bool Enabled { get; }

        public abstract ILocationSelection BuildLocationSelection();

        public bool Equals(LocationType other)
        {
            if (other == null)
            {
                return false;
            }

            return this.GetHashCode() == other.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return (obj as LocationType) != null && this.Equals((LocationType)obj);
        }

        public override int GetHashCode()
        {
            return this.DisplayNameResourceId.GetHashCode() ^ this.Enabled.GetHashCode();
        }

        protected LocationType(int displayNameResourceId, bool enabled)
        {
            this.DisplayNameResourceId = displayNameResourceId;
            this.Enabled = enabled;
        }
    }
}