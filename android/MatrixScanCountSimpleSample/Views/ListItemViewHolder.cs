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

using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using MatrixScanCountSimpleSample.Data;

namespace MatrixScanCountSimpleSample.Views
{
	public class ListItemViewHolder : RecyclerView.ViewHolder
    {
        private readonly TextView productDescriptionTextView;
        private readonly TextView gtinTextView;
        private readonly TextView quantityTextView;

        public ListItemViewHolder(View itemView) : base(itemView)
        {
            this.productDescriptionTextView = itemView.FindViewById<TextView>(Resource.Id.product_description);
            this.gtinTextView = itemView.FindViewById<TextView>(Resource.Id.gtin_text);
            this.quantityTextView = itemView.FindViewById<TextView>(Resource.Id.quantity_text);
        }

        public void Bind(int section, int position, ScanItem scanItem)
        {
            switch (section)
            {
                case 0:
                    {
                        var number = position + 1;
                        this.productDescriptionTextView.Text = $"Non-unique item {number}";
                        this.quantityTextView.Visibility = ViewStates.Visible;
                        this.quantityTextView.Text = $"Qty: {scanItem.Quantity}";
                    }
                    break;
                case 1:
                    {
                        var number = position + 1;
                        this.quantityTextView.Visibility = ViewStates.Gone;
                        this.productDescriptionTextView.Text = $"Item {number}";
                    }
                    break;
            }

            this.gtinTextView.Text = $"{scanItem.Symbology}: {scanItem.BarcodeData}";
        }
    }
}
