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
using System.Linq;
using System.Collections.Generic;
using UIKit;
using Foundation;

namespace IdCaptureExtendedSample.ModeCollection
{
    public delegate void ModeCollectionViewControllerSelectedModeDelegate(Mode mode);

    public partial class ModeCollectionViewController : UICollectionViewController
    {
        private readonly List<Mode> items = Enum.GetValues(typeof(Mode)).OfType<Mode>().ToList();
        private readonly ModeCollectionViewControllerSelectedModeDelegate selectedModeDelegate;

        public ModeCollectionViewController(UICollectionViewFlowLayout layout,
                                            ModeCollectionViewControllerSelectedModeDelegate selectedModeDelegate = null) : base(layout)
        {
            this.selectedModeDelegate = selectedModeDelegate;
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            this.CollectionView.RegisterClassForCell(typeof(ModeCell), reuseIdentifier: ModeCell.Key);
            this.CollectionView.ContentInset = new UIEdgeInsets(top: 8, left: 16, bottom: 8, right: 16);
            this.CollectionView.ShowsHorizontalScrollIndicator = false;
            this.CollectionView.TranslatesAutoresizingMaskIntoConstraints = false;
            this.CollectionView.TopAnchor.ConstraintEqualTo(this.View.TopAnchor).Active = true;
            this.CollectionView.BottomAnchor.ConstraintEqualTo(this.View.SafeAreaLayoutGuide.BottomAnchor).Active = true;
            this.CollectionView.LeadingAnchor.ConstraintEqualTo(this.View.LeadingAnchor).Active = true;
            this.CollectionView.TrailingAnchor.ConstraintEqualTo(this.View.TrailingAnchor).Active = true;
            this.CollectionView.BackgroundColor = null;
            this.View.BackgroundColor = new UIColor(red: 27 / 255, green: 32 / 255, blue: 38 / 255, alpha: 1);
        }

        public override nint NumberOfSections(UICollectionView collectionView)
        {
            return 1;
        }

        public override nint GetItemsCount(UICollectionView collectionView, nint section)
        {
            return this.items.Count;
        }

        public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            ModeCell cell = this.CollectionView.DequeueReusableCell(reuseIdentifier: ModeCell.Key, indexPath) as ModeCell;
            cell.Label.Text = this.items[indexPath.Row].ToString();
            return cell;
        }

        public override void ItemSelected(UICollectionView collectionView, NSIndexPath indexPath)
        {
            this.selectedModeDelegate?.Invoke(this.items[indexPath.Row]);
        }

        public void SelectItem(int index)
        {
            this.CollectionView.SelectItem(NSIndexPath.FromItemSection(item: index, section: 0), animated: true, scrollPosition: UICollectionViewScrollPosition.Left);
        }
    }
}

