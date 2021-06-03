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
    public class TorchStateType : Enumeration
    {
        public static readonly TorchStateType On = new TorchStateType(TorchState.On);
        public static readonly TorchStateType Off = new TorchStateType(TorchState.Off);
        public static readonly TorchStateType Auto = new TorchStateType(TorchState.Auto);

        public TorchState TorchState { get; }

        public TorchStateType(TorchState torchState) : base((int)torchState, torchState.ToString())
        {
            this.TorchState = torchState;
        }

        public static TorchStateType Create(TorchState torchState)
        {
            return torchState switch
            {
                TorchState.On => On,
                TorchState.Auto => Auto,
                _ => Off,
            };
        }
    }
}
