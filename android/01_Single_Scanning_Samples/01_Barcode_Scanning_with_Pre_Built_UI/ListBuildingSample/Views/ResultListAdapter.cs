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
using Android.Views;
using AndroidX.RecyclerView.Widget;
using ListBuildingSample.Models;

namespace ListBuildingSample.Views
{
    public class ResultListAdapter : RecyclerView.Adapter
    {
        private readonly List<ListItem> items = new List<ListItem>();

        public override int ItemCount => this.items.Count;

        public event EventHandler ListChanged;

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            if (holder is ListItemViewHolder listItemViewHolder)
            {
                ListItem item = this.items[position];
                listItemViewHolder.Bind(item, position);
            }
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            return new ListItemViewHolder(
                LayoutInflater.From(parent.Context)
                              .Inflate(Resource.Layout.result_item, parent, false));
        }

        public void AddListItem(ListItem item)
        {
            this.items.Add(item);
            this.NotifyItemInserted(this.ItemCount - 1);
            this.OnListChanged(EventArgs.Empty);
        }

        public void ClearResults()
        {
            this.items.Clear();
            this.NotifyDataSetChanged();
            this.OnListChanged(EventArgs.Empty);
        }

        protected virtual void OnListChanged(EventArgs eventArgs)
        {
            this.ListChanged?.Invoke(this, eventArgs);
        }
    }
}
