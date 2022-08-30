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
using Scandit.DataCapture.Core.Common.Feedback;

namespace BarcodeCaptureSettingsSample.DataSource.Settings.BarcodeCapture
{
    public class VibrationType : Enumeration
    {
        public static readonly VibrationType None = new VibrationType(null, 0, "No Vibration");
        public static readonly VibrationType Default = new VibrationType(Vibration.DefaultVibration, 1, "Default Vibration");
        public static readonly VibrationType SelectionHapticFeedback = new VibrationType(Vibration.SelectionHapticFeedback, 2, "Selection Haptic Feedback");
        public static readonly VibrationType SuccessHapticFeedback = new VibrationType(Vibration.SuccessHapticFeedback, 3, "Success Haptic Feedback");

        public Vibration Vibration { get; private set; }

        public VibrationType(Vibration vibration, int id, string name) : base(id, name)
        {
            this.Vibration = vibration;
        }

        public static VibrationType Create(Vibration vibration)
        {
            if (vibration == Default.Vibration)
            {
                return Default;
            }
            else if (vibration == SelectionHapticFeedback.Vibration)
            {
                return SelectionHapticFeedback;
            }   
            else if (vibration == SuccessHapticFeedback.Vibration)
            {
                return SuccessHapticFeedback;
            }
            else
            {
                return None;
            }
        }
    }
}
