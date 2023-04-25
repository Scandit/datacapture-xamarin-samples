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

using UIKit;

namespace MatrixScanCountSimpleSample.Extensions
{
    public static class UIButtonExtensions
    {
        public static void StyleAsPrimaryButton(this UIButton button)
        {
            Style(button, CompleteButtonStyle.Primary);
        }

        public static void StyleAsSecondaryButton(this UIButton button)
        {
            Style(button, CompleteButtonStyle.Secondary);
        }

        private static void Style(UIButton button, CompleteButtonStyle style)
        {
            button.SetTitleColor(style.NormalTitleColor, UIControlState.Normal);
            button.SetBackgroundImage(style.NormalBackgroundColor.ToImage(), UIControlState.Normal);

            button.SetTitleColor(style.PressedTitleColor, UIControlState.Highlighted);
            button.SetBackgroundImage(style.PressedBackgroundColor.ToImage(), UIControlState.Highlighted);

            button.SetTitleColor(style.InactiveTitleColor, UIControlState.Disabled);
            button.SetBackgroundImage(style.InactiveBackgroundColor.ToImage(), UIControlState.Disabled);
        }
    }
}
