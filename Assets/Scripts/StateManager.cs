using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;
using System;
using System.Text;
using System.ComponentModel.Design;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine.UIElements;
using UnityEngine.Rendering;

public class StateManager : MonoBehaviour
{
    // Scripts
    public ColorManager colorManager; // Color Manager Script 불러오기

    // Enum 상태 추적 변수
    private GameState currentGameState; // 현재 게임 상태를 추적하는 변수
    private EccentricityState currentEccentricityState; // 현재 이심률 상태를 추적하는 변수
    private ReferenceColor currentReferenceColor; // 현재 reference color 추적하는 변수
    private TargetDisk currentTargetDisk; // 현재 target(정답) disk
    private DimensionState currentDimensionState; // 현재 변경할 color space 축 상태를 추적하는 변수

    // Experiment Setting
    public int numberOfTrialsPerEccentricity; // 하나의 시야각 당 진행할 trial 수 설정.
    [SerializeField] private float diskShowingTime = 0.5f; // Disk Showing Time(Unity의 Inspector창에서 조절할 것) 기본값 0.5초

    // Material to apply
    public Material referenceMaterial;
    public Material targetMaterial;

    // Color Vector
    private Vector3 referenceColorInDKL;
    private Vector3 targetColorInRGB;

    // Game Objects
    [SerializeField] private TMP_Text startText; // 시작 텍스트
    [SerializeField] private GameObject diskManager_Eccentricity10; // Disk Manager Object - 10 Eccentricity
    [SerializeField] private GameObject diskManager_Eccentricity25;// Disk Manager Object - 25 Eccentricity
    [SerializeField] private GameObject diskManager_Eccentricity35;// Disk Manager Object - 35 Eccentricity

    // Trial 상태 변수
    private int trialNumber = 1; // Trial 번호. CSV 저장용
    private int reversalCount; // 현재 reversal count
    private int currentEccentricityTrialsCompleted = 0; // Eccentricity 별 현재 진행된 trial 수
    private bool isAnswered; // 답변 들어왔는지 파악하는 bool

    // File management
    private string csvFilePath;
    private bool isFirstEntry = true; // 파일이 처음 만들어질 수 있도록 하는 bool

