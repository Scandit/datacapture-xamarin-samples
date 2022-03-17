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
using Android.Content;
using Android.OS;
using AndroidX.AppCompat.App;
using AndroidX.Fragment.App;
using Scandit.DataCapture.ID.Data;

namespace IdCaptureExtendedSample.Scan
{
    public class AskScanBackSideDialogFragment : DialogFragment
    {
        private CapturedId capturedId;

        public event EventHandler BackSideScanAccepted;
        public event EventHandler<CapturedId> BackSideScanSkipped;

        public static AskScanBackSideDialogFragment Create(CapturedId capturedId)
        {
            return new AskScanBackSideDialogFragment()
            {
                capturedId = capturedId
            };
        }

        public override Android.App.Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            return new AlertDialog.Builder(this.RequireContext())
                                  .SetTitle(Resource.String.alert_scan_back_title)
                                  .SetMessage(Resource.String.alert_scan_back_message)
                                  .SetPositiveButton(Resource.String.scan, (object sender, DialogClickEventArgs args) =>
                                  {
                                      // If we continue with scanning the back of the document, the IdCapture settings
                                      // will automatically allow only for this to be scanned, blocking you from
                                      // scanning other front of IDs.The next `OnIdCaptured` will contain data from
                                      // both the front and the back scans.
                                      this.BackSideScanAccepted?.Invoke(this, EventArgs.Empty);
                                  })
                                  .SetNegativeButton(Resource.String.skip, (object sender, DialogClickEventArgs args) =>
                                  {
                                      if (sender is AlertDialog dialog)
                                      {
                                          dialog.Dismiss();
                                      }
                                      // If we want to skip scanning the back of the document, we have to call
                                      // `IdCapture.Reset()` to allow for another front IDs to be scanned.
                                      this.BackSideScanSkipped?.Invoke(this, this.capturedId);
                                  })
                                  .SetCancelable(false)
                                  .Show();
        }
    }
}
