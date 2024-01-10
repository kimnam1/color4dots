using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;
using System;
using System.Text;
using System.ComponentModel.Design;
using JetBrains.Annotations;

public class StateManager : MonoBehaviour
{
    public ColorManager colorManager; // Color Manager Script 불러오기
    public TextMeshProUGUI startText; // 시작 텍스트
    private GameState currentGameState; // 현재 게임 상태를 추적하는 변수
    private EccentricityState currentEccentricityState; // 현재 이심률 상태를 추적하는 변수
    private DimensionState currentDimensionState; // 현재 변경할 color space 축 상태를 추적하는 변수
    private ReferenceColor currentReferenceColor; // 현재 reference color
    private Vector3 referenceColorInRGB;
    private Vector3 targetColorInRGB;
    private int trialNumber = 1; // Trial 번호.
    private int reversalCount; // 현재 reversal count
    private TargetDisk currentTargetDisk; // 현재 target(정답) disk
    public int numberOfTrialsPerEccentricity; // 실험 trial 설정.
    private int currentEccentricityTrialsCompleted = 0; // Eccentricity 별 현재 진행된 trial 수
    private int totalTrialsCompleted = 0; // 총 완료한 시행 횟수



    // Material to apply
    public Material referenceMaterial;
    public Material targetMaterial;

    private string csvFilePath;
    private bool isFirstEntry = true;


    // Disk Showing Time(Unity의 Inspector창에서 조절할 것)
    [SerializeField] private float diskShowingTime = 0.5f; // 기본값 0.5초
    // Disk Manager Object 변수
    [SerializeField] private GameObject diskManager_Eccentricity10;
    [SerializeField] private GameObject diskManager_Eccentricity25;
    [SerializeField] private GameObject diskManager_Eccentricity35;


