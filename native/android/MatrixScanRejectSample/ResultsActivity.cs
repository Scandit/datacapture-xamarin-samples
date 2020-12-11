//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
//
//        http://www.apache.org/licenses/LICENSE-2.0
//
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using MatrixScanRejectSample.Data;

namespace MatrixScanRejectSample
{
    [Activity(Label = "@string/scan_results")]
    public class ResultsActivity : Activity
    {
        public const int RESULT_CODE_CLEAN = 1;
        private const String ARG_SCAN_RESULTS = "scan-results";

        public static Intent GetIntent(Context context, HashSet<ScanResult> scanResults)
        {
            return new Intent(context, typeof(ResultsActivity))
                .PutExtra(ARG_SCAN_RESULTS, scanResults.ToArray());
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_results);

            // Setup recycler view.
            var recyclerView = FindViewById<RecyclerView>(Resource.Id.recycler_view);
            recyclerView.SetLayoutManager(new LinearLayoutManager(this));
            recyclerView.AddItemDecoration(
                    new DividerItemDecoration(recyclerView.Context, LinearLayoutManager.Vertical));

            // Receive results from previous screen and set recycler view items.
            var scanResults = Intent.GetParcelableArrayExtra(ARG_SCAN_RESULTS);
            recyclerView.SetAdapter(new ScanResultsAdapter(scanResults));

            FindViewById<Button>(Resource.Id.done_button).Click += DoneButton_Click;
        }

        private void DoneButton_Click(object sender, EventArgs e)
        {
            SetResult((Result)RESULT_CODE_CLEAN);
            Finish();
        }

        private class ScanResultsAdapter : RecyclerView.Adapter
        {
            private readonly IParcelable[] source;

            internal ScanResultsAdapter(IParcelable[] source)
            {
                this.source = source;
            }

            public override int ItemCount => source.Length;

            public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
            {
                (holder as ViewHolder)?.Update(source.GetValue(position) as ScanResult);
            }

            public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
            {
                return new ViewHolder(
                   LayoutInflater.From(parent.Context).Inflate(Resource.Layout.scan_result_item, parent, false));
            }
        }

        private class ViewHolder : RecyclerView.ViewHolder
        {
            private TextView dataTextView;
            private TextView typeTextView;

            internal ViewHolder(View itemView) : base(itemView)
            {
                dataTextView = itemView.FindViewById<TextView>(Resource.Id.data_text);
                typeTextView = itemView.FindViewById<TextView>(Resource.Id.type_text);
            }

            public void Update(ScanResult scanResult)
            {
                dataTextView.Text = scanResult.Data;
                typeTextView.Text = scanResult.ReadableName;
            }
        }
    }
}
