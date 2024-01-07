using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorManager : MonoBehaviour
{
    // 조정할 값(10퍼센트에 해당하는 값)
    public float adjustValueDKLx;
    public float adjustValueDKLy;

    // 색 입힐 Materials
    public Material referenceMaterial;
    public Material targetMaterial;

    // Current Reference Color Coordinate
    public List<Vector3> referenceColorsDKL;

    void Start()
    {
        // 리스트 초기화
        referenceColorsDKL = new List<Vector3>();

        // Reference Color 선언
        referenceColorsDKL.Add(new Vector3(100.0f, 100.0f, 100.0f));
        // 추가 가능

        // referenceColorsDKL List 사용.
        foreach (Vector3 color in referenceColorsDKL)
        {
            Debug.Log("Colors in DKL: " + color);
        }
    }
    Vector3 RGBtoXYZ(Vector3 rgb)
    {
        Matrix4x4 rgb2xyz = new Matrix4x4();
        rgb2xyz.SetRow(0, new Vector4(0.4124564f, 0.3575761f, 0.1804375f, 0));
        rgb2xyz.SetRow(1, new Vector4(0.2126729f, 0.7151522f, 0.0721750f, 0));
        rgb2xyz.SetRow(2, new Vector4(0.0193339f, 0.1191920f, 0.9503041f, 0));
        rgb2xyz.SetRow(3, new Vector4(0, 0, 0, 1));

        return rgb2xyz.MultiplyVector(rgb);
    }
    Vector3 XYZtoLMS(Vector3 xyz)
    {
        Matrix4x4 xyz2lms = new Matrix4x4();
        xyz2lms.SetRow(0, new Vector4(0.8951f, 0.2664f, -0.1614f, 0));
        xyz2lms.SetRow(1, new Vector4(-0.7502f, 1.7135f, 0.0367f, 0));
        xyz2lms.SetRow(2, new Vector4(0.0389f, -0.0685f, 1.0296f, 0));
        xyz2lms.SetRow(3, new Vector4(0, 0, 0, 1));

        return xyz2lms.MultiplyVector(xyz);
    }
    Vector3 LMStoDKL(Vector3 lms)
    {
        Matrix4x4 lms2dkl = new Matrix4x4();
        lms2dkl.SetRow(0, new Vector4(1, -1, 0, 0));
        lms2dkl.SetRow(1, new Vector4(-1, -1, 1, 0));
        lms2dkl.SetRow(2, new Vector4(1, 1, 0, 0));
        lms2dkl.SetRow(3, new Vector4(0, 0, 0, 1));

        return lms2dkl.MultiplyVector(lms);
    }
    Vector3 DKLtoLMS(Vector3 dkl)
    {
        Matrix4x4 dkl2lms = new Matrix4x4();
        dkl2lms.SetRow(0, new Vector4(0.5f, 0, 0.5f, 0));
        dkl2lms.SetRow(1, new Vector4(-0.5f, 0, 0.5f, 0));
        dkl2lms.SetRow(2, new Vector4(0, 1, 1, 0));
        dkl2lms.SetRow(3, new Vector4(0, 0, 0, 1));

        return dkl2lms.MultiplyVector(dkl);
    }
    Vector3 LMStoXYZ(Vector3 lms)
    {
        Matrix4x4 lms2xyz = new Matrix4x4();
        lms2xyz.SetRow(0, new Vector4(0.9869929f, -0.1470543f, 0.1599627f, 0));
        lms2xyz.SetRow(1, new Vector4(0.4323053f, 0.5183603f, 0.0492912f, 0));
        lms2xyz.SetRow(2, new Vector4(-0.0085287f, 0.0400428f, 0.9684867f, 0));
        lms2xyz.SetRow(3, new Vector4(0, 0, 0, 1));

        return lms2xyz.MultiplyVector(lms);
    }
    Vector3 XYZtoRGB(Vector3 xyz)
    {
        Matrix4x4 xyz2rgb = new Matrix4x4();
        xyz2rgb.SetRow(0, new Vector4(3.240455f, -1.537139f, -0.4985315f, 0));
        xyz2rgb.SetRow(1, new Vector4(-0.969266f, 1.876011f, 0.041556f, 0));
        xyz2rgb.SetRow(2, new Vector4(0.055643f, -0.204026f, 1.057225f, 0));
        xyz2rgb.SetRow(3, new Vector4(0, 0, 0, 1));

        return xyz2rgb.MultiplyVector(xyz);
    }

    Vector3 CalculateTargetDiskColor(Vector3 referenceColor)
    {
        adjustValueDKLx = 10.0f;
        adjustValueDKLy = 10.0f;

        Vector3 adjustedColor = new Vector3(
            referenceColor.x + adjustValueDKLx,
            referenceColor.y + adjustValueDKLy,
            referenceColor.z
        );
        return adjustedColor;
    }
}
