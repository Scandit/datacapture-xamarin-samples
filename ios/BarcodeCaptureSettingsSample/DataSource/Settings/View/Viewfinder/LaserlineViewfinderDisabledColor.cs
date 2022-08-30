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
    public class LaserlineViewfinderDisabledColor : Enumeration
    {
        public static readonly LaserlineViewfinderDisabledColor Default = new LaserlineViewfinderDisabledColor(0, "Default", LaserlineViewfinder.Create().DisabledColor);
        public static readonly LaserlineViewfinderDisabledColor Blue = new LaserlineViewfinderDisabledColor(1, "Blue", UIColor.Blue);
        public static readonly LaserlineViewfinderDisabledColor Red = new LaserlineViewfinderDisabledColor(2, "Red", UIColor.Red);

        public UIColor UIColor { get; }

        public LaserlineViewfinderDisabledColor(int key, string value, UIColor color) : base(key, value)
        {
            this.UIColor = color;
        }
    }
}
