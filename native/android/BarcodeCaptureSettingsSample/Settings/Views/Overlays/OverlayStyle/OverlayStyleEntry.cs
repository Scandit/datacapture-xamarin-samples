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
using Scandit.DataCapture.Barcode.UI.Overlay;

namespace BarcodeCaptureSettingsSample.Settings.Views.Overlays
{
    public class OverlayStyleEntry : IEquatable<OverlayStyleEntry>
    {
        public readonly BarcodeCaptureOverlayStyle style;
        public readonly bool enabled;

        public OverlayStyleEntry(BarcodeCaptureOverlayStyle style, bool enabled)
        {
            this.style = style;
            this.enabled = enabled;
        }

        public bool Equals(OverlayStyleEntry other)
        {
            if (this == null || other == null)
            {
                return false;
            }
            if (this.style != other.style || this.enabled != other.enabled)
            {
                return false;
            }
            return true;
        }
    }
}
