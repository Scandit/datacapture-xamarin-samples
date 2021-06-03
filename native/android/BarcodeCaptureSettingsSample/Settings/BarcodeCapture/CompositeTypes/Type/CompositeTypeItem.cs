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
using Scandit.DataCapture.Barcode.Data;

namespace BarcodeCaptureSettingsSample.Settings.BarcodeCapture.CompositeTypes.Type
{
    public abstract class CompositeTypeItem : IEquatable<CompositeTypeItem>
    {
        public int DisplayNameResourceId { get; }

        public CompositeType CompositeType { get; }

        public bool Enabled { get; }

        public CompositeTypeItem(int displayNameRes, bool enabled, CompositeType compositeType)
        {
            this.DisplayNameResourceId = displayNameRes;
            this.CompositeType = compositeType;
            this.Enabled = enabled;
        }

        public bool Equals(CompositeTypeItem other)
        {
            if (other == null)
            {
                return false;
            }

            return this.GetHashCode() == other.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return (obj as CompositeTypeItem) != null && this.Equals((CompositeTypeItem)obj);
        }

        public override int GetHashCode()
        {
            return this.CompositeType.GetHashCode() ^ this.Enabled.GetHashCode();
        }
    }
}
