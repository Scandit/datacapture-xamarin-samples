# Scandit Xamarin Samples

This repository contains both simple and advanced samples that show you how use various features of the Scandit Data Capture SDK. The simple samples allow you to get going quickly, while the advanced samples show you how to use additional settings and setup the scanner for the best performance and user experience.

## **Pre-Built Barcode Scanning Components**

Scandit offers building blocks that can be integrated in just a few lines of code. The pre-built camera UI has been designed and user-tested to achieve superior process efficiency, ergonomics and usability.

### High Speed Single Barcode Scanning (**SparkScan)**

SparkScan is a camera-based solution for high-speed single scanning and scan-intensive workflows. It includes an out-of-the-box UI optimized for an efficient and frictionless user experience.

![SparkScan.png](https://github.com/Scandit/.github/blob/main/images/SparkScan.png)

**List Building Sample** (Xamarin ([iOS](<https://github.com/Scandit/datacapture-xamarin-samples/tree/master/ios/01_Single_Scanning_Samples/01_Barcode_Scanning_with_Pre_Built_UI/ListBuildingSample>), [Android](<https://github.com/Scandit/datacapture-xamarin-samples/tree/master/android/01_Single_Scanning_Samples/01_Barcode_Scanning_with_Pre_Built_UI/ListBuildingSample>)))

### Counting and Receiving in Batches (MatrixScan Count)

MatrixScan Count is an out-of-the-box scan and count solution for counting and receiving multiple items at once, in which user interface (UI) elements and interactions are built into a workflow.

![MSCount.png](https://github.com/Scandit/.github/blob/main/images/MSCount.png)

**MatrixScan Count Simple Sample** (Xamarin ([iOS](https://github.com/Scandit/datacapture-xamarin-samples/tree/master/ios/03_Advanced_Batch_Scanning_Samples/03_Counting_and_Receiving/MatrixScanCountSimpleSample), [Android](https://github.com/Scandit/datacapture-xamarin-samples/tree/master/android/03_Advanced_Batch_Scanning_Samples/03_Counting_and_Receiving/MatrixScanCountSimpleSample)))

### Scan One of Many Barcodes (Barcode Selection)

Barcode Selection is a pre-built component that provides a UI for selecting the right code when codes are crowded (and cannot be selected programmatically).

Consider Barcode Selection when **accuracy** is more important than **speed**.

- **Aim to Select** allows you to select one barcode at a time using an aimer, and tapping to confirm the selection. It is especially convenient for one-handed operation.

  ![AimToSelect.png](https://github.com/Scandit/.github/blob/main/images/AimToSelect.png)

- **Tap to select** is quicker when you need to select several barcodes, as demonstrated by the **Catalog Reordering Sample** (yep, those are teeth).

  ![TapToSelect.png](https://github.com/Scandit/.github/blob/main/images/TapToSelect.png)

**Barcode Selection Simple Sample (**Xamarin ([iOS](https://github.com/Scandit/datacapture-xamarin-samples/tree/master/ios/01_Single_Scanning_Samples/02_Barcode_Scanning_with_Low_Level_API/BarcodeSelectionSimpleSample), [Android](https://github.com/Scandit/datacapture-xamarin-samples/tree/master/android/01_Single_Scanning_Samples/02_Barcode_Scanning_with_Low_Level_API/BarcodeSelectionSimpleSample)))

## Fully-flexible API

The fully-flexible API provides the camera interface, viewfinders and minimal guidance.

### ID Scanning and Verification Samples

ID Scanning Samples demonstrate the features of the ID Capture API and demonstrate workflows such as Age Verified Delivery and US Drivers’ License Verification.

![IDScanning.png](https://github.com/Scandit/.github/blob/main/images/IDScanning.png)

**ID Capture Simple Sample** (Xamarin ([iOS](https://github.com/Scandit/datacapture-xamarin-samples/tree/master/ios/02_ID_Scanning_Samples/IdCaptureSimpleSample), [Android](https://github.com/Scandit/datacapture-xamarin-samples/tree/master/android/02_ID_Scanning_Samples/IdCaptureSimpleSample)))

**US Drivers’ License Verification Sample** ([Xamarin Forms](https://github.com/Scandit/datacapture-xamarin-forms-samples/tree/master02_ID_Scanning_Samples/USDLVerificationSample))

**ID Capture Extended Sample** (Xamarin ([iOS](https://github.com/Scandit/datacapture-xamarin-samples/tree/master/ios/02_ID_Scanning_Samples/IdCaptureExtendedSample), [Android](https://github.com/Scandit/datacapture-xamarin-samples/tree/master/android/02_ID_Scanning_Samples/IdCaptureExtendedSample)))

### Barcode Capture Samples

**Barcode Capture Simple Sample (**Xamarin ([iOS](https://github.com/Scandit/datacapture-xamarin-samples/tree/master/ios/01_Single_Scanning_Samples/02_Barcode_Scanning_with_Low_Level_API/BarcodeCaptureSimpleSample), [Android](https://github.com/Scandit/datacapture-xamarin-samples/tree/master/android/01_Single_Scanning_Samples/02_Barcode_Scanning_with_Low_Level_API/BarcodeCaptureSimpleSample)))

**Barcode Capture Reject Sample** ([i](https://github.com/Scandit/datacapture-ios-samples/tree/master/01_Single_Scanning_Samples/02_Barcode_Scanning_with_Low_Level_API/BarcodeCaptureRejectSample)Xamarin ([iOS](https://github.com/Scandit/datacapture-xamarin-samples/tree/master/ios/01_Single_Scanning_Samples/02_Barcode_Scanning_with_Low_Level_API/BarcodeCaptureRejectSample), [Android](https://github.com/Scandit/datacapture-xamarin-samples/tree/master/android/01_Single_Scanning_Samples/02_Barcode_Scanning_with_Low_Level_API/BarcodeCaptureRejectSample)))

**Barcode Capture Settings Sample** (Xamarin ([iOS](https://github.com/Scandit/datacapture-xamarin-samples/tree/master/ios/01_Single_Scanning_Samples/02_Barcode_Scanning_with_Low_Level_API/BarcodeCaptureSettingsSample), [Android](https://github.com/Scandit/datacapture-xamarin-samples/tree/master/android/01_Single_Scanning_Samples/02_Barcode_Scanning_with_Low_Level_API/BarcodeCaptureSettingsSample)))

### MatrixScan AR Sam**ples**

**MatrixScan Simple Sample** (Xamarin ([iOS](https://github.com/Scandit/datacapture-xamarin-samples/tree/master/ios/03_Advanced_Batch_Scanning_Samples/01_Batch_Scanning_and_AR_Info_Lookup/MatrixScanSimpleSample), [Android](https://github.com/Scandit/datacapture-xamarin-samples/tree/master/android/03_Advanced_Batch_Scanning_Samples/01_Batch_Scanning_and_AR_Info_Lookup/MatrixScanSimpleSample)))

**MatrixScan Bubbles Sample** (Xamarin ([iOS](https://github.com/Scandit/datacapture-xamarin-samples/tree/master/ios/03_Advanced_Batch_Scanning_Samples/01_Batch_Scanning_and_AR_Info_Lookup/MatrixScanBubblesSample), [Android](https://github.com/Scandit/datacapture-xamarin-samples/tree/master/android/03_Advanced_Batch_Scanning_Samples/01_Batch_Scanning_and_AR_Info_Lookup/MatrixScanBubblesSample)))

**MatrixScan Reject Sample** (Xamarin ([iOS](https://github.com/Scandit/datacapture-xamarin-samples/tree/master/ios/03_Advanced_Batch_Scanning_Samples/01_Batch_Scanning_and_AR_Info_Lookup/MatrixScanRejectSample), [Android](https://github.com/Scandit/datacapture-xamarin-samples/tree/master/android/03_Advanced_Batch_Scanning_Samples/01_Batch_Scanning_and_AR_Info_Lookup/MatrixScanRejectSample)))

## Samples on Other Frameworks

Samples on other frameworks are located at [https://github.com/scandit](https://github.com/scandit).

## Documentation

The Scandit Data Capture SDK documentation can be found here: Xamarin ([iOS](https://docs.scandit.com/data-capture-sdk/xamarin.ios/index.html), [Android](https://docs.scandit.com/data-capture-sdk/xamarin.android/index.html), [Forms](https://docs.scandit.com/data-capture-sdk/xamarin.forms/index.html))

## Sample Barcodes

Once you get the sample up and running, go find some barcodes to scan. Don’t feel like getting up from your desk? Here’s a [handy pdf of barcodes](https://github.com/Scandit/.github/blob/main/images/PrintTheseBarcodes.pdf) you can print out.

## Trial Signup

To add the Scandit Data Capture SDK to your app, sign up for your Scandit Developer Account and get instant access to your license key: [https://ssl.scandit.com/dashboard/sign-up?p=test](https://ssl.scandit.com/dashboard/sign-up?p=test)

## Support

Our support engineers can be reached at [support@scandit.com](mailto:support@scandit.com).

## License

[Apache 2.0](http://www.apache.org/licenses/LICENSE-2.0)
