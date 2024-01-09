using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorManager : MonoBehaviour
{
    // 조정할 값(10퍼센트에 해당하는 값)
    public float adjustValueDKLX;
    public float adjustValueDKLY;

    // 색 입힐 Materials
    public Material referenceMaterial;
    public Material targetMaterial;

    // Current Reference Colors Coordinate
    public List<Vector3> referenceColorsDKL;

    void Start()
    {
        // 리스트 초기화
        referenceColorsDKL = new List<Vector3>();

        // Reference Color 선언
        referenceColorsDKL.Add(new Vector3(93.0f, -116.0f, 121.0f));
        referenceColorsDKL.Add(new Vector3(-119.8981253f, -348.1355644f, 370.4842407f));
        referenceColorsDKL.Add(new Vector3(1.06440418f, 237.1414263f, 12.88814752f));
        referenceColorsDKL.Add(new Vector3(-26.30661874f, -464.681383f, 492.4826335f));
        referenceColorsDKL.Add(new Vector3(100.0f, 200.0f, 100.0f));

        // 추가 가능 : 추가 시 StateManager의 RunSinglTrial에서 개수에 맞게 변경해줘야 함.


    }

    void Update()
    {

    }
    public Vector3 RGBtoXYZ(Vector3 rgb)
    {
        Matrix4x4 rgb2xyz = new Matrix4x4();
        rgb2xyz.SetRow(0, new Vector4(0.4124564f, 0.3575761f, 0.1804375f, 0));
        rgb2xyz.SetRow(1, new Vector4(0.2126729f, 0.7151522f, 0.0721750f, 0));
        rgb2xyz.SetRow(2, new Vector4(0.0193339f, 0.1191920f, 0.9503041f, 0));
        rgb2xyz.SetRow(3, new Vector4(0, 0, 0, 1));

        return rgb2xyz.MultiplyVector(rgb);
    }
    public Vector3 XYZtoLMS(Vector3 xyz)
    {
        Matrix4x4 xyz2lms = new Matrix4x4();
        xyz2lms.SetRow(0, new Vector4(0.8951f, 0.2664f, -0.1614f, 0));
        xyz2lms.SetRow(1, new Vector4(-0.7502f, 1.7135f, 0.0367f, 0));
        xyz2lms.SetRow(2, new Vector4(0.0389f, -0.0685f, 1.0296f, 0));
        xyz2lms.SetRow(3, new Vector4(0, 0, 0, 1));

        return xyz2lms.MultiplyVector(xyz);
    }
    public Vector3 LMStoDKL(Vector3 lms)
    {
        Matrix4x4 lms2dkl = new Matrix4x4();
        lms2dkl.SetRow(0, new Vector4(1, -1, 0, 0));
        lms2dkl.SetRow(1, new Vector4(-1, -1, 1, 0));
        lms2dkl.SetRow(2, new Vector4(1, 1, 0, 0));
        lms2dkl.SetRow(3, new Vector4(0, 0, 0, 1));

        return lms2dkl.MultiplyVector(lms);
    }
    public Vector3 DKLtoLMS(Vector3 dkl)
    {
        Matrix4x4 dkl2lms = new Matrix4x4();
        dkl2lms.SetRow(0, new Vector4(0.5f, 0, 0.5f, 0));
        dkl2lms.SetRow(1, new Vector4(-0.5f, 0, 0.5f, 0));
        dkl2lms.SetRow(2, new Vector4(0, 1, 1, 0));
        dkl2lms.SetRow(3, new Vector4(0, 0, 0, 1));

        return dkl2lms.MultiplyVector(dkl);
    }
    public Vector3 LMStoXYZ(Vector3 lms)
    {
        Matrix4x4 lms2xyz = new Matrix4x4();
        lms2xyz.SetRow(0, new Vector4(0.9869929f, -0.1470543f, 0.1599627f, 0));
        lms2xyz.SetRow(1, new Vector4(0.4323053f, 0.5183603f, 0.0492912f, 0));
        lms2xyz.SetRow(2, new Vector4(-0.0085287f, 0.0400428f, 0.9684867f, 0));
        lms2xyz.SetRow(3, new Vector4(0, 0, 0, 1));

        return lms2xyz.MultiplyVector(lms);
    }
    public Vector3 XYZtoRGB(Vector3 xyz)
    {
        Matrix4x4 xyz2rgb = new Matrix4x4();
        xyz2rgb.SetRow(0, new Vector4(3.240455f, -1.537139f, -0.4985315f, 0));
        xyz2rgb.SetRow(1, new Vector4(-0.969266f, 1.876011f, 0.041556f, 0));
        xyz2rgb.SetRow(2, new Vector4(0.055643f, -0.204026f, 1.057225f, 0));
        xyz2rgb.SetRow(3, new Vector4(0, 0, 0, 1));

        return xyz2rgb.MultiplyVector(xyz);
    }
    public Vector3 DKLtoRGB(Vector3 xyz)
    {
        Debug.Log("(ColorManager.cs)DKL " + xyz + " is Changed to RGB!!" + ", RGB:" + XYZtoRGB(LMStoXYZ(DKLtoLMS(xyz))));
        return XYZtoRGB(LMStoXYZ(DKLtoLMS(xyz)));
    }

    public Vector3 RGBtoDKL(Vector3 rgb)
    {
        Debug.Log("(ColorManager.cs)RGB " + rgb + "is Changed to DKL!!" + ", DKL:" + LMStoDKL(XYZtoLMS(RGBtoXYZ(rgb))));
        return LMStoDKL(XYZtoLMS(RGBtoXYZ(rgb)));
    }

    public Vector3 CalculateTargetColor(Vector3 referenceColor)
    {
        adjustValueDKLX = 0.0f;
        adjustValueDKLY = 0.0f;

        Vector3 adjustedColor = new Vector3(
            referenceColor.x - 50.0f,
            referenceColor.y,
            referenceColor.z
        );
        Debug.Log("(ColorManager.cs/CalculateTargetColor)Calculate Target Color: Target Color is " + adjustedColor);
        return adjustedColor;
    }

    public void SetMaterialColor(Material materialToSet, Vector3 color)
    {
        Color colorToApply = new Color(color.x, color.y, color.z);

        materialToSet.color = colorToApply;
    }
}
