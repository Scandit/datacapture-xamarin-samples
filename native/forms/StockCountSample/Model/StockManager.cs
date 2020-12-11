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

using System.Collections.Generic;
using System.Linq;

namespace StockCountSample.Model
{
    public class StockManager
    {
        private readonly Dictionary<Product, int> warehouse;

        public IEnumerable<Stock> Inventory => this.warehouse.Select(item => Stock.Create(item.Key, item.Value));

        public StockManager()
        {
            this.warehouse = new Dictionary<Product, int>();
        }

        public int TotalStocksCount =>
            this.warehouse
                .Values
                .Aggregate(0, (count, value) => count += value);

        public int GetProductQuantity(Product product)
        {
            if (product != null && this.warehouse.ContainsKey(product))
            {
                return this.warehouse[product];
            }

            return 0;
        }

        public void IncreaseProductQuantity(Product product)
        {
            if (product != null && this.warehouse.ContainsKey(product))
            {
                this.warehouse[product] += 1;
            }
        }

        public void DecreaseProductQuantity(Product product)
        {
            if (product != null && this.warehouse.ContainsKey(product))
            {
                if (this.warehouse[product] > 0)
                {
                    this.warehouse[product] -= 1;
                }
            }
        }

        public void Empty()
        {
            this.warehouse.Clear();
        }

        public void Remove(Product product)
        {
            if (product != null)
            {
                this.warehouse.Remove(product);
            }
        }

        public void AddProducts(IList<Product> products)
        {
            foreach (var product in products)
            {
                if (this.warehouse.TryGetValue(product, out int value))
                {
                    this.warehouse[product] = value + 1;
                }
                else
                {
                    this.warehouse[product] = 1;
                }
            }
        }
    }
}
