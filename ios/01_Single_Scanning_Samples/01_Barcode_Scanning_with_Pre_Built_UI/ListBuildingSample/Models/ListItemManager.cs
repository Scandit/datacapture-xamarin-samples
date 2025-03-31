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
using System.Threading;

namespace ListBuildingSample.Models
{
    public class ListItemManager
    {
        private readonly List<ListItem> items = new List<ListItem>();

        private ListItemManager()
        { }

        private static readonly Lazy<ListItemManager> instance = new Lazy<ListItemManager>(() =>
        {
            return new ListItemManager();
        }, LazyThreadSafetyMode.PublicationOnly);

        public static ListItemManager Instance => instance.Value;

        public event EventHandler ListsChanged;

        public IEnumerable<ListItem> Inventory => this.items;

        public int TotalItemsCount => this.items.Count;

        public void Clear()
        {
            this.items.Clear();
            this.OnProductsChanged(EventArgs.Empty);
        }

        public void AddItem(ListItem item)
        {
            if (item == null)
            {
                return;
            }

            this.items.Add(item);
            this.OnProductsChanged(EventArgs.Empty);
        }

        protected virtual void OnProductsChanged(EventArgs eventArgs)
        {
            this.ListsChanged?.Invoke(this, eventArgs);
        }
    }
}
