using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

using System.Text;
using System.ComponentModel.Design;
using JetBrains.Annotations;

public class StateManager : MonoBehaviour
{
    public ColorManager colorManager; // Color Manager Script 불러오기
    private GameState currentGameState; // 현재 게임 상태를 추적하는 변수
    private EccentricityState currentEccentricityState; // 현재 이심률 상태를 추적하는 변수
    private DimensionState currentDimensionState; // 현재 변경할 color space 축 상태를 추적하는 변수
    private ReferenceColor currentReferenceColor; // 현재 reference color
    private int reversalCount; // 현재 reversal count

    private int answerNumber; // 정답 처리 변수
    private TargetDisk currentTargetDisk; // 현재 target(정답) disk
    public int numberOfTrials; // 실험 trial 설정.

    // Disk Manager Object 변수
    private GameObject diskManager_Eccentricity10;
    private GameObject diskManager_Eccentricity25;
    private GameObject diskManager_Eccentricity35;

    // Material to apply
    public Material referenceMaterial;
    public Material targetMaterial;

    [SerializeField]
    // Disk Showing Time(Unity의 Inspector창에서 조절할 것)
    private float diskShowingTime = 0.5f; // 기본값 0.5초



    public TextMeshProUGUI startText; // 시작 텍스트

    // Start is called before the first frame update
    void Start()
    {
        // 디스크 매니저 변수 할당
        diskManager_Eccentricity10 = GameObject.Find("DiskManager_Eccentricity10");
        diskManager_Eccentricity25 = GameObject.Find("DiskManager_Eccentricity25");
        diskManager_Eccentricity35 = GameObject.Find("DiskManager_Eccentricity35");
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
                    ChangeEccentricityState(EccentricityState.Eccentricity10);
                }
                break;
            case GameState.InGame:
                // 게임 진행 중 상태에서의 로직
                if (Input.GetKeyDown(KeyCode.Q))
                {
                    ChangeEccentricityState(EccentricityState.Eccentricity10);
                }
                if (Input.GetKeyDown(KeyCode.W))
                {
                    ChangeEccentricityState(EccentricityState.Eccentricity25);
                }
                if (Input.GetKeyDown(KeyCode.E))
                {
                    ChangeEccentricityState(EccentricityState.Eccentricity35);
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
            case EccentricityState.Eccentricity10:
                // Eccentricity 10으로 변경될 때 실행 로직
                Debug.Log("Eccentricity Changed to 10");
                break;
            case EccentricityState.Eccentricity25:
                // Eccentricitiy 25로 변경될 때 실행 로직
                Debug.Log("Eccentricity Changed to 25");
                break;
            case EccentricityState.Eccentricity35:
                // Eccentricitiy 35로 변경될 때 실행 로직
                Debug.Log("Eccentricity Changed to 35");
                break;
        }
    }
    int SelectRandomTargetDiskNumber()
    {
        int randomTargetDiskNumber = Random.Range(1, 5);
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
            answerNumber = 1;
            Debug.Log("(StateManage.cs/HandleDiskSelection)Disk 1 as answer");
            ProcessAnswer();
            ChangeGameState(GameState.InGame);
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            answerNumber = 2;
            Debug.Log("(StateManage.cs/HandleDiskSelection)Disk 2 as answer");
            ProcessAnswer();
            ChangeGameState(GameState.InGame);
        }
        else if (Input.GetKeyDown(KeyCode.Z))
        {
            answerNumber = 3;
            Debug.Log("(StateManage.cs/HandleDiskSelection)Disk 3 as answer");
            ProcessAnswer();
            ChangeGameState(GameState.InGame);
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            answerNumber = 4;
            Debug.Log("(StateManage.cs/HandleDiskSelection)Disk 4 as answer");
            ProcessAnswer();
            ChangeGameState(GameState.InGame);
        }
    }

    IEnumerator RunSingleTrial()
    {
        // 랜덤 디스크 1개 설정
        int randomTargetDiskNumber = SelectRandomTargetDiskNumber();
        Vector3 trialReferenceColorInRGB = new Vector3(1.06440418f, 237.1414263f, 12.88814752f);

        // reference color 불러오기
        switch (currentReferenceColor)
        {
            case ReferenceColor.ReferenceColor1:
                trialReferenceColorInRGB = colorManager.DKLtoRGB(colorManager.referenceColorsDKL[0]);
                break;
            case ReferenceColor.ReferenceColor2:
                trialReferenceColorInRGB = colorManager.DKLtoRGB(colorManager.referenceColorsDKL[1]);
                break;
            case ReferenceColor.ReferenceColor3:
                trialReferenceColorInRGB = colorManager.DKLtoRGB(colorManager.referenceColorsDKL[2]);
                break;
            case ReferenceColor.ReferenceColor4:
                trialReferenceColorInRGB = colorManager.DKLtoRGB(colorManager.referenceColorsDKL[3]);
                break;
            case ReferenceColor.ReferenceColor5:
                trialReferenceColorInRGB = colorManager.DKLtoRGB(colorManager.referenceColorsDKL[4]);
                break;
        }

        // target color 계산
        Vector3 targetColorInRGB = colorManager.DKLtoRGB(colorManager.CalculateTargetColor(colorManager.RGBtoDKL(trialReferenceColorInRGB)));

        // 이심률 변경
        EccentricityState currentEccentricityState = GetCurrentEccentricityState();
        switch (currentEccentricityState)
        {
            case EccentricityState.Eccentricity10:
                ActivateCurrentEccentricityDiskManager();
                break;
            case EccentricityState.Eccentricity25:
                ActivateCurrentEccentricityDiskManager();
                break;
            case EccentricityState.Eccentricity35:
                ActivateCurrentEccentricityDiskManager();
                break;
        }

        // ref mat에 색 입히기
        colorManager.SetMaterialColor(colorManager.referenceMaterial, trialReferenceColorInRGB);
        Debug.Log("(RunSigleTrial)Reference Color: " + trialReferenceColorInRGB);

        // target mat에 색 입히기
        colorManager.SetMaterialColor(colorManager.targetMaterial, targetColorInRGB);
        Debug.Log("(RunSigleTrial)Target Color: " + targetColorInRGB);

        // 500ms 보여주고 끄기(끄는건 WaitingForResonse에서 자동 동작함.)
        yield return new WaitForSeconds(diskShowingTime);
        ChangeGameState(GameState.WaitingForResponse);

        // 피실험자 응답 기다리기
        yield return new WaitUntil(() => currentGameState != GameState.WaitingForResponse);

        // trial 횟수와 reversal 업데이트 필요.

        // CSV 저장

    }
    IEnumerator RunExperiment()
    {
        for (int i = 0; i < numberOfTrials; i++)
        {
            yield return StartCoroutine(RunSingleTrial());
        }
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
            case EccentricityState.Eccentricity10:
                Debug.Log("(DiskManager.cs/ActivateCurrentEccentricityDiskManager) Current Eccentricity " + GetCurrentEccentricityState() + " is Activated");
                diskManager_Eccentricity10.SetActive(true);
                break;
            case EccentricityState.Eccentricity25:
                Debug.Log("(DiskManager.cs/ActivateCurrentEccentricityDiskManager) Current Eccentricity " + GetCurrentEccentricityState());
                diskManager_Eccentricity25.SetActive(true);
                break;
            case EccentricityState.Eccentricity35:
                Debug.Log("(DiskManager.cs/ActivateCurrentEccentricityDiskManager) Current Eccentricity is " + GetCurrentEccentricityState());
                diskManager_Eccentricity35.SetActive(true);
                break;
        }
    }
    void ProcessAnswer()
    {
        bool isCorrect = currentTargetDiskNumber() == answerNumber;
        Debug.Log(isCorrect ? "(ProcessAnswer)Correct Answer" : "(ProcessAnswer)Wrong Answer");

        // 여기서 필요한 추가 로직을 구현합니다. 예: 점수 업데이트, 다음 시행 준비 등

        // CSV 파일에 저장하는 로직
        SaveToCSV(answerNumber, isCorrect);

        // 다음 시행을 위한 상태 변경
        ChangeGameState(GameState.InGame);
    }

    void SaveToCSV(int answer, bool isCorrect)
    {
        string filePath = Path.Combine(Application.persistentDataPath, "experiment_results.csv");

        // CSV 파일에 데이터 저장 로직 구현
        // 예: File.AppendAllText("경로", "데이터");
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
    Eccentricity10,
    Eccentricity25,
    Eccentricity35
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