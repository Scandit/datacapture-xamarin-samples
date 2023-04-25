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

namespace MatrixScanCountSimpleSample
{
    public struct CompleteButtonStyle
    {
        public UIColor NormalTitleColor { get; private set; }
        public UIColor NormalBackgroundColor { get; private set; }
        public UIColor PressedTitleColor { get; private set; }
        public UIColor PressedBackgroundColor { get; private set; }
        public UIColor InactiveTitleColor { get; private set; }
        public UIColor InactiveBackgroundColor { get; private set; }

        public static CompleteButtonStyle Primary = new CompleteButtonStyle
        {
            NormalTitleColor = UIColor.White,
            NormalBackgroundColor = UIColor.Black,
            PressedTitleColor = UIColor.White,
            PressedBackgroundColor = new UIColor(red: 0.239f, green: 0.282f, blue: 0.322f, alpha: 1.0f),
            InactiveTitleColor = new UIColor(red: 0.529f, green: 0.584f, blue: 0.631f, alpha: 1.0f),
            InactiveBackgroundColor = new UIColor(red: 0.855f, green: 0.882f, blue: 0.906f, alpha: 1.0f)
        };

        public static CompleteButtonStyle Secondary = new CompleteButtonStyle
        {
            NormalTitleColor = new UIColor(red: 0.071f, green: 0.086f, blue: 0.098f, alpha: 1.0f),
            NormalBackgroundColor = UIColor.White,
            PressedTitleColor = UIColor.White,
            PressedBackgroundColor = UIColor.Black,
            InactiveTitleColor = new UIColor(red: 0.529f, green: 0.584f, blue: 0.631f, alpha: 1.0f),
            InactiveBackgroundColor = new UIColor(red: 0.855f, green: 0.882f, blue: 0.906f, alpha: 1.0f)
        };
    }
}
