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
using Android.Content;
using Android.OS;
using Android.Views;
using AndroidX.RecyclerView.Widget;
using MatrixScanCountSimpleSample.Data;

namespace MatrixScanCountSimpleSample.Views
{
	public class ScanResultsAdapter : RecyclerView.Adapter
    {
        private readonly Context context;
        private readonly List<ScanItem> nonUniqueItems = new List<ScanItem>();
        private readonly List<ScanItem> uniqueItems = new List<ScanItem>();

        public ScanResultsAdapter(Context context, IList<ScanItem> items)
        {
            this.context = context;

            foreach (ScanItem item in items)
            {
                if (item.Quantity == 1)
                {
                    this.uniqueItems.Add(item);
                }
                else
                {
                    this.nonUniqueItems.Add(item);
                }
            }
        }

        public override int ItemCount
        {
            get
            {
                int totalCount = 0;
                for (int i = 0; i < this.SectionCount; i++)
                {
                    totalCount += this.GetItemCountForSection(i);
                }

                return totalCount;
            }
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            Tuple<int, int> positionInSection = this.GetPositionInSection(position);

            if (holder is ListItemViewHolder listItemViewHolder)
            {
                listItemViewHolder.Bind(
                    positionInSection.Item1,
                    positionInSection.Item2,
                    this.GetItemForPositionInSection(
                        positionInSection.Item1, positionInSection.Item2));
            }
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            return new ListItemViewHolder(
                LayoutInflater.From(parent.Context)
                              .Inflate(Resource.Layout.scan_result_item, parent, false));
        }

        private int SectionCount => 2;

        private int GetItemCountForSection(int section)
        {
            switch (section)
            {
                case 0:
                    return this.nonUniqueItems.Count;
                case 1:
                    return this.uniqueItems.Count;
                default:
                    return 0;
            }
        }

        private Tuple<int, int> GetPositionInSection(int position)
        {
            int accumulatedItemCount = 0;
            for (int i = 0; i < this.SectionCount; i++)
            {
                if (position < accumulatedItemCount + this.GetItemCountForSection(i))
                {
                    return new Tuple<int, int>(i, position - accumulatedItemCount);
                }
                accumulatedItemCount += this.GetItemCountForSection(i);
            }
            return new Tuple<int, int>(-1, 0);
        }

        private ScanItem GetItemForPositionInSection(int section, int position)
        {
            switch (section)
            {
                case 0:
                    return this.nonUniqueItems[position];
                case 1:
                    return this.uniqueItems[position];
                default:
                    return null;
            }
        }
    }
}
