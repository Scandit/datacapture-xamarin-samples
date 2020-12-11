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

using StockCountSample.Model;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace StockCountSample.ViewModel
{
    public class MainViewModel : BaseViewModel
    {
        private readonly StockManager stockManager = new StockManager();

        public IEnumerable<Stock> Inventory => this.stockManager.Inventory.ToList();

        public string InventoryCountText { get { return string.Format("{0} Items", this.stockManager.TotalStocksCount); } }

        public IList<Product> ScannedProducts { get; set; }
 
        public bool ShowScannedProducts { get; private set; }

        public ICommand AddCommand { get; private set; }

        public ICommand AcceptCommand { get; private set; }

        public ICommand ClearCommand { get; private set; }

        public ICommand IncreaseQuantityCommand { get; private set; }

        public ICommand DecreaseQuantityCommand { get; private set; }

        public MainViewModel()
        {
            this.AddCommand = new Command(() =>
            {
                if (this.ScannedProducts == null || this.ScannedProducts.Count == 0)
                {
                    return;
                }

                this.stockManager.AddProducts(this.ScannedProducts);
                this.ShowScannedProducts = true;
                this.OnPropertyChanged(nameof(this.Inventory));
                this.OnPropertyChanged(nameof(this.InventoryCountText));
                this.OnPropertyChanged(nameof(this.ShowScannedProducts));

                Vibration.Vibrate();
            });

            this.AcceptCommand = new Command(() =>
            {
                this.ShowScannedProducts = false;
                this.OnPropertyChanged(nameof(this.ShowScannedProducts));                
            });

            this.ClearCommand = new Command(() =>
            {
                this.stockManager.Empty();
                this.ShowScannedProducts = false;

                this.OnPropertyChanged(nameof(this.ShowScannedProducts));
            });

            this.IncreaseQuantityCommand = new Command<Product>(product =>
            {
                this.stockManager.IncreaseProductQuantity(product);
                this.OnPropertyChanged(nameof(this.Inventory));
                this.OnPropertyChanged(nameof(this.InventoryCountText));
            });

            this.DecreaseQuantityCommand = new Command<Product>(product =>
            {
                this.stockManager.DecreaseProductQuantity(product);
                this.OnPropertyChanged(nameof(this.Inventory));
                this.OnPropertyChanged(nameof(this.InventoryCountText));
            });
        }
    }
}
