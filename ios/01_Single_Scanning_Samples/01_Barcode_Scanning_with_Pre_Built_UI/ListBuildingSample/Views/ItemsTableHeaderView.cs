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
using CoreFoundation;
using ListBuildingSample.Models;
using UIKit;

namespace ListBuildingSample.Views
{
    public class ItemsTableHeaderView : UIView
    {
        public static int HeaderHeight = 50;
        public static int LabelLeadingMargin = 15;
        public static int LabelTopMargin = 10;
        public static string DefaultLabel = "0 items";

        public UILabel ItemsCountLabel { get; private set; }

        public ItemsTableHeaderView()
        {
            this.Initialize();
            ListItemManager.Instance.ListsChanged += ListsChanged;
        }

        private void Initialize()
        {
            this.ItemsCountLabel = new UILabel
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                TextAlignment = UITextAlignment.Left,
                Font = UIFont.BoldSystemFontOfSize(18),
                TextColor = UIColor.Black,
                Text = DefaultLabel
            };
            this.Add(this.ItemsCountLabel);            
            this.AddConstraints(new[]
            {
                this.ItemsCountLabel.LeadingAnchor.ConstraintEqualTo(this.LeadingAnchor, LabelLeadingMargin),
                this.ItemsCountLabel.TopAnchor.ConstraintEqualTo(this.TopAnchor, LabelTopMargin),
            });
        }

        private void ListsChanged(object sender, EventArgs e)
        {
            DispatchQueue.MainQueue.DispatchAsync(() =>
            {
                var totalCount = ListItemManager.Instance.TotalItemsCount;
                var label = totalCount != 1 ? "items" : "item";
                this.ItemsCountLabel.Text = $"{totalCount} {label}";
            });
        }
    }
}
