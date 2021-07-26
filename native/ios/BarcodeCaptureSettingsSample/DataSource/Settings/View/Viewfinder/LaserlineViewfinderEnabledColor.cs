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
    public class LaserlineViewfinderEnabledColor : Enumeration
    {
        public static readonly LaserlineViewfinderEnabledColor Default = new LaserlineViewfinderEnabledColor(0, "Default", LaserlineViewfinder.Create().EnabledColor);
        public static readonly LaserlineViewfinderEnabledColor Red = new LaserlineViewfinderEnabledColor(1, "Red", UIColor.Red);
        public static readonly LaserlineViewfinderEnabledColor White = new LaserlineViewfinderEnabledColor(2, "White", UIColor.White);
        
        public UIColor UIColor { get; }
        
        public LaserlineViewfinderEnabledColor(int id, string name, UIColor color) : base(id, name)
        {
            this.UIColor = color;
        }
    }

    public class LaserlineViewfinderAnimatedEnabledColor : Enumeration
    {
        public static readonly LaserlineViewfinderAnimatedEnabledColor Default = new LaserlineViewfinderAnimatedEnabledColor(0, "Default", LaserlineViewfinder.Create(LaserlineViewfinderStyle.Animated).EnabledColor);
        public static readonly LaserlineViewfinderAnimatedEnabledColor Blue = new LaserlineViewfinderAnimatedEnabledColor(1, "Blue", UIColor.Blue);
        public static readonly LaserlineViewfinderAnimatedEnabledColor Red = new LaserlineViewfinderAnimatedEnabledColor(2, "Red", UIColor.Red);

        public UIColor UIColor { get; }

        public LaserlineViewfinderAnimatedEnabledColor(int id, string name, UIColor color) : base(id, name)
        {
            this.UIColor = color;
        }
    }
}
