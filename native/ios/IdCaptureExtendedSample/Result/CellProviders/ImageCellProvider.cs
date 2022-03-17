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
using UIKit;

namespace IdCaptureExtendedSample.Result.CellProviders
{
    public class ImageCellProvider : ICellProvider
    {
        private const string cellIdentifier = "ImageCell";

        private readonly UIImage image;
        private readonly string title;

        public ImageCellProvider(string title, UIImage image = null)
        {
            this.title = title;
            this.image = image;
        }

        public nfloat Height
        {
            get
            {
                if (this.image == null)
                {
                    return UITableView.AutomaticDimension;
                }

                var screenSize = UIScreen.MainScreen.Bounds.Size;
                var ratio = this.image.Size.Height / this.image.Size.Width;

                // We keep 160 px for the label and some margins.
                return (float)Math.Max((screenSize.Width - 160) * ratio, 140);
            }
        }

        public UITableViewCell DequeueAndConfigureCell(UITableView tableView)
        {
            UITableViewCell cell = tableView.DequeueReusableCell(cellIdentifier);

            if (cell == null)
            {
                cell = new UITableViewCell(UITableViewCellStyle.Subtitle, cellIdentifier);
            }

            cell.DetailTextLabel.Text = title;
            cell.ImageView.Image = image;
            cell.ImageView.ContentMode = UIViewContentMode.ScaleToFill;

            return cell;
        }
    }
}
