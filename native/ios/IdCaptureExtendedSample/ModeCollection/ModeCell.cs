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

using System;
using CoreGraphics;
using Foundation;
using UIKit;

namespace IdCaptureExtendedSample.ModeCollection
{
    public partial class ModeCell : UICollectionViewCell
    {
        public static readonly NSString Key = new NSString("ModeCell");

        public UILabel Label { get; } = new UILabel();

        [Export("initWithFrame:")]
        public ModeCell(CGRect frame) : base(frame)
        {
            this.Setup();
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            this.ContentView.Layer.CornerRadius = this.ContentView.Frame.Height / 2;
        }

        public override bool Selected
        {
            get => base.Selected;
            set
            {
                base.Selected = value;
                this.Update();
            }
        }

        public override bool Highlighted
        {
            get => base.Highlighted;
            set
            {
                base.Highlighted = value;
                this.Update();
            }
        }

        public override void PrepareForReuse()
        {
            base.PrepareForReuse();
            this.ContentView.BackgroundColor = null;
        }

        protected ModeCell(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        private void Setup()
        {
            this.ContentView.Layer.BorderWidth = 2;
            this.ContentView.Layer.BorderColor = UIColor.White.CGColor;
            this.ContentView.ClipsToBounds = true;
            this.ContentView.AddSubview(this.Label);
            this.Label.TranslatesAutoresizingMaskIntoConstraints = false;
            this.Label.LeadingAnchor.ConstraintEqualTo(this.ContentView.LeadingAnchor, constant: 24).Active = true;
            this.Label.TrailingAnchor.ConstraintEqualTo(this.ContentView.TrailingAnchor, constant: -24).Active = true;
            this.Label.TopAnchor.ConstraintEqualTo(this.ContentView.TopAnchor, constant: 8).Active = true;
            this.Label.BottomAnchor.ConstraintEqualTo(this.ContentView.BottomAnchor, constant: -8).Active = true;
            this.Update();
        }

        private void Update()
        {
            if (this.Selected || this.Highlighted)
            {
                this.ContentView.BackgroundColor = UIColor.White;
                this.Label.TextColor = new UIColor(red: 27 / 255, green: 32 / 255, blue: 38 / 255, alpha: 1);
            }
            else
            {
                this.ContentView.BackgroundColor = null;
                this.Label.TextColor = UIColor.White;
            }
        }
    }
}
