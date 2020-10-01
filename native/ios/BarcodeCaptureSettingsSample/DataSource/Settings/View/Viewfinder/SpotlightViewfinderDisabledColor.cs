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
using Scandit.DataCapture.Core.UI.Viewfinder;
using UIKit;

namespace BarcodeCaptureSettingsSample.DataSource.Settings.View.Viewfinder
{
    public class SpotlightViewfinderDisabledColor : Enumeration
    {
        public static readonly SpotlightViewfinderDisabledColor Default = new SpotlightViewfinderDisabledColor(0, "Default", SpotlightViewfinder.Create().DisabledBorderColor);
        public static readonly SpotlightViewfinderDisabledColor Blue = new SpotlightViewfinderDisabledColor(1, "Black", UIColor.Black);
        public static readonly SpotlightViewfinderDisabledColor Red = new SpotlightViewfinderDisabledColor(2, "Red", UIColor.Red);

        public UIColor UIColor { get; }

        public SpotlightViewfinderDisabledColor(int key, string value, UIColor color) : base(key, value)
        {
            this.UIColor = color;
        }
    }
}
