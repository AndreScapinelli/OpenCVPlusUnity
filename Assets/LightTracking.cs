using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenCvSharp;
using UnityEngine.Windows.WebCam;
using OpenCvSharp.Demo;
using Unity.VisualScripting;

public class LightTracking : WebCamera
{
    private Mat image;
    public Color lowerColor;
    public Color upperColor;

    protected override bool ProcessTexture(WebCamTexture input, ref Texture2D output)
    {
        image = OpenCvSharp.Unity.TextureToMat(input);

        // Convert BGR to HSV
        Cv2.CvtColor(image, image, ColorConversionCodes.BGR2GRAY);

        // Define the red color range in HSV
        //Scalar lowerRed = new Scalar(RGBtoHSV(lowerColor).x, RGBtoHSV(lowerColor).y, RGBtoHSV(lowerColor).z);
        //Scalar upperRed = new Scalar(RGBtoHSV(upperColor).x, RGBtoHSV(upperColor).y, RGBtoHSV(upperColor).z);
        Scalar lowerRed = new Scalar(255, 255, 255);
        Scalar upperRed = new Scalar(255, 255, 255);

        // Create a mask for the red color
        Mat mask = new Mat();
        Cv2.InRange(image, lowerRed, upperRed, mask);

        // Find the location of the maximum value in the mask
        Point maxLoc;
        double minVal, maxVal;
        Cv2.MinMaxLoc(mask, out minVal, out maxVal, out _, out maxLoc);

        // Draw a circle around the detected point
        Cv2.Circle(image, maxLoc, 20, Scalar.Red, 2, LineTypes.AntiAlias);

        if (output == null)
        {
            output = OpenCvSharp.Unity.MatToTexture(image);
        }
        else
        {
            OpenCvSharp.Unity.MatToTexture(image, output);
        }

        // Release the resources
        mask.Release();
        image.Release();

        return true;
    }

    Vector3 RGBtoHSV(Color rgbColor)
    {
        // Convert RGB to OpenCV Mat
        Mat bgrMat = new Mat(1, 1, MatType.CV_8UC3, new Scalar(rgbColor.r * 255, rgbColor.g * 255, rgbColor.b * 255));

        // Convert BGR to HSV
        Mat hsvMat = new Mat();
        Cv2.CvtColor(bgrMat, hsvMat, ColorConversionCodes.BGR2HSV);

        // Extract HSV values
        Vec3b hsvPixel = hsvMat.Get<Vec3b>(0, 0);
        Vector3 hsvValues = new Vector3(hsvPixel.Item0, hsvPixel.Item1, hsvPixel.Item2);

        // Release resources
        bgrMat.Release();
        hsvMat.Release();

        return hsvValues;
    }
}
