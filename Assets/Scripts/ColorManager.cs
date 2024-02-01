using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Video;

public class ColorManager : MonoBehaviour
{
    public StateManager stateManager; // State Manager Script 불러오기

    // 조정할 값(10퍼센트에 해당하는 값)
    public float adjustValueDKLX;
    public float adjustValueDKLY;
    private float DKLxUpperLimit = 255.0f;
    private float DKLxBottomLimit = 0.0f;
    private float DKLyUpperLimit = 200.0f;
    private float DKLyBottomLimit = -200.0f;

    // 색 입힐 Materials
    public Material referenceMaterial;
    public Material targetMaterial;

    // Current Reference Colors Coordinate
    public List<Vector3> referenceColorsDKL;
    public List<Vector3> referenceColorsRGB;

    void Start()
    {
        // 리스트 초기화
        referenceColorsDKL = new List<Vector3>();

        // Reference Color 선언
        referenceColorsDKL.Add(new Vector3(127f, -0.5f, 0f));
        referenceColorsDKL.Add(new Vector3(63.5f, -0.5f, 0f));
        referenceColorsDKL.Add(new Vector3(190.5f, -0.5f, 0f));
        referenceColorsDKL.Add(new Vector3(127f, -100.25f, 0f));
        referenceColorsDKL.Add(new Vector3(127f, 99.25f, 0f));

        // RGB 리스트 초기화
        referenceColorsRGB = new List<Vector3>();

        // Reference Color in RGB 선언
        referenceColorsRGB.Add(new Vector3(64f, 128f, 0f));
        referenceColorsRGB.Add(new Vector3(128f, 64f, 0f));
        referenceColorsRGB.Add(new Vector3(128f, 128f, 0f));
        referenceColorsRGB.Add(new Vector3(192f, 128f, 0f));
        referenceColorsRGB.Add(new Vector3(128f, 192f, 0f));

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


    public Vector3 RGBtoDKLCartbyPsychoPy(Vector3 rgb)
    {
        Matrix4x4 rgb2dkl = new Matrix4x4();
        rgb2dkl.SetRow(0, new Vector4(0.25145542f, 0.64933633f, 0.09920825f, 0));
        rgb2dkl.SetRow(1, new Vector4(0.78737943f, -0.55586618f, -0.23151325f, 0));
        rgb2dkl.SetRow(2, new Vector4(0.26562825f, 0.63933074f, -0.90495899f, 0));
        rgb2dkl.SetRow(3, new Vector4(0, 0, 0, 1));

        return rgb2dkl.MultiplyVector(rgb);
    }
    
    public Vector3 DKLCarttoRGBbyPsychoPy(Vector3 dkl)
    {
        Matrix4x4 dkl2rgb = new Matrix4x4();
        dkl2rgb.SetRow(0, new Vector4(1.0f, 0.999999998f, -0.146199995f, 0));
        dkl2rgb.SetRow(1, new Vector4(1.0f, -0.389999999f, 0.209400005f, 0));
        dkl2rgb.SetRow(2, new Vector4(1.0f, 0.018f, -0.999999998f, 0));
        dkl2rgb.SetRow(3, new Vector4(0, 0, 0, 1));

        return dkl2rgb.MultiplyVector(dkl);
    }


    public Vector3 CalculateTargetColor(Vector3 referenceColor, DimensionState dimensionState, int difficulty)
    {
        float defaultDifficultyStepValueDKLx = 2.55f; // L-M dimension에서 최대값에서 최소값 뺀 전체의 1%를 기본 step value로 잡음
        float defaultDifficultyStepValueDKLy = 4f; // S-(L+M) dimension에서 최대값에서 최소값 뺀 전체의 1%를 기본 step value로 잡음

        Vector3 adjustedColor = new Vector3(0f, 0f, 0f);

        if (dimensionState == DimensionState.xPositive)
        {
            float defaultDifficultyValue = (DKLxUpperLimit + referenceColor[0])/2; // X+의 Upper Limit에서 현재 Reference Color X 구간의 중간
            
            if (difficulty > 50){
                float difficultyStepValue = defaultDifficultyValue / 50; // X+의 Upper Limit에서 현재 Reference Color X 값 뺀 구간을 50으로 나눔 = 1%
            }
            else{
                float difficultyStepValue = defaultDifficultyStepValueDKLx; // 2.55f 씩 늘어남.
            }

            float difficultyAppliedValue = defaultDifficultyValue - (difficultyStepValue * (difficulty - 50));

            adjustedColor = new Vector3(
                referenceColor.x + difficultyAppliedValueX,
                referenceColor.y,
                referenceColor.z
                );
        }
        else if (dimensionState == DimensionState.xNegative)
        {
            adjustedColor = new Vector3(
                referenceColor.x - difficultyAppliedValueX,
                referenceColor.y,
                referenceColor.z
                );
        }
        else if (dimensionState == DimensionState.yPositive)
        {
            adjustedColor = new Vector3(
                referenceColor.x,
                referenceColor.y + difficultyAppliedValueY,
                referenceColor.z
                );
        }
        else if (dimensionState == DimensionState.yNegative)
        {
            adjustedColor = new Vector3(
                referenceColor.x,
                referenceColor.y - difficultyAppliedValueY,
                referenceColor.z
                );

        }
        Debug.Log("(ColorManager.cs/CalculateTargetColor)Calculate Target Color: Target Color is " + adjustedColor);
        return adjustedColor;
    }

    public Vector3 CalculateTargetColorTestRGB(Vector3 referenceColor, DimensionState dimensionState, int difficulty)
    {
        float adjustValueInRGB = 256f * 25 / 100; // RGB의 기본 난이도 25%
        float difficultyStepValueInRGB = 256f * 25 / 100 / stateManager.defaultDifficulty;// 기본 Difficulty 50에 전체 0~100이니까 Difficulty 한단계 당 난이도 25%의 1/50 = 1/2%
        float difficultyAppliedValue = adjustValueInRGB - ((difficulty - 50) * difficultyStepValueInRGB);
        Vector3 adjustedColor = new Vector3(0f, 0f, 255f); // 기본 파란색. 아래 switch 제대로 실행 안되면 파란색으로 보이게

        if (dimensionState == DimensionState.xPositive)
        {
            adjustedColor = new Vector3(
                referenceColor.x + difficultyAppliedValue,
                referenceColor.y,
                referenceColor.z
                );
        }
        else if (dimensionState == DimensionState.xNegative)
        {
            adjustedColor = new Vector3(
                referenceColor.x - difficultyAppliedValue,
                referenceColor.y,
                referenceColor.z
                );
        }
        else if (dimensionState == DimensionState.yPositive)
        {
            adjustedColor = new Vector3(
                referenceColor.x,
                referenceColor.y + difficultyAppliedValue,
                referenceColor.z
                );
        }
        else if (dimensionState == DimensionState.yNegative)
        {
            adjustedColor = new Vector3(
                referenceColor.x,
                referenceColor.y - difficultyAppliedValue,
                referenceColor.z
                );

        }

        Debug.Log("(ColorManager.cs/CalculateTargetColorTestRGB)Calculate Target Color: Target Color is " + adjustedColor);

        return adjustedColor;
    }

    public void SetMaterialColor(Material materialToSet, Vector3 colorInRGB)
    {
        Color colorToApply = new Color(colorInRGB.x, colorInRGB.y, colorInRGB.z);

        materialToSet.color = colorToApply;
    }

}
