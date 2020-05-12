# Available Samples

We have created simple samples that show you how to capture barcodes and how to use MatrixScan functionality.
The simple sample allows you to get going quickly.

## Barcode Capture Samples

|                               Simple Sample                              |                            Settings Sample                             | 
|:------------------------------------------------------------------------:|:------------------------------------------------------------------------:|
![alt text](/images/sample-bc-simple-1.jpg?raw=true "Simple Sample") ![alt text](/images/sample-bc-simple-2.jpg?raw=true "Simple Sample") | ![alt text](/images/sample-bc-settings-1.jpg?raw=true "Settings Sample") ![alt text](/images/sample-bc-settings-2.jpg?raw=true "Settings Sample") | 
| Basic sample that uses the camera to read a single barcode.              | Demonstrates how you can adapt the scanner settings best to your needs and experiment with all the options. |

## MatrixScan Samples

|                            Simple Sample                                 |                            Stock Count Sample                             | 
|:------------------------------------------------------------------------:|:------------------------------------------------------------------------:|
![alt text](/images/sample-ms-simple-1.jpg?raw=true "Simple Sample") ![alt text](/images/sample-ms-simple-2.jpg?raw=true "Simple Sample") | ![alt text](/images/sample-ms-bubble-1.jpg?raw=true "Stock Count Sample") ![alt text](/images/sample-ms-bubble-2.jpg?raw=true "Stock Count Sample") | 
| Very simple sample which shows how you can highlight barcodes on screen. | Demonstrates the use of more advanced augmented reality use cases with the SDK. | 

# Run the Samples

The best way to start working with the Scandit Data Capture SDK is to run one of our sample apps.

Before you can run a sample app, you need to go through a few simple steps:

  1. Clone or download the samples repository.

  2. Open the `Samples.sln` solution file in Visual Studio.

  3. Set the license key. To do this, sign in to your Scandit account and find your license key at <https://ssl.scandit.com/licenses/>. 
    Once you have the license key, add it to the sample:

      ```csharp
      // Enter your Scandit License key here.
      public const string SCANDIT_LICENSE_KEY = "-- ENTER YOUR SCANDIT LICENSE KEY HERE --";
      ```
     
      `SCANDIT_LICENSE_KEY` variables are placed in each sample project.

  4. Right-click the desired sample in Visual Studio and choose "Run project" (Mac) or "Set as StartUp Project" and press F5 (Windows) to start it. We recommend running our samples on a physical device as otherwise no camera is available.

# Documentation & Getting Started Guides

If you want to learn more, check the complete documentation and getting started guides available at 
  - [Xamarin.Android](https://docs.scandit.com/data-capture-sdk/xamarin.android/)
  - [Xamarin.iOS](https://docs.scandit.com/data-capture-sdk/xamarin.ios/)
