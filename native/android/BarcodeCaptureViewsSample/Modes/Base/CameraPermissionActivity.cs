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

using Android;
using Android.Annotation;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using AndroidX.AppCompat.App;

namespace BarcodeCaptureViewsSample.Modes.Base
{
    public abstract class CameraPermissionActivity : AppCompatActivity
    {
        private const string CAMERA_PERMISSION = Manifest.Permission.Camera;
        private const int CAMERA_PERMISSION_REQUEST = 0;

        private bool userDeniedPermissionOnce;
        private bool paused = true;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        protected override void OnPause()
        {
            base.OnPause();
            this.paused = true;
        }

        protected override void OnResume()
        {
            base.OnResume();
            this.paused = false;
        }

        protected bool HasCameraPermission()
        {
            return Build.VERSION.SdkInt < BuildVersionCodes.M
                    || CheckSelfPermission(CAMERA_PERMISSION) == Permission.Granted;
        }

        [TargetApi(@Value = (int)BuildVersionCodes.M)]
        protected void RequestCameraPermission()
        {
            // For Android M and onwards we need to request the camera permission from the user.
            if (!this.HasCameraPermission())
            {
                // The user already denied the permission once, we don't ask twice.
                if (!this.userDeniedPermissionOnce)
                {
                    this.RequestPermissions(new string[] { CAMERA_PERMISSION }, CAMERA_PERMISSION_REQUEST);
                }
            }
            else
            {
                // We already have the permission or don't need it.
                this.OnCameraPermissionGranted();
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            if (requestCode == CAMERA_PERMISSION_REQUEST)
            {
                if (grantResults.Length > 0 && grantResults[0] == Permission.Granted)
                {
                    this.userDeniedPermissionOnce = false;
                    if (!paused)
                    {
                        // Only call the function if not paused - camera should not be used otherwise.
                        this.OnCameraPermissionGranted();
                    }
                }
                else
                {
                    // The user denied the permission - we are not going to ask again.
                    this.userDeniedPermissionOnce = true;
                }
            }
            else
            {
                base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            }
        }

        protected abstract void OnCameraPermissionGranted();
    }
}
