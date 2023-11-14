using OpenCvSharp;
using OpenCvSharp.Demo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContourFinder : WebCamera
{
    [SerializeField] private FlipMode ImageFlip;
    [SerializeField] private float Threshold = 96.4f;
    [SerializeField] private bool ShowProcessingImage = true;
    [SerializeField] private float CurveAccuracy = 10f;
    [SerializeField] private float MinArea = 5000f;
    [SerializeField] private float MaxArea = 5000f;

    public Mat image;
    public Mat processImage = new Mat();
    private Point[][] countours;
    private HierarchyIndex[] hierarchy;
    private Vector2[] vectorList;

    protected override bool ProcessTexture(WebCamTexture input, ref Texture2D output)
    {
        image = OpenCvSharp.Unity.TextureToMat(input);

        Cv2.Flip(image, image, ImageFlip);
        Cv2.CvtColor(image, processImage, ColorConversionCodes.BGR2GRAY);
        Cv2.Threshold(processImage, processImage, Threshold, 255, ThresholdTypes.Binary);
        Cv2.FindContours(processImage, out countours, out hierarchy, RetrievalModes.Tree, ContourApproximationModes.ApproxSimple, null);
        
        Debug.Log(countours.Length);

        for (int i = 0; i < countours.Length; i++)
        {

        }


        foreach (Point[] vertice in countours)
        {
            Point[] points = Cv2.ApproxPolyDP(vertice, CurveAccuracy, true);
            var area = Cv2.ContourArea(vertice);

            Vector2[] vPoints = toVector2(vertice);

            if (area > MinArea && area < MaxArea)
            {
                drawContour(processImage, new Scalar(127, 127, 127), 2, points);

                Vector2 midPoint = CalculateMidPoint(vPoints);
                Cv2.Circle(processImage, new Point(midPoint.x, midPoint.y), 20, Scalar.Red);
            }
        }

        if (output == null)
        {
            output = OpenCvSharp.Unity.MatToTexture(ShowProcessingImage ? processImage : image);
        }
        else
        {
            OpenCvSharp.Unity.MatToTexture(ShowProcessingImage ? processImage : image, output);
        }

        return true;
    }

    private void drawContour(Mat Image, Scalar Color, int Thickness, Point[] Points)
    {
        for (int i = 1; i < Points.Length; i++)
        {
            Cv2.Line(Image, Points[i - 1], Points[i], Color, Thickness);
        }
        Cv2.Line(Image, Points[Points.Length - 1], Points[0], Color, Thickness);
    }

    private Vector2[] toVector2(Point[] points)
    {
        vectorList = new Vector2[points.Length];
        for (int i = 0; i < points.Length; i++)
        {
            vectorList[i] = new Vector2(points[i].X, points[i].Y);
        }
        return vectorList;
    }
    private Vector2 CalculateMidPoint(Vector2[] points)
    {
        if (points.Length == 0)
            return Vector2.zero;

        float sumX = 0f;
        float sumY = 0f;

        foreach (Vector2 point in points)
        {
            sumX += point.x;
            sumY += point.y;
        }

        float midX = sumX / points.Length;
        float midY = sumY / points.Length;

        return new Vector2(midX, midY);
    }
}