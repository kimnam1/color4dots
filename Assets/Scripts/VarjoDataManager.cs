using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.IO;
using UnityEngine.Assertions.Comparers;
using System.Runtime.CompilerServices;
using System;
using JetBrains.Annotations;

public class VarjoDataManager : MonoBehaviour
{
    public GameObject Gaze, Head;
    [SerializeField] StateManager stateManager;

    private string csvFilePath;
    float[] RawDataArray = new float[8];
    float frameNumber = 0;

    private bool DataLoggingStart = true;
    private string testStartTime;


    // Start is called before the first frame update
    void Start()
    {
        testStartTime = GetFormattedDateTime();
    }
    void Awake()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

    void FixedUpdate()
    {
        frameNumber += Time.fixedDeltaTime;
        // epoch time을 nanoseconds로 구함
        DateTime timeSinceEpoch = (DateTime.UtcNow);
        long nanosecondsSinceEpoch = (long)timeSinceEpoch.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds * 1000000;
        SaveRawDataToCSV(nanosecondsSinceEpoch);
    }

    void SaveRawDataToCSV(long nanosecondsSinceEpoch)
    {
        //파일 이름 정하기
        string fileName = "RawData_" + testStartTime + ".csv";

        // File Path 정함
        csvFilePath = Path.Combine(Application.dataPath, "CSV", fileName);

        // 파일 없을 시 새로 만듦
        if (!File.Exists(csvFilePath))
        {
            File.AppendAllText(csvFilePath, $"Frame No.;Epoch Time;Game State;Trial Number;Eccentricity;Gaze_x;Gaze_y;Gaze_z;Head_x;Head_y;Head_z" + "\n");
        }

        string newDataLine = $"{frameNumber};{nanosecondsSinceEpoch};{stateManager.currentGameState};{stateManager.trialNumber};{stateManager.GetCurrentEccentricityState()};{Gaze.transform.position.x};{Gaze.transform.position.y};{Gaze.transform.position.z};{Head.transform.position.x};{Head.transform.position.y};{Head.transform.position.z}";
        File.AppendAllText(csvFilePath, newDataLine + "\n");
    }


    private string GetFormattedDateTime()
    {
        DateTime now = DateTime.Now;

        return now.ToString("MMdd_HH-mm-ss");
    }
}
