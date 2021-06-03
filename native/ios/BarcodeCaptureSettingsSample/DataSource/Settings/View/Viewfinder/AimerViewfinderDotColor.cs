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
    public class AimerViewfinderDotColor : Enumeration
    {
        public static readonly AimerViewfinderDotColor Default = new AimerViewfinderDotColor(0, "Default", AimerViewfinder.Create().DotColor);
        public static readonly AimerViewfinderDotColor Blue = new AimerViewfinderDotColor(1, "Blue", UIColor.Blue);
        public static readonly AimerViewfinderDotColor Red = new AimerViewfinderDotColor(2, "Red", UIColor.Red);

        public UIColor UIColor { get; }

        public AimerViewfinderDotColor(int id, string name, UIColor color) : base(id, name)
        {
            this.UIColor = color;
        }
    }
}
