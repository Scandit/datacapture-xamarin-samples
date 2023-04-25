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
using System.Collections.Generic;
using System.Linq;
using CoreGraphics;
using Foundation;
using MatrixScanCountSimpleSample.Data;
using MatrixScanCountSimpleSample.Extensions;
using UIKit;

namespace MatrixScanCountSimpleSample
{
    public partial class ListViewController : UIViewController
    {
        private IListViewControllerListener listener;

        // Top "navigation" bar
        private readonly UIView topBar = new UIView();
        private readonly UIButton backButton = new UIButton(UIButtonType.Custom);
        private readonly UILabel titleLabel = new UILabel();

        private readonly UILabel countLabel = new UILabel();
        private readonly UITableView tableView = new UITableView();

        // Bottom container with buttons
        private readonly UIView bottomContainer = new UIView();
        private readonly UIButton resumeScanningButton = new UIButton(UIButtonType.Custom);
        private readonly UIButton clearListButton = new UIButton(UIButtonType.Custom);

        private readonly bool isOrderCompleted;
        private readonly IList<ScanItem> scanResults;
        private readonly int numberOfItems;

        public ListViewController(IListViewControllerListener listener,
                                  IList<ScanItem> scanResults,
                                  bool isOrderCompleted)
        {
            this.listener = listener;

            if (scanResults == null)
            {
                throw new ArgumentNullException(nameof(scanResults));
            }

            this.scanResults = scanResults;
            this.numberOfItems = GetScanResultsCount(scanResults);
            this.isOrderCompleted = isOrderCompleted;
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            this.View.BackgroundColor = UIColor.White;

            this.InitializeNavigationBar();
            this.InitializeCountLabel();
            this.InitializeBottomContainer();
            this.InitializeTableView();
        }

        private static int GetScanResultsCount(IList<ScanItem> scanResults)
        {
            return scanResults.Sum(i => i.Quantity);
        }

        private void DismissList()
        {
            if (this.isOrderCompleted)
            {
                this.listener?.RestartScanning();
            }
            else
            {
                this.listener?.ResumeScanning();
            }
        }

        private void OnTapBack(object sender, EventArgs e)
        {
            this.DismissList();
        }

        private void OnTapResumeScanning(object sender, EventArgs e)
        {
            this.DismissList();
        }

        private void OnTapClearList(object sender, EventArgs e)
        {
            this.listener?.RestartScanning();
        }

        private void InitializeNavigationBar()
        {
            this.topBar.TranslatesAutoresizingMaskIntoConstraints = false;
            this.topBar.BackgroundColor = new UIColor(red: 0.071f, green: 0.086f, blue: 0.098f, alpha: 1.0f);
            this.View.AddSubview(this.topBar);
            this.View.AddConstraints(new[]{
                this.topBar.TopAnchor.ConstraintEqualTo(this.View.TopAnchor),
                this.topBar.LeadingAnchor.ConstraintEqualTo(this.View.LeadingAnchor),
                this.topBar.TrailingAnchor.ConstraintEqualTo(this.View.TrailingAnchor),
                this.topBar.BottomAnchor.ConstraintEqualTo(
                    this.View.SafeAreaLayoutGuide.TopAnchor, constant: 44)
            });

            this.backButton.TranslatesAutoresizingMaskIntoConstraints = false;
            this.backButton.SetImage(UIImage.FromBundle("BackArrow"), UIControlState.Normal);
            this.backButton.AddTarget(this.OnTapBack, UIControlEvent.TouchUpInside);
            this.topBar.AddSubview(this.backButton);
            this.topBar.AddConstraints(new[]
            {
                this.backButton.LeadingAnchor.ConstraintEqualTo(this.topBar.LeadingAnchor, constant: 8),
                this.backButton.BottomAnchor.ConstraintEqualTo(this.topBar.BottomAnchor),
                this.backButton.HeightAnchor.ConstraintEqualTo(constant: 44),
                this.backButton.WidthAnchor.ConstraintEqualTo(constant: 44)
            });

            this.titleLabel.TranslatesAutoresizingMaskIntoConstraints = false;
            this.titleLabel.Text = "Scanned Items";
            this.titleLabel.Font = UIFont.SystemFontOfSize(size: 16, UIFontWeight.Bold);
            this.titleLabel.TextColor = UIColor.White;
            this.topBar.AddSubview(this.titleLabel);
            this.topBar.AddConstraints(new[]
            {
                this.titleLabel.CenterXAnchor.ConstraintEqualTo(this.topBar.CenterXAnchor),
                this.titleLabel.BottomAnchor.ConstraintEqualTo(this.topBar.BottomAnchor),
                this.titleLabel.HeightAnchor.ConstraintEqualTo(constant: 44)
            });
        }

