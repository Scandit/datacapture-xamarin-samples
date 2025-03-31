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
using Foundation;
using UIKit;
using ListBuildingSample.Models;

namespace ListBuildingSample.Views
{
    public class ItemTableViewCell : UITableViewCell
    {
        public static readonly NSString Key = new NSString("ItemTableViewCell");

        public static int BarcodeImageLeadingMargin = 15;
        public static int BarcodeImageTopMargin = 10;
        public static int BarcodeImageSize = 50;
        public static int TitleLeadingMargin = 75;
        public static int TitleTopMargin = 15;
        public static int TitleHeight = 20;
        public static int SubtitleLeadingMargin = 75;
        public static int SubtitleTopMargin = 35;
        public static int SubtitleHeight = 20;

        private ListItem item;

        public UIImageView BarcodeImage { get; private set; }
        public UILabel Title { get; private set; }
        public UILabel Subtitle { get; private set; }

        public ItemTableViewCell(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        public void Configure(ListItem item)
        {
            this.item = item ?? throw new ArgumentNullException(nameof(item));

            if (this.BarcodeImage == null)
            {
                this.TryCreateBarcodeImage();
            }
            if (this.Title == null)
            {
                this.CreateTitle();
            }
            if (this.Subtitle == null)
            {
                this.CreateSubtitle();
            }

            if (this.BarcodeImage != null && this.item.Image?.CGImage != null)
            {
                this.BarcodeImage.Image = new UIImage(this.item.Image.CGImage, this.item.Image.CurrentScale, UIImageOrientation.Right);
            }

            this.Title.Text = $"Item {this.item.Number}";
            this.Subtitle.Text = $"{this.item.Symbology}: {this.item.Data}";
        }

        private void TryCreateBarcodeImage()
        {
            if (this.item.Image?.CGImage != null)
            {
                this.BarcodeImage = new UIImageView
                {
                    TranslatesAutoresizingMaskIntoConstraints = false,
                    Image = new UIImage(this.item.Image.CGImage, this.item.Image.CurrentScale, UIImageOrientation.Right)
                };

                this.ContentView.AddSubview(this.BarcodeImage);
                this.ContentView.AddConstraints(new[]
                {
                    this.BarcodeImage.LeadingAnchor.ConstraintEqualTo(this.ContentView.LeadingAnchor, BarcodeImageLeadingMargin),
                    this.BarcodeImage.TopAnchor.ConstraintEqualTo(this.ContentView.TopAnchor, BarcodeImageTopMargin),
                    this.BarcodeImage.HeightAnchor.ConstraintEqualTo(BarcodeImageSize),
                    this.BarcodeImage.WidthAnchor.ConstraintEqualTo(BarcodeImageSize)
                });
            }
            else
            {
                this.BarcodeImage = null;
            }
        }

        private void CreateTitle()
        {
            this.Title = new UILabel
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                TextAlignment = UITextAlignment.Left,
                Font = UIFont.BoldSystemFontOfSize(18),
                TextColor = UIColor.Black,
                Text = $"Item {this.item.Number}"
            };
            this.ContentView.AddSubview(this.Title);
            this.ContentView.AddConstraints(new[]
            {
                this.Title.LeadingAnchor.ConstraintEqualTo(this.ContentView.LeadingAnchor, TitleLeadingMargin),
                this.Title.TopAnchor.ConstraintEqualTo(this.ContentView.TopAnchor, TitleTopMargin),
                this.Title.HeightAnchor.ConstraintEqualTo(TitleHeight),
            });
        }

        private void CreateSubtitle()
        {
            this.Subtitle = new UILabel
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                TextAlignment = UITextAlignment.Left,
                Font = UIFont.SystemFontOfSize(18),
                TextColor = UIColor.Gray,
                Text = $"{this.item.Symbology}: {this.item.Data}",
            };
            this.ContentView.AddSubview(this.Subtitle);
            this.ContentView.AddConstraints(new[]
            {
                this.Subtitle.LeadingAnchor.ConstraintEqualTo(this.ContentView.LeadingAnchor, SubtitleLeadingMargin),
                this.Subtitle.TopAnchor.ConstraintEqualTo(this.ContentView.TopAnchor, SubtitleTopMargin),
                this.Subtitle.HeightAnchor.ConstraintEqualTo(SubtitleHeight)
            });
        }
    }
}
