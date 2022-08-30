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
using BarcodeCaptureSettingsSample.Extensions;
using Scandit.DataCapture.Core.Common.Geometry;

namespace BarcodeCaptureSettingsSample.DataSource.Settings.View.Logo
{
    public class NamedAnchor : Enumeration
    {
        public static readonly NamedAnchor TopLeft = new NamedAnchor(0, Anchor.TopLeft);
        public static readonly NamedAnchor TopCenter = new NamedAnchor(1, Anchor.TopCenter);
        public static readonly NamedAnchor TopRight = new NamedAnchor(2, Anchor.TopRight);
        public static readonly NamedAnchor CenterLeft = new NamedAnchor(3, Anchor.CenterLeft);
        public static readonly NamedAnchor Center = new NamedAnchor(4, Anchor.Center);
        public static readonly NamedAnchor CenterRight = new NamedAnchor(5, Anchor.CenterRight);
        public static readonly NamedAnchor BottomLeft = new NamedAnchor(6, Anchor.BottomLeft);
        public static readonly NamedAnchor BottomCenter = new NamedAnchor(7, Anchor.BottomCenter);
        public static readonly NamedAnchor BottomRight = new NamedAnchor(8, Anchor.BottomRight);

        private NamedAnchor(int id, Anchor anchor) : base(id, anchor.Description())
        {
            this.Anchor = anchor;
        }

        public Anchor Anchor { get; }

        public static NamedAnchor Create(Anchor anchor)
        {
            switch (anchor)
            {
                case Anchor.TopLeft:
                    return TopLeft;
                case Anchor.TopCenter:
                    return TopCenter;
                case Anchor.TopRight:
                    return TopRight;
                case Anchor.CenterLeft:
                    return CenterLeft;
                case Anchor.Center:
                    return Center;
                case Anchor.CenterRight:
                    return CenterRight;
                case Anchor.BottomLeft:
                    return BottomLeft;
                case Anchor.BottomCenter:
                    return BottomCenter;
                case Anchor.BottomRight:
                    return BottomRight;
                default:
                    return Center;
            }
        }
    }
}