    // Start is called before the first frame update
    void Start()
    {
        // CSV file name/path
        string fileName = "TestResults_" + GetFormattedDateTime() + ".csv";
        csvFilePath = Path.Combine(Application.dataPath, "CSV", fileName);

        // 실험시작
        ChangeGameState(GameState.Start);
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentGameState)
        {
            case GameState.Start:
                // 시작 상태에서의 로직
                if (Input.anyKeyDown)
                {
                    StartCoroutine(ExperimentPreparation()); // 시작 전 5초 대기 메소드. 대기 후 RunExperiment 시작됨.
                }
                break;
            case GameState.InGame:
                // 게임 진행 중 상태에서의 로직
                HandleDiskSelection();
                break;
            case GameState.Pause:
                // 일시 정지 상태에서의 로직
                break;
            case GameState.End:
                // 게임 종료 상태에서의 로직
                DeactivateDiskManagers();
                StopAllCoroutines();
                startText.text = "Experiment Over";
                break;
        }
    }




    // 실험 실행 메소드


    // 시야각 별 실험 실행 메소드
    IEnumerator RunExperiment()
    {
        while (currentEccentricityTrialsCompleted < numberOfTrialsPerEccentricity)
        {
            Debug.Log($"Current Eccentricity Trial Completed: {currentEccentricityTrialsCompleted}");
            isAnswered = false;
            StartCoroutine(RunSingleTrial());
            yield return new WaitUntil(() => isAnswered);

            currentEccentricityTrialsCompleted++;
        }

        if (currentEccentricityState == EccentricityState.Eccentricity_35 && currentEccentricityTrialsCompleted == numberOfTrialsPerEccentricity)
        {
            ChangeGameState(GameState.End);
        }

        // 위에서 numberOfTrialsPerEccentricity만큼 반복했으므로 Eccentricity 변경 또는 종료 로직 추가
        currentEccentricityTrialsCompleted = 0;
        ChangeEccentricityStateToNext();
    }
    // Trial 하나 실행 메소드
    IEnumerator RunSingleTrial()
    {
        Debug.Log("================================================================");
        Debug.Log($"Run Single Trial! Trial number: {trialNumber}");
        // 랜덤 디스크 1개 설정
        int randomTargetDiskNumber = SelectRandomTargetDiskNumber();

        // reference color 불러오기
        switch (currentReferenceColor)
        {
            case ReferenceColor.ReferenceColor1:
                referenceColorInDKL = colorManager.referenceColorsDKL[0];
                break;
            case ReferenceColor.ReferenceColor2:
                referenceColorInDKL = colorManager.referenceColorsDKL[1];
                break;
            case ReferenceColor.ReferenceColor3:
                referenceColorInDKL = colorManager.referenceColorsDKL[2];
                break;
            case ReferenceColor.ReferenceColor4:
                referenceColorInDKL = colorManager.referenceColorsDKL[3];
                break;
            case ReferenceColor.ReferenceColor5:
                referenceColorInDKL = colorManager.referenceColorsDKL[4];
                break;
        }
        Vector3 referenceColorInRGB = colorManager.DKLtoRGB(referenceColorInDKL);

        // target color 계산
        targetColorInRGB = colorManager.DKLtoRGB(colorManager.CalculateTargetColor(referenceColorInDKL));

        // ref mat에 색 입히기
        colorManager.SetMaterialColor(colorManager.referenceMaterial, new Vector3(referenceColorInRGB.x / 255, referenceColorInRGB.y / 255, referenceColorInRGB.z / 255));
        Debug.Log("(RunSigleTrial)Reference Color: " + referenceColorInRGB + " is applied to material");

        // target mat에 색 입히기
        colorManager.SetMaterialColor(colorManager.targetMaterial, new Vector3(targetColorInRGB.x / 255, targetColorInRGB.y / 255, targetColorInRGB.z / 255));
        Debug.Log("(RunSigleTrial)Target Color: " + targetColorInRGB + " is applied to material");

        // 이심률 변경 및 해당하는 디스크 켜기
        EccentricityState currentEccentricityState = GetCurrentEccentricityState();
        ActivateCurrentEccentricityDiskManager();

        // Trial 시작이므로 isAnswered false
        isAnswered = false;

        float elapsedTime = 0.0f;
        while (elapsedTime < diskShowingTime)
        {
            elapsedTime += Time.deltaTime;

            if (isAnswered)
            {
                break;
            }

            yield return null;
        }

        DeactivateDiskManagers();

        if (isAnswered)
        {
            Debug.Log("Answer received within 0.5 seconds.");
            yield return new WaitForSeconds(1.0f);
        }
        else
        {
            Debug.Log("Waiting for answer after 0.5 seconds");
            yield return new WaitUntil(() => isAnswered);
        }

        // trial 횟수와 reversal 업데이트 필요.

        // CSV 저장

    }

    // 5초 대기 후 실험 시작 메소드
    IEnumerator ExperimentPreparation()
    {
        // 5초 대기/남은 시간 보여주기
        for (int i = 0; i < 5; i++)
        {
            startText.text = $"Start in {5 - i}";
            yield return new WaitForSeconds(1f);
        }

        // 대기 후 실험 상태로 setting
        ChangeGameState(GameState.InGame);
        ChangeEccentricityState(EccentricityState.Eccentricity_10);

        // 실험 시작
        StartCoroutine(RunExperiment());
    }

    // 실험 실행 메소드 끝

    // 정답 입력 처리 메소드

    // 키보드 입력에 따른 정답 저장 및 ProcessAnswer 실행
    void HandleDiskSelection()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            Debug.Log("(StateManage.cs/HandleDiskSelection)Disk 1 as answer");
            ProcessAnswer(1);
            ChangeGameState(GameState.InGame);
            isAnswered = true;
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            Debug.Log("(StateManage.cs/HandleDiskSelection)Disk 2 as answer");
            ProcessAnswer(2);
            ChangeGameState(GameState.InGame);
            isAnswered = true;

        }
        else if (Input.GetKeyDown(KeyCode.Z))
        {
            Debug.Log("(StateManage.cs/HandleDiskSelection)Disk 3 as answer");
            ProcessAnswer(3);
            ChangeGameState(GameState.InGame);
            isAnswered = true;

        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            Debug.Log("(StateManage.cs/HandleDiskSelection)Disk 4 as answer");
            ProcessAnswer(4);
            ChangeGameState(GameState.InGame);
            isAnswered = true;

        }
    }

    // 답변이 변수로 들어오면 CSV로 저장하는 메소드
    void ProcessAnswer(int selectedAnswerNumber)
    {
        if (selectedAnswerNumber == currentTargetDiskNumber())
        {
            Debug.Log("(ProcessAnswer)Correct Answer");
        }
        else
        {
            Debug.Log("(ProcessAnswer)Wrong Answer");
        }

        // CSV 파일에 저장하는 로직
        SaveToCSV(selectedAnswerNumber, selectedAnswerNumber == currentTargetDiskNumber());

        // 다음 시행을 위한 상태 변경
        trialNumber++;
        ChangeGameState(GameState.InGame);
    }


    // ChangeState Methods
    public void ChangeGameState(GameState newGameState)
    {
        currentGameState = newGameState;

        // 상태 변경 시 필요한 로직을 여기에 추가
        switch (newGameState)
        {
            case GameState.Start:
                // 게임 시작으로 변경 시 실행 로직
                Debug.Log("Game State : Start");

                break;
            case GameState.InGame:
                // 게임 진행으로 변경 시 실행 로직
                Debug.Log("Game State : In Game");
                startText.gameObject.SetActive(false);

                break;
            case GameState.Pause:
                // 게임 일시 정지로 변경 시 실행 로직

                break;
            case GameState.End:
                // 게임 종료로 변경 시 실행 로직
                startText.gameObject.SetActive(true);
                break;
        }
    }
    public void ChangeEccentricityState(EccentricityState newEccentricityState)
    {
        currentEccentricityState = newEccentricityState;

        switch (newEccentricityState)
        {
            case EccentricityState.Eccentricity_10:
                // Eccentricity 10으로 변경될 때 실행 로직
                Debug.Log("Eccentricity Changed to 10");
                break;
            case EccentricityState.Eccentricity_25:
                // Eccentricitiy 25로 변경될 때 실행 로직
                StartCoroutine(RunExperiment());
                Debug.Log("Eccentricity Changed to 25");
                break;
            case EccentricityState.Eccentricity_35:
                // Eccentricitiy 35로 변경될 때 실행 로직
                StartCoroutine(RunExperiment());
                Debug.Log("Eccentricity Changed to 35");
                break;
        }
    }
    // Eccentricity 다음으로 변경하기
    public void ChangeEccentricityStateToNext()
    {
        switch (currentEccentricityState)
        {
            case EccentricityState.Eccentricity_10:
                ChangeEccentricityState(EccentricityState.Eccentricity_25);
                break;
            case EccentricityState.Eccentricity_25:
                ChangeEccentricityState(EccentricityState.Eccentricity_35);
                break;
            case EccentricityState.Eccentricity_35:
                break;
        }

        // Eccentricity 변경 후 관련 변수 초기화 또는 업데이트
        currentEccentricityTrialsCompleted = 0;

        // 현재 실행 중인 코루틴 중지 후 Eccentricity 변경하고 다시 실행(Eccentricity 변경 타이밍에 답변 기다리지 않는 문제 해결 목적)
        StopCoroutine(RunSingleTrial());
        StartCoroutine(RunSingleTrial());
    }


    // 저장 메소드
    void SaveToCSV(int answer, bool isCorrect)
    {
        // CSV 파일에 데이터 저장 로직
        string newDataLine = $"{currentEccentricityState};{trialNumber};{referenceColorInDKL};{targetColorInRGB};{currentTargetDisk};{answer};{isCorrect};{reversalCount}";

        // 파일이 없으면 새로 만들고, 있으면 데이터 추가
        if (!File.Exists(csvFilePath) || isFirstEntry)
        {
            isFirstEntry = false;
            File.WriteAllText(csvFilePath, "Eccentricity;Trial No.;ReferenceColorInRGB;TargetColorInRGB;TargetDisk;Answer;IsCorrect;resersalCount\n");
        }

        File.AppendAllText(csvFilePath, newDataLine + "\n");
    }

    private string GetFormattedDateTime()
    {
        DateTime now = DateTime.Now;

        return now.ToString("MMdd_HH-mm-ss");
    }


    // Disk 처리 메소드
    int SelectRandomTargetDiskNumber()
    {
        int randomTargetDiskNumber = UnityEngine.Random.Range(1, 5);
        Debug.Log("Target Disk Number: " + randomTargetDiskNumber);

        switch (randomTargetDiskNumber)
        {
            case 1:
                currentTargetDisk = TargetDisk.Disk1;
                break;
            case 2:
                currentTargetDisk = TargetDisk.Disk2;
                break;
            case 3:
                currentTargetDisk = TargetDisk.Disk3;
                break;
            case 4:
                currentTargetDisk = TargetDisk.Disk4;
                break;
        }
        return randomTargetDiskNumber;
    }
    int currentTargetDiskNumber()
    {
        int targetDiskNumber = 0;
        switch (currentTargetDisk)
        {
            case TargetDisk.Disk1:
                targetDiskNumber = 1;
                break;
            case TargetDisk.Disk2:
                targetDiskNumber = 2;
                break;
            case TargetDisk.Disk3:
                targetDiskNumber = 3;
                break;
            case TargetDisk.Disk4:
                targetDiskNumber = 4;
                break;
        }
        return targetDiskNumber;
    }
    public void DeactivateDiskManagers()
    {
        // 모든 디스크 비활성화
        diskManager_Eccentricity10.SetActive(false);
        diskManager_Eccentricity25.SetActive(false);
        diskManager_Eccentricity35.SetActive(false);
    }
    public void ActivateCurrentEccentricityDiskManager()
    {
        // 모든 디스크 비활성화
        diskManager_Eccentricity10.SetActive(false);
        diskManager_Eccentricity25.SetActive(false);
        diskManager_Eccentricity35.SetActive(false);

        // Current Eccentricity에 맞는 디스크 활성화.
        switch (GetCurrentEccentricityState())
        {
            case EccentricityState.Eccentricity_10:
                Debug.Log("(DiskManager.cs/ActivateCurrentEccentricityDiskManager) Current Eccentricity " + GetCurrentEccentricityState() + " is Activated");
                diskManager_Eccentricity10.SetActive(true);
                break;
            case EccentricityState.Eccentricity_25:
                Debug.Log("(DiskManager.cs/ActivateCurrentEccentricityDiskManager) Current Eccentricity " + GetCurrentEccentricityState());
                diskManager_Eccentricity25.SetActive(true);
                break;
            case EccentricityState.Eccentricity_35:
                Debug.Log("(DiskManager.cs/ActivateCurrentEccentricityDiskManager) Current Eccentricity is " + GetCurrentEccentricityState());
                diskManager_Eccentricity35.SetActive(true);
                break;
        }
    }
    public EccentricityState GetCurrentEccentricityState()
    {
        return currentEccentricityState;
    }

    public TargetDisk GetCurrentTargetDisk()
    {
        return currentTargetDisk;
    }

}



public enum GameState
{
    Start,
    InGame,
    Pause,
    End
}

public enum EccentricityState
{
    Eccentricity_10,
    Eccentricity_25,
    Eccentricity_35
}

public enum DimensionState
{
    xPositive, // L-M 양의 방향
    xNegative, // L-M 음의 방향
    yPositive, // S-(L+M) 양의 방향
    yNegative // S-(L+M) 음의 방향
}

public enum ReferenceColor
{
    ReferenceColor1,
    ReferenceColor2,
    ReferenceColor3,
    ReferenceColor4,
    ReferenceColor5
}

public enum TargetDisk
{
    Disk1,
    Disk2,
    Disk3,
    Disk4
}