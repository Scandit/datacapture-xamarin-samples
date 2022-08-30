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

namespace BarcodeCaptureSettingsSample.Settings.BarcodeCapture.Symbologies.SpecificSymbology
{
    public class SymbologyItem : IEquatable<SymbologyItem>
    {
        public string Name { get; set; }
        public bool Enabled { get; set; }

        public bool Equals(SymbologyItem other)
        {
            if (other == null)
            {
                return false;
            }

            return this.GetHashCode() == other.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return (obj as SymbologyItem) != null && this.Equals((SymbologyItem)obj);
        }

        public override int GetHashCode()
        {
            return this.Name.GetHashCode() ^ this.Enabled.GetHashCode();
        }
    }
}
