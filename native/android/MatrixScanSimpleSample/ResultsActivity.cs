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
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using static Android.Support.V7.Widget.RecyclerView;

namespace MatrixScanSimpleSample
{
    [Activity(Label = "Scan Results")]
    public class ResultsActivity : Activity
    {
        private readonly static string ScanResultsId = "scan-results";

        public static Intent GetIntent(Context context, HashSet<ScanResult> scanResults)
        {
            Intent intent = new Intent(context, typeof(ResultsActivity));
            var data = JsonConvert.SerializeObject(scanResults.ToArray());
            intent.PutExtra(ScanResultsId, data);
            return intent;
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            this.SetContentView(Resource.Layout.activity_results);

            // Setup recycler view.
            RecyclerView recyclerView = this.FindViewById<RecyclerView>(Resource.Id.recycler_view);
            recyclerView.SetLayoutManager(new LinearLayoutManager(this));
            recyclerView.AddItemDecoration(
                    new DividerItemDecoration(recyclerView.Context, LinearLayoutManager.Vertical));

            // Receive results from previous screen and set recycler view items.
            string data = this.Intent.GetStringExtra(ScanResultsId) ?? string.Empty;
            var scanResults = JsonConvert.DeserializeObject<List<ScanResult>>(data);
            recyclerView.SetAdapter(new ScanResultsAdapter(this, scanResults));

            Button doneButton = FindViewById<Button>(Resource.Id.done_button);
            doneButton.Click += this.DoneButtonClick;

            this.ActionBar?.SetDisplayShowHomeEnabled(true);
            this.ActionBar?.SetDisplayHomeAsUpEnabled(true);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item.ItemId == Resource.Id.home)
            {
                this.OnBackPressed();
                return true;
            }

            return base.OnOptionsItemSelected(item);
        }

        private void DoneButtonClick(object sender, EventArgs e)
        {
            this.SetResult(Result.Ok);
            this.Finish();
        }
    }

    internal class ScanResultsAdapter : RecyclerView.Adapter
    {
        private readonly Context context;
        private readonly List<ScanResult> items;

        public ScanResultsAdapter(Context context, List<ScanResult> items)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
            this.items = items ?? new List<ScanResult>();
        }

        public override int ItemCount => this.items.Count;

        public override void OnBindViewHolder(ViewHolder holder, int position)
        {
            (holder as BarcodeViewHolder)?.Update(this.items[position]);
        }

        public override ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            return new BarcodeViewHolder(
                LayoutInflater.From(context).Inflate(Resource.Layout.scan_result_item, parent, false));
        }
    }

    internal class BarcodeViewHolder : ViewHolder
    {
        public TextView Data { get; private set; }
        public TextView Type { get; private set; }

        public BarcodeViewHolder(View itemView) : base(itemView)
        {
            this.Data = itemView.FindViewById<TextView>(Resource.Id.data_text);
            this.Type = itemView.FindViewById<TextView>(Resource.Id.type_text);
        }

        public void Update(ScanResult result)
        {
            this.Data.Text = result.Data;
            this.Type.Text = result.Symbology;
        }
    }
}
