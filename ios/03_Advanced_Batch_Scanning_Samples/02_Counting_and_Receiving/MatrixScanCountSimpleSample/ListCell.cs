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
using System.Reflection;
using Foundation;
using MatrixScanCountSimpleSample.Data;
using UIKit;

namespace MatrixScanCountSimpleSample
{
    public class ListCell : UITableViewCell
    {
        public new static readonly NSString ReuseIdentifier = new NSString("ListCell");

        public struct Constants
        {
            public static readonly UIColor ImageBackgroundcolor = new UIColor(
                red: 0.945f, green: 0.961f, blue: 0.973f, alpha: 1.0f);
            public static readonly nfloat ImageSize = 48f;

            public static readonly UIFont SymbologyFont = UIFont.SystemFontOfSize(size: 16, UIFontWeight.Semibold);
            public static readonly UIColor DataTextColor = new UIColor(
                red: 0.529f, green: 0.584f, blue: 0.631f, alpha: 1.0f);
            public static readonly UIFont DataFont = UIFont.SystemFontOfSize(size: 14, UIFontWeight.Regular);
            public static readonly UIColor QuantityTextColor = new UIColor(
                red: 0.376f, green: 0.435f, blue: 0.482f, alpha: 1.0f);
            public static readonly UIFont QuantityFont = UIFont.SystemFontOfSize(size: 16, UIFontWeight.Semibold);

            // Creates a separation to the next cell
            public static readonly nfloat ContainerBottomMargin = 4f;
            public static readonly nfloat ImageMargin = 16f;
            public static readonly nfloat ImageToNameHorizontalSpacing = 16f;
            public static readonly nfloat NameToDataVerticalSpacing = 4f;
            public static readonly nfloat QuantityMargin = 16f;
        }

        private UIView containerView;
        private UIImageView itemImageView;
        private UILabel nameLabel;
        private UILabel dataLabel;
        private UILabel quantityLabel;

        public ListCell(IntPtr handle) : base(handle)
        {
            this.CreateContainerView();
            this.CreateItemImageView();
            this.CreateNameLabel();
            this.CreateDataLabel();
            this.CreateQuantityLabel();
        }

        public override void PrepareForReuse()
        {
            base.PrepareForReuse();

            this.itemImageView.Image = null;
            this.nameLabel.Text = null;
            this.dataLabel.Text = null;
            this.quantityLabel.Text = null;
            this.quantityLabel.Hidden = true;
        }

        public void Configure(ScanItem item, int index)
        {
            int displayIndex = index + 1;
            this.nameLabel.Text = item.Quantity > 1 ? $"Non-unique item {displayIndex}" :
                                                      $"Item {displayIndex}";
            this.dataLabel.Text = $"{item.Symbology}: {item.BarcodeData}";

            if (item.Quantity > 1)
            {
                this.quantityLabel.Hidden = false;
                this.quantityLabel.Text = $"Qty: {item.Quantity}";
            }
        }

        private void CreateContainerView()
        {
            this.BackgroundColor = UIColor.Clear;
            this.ContentView.BackgroundColor = UIColor.Clear;
            this.containerView = new UIView();
            this.containerView.TranslatesAutoresizingMaskIntoConstraints = false;
            this.containerView.BackgroundColor = UIColor.White;
            this.ContentView.AddSubview(this.containerView);

            this.ContentView.AddConstraints(new[]
            {
                this.containerView.TopAnchor.ConstraintEqualTo(this.ContentView.TopAnchor),
                this.containerView.LeadingAnchor.ConstraintEqualTo(this.ContentView.LeadingAnchor),
                this.containerView.TrailingAnchor.ConstraintEqualTo(this.ContentView.TrailingAnchor),
                this.ContentView.BottomAnchor.ConstraintEqualTo(this.containerView.BottomAnchor,
                                                    constant: Constants.ContainerBottomMargin)
            });
        }

        private void CreateItemImageView()
        {
            this.itemImageView = new UIImageView();
            this.itemImageView.TranslatesAutoresizingMaskIntoConstraints = false;
            this.itemImageView.BackgroundColor = Constants.ImageBackgroundcolor;
            this.containerView.AddSubview(this.itemImageView);
            this.containerView.AddConstraints(new[]
            {
                this.itemImageView.LeadingAnchor.ConstraintEqualTo(
                    this.containerView.LeadingAnchor, constant: Constants.ImageMargin),
                this.itemImageView.TopAnchor.ConstraintEqualTo(
                    this.containerView.TopAnchor, constant: Constants.ImageMargin),
                this.itemImageView.WidthAnchor.ConstraintEqualTo(constant: Constants.ImageSize),
                this.itemImageView.HeightAnchor.ConstraintEqualTo(constant: Constants.ImageSize)
            });
        }

        private void CreateNameLabel()
        {
            this.nameLabel = new UILabel();
            this.nameLabel.TranslatesAutoresizingMaskIntoConstraints = false;
            this.containerView.AddSubview(this.nameLabel);
            this.nameLabel.Font = Constants.SymbologyFont;
            this.containerView.AddConstraints(new[]
            {
                this.nameLabel.TopAnchor.ConstraintEqualTo(this.itemImageView.TopAnchor),
                this.nameLabel.LeadingAnchor.ConstraintEqualTo(
                    this.itemImageView.TrailingAnchor, constant: Constants.ImageToNameHorizontalSpacing)
            });
        }

        private void CreateDataLabel()
        {
            this.dataLabel = new UILabel();
            this.dataLabel.TranslatesAutoresizingMaskIntoConstraints = false;
            this.containerView.AddSubview(dataLabel);
            this.dataLabel.TextColor = Constants.DataTextColor;
            this.dataLabel.Font = Constants.DataFont;
            this.containerView.AddConstraints(new[]
            {
                this.dataLabel.LeadingAnchor.ConstraintEqualTo(this.nameLabel.LeadingAnchor),
                this.dataLabel.TopAnchor.ConstraintEqualTo(
                    this.nameLabel.BottomAnchor, constant: Constants.NameToDataVerticalSpacing)
            });
        }

        private void CreateQuantityLabel()
        {
            this.quantityLabel = new UILabel();
            this.quantityLabel.TranslatesAutoresizingMaskIntoConstraints = false;
            this.containerView.AddSubview(this.quantityLabel);
            this.quantityLabel.TextColor = Constants.QuantityTextColor;
            this.quantityLabel.Font = Constants.QuantityFont;
            this.containerView.AddConstraints(new[]
            {
                this.quantityLabel.TrailingAnchor.ConstraintEqualTo(
                    this.containerView.TrailingAnchor, constant: -Constants.QuantityMargin),
                this.quantityLabel.CenterYAnchor.ConstraintEqualTo(this.containerView.CenterYAnchor)
            });
        }
    }
}