    // Start is called before the first frame update
    void Start()
    {
        // CSV file name/path
        string fileName = "TestResults_" + GetFormattedDateTime() + ".csv";
        csvFilePath = Path.Combine(Application.dataPath, "CSV", fileName);
        // 실험시작
        ChangeGameState(GameState.Start);
        StartCoroutine(RunExperiment());
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
                    ChangeGameState(GameState.InGame);
                    ChangeEccentricityState(EccentricityState.Eccentricity_10);
                }
                break;
            case GameState.InGame:
                // 게임 진행 중 상태에서의 로직
                if (Input.GetKeyDown(KeyCode.Q))
                {
                    ChangeEccentricityState(EccentricityState.Eccentricity_10);
                }
                if (Input.GetKeyDown(KeyCode.W))
                {
                    ChangeEccentricityState(EccentricityState.Eccentricity_25);
                }
                if (Input.GetKeyDown(KeyCode.E))
                {
                    ChangeEccentricityState(EccentricityState.Eccentricity_35);
                }
                break;
            case GameState.WaitingForResponse:
                // 응답 대기 상태
                DeactivateDiskManagers();
                HandleDiskSelection();
                break;
            case GameState.Pause:
                // 일시 정지 상태에서의 로직
                break;
            case GameState.End:
                // 게임 종료 상태에서의 로직
                break;
        }
    }

    public void ChangeGameState(GameState newGameState)
    {
        currentGameState = newGameState;

        // 상태 변경 시 필요한 로직을 여기에 추가
        switch (newGameState)
        {
            case GameState.Start:
                // 게임 시작으로 변경 시 실행 로직
                Debug.Log("Game State : Start");
                startText.gameObject.SetActive(true);
                startText.text = "Press Any Button To Start";

                break;
            case GameState.InGame:
                // 게임 진행으로 변경 시 실행 로직
                Debug.Log("Game State : In Game");
                startText.gameObject.SetActive(false);

                break;
            case GameState.WaitingForResponse:
                // Response Waiting 
                break;
            case GameState.Pause:
                // 게임 일시 정지로 변경 시 실행 로직

                break;
            case GameState.End:
                // 게임 종료로 변경 시 실행 로직

                break;
        }
    }
    public void DeactivateDiskManagers()
    {
        // 모든 디스크 비활성화
        diskManager_Eccentricity10.SetActive(false);
        diskManager_Eccentricity25.SetActive(false);
        diskManager_Eccentricity35.SetActive(false);
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
                Debug.Log("Eccentricity Changed to 25");
                break;
            case EccentricityState.Eccentricity_35:
                // Eccentricitiy 35로 변경될 때 실행 로직
                Debug.Log("Eccentricity Changed to 35");
                break;
        }
    }
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

    void HandleDiskSelection()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            Debug.Log("(StateManage.cs/HandleDiskSelection)Disk 1 as answer");
            ProcessAnswer(1);
            ChangeGameState(GameState.InGame);
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            Debug.Log("(StateManage.cs/HandleDiskSelection)Disk 2 as answer");
            ProcessAnswer(2);
            ChangeGameState(GameState.InGame);
        }
        else if (Input.GetKeyDown(KeyCode.Z))
        {
            Debug.Log("(StateManage.cs/HandleDiskSelection)Disk 3 as answer");
            ProcessAnswer(3);
            ChangeGameState(GameState.InGame);
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            Debug.Log("(StateManage.cs/HandleDiskSelection)Disk 4 as answer");
            ProcessAnswer(4);
            ChangeGameState(GameState.InGame);
        }
    }

    IEnumerator RunSingleTrial()
    {
        // 랜덤 디스크 1개 설정
        int randomTargetDiskNumber = SelectRandomTargetDiskNumber();

        // reference color 불러오기
        switch (currentReferenceColor)
        {
            case ReferenceColor.ReferenceColor1:
                referenceColorInRGB = colorManager.DKLtoRGB(colorManager.referenceColorsDKL[0]);
                break;
            case ReferenceColor.ReferenceColor2:
                referenceColorInRGB = colorManager.DKLtoRGB(colorManager.referenceColorsDKL[1]);
                break;
            case ReferenceColor.ReferenceColor3:
                referenceColorInRGB = colorManager.DKLtoRGB(colorManager.referenceColorsDKL[2]);
                break;
            case ReferenceColor.ReferenceColor4:
                referenceColorInRGB = colorManager.DKLtoRGB(colorManager.referenceColorsDKL[3]);
                break;
            case ReferenceColor.ReferenceColor5:
                referenceColorInRGB = colorManager.DKLtoRGB(colorManager.referenceColorsDKL[4]);
                break;
        }

        // target color 계산
        targetColorInRGB = colorManager.DKLtoRGB(colorManager.CalculateTargetColor(colorManager.RGBtoDKL(referenceColorInRGB)));

        // 이심률 변경
        EccentricityState currentEccentricityState = GetCurrentEccentricityState();
        ActivateCurrentEccentricityDiskManager();

        // ref mat에 색 입히기
        colorManager.SetMaterialColor(colorManager.referenceMaterial, referenceColorInRGB);
        Debug.Log("(RunSigleTrial)Reference Color: " + referenceColorInRGB);

        // target mat에 색 입히기
        colorManager.SetMaterialColor(colorManager.targetMaterial, targetColorInRGB);
        Debug.Log("(RunSigleTrial)Target Color: " + targetColorInRGB);

        bool isAnswered = false;

        // 시간 내에 답변을 해버릴 경우
        float elapsedTime = 0f;
        while (elapsedTime < diskShowingTime)
        {
            elapsedTime += Time.deltaTime;

            // 답변을 받으면 즉시 다음 게임으로 진행
            if (!isAnswered && (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.X)))
            {
                isAnswered = true; // 답변이 들어왔음을 표시
                int answerNumber = GetAnswerNumberFromInput(); // 어떤 답변을 눌렀는지 확인
                ProcessAnswer(answerNumber); // 답변 처리
                ChangeGameState(GameState.InGame);
            }

            yield return null;
        }
        // 시간 초과 후 답변을 받지 못한 경우 WaitingForResponse 상태로 전환
        if (!isAnswered)
        {
            ChangeGameState(GameState.WaitingForResponse);
            Debug.Log("Game State: Waiting to Response");
        }

        // 피실험자 응답 기다리기
        yield return new WaitUntil(() => currentGameState != GameState.WaitingForResponse);

        // trial 횟수와 reversal 업데이트 필요.

        // CSV 저장

    }
    IEnumerator RunExperiment()
    {
        while (totalTrialsCompleted < numberOfTrialsPerEccentricity)
        {
            for (int i = 0; i < numberOfTrialsPerEccentricity; i++)
            {
                yield return StartCoroutine(RunSingleTrial());
            }

            // Eccentricity 변경 또는 종료 로직 추가
            if (currentEccentricityTrialsCompleted < numberOfTrialsPerEccentricity)
            {
                ChangeEccentricityStateToNext();
            }

        }
    }

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
                // 실험 종료 상태로 변경
                ChangeGameState(GameState.End);
                break;
        }

        // Eccentricity 변경 후 관련 변수 초기화 또는 업데이트
        currentEccentricityTrialsCompleted = 0;
        totalTrialsCompleted += numberOfTrialsPerEccentricity;
    }

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
        trialNumber += 1;
        ChangeGameState(GameState.InGame);
    }

    void SaveToCSV(int answer, bool isCorrect)
    {
        // CSV 파일에 데이터 저장 로직
        string newDataLine = $"{currentEccentricityState};{trialNumber};{referenceColorInRGB};{targetColorInRGB};{currentTargetDisk};{answer};{isCorrect};{reversalCount}";

        // 파일이 없으면 새로 만들고, 있으면 데이터 추가
        if (!File.Exists(csvFilePath) || isFirstEntry)
        {
            isFirstEntry = false;
            File.WriteAllText(csvFilePath, "Eccentricity;Trial No.;ReferenceColorInRGB;TargetColorInRGB;TargetDisk;Answer;IsCorrect;resersalCount\n");
        }

        File.AppendAllText(csvFilePath, newDataLine + "\n");
    }
    int GetAnswerNumberFromInput()
    {
        if (Input.GetKeyDown(KeyCode.A)) return 1;
        if (Input.GetKeyDown(KeyCode.S)) return 2;
        if (Input.GetKeyDown(KeyCode.Z)) return 3;
        if (Input.GetKeyDown(KeyCode.X)) return 4;
        return 0; // 어떤 답변도 누르지 않은 경우
    }
    private string GetFormattedDateTime()
    {
        DateTime now = DateTime.Now;

        return now.ToString("MMdd_HH-mm-ss");
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
    WaitingForResponse,
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