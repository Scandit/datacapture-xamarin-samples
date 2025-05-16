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

using CoreGraphics;
using Foundation;
using UIKit;

namespace MatrixScanBubblesSample.Overlay
{
    public class StockOverlay : UIView
    {
        private readonly UIView contentView = new UIView(CGRect.Null)
        {
            BackgroundColor = UIColor.White.ColorWithAlpha(0.3f),
            ClipsToBounds = true,
        };
        private readonly UILabel taskLabel = new UILabel(CGRect.Empty)
        {
            TextColor = UIColor.FromRGBA(0.29f, 0.29f, 0.29f, 1),
            BackgroundColor = UIColor.Clear,
            Lines = 2,
            TextAlignment = UITextAlignment.Center
        };
        private readonly UILabel barcodeLabel = new UILabel(CGRect.Empty)
        {
            Font = UIFont.BoldSystemFontOfSize(14),
            TextColor = UIColor.FromRGBA(0.29f, 0.29f, 0.29f, 1),
            BackgroundColor = UIColor.Clear,
            Lines = 1,
            Hidden = true,
            TextAlignment = UITextAlignment.Center
        };
        private UIVisualEffectView effectView = new UIVisualEffectView(UIBlurEffect.FromStyle(UIBlurEffectStyle.ExtraLight));

        public static StockOverlay Create(int shelfCount, int backroomCount, string barcodeData)
        {
            var overlay = new StockOverlay(shelfCount, backroomCount, barcodeData);
            return overlay;
        }

        public bool ShouldUpdateConstraints { get; private set; } = true;

        private StockOverlay(int shelfCount, int backRoomCount, string barcodeData) : base(new CGRect(0, 0, 300, 60))
        {
            this.contentView.Layer.MasksToBounds = true;
            this.contentView.Layer.CornerRadius = 30;


            var mutableString = new NSMutableAttributedString("Report stock count\n", UIFont.BoldSystemFontOfSize(14));
            mutableString.Append(new NSAttributedString($"Shelf: {shelfCount}, Back room: {backRoomCount}", UIFont.BoldSystemFontOfSize(14)));
            this.taskLabel.AttributedText = mutableString;

            this.barcodeLabel.Text = barcodeData;

            this.AddSubview(this.contentView);
            this.contentView.AddSubviews(this.effectView, this.taskLabel, this.barcodeLabel);
            this.contentView.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                this.taskLabel.Hidden = !this.taskLabel.Hidden;
                this.barcodeLabel.Hidden = !this.taskLabel.Hidden;
            }));
            this.UpdateConstraints();
        }

        public override void UpdateConstraints()
        {
            if (this.ShouldUpdateConstraints)
            {
                this.contentView.TranslatesAutoresizingMaskIntoConstraints = false;
                this.effectView.TranslatesAutoresizingMaskIntoConstraints = false;
                this.taskLabel.TranslatesAutoresizingMaskIntoConstraints = false;
                this.barcodeLabel.TranslatesAutoresizingMaskIntoConstraints = false;
                this.AddConstraints(new[]
                {
                    this.WidthAnchor.ConstraintLessThanOrEqualTo(300),
                    this.HeightAnchor.ConstraintEqualTo(60),

                    this.contentView.LeadingAnchor.ConstraintEqualTo(this.LeadingAnchor),
                    this.contentView.TrailingAnchor.ConstraintEqualTo(this.TrailingAnchor),
                    this.contentView.TopAnchor.ConstraintEqualTo(this.TopAnchor),
                    this.contentView.BottomAnchor.ConstraintEqualTo(this.BottomAnchor),

                    this.taskLabel.LeadingAnchor.ConstraintEqualTo(this.contentView.LeadingAnchor, 7),
                    this.taskLabel.TopAnchor.ConstraintEqualTo(this.contentView.TopAnchor, 13),
                    this.taskLabel.TrailingAnchor.ConstraintEqualTo(this.contentView.TrailingAnchor, -24),
                    this.taskLabel.BottomAnchor.ConstraintEqualTo(this.contentView.BottomAnchor, -12),

                    this.barcodeLabel.CenterYAnchor.ConstraintEqualTo(this.taskLabel.CenterYAnchor),
                    this.barcodeLabel.LeadingAnchor.ConstraintEqualTo(this.taskLabel.LeadingAnchor),
                    this.barcodeLabel.WidthAnchor.ConstraintEqualTo(this.taskLabel.WidthAnchor),
                    this.barcodeLabel.HeightAnchor.ConstraintEqualTo(this.taskLabel.HeightAnchor, 0.5f),

                    this.effectView.LeadingAnchor.ConstraintEqualTo(this.contentView.LeadingAnchor),
                    this.effectView.TopAnchor.ConstraintEqualTo(this.contentView.TopAnchor),
                    this.effectView.TrailingAnchor.ConstraintEqualTo(this.contentView.TrailingAnchor),
                    this.effectView.BottomAnchor.ConstraintEqualTo(this.contentView.BottomAnchor),

                });
                this.contentView.SetContentHuggingPriority((float)UILayoutPriority.Required, UILayoutConstraintAxis.Horizontal);
                this.SetContentHuggingPriority((float)UILayoutPriority.Required, UILayoutConstraintAxis.Horizontal);
                this.ShouldUpdateConstraints = !this.ShouldUpdateConstraints;
            }
            base.UpdateConstraints();
        }
    }
}
