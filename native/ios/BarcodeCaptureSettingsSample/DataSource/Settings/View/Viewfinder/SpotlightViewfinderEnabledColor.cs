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
    public class SpotlightViewfinderEnabledColor : Enumeration
    {
        public static readonly SpotlightViewfinderEnabledColor Default = new SpotlightViewfinderEnabledColor(0, "Default", SpotlightViewfinder.Create().EnabledBorderColor);
        public static readonly SpotlightViewfinderEnabledColor Red = new SpotlightViewfinderEnabledColor(1, "Blue", UIColor.Blue);
        public static readonly SpotlightViewfinderEnabledColor White = new SpotlightViewfinderEnabledColor(2, "Black", UIColor.Black);

        public UIColor UIColor { get; }

        public SpotlightViewfinderEnabledColor(int id, string name, UIColor color) : base(id, name)
        {
            this.UIColor = color;
        }
    }
}
