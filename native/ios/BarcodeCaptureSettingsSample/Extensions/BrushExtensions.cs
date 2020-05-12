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

using BarcodeCaptureSettingsSample.Model;
using Scandit.DataCapture.Core.UI.Style;
using UIKit;

namespace BarcodeCaptureSettingsSample.Extensions
{
    public static class BrushExtensions
    {
        public static readonly Brush DefaultBrush = SettingsManager.Instance.DefaultBrush;
        public static readonly Brush RedBrush = new Brush(UIColor.Red.ColorWithAlpha(.2f), UIColor.Red, 1);
        public static readonly Brush GreenBrush = new Brush(UIColor.Green.ColorWithAlpha(.2f), UIColor.Green, 1);
    }
}
