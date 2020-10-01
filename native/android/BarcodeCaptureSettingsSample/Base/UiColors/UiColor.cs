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

using System.Collections.Generic;

namespace BarcodeCaptureSettingsSample.Base.UiColors
{
    public class UiColor
    {
        private static readonly Dictionary<int, UiColor> Colors = new Dictionary<int, UiColor>
        {
            { Android.Graphics.Color.Red, new UiColor ( Android.Graphics.Color.Red, Resource.String.red ) },
            { Android.Graphics.Color.White, new UiColor ( Android.Graphics.Color.White, Resource.String.white ) },
            { Android.Graphics.Color.Blue, new UiColor ( Android.Graphics.Color.Blue, Resource.String.blue ) },
            { Android.Graphics.Color.Black, new UiColor ( Android.Graphics.Color.Black, Resource.String.black ) },
            { Android.Graphics.Color.Green, new UiColor ( Android.Graphics.Color.Green, Resource.String.green ) }
        };

        public int Color { get; }

        public int DisplayNameResourceId { get; }

        public static UiColor Red => Colors[Android.Graphics.Color.Red];

        public static UiColor White => Colors[Android.Graphics.Color.White];

        public static UiColor Blue => Colors[Android.Graphics.Color.Blue];

        public static UiColor Black => Colors[Android.Graphics.Color.Black];

        public static UiColor Green => Colors[Android.Graphics.Color.Green];

        public UiColor(int color, int displayNameResourceId)
        {
            this.Color = color;
            this.DisplayNameResourceId = displayNameResourceId;
        }
    }
}