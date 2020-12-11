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

using System.ComponentModel;
using System.Threading.Tasks;
using StockCountSample.ViewModel;
using Xamarin.Forms;

namespace StockCountSample.View
{
    [DesignTimeVisible(false)]
    public partial class MainPage
    {
        private bool inventoryListViewExpanded = false;

        private Rectangle fullScreen;
        private Rectangle splitScreenTop;
        private Rectangle splitScreenBottom;

        public MainPage()
        {
            NavigationPage.SetHasNavigationBar(this, false);

            this.InitializeComponent();
            this.BindingContext = new MainViewModel();
        }

        public void OnSleep()
        {
            this.Scanner.OnSleep();
        }

        public void OnResume()
        {
            this.Scanner.OnResume();
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);

            this.fullScreen = new Rectangle(0, 0, width, height);
            this.splitScreenTop = new Rectangle(0, 0, width, 0.7 * height);
            this.splitScreenBottom = new Rectangle(0, 0.6 * height, width, 0.4 * height);
        }

        public void InventoryCountClicked(System.Object sender, System.EventArgs e)
        {
            this.ToggleInventoryListViewSize();
        }

        private void ToggleInventoryListViewSize()
        {
            if (this.inventoryListViewExpanded)
            {
                this.InventoryListView
                    .LayoutTo(this.splitScreenBottom)
                    .ContinueWith((Task<bool> task) => this.inventoryListViewExpanded = false);
            }
            else
            {
                this.InventoryListView
                    .LayoutTo(this.fullScreen)
                    .ContinueWith((Task<bool> task) => this.inventoryListViewExpanded = true);
            }
        }

        public void InventoryListViewPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == "IsVisible")
            {
                if (this.InventoryListView.IsVisible)
                {
                    if (this.splitScreenTop != Rectangle.Zero)
                    {
                        this.BarcodePicker.LayoutTo(this.splitScreenTop);
                        this.InvalidateMeasure();
                    }
                }
                else
                {
                    if (this.fullScreen != Rectangle.Zero)
                    {
                        this.BarcodePicker.LayoutTo(this.fullScreen);
                        this.InvalidateMeasure();
                    }
                }
            }
        }
    }
}
