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
    public class Resolution : Enumeration
    {
        public static readonly Resolution HD = new Resolution("HD", VideoResolution.Hd);
        public static readonly Resolution FullHD = new Resolution("Full HD", VideoResolution.FullHd);
        public static readonly Resolution UltraHD4K = new Resolution("Ultra HD 4K", VideoResolution.Uhd4k);
        public static readonly Resolution Auto = new Resolution("Auto", VideoResolution.Auto);

        public VideoResolution VideoResolution { get; }

        public Resolution(string name, VideoResolution resolution) : base((int)resolution, name)
        {
            this.VideoResolution = resolution;
        }

        public static Resolution Create(VideoResolution resolution)
        {
            return resolution switch
            {
                VideoResolution.Hd => HD,
                VideoResolution.FullHd => FullHD,
                VideoResolution.Uhd4k => UltraHD4K,
                VideoResolution.Auto => Auto,
                _ => Auto,
            };
        }
    }
}
