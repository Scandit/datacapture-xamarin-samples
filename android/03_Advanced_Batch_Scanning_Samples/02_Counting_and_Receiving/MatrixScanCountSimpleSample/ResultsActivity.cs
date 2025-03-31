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
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.RecyclerView.Widget;
using MatrixScanCountSimpleSample.Data;
using MatrixScanCountSimpleSample.Views;

namespace MatrixScanCountSimpleSample
{
    [Activity(Label = "ResultsActivity")]
    public class ResultsActivity : AppCompatActivity
    {
        private static readonly string ARG_DONE_BUTTON_STYLE = "done-button-style";

        private DoneButtonStyle doneButtonStyle = DoneButtonStyle.Resume;

        public static Intent GetIntent(Context context, DoneButtonStyle style)
        {
            return new Intent(context, typeof(ResultsActivity))
                       .PutExtra(ARG_DONE_BUTTON_STYLE, (int)style);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item.ItemId == Android.Resource.Id.Home)
            {
                this.OnBackPressed();
                return true;
            }
            else
            {
                return base.OnOptionsItemSelected(item);
            }
        }

        protected override void OnCreate (Bundle savedInstanceState)
        {
            base.OnCreate (savedInstanceState);

            this.SetContentView(Resource.Layout.activity_results);
            this.doneButtonStyle = (DoneButtonStyle)this.Intent.GetIntExtra(
                ARG_DONE_BUTTON_STYLE, (int)DoneButtonStyle.Resume);

            // Setup recycler view.
            RecyclerView recyclerView = this.FindViewById<RecyclerView>(Resource.Id.recycler_view);
            recyclerView.SetLayoutManager(new LinearLayoutManager(this));
            recyclerView.AddItemDecoration(
                new DividerItemDecoration(recyclerView.Context, LinearLayoutManager.Vertical)
            );

            // Receive results from previous screen and set recycler view items.
            IList<ScanItem> scanResults = BarcodeManager.Instance.GetScanResults().Values.ToList();
            recyclerView.SetAdapter(new ScanResultsAdapter(this, scanResults));

            Button doneButton = this.FindViewById<Button>(Resource.Id.done_button);

            if (this.doneButtonStyle == DoneButtonStyle.Resume)
            {
                doneButton.SetText(Resource.String.resume_scanning);
            }
            else if (this.doneButtonStyle == DoneButtonStyle.NewScan)
            {
                doneButton.SetText(Resource.String.start_new_scanning);
            }

            doneButton.Click += (object sender, System.EventArgs e) =>
            {
                this.Finish();
            };

            Button clearButton = this.FindViewById<Button>(Resource.Id.clear_button);
            clearButton.Click += (object sender, System.EventArgs e) =>
            {
                this.SetResult(Result.FirstUser);
                this.Finish();
            };

            TextView resultsAmount = this.FindViewById<TextView>(Resource.Id.result_items_count);

            if (scanResults != null)
            {
                resultsAmount.Text = $"Items ({this.GetScanResultsCount(scanResults)})";
            }

            this.SupportActionBar?.SetDisplayShowHomeEnabled(true);
            this.SupportActionBar?.SetDisplayHomeAsUpEnabled(true);
        }

        protected override void OnPause()
        {
            // Pause camera if the app is going to background.
            if (!this.IsFinishing)
            {
                CameraManager.Instance.PauseFrameSource();

                // Save current barcodes as additional barcodes.
                BarcodeManager.Instance.SaveCurrentBarcodesAsAdditionalBarcodes();
            }

            base.OnPause();
        }

        private int GetScanResultsCount(IList<ScanItem> scanResults)
        {
            return scanResults.Sum(i => i.Quantity);
        }
    }
}
