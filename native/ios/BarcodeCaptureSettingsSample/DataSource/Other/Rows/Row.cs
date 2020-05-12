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

using Foundation;
using UIKit;

namespace BarcodeCaptureSettingsSample.DataSource.Other.Rows
{
    public abstract class Row
    {
        public string Title { get; }

        public abstract string ReuseIdentifier { get; }
        
        public abstract string DetailText { get; }
        
        public abstract UITableViewCellAccessory Accessory { get; }

        public abstract void CellSelected(Row row, NSIndexPath indexPath);

        public virtual void Bind(ref UITableViewCell cell)
        {
            cell.TextLabel.Text = this.Title;
            cell.DetailTextLabel.Text = this.DetailText;
            cell.Accessory = this.Accessory;
        }

        protected Row(string title)
        {
            this.Title = title;
        }
    }
}