        private void InitializeCountLabel()
        {
            this.countLabel.TranslatesAutoresizingMaskIntoConstraints = false;
            this.countLabel.Font = UIFont.SystemFontOfSize(size: 12, UIFontWeight.Regular);
            this.countLabel.TextColor = new UIColor(red: 0.239f, green: 0.282f, blue: 0.322f, alpha: 1.0f);
            this.View.AddSubview(this.countLabel);
            this.View.AddConstraints(new[]
            {
                this.countLabel.TopAnchor.ConstraintEqualTo(this.topBar.BottomAnchor, constant: 16),
                this.countLabel.LeadingAnchor.ConstraintEqualTo(this.View.SafeAreaLayoutGuide.LeadingAnchor, constant: 16)
            });
            this.countLabel.Text = $"Items {this.numberOfItems}";
        }

        private void InitializeBottomContainer()
        {
            this.bottomContainer.BackgroundColor = UIColor.White;
            this.resumeScanningButton.AddTarget(this.OnTapResumeScanning, UIControlEvent.TouchUpInside);
            this.resumeScanningButton.SetTitle(isOrderCompleted ? "SCAN NEXT ORDER" : "RESUME SCANNING", UIControlState.Normal);
            this.resumeScanningButton.TitleLabel.Font = UIFont.SystemFontOfSize(size: 16, UIFontWeight.Bold);

            this.clearListButton.AddTarget(this.OnTapClearList, UIControlEvent.TouchUpInside);
            this.clearListButton.SetTitle("CLEAR LIST", UIControlState.Normal);
            this.clearListButton.TitleLabel.Font = UIFont.SystemFontOfSize(size: 16, UIFontWeight.Bold);
            this.clearListButton.TranslatesAutoresizingMaskIntoConstraints = false;
            this.bottomContainer.TranslatesAutoresizingMaskIntoConstraints = false;
            this.resumeScanningButton.TranslatesAutoresizingMaskIntoConstraints = false;
            this.View.AddSubview(this.bottomContainer);
            this.bottomContainer.AddSubview(this.resumeScanningButton);
            this.bottomContainer.AddSubview(this.clearListButton);
            this.View.AddConstraints(new[]
            {
                this.bottomContainer.LeadingAnchor.ConstraintEqualTo(this.View.LeadingAnchor),
                this.bottomContainer.TrailingAnchor.ConstraintEqualTo(this.View.TrailingAnchor),
                this.bottomContainer.BottomAnchor.ConstraintEqualTo(this.View.BottomAnchor)
            });
            this.bottomContainer.AddConstraints(new[]
            {
                this.resumeScanningButton.TopAnchor.ConstraintEqualTo(this.bottomContainer.TopAnchor, constant: 16),
                this.resumeScanningButton.LeadingAnchor.ConstraintEqualTo(this.bottomContainer.LeadingAnchor,
                    constant: 32),
                this.resumeScanningButton.TrailingAnchor.ConstraintEqualTo(this.bottomContainer.TrailingAnchor,
                    constant: -32),
                this.resumeScanningButton.HeightAnchor.ConstraintEqualTo(constant: 51),
                this.clearListButton.TopAnchor.ConstraintEqualTo(this.resumeScanningButton.BottomAnchor, constant: 16),
                this.clearListButton.LeadingAnchor.ConstraintEqualTo(this.bottomContainer.LeadingAnchor, constant: 32),
                this.clearListButton.TrailingAnchor.ConstraintEqualTo(this.bottomContainer.TrailingAnchor, constant: -32),
                this.clearListButton.BottomAnchor.ConstraintEqualTo(this.bottomContainer.SafeAreaLayoutGuide.BottomAnchor,
                    constant: -16),
                this.clearListButton.HeightAnchor.ConstraintEqualTo(constant: 51)
            });
        }

        private void InitializeTableView()
        {
            this.tableView.TranslatesAutoresizingMaskIntoConstraints = false;
            this.tableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;
            this.tableView.BackgroundColor = new UIColor(red: 0.973f, green: 0.980f, blue: 0.988f, alpha: 1.0f);
            this.tableView.Source = new TableSource(this.scanResults);
            this.tableView.RegisterClassForCellReuse(typeof(ListCell), reuseIdentifier: ListCell.ReuseIdentifier);
            this.tableView.RowHeight = 85;
            this.tableView.AllowsSelection = false;
            this.tableView.ShowsVerticalScrollIndicator = false;
            this.View.AddSubview(tableView);
            this.View.AddConstraints(new[]
            {
                this.tableView.TopAnchor.ConstraintEqualTo(this.countLabel.BottomAnchor, constant: 8),
                this.tableView.LeadingAnchor.ConstraintEqualTo(this.View.LeadingAnchor),
                this.tableView.TrailingAnchor.ConstraintEqualTo(this.View.TrailingAnchor),
                this.tableView.BottomAnchor.ConstraintEqualTo(this.bottomContainer.TopAnchor)
            });

            this.resumeScanningButton.StyleAsPrimaryButton();
            this.clearListButton.StyleAsSecondaryButton();

            this.bottomContainer.Layer.ShadowColor = UIColor.Black.CGColor;
            this.bottomContainer.Layer.ShadowOffset = new CGSize(width: 0, height: -3);
            this.bottomContainer.Layer.ShadowRadius = 2.0f;
            this.bottomContainer.Layer.ShadowOpacity = 0.15f;
        }
    }
}
