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
using Scandit.DataCapture.Core.Source;

namespace BarcodeCaptureSettingsSample.DataSource.Settings.Camera
{
    public class FocusGestureStrategyType : Enumeration
    {
        public static readonly FocusGestureStrategyType None = new FocusGestureStrategyType(FocusGestureStrategy.None);
        public static readonly FocusGestureStrategyType Manual = new FocusGestureStrategyType(FocusGestureStrategy.Manual);
        public static readonly FocusGestureStrategyType ManualUntilCapture = new FocusGestureStrategyType(FocusGestureStrategy.ManualUntilCapture);
        public static readonly FocusGestureStrategyType AutoOnLocation = new FocusGestureStrategyType(FocusGestureStrategy.AutoOnLocation);

        public FocusGestureStrategy FocusGestureStrategy { get; }

        public FocusGestureStrategyType(FocusGestureStrategy focusGuesterStrategy) : base((int)focusGuesterStrategy, focusGuesterStrategy.ToString())
        {
            this.FocusGestureStrategy = focusGuesterStrategy;
        }

        public static FocusGestureStrategyType Create(FocusGestureStrategy focusRange)
        {
            return focusRange switch
            {
                FocusGestureStrategy.None => None,
                FocusGestureStrategy.Manual => Manual,
                FocusGestureStrategy.ManualUntilCapture => ManualUntilCapture,
                FocusGestureStrategy.AutoOnLocation => AutoOnLocation,
                _ => ManualUntilCapture,
            };
        }
    }
}
