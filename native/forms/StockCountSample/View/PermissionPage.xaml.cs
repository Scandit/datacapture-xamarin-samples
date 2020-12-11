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

using Xamarin.Essentials;
using Xamarin.Forms;

namespace StockCountSample.View
{
    public partial class PermissionPage : ContentPage
    {
        private readonly MainPage scannerPage;

        public PermissionPage(MainPage scannerPage)
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);

            this.scannerPage = scannerPage;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            var task = await Permissions.CheckStatusAsync<Permissions.Camera>();
            if (task != PermissionStatus.Granted)
            {
                var result = await Permissions.RequestAsync<Permissions.Camera>();
                if (result == PermissionStatus.Granted)
                {
                    Application.Current.MainPage = new NavigationPage(this.scannerPage);
                }
            }
            else
            {
                Application.Current.MainPage = new NavigationPage(this.scannerPage);
            }
        }
    }
}
