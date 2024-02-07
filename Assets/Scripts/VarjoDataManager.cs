using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.IO;
using UnityEngine.Assertions.Comparers;
using System.Runtime.CompilerServices;
using System;

public class VarjoDataManager : MonoBehaviour
{
    public GameObject Gaze, Head;
    private string csvFilePath;
    float[] RawDataArray = new float[8];
    float FrameNumber = 0;


    // Start is called before the first frame update
    void Start()
    {
        //파일 이름 정하기
        string fileName = "RawData_" + GetFormattedDateTime() + ".csv";
        // Path.Combine 설명하는 주석
        // Application.dataPath : Assets 폴더의 경로
        // "CSV" : CSV 폴더의 경로
        // fileName : 파일 이름
        csvFilePath = Path.Combine(Application.dataPath, "CSV", fileName);
        if (!File.Exists(csvFilePath))
        {
            File.WriteAllText(csvFilePath, $"Frame No.;Task Count;Gaze_x;Gaze_y;Gaze_z;Head_x;Head_y;Head_z");
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    void FixedUpdate()
    {
        FrameNumber += Time.fixedDeltaTime;
        SaveRawData();
    }

    void SaveRawData()
    {
        // 파일이 없다면 새로 만듦

        // CSV 파일에 데이터 저장 로직
        string newDataLine = $"{FrameNumber};{Gaze.transform.position.x};{Gaze.transform.position.y};{Gaze.transform.position.z};{Head.transform.position.x};{Head.transform.position.y};{Head.transform.position.z}\n";




        File.WriteAllText(csvFilePath, "Time;GazeX;GazeY;GazeZ;HeadX;HeadY;HeadZ\n");
    }
    private string GetFormattedDateTime()
    {
        DateTime now = DateTime.Now;

        return now.ToString("MMdd_HH-mm-ss");
    }
}
