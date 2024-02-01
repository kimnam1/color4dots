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
    [Header("Scripts")]
    // Scripts
    public ColorManager colorManager; // Color Manager Script 불러오기


    // Enum 상태 추적 변수
    private GameState currentGameState; // 현재 게임 상태를 추적하는 변수
    private EccentricityState currentEccentricityState; // 현재 이심률 상태를 추적하는 변수
    public ReferenceColorState currentReferenceColorState; // 현재 reference color 추적하는 변수
    private TargetDisk currentTargetDisk; // 현재 target(정답) disk
    private DimensionState currentDimensionState; // 현재 변경할 color space 축 상태를 추적하는 변수

    [Header("Experiment Setting")]
    // Experiment Setting
    [SerializeField] private float diskShowingTime = 0.5f; // Disk Showing Time(Unity의 Inspector창에서 조절할 것) 기본값 0.5초
    public int maxTrialPerCondition = 50; // 1 Condition 당 최대 Trial 수
    public int maxReversalPerCondition = 3; // 1 condition 당 최대 reversal 수

    [Header("Materials")]
    // Material to apply
    public Material referenceMaterial;
    public Material targetMaterial;

    // Color Vector
    private Vector3 referenceColorInDKL;
    private Vector3 targetColorInRGB;

    [Header("Game Objects")]
    // Game Objects
    [SerializeField] private TMP_Text gameText; // 시작 텍스트
    [SerializeField] private GameObject diskManager_Eccentricity10; // Disk Manager Object - 10 Eccentricity
    [SerializeField] private GameObject diskManager_Eccentricity25;// Disk Manager Object - 25 Eccentricity
    [SerializeField] private GameObject diskManager_Eccentricity35;// Disk Manager Object - 35 Eccentricity
    [SerializeField] private TMP_Text answerText; // 답 나오는 텍스트. Test 용
    [SerializeField] private TMP_Text debugText; // Debug용 텍스트

    // Trial 상태 변수
    private int trialNumber = 1; // Trial 번호. CSV 저장용
    private int reversalCount; // 현재 reversal count
    private int currentConditionTrialCompleted = 0; // Eccentricity 별 현재 진행된 trial 수
    private bool isAnswered; // 답변 들어왔는지 파악하는 bool
    private bool isRight; // 맞았는지 틀렸는지 파악하는 bool
    private bool previousAnswerCorrect; // 전단계 정답 여부 bool. reversalCount에 필요.

    // 난이도 변수
    public int defaultDifficulty = 50;
    private int difficulty = 50;
    private bool isConditionFirst = true;

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
        //Debug용 정보
        debugText.text = $"Trial Info\nDifficulty: {difficulty}\nEccen.: {currentEccentricityState}\nRef Color: {referenceMaterial.color}\nTargetColor: {targetMaterial.color}\nDKL: ";
        switch (currentTargetDisk)
        {
            case TargetDisk.Disk1:
                answerText.text = "1";
                break;
            case TargetDisk.Disk2:
                answerText.text = "2";
                break;
            case TargetDisk.Disk3:
                answerText.text = "3";
                break;
            case TargetDisk.Disk4:
                answerText.text = "4";
                break;
        }
        // Debug용 정보 끝

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (currentGameState == GameState.Pause)
            {
                ChangeGameState(GameState.InGame);
            }
            else
            {
                ChangeGameState(GameState.Pause);
            }
        }

        switch (currentGameState)
        {
            case GameState.Start:
                // 시작 상태에서의 로직
                // 시작할 때는 배경이랑 같은색으로 만들어두자.
                colorManager.SetMaterialColor(colorManager.referenceMaterial, new Vector3(0.5f, 0.5f, 0.5f));
                colorManager.SetMaterialColor(colorManager.targetMaterial, new Vector3(0.5f, 0.5f, 0.5f));

                // Test disk color apply

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
                DeactivateDiskManagers();
                gameText.gameObject.SetActive(true);
                gameText.text = "Pause";

                break;
            case GameState.End:
                // 게임 종료 상태에서의 로직
                DeactivateDiskManagers();
                StopAllCoroutines();
                gameText.text = "Finished";
                break;
        }
    }




    // 실험 실행 메소드


    // Condition 별 실험 실행 메소드
    IEnumerator RunSingleCondition()
    {
        while (reversalCount < maxReversalPerCondition && currentConditionTrialCompleted < maxTrialPerCondition)
        {
            Debug.Log($"Current Condition Trial Completed: {currentConditionTrialCompleted}");
            isAnswered = false; // 시작 전 isAnswered False
            isRight = false; // 시작 전 isRight False(일단 틀렸다고 하고 시작하는게 맞나?)
            StartCoroutine(RunSingleTrial());
            yield return new WaitUntil(() => isAnswered);

            currentConditionTrialCompleted++;
        }

        // 위에서 한 condition에 대한 반복 끝났으므로 Next Condition으로 변경 또는 종료 로직 추가
        currentConditionTrialCompleted = 0; // 새로 시작해야하니까 ConditionTrialCompleted = 0
        reversalCount = 0;
        difficulty = 50; // 새로 시작하니까 difficulty 기본값 10
        ChangeConditionToNext();
    }


    // Trial 하나 실행 메소드
    IEnumerator RunSingleTrial()
    {
        Debug.Log("================================================================");
        Debug.Log($"Run Single Trial! Trial number: {trialNumber}");
        // 랜덤 디스크 1개 설정
        SelectRandomTargetDiskNumber();

        // reference color 불러오기 DKL
        switch (currentReferenceColorState)
        {
            case ReferenceColorState.ReferenceColor1:
                referenceColorInDKL = colorManager.referenceColorsDKL[0];
                break;
            case ReferenceColorState.ReferenceColor2:
                referenceColorInDKL = colorManager.referenceColorsDKL[1];
                break;
            case ReferenceColorState.ReferenceColor3:
                referenceColorInDKL = colorManager.referenceColorsDKL[2];
                break;
            case ReferenceColorState.ReferenceColor4:
                referenceColorInDKL = colorManager.referenceColorsDKL[3];
                break;
            case ReferenceColorState.ReferenceColor5:
                referenceColorInDKL = colorManager.referenceColorsDKL[4];
                break;
        }
        // reference color 불러오기 RGB
        // switch (currentReferenceColorState)
        // {
        //     case ReferenceColorState.ReferenceColor1:
        //         referenceColorInDKL = colorManager.referenceColorsRGB[0];
        //         break;
        //     case ReferenceColorState.ReferenceColor2:
        //         referenceColorInDKL = colorManager.referenceColorsRGB[1];
        //         break;
        //     case ReferenceColorState.ReferenceColor3:
        //         referenceColorInDKL = colorManager.referenceColorsRGB[2];
        //         break;
        //     case ReferenceColorState.ReferenceColor4:
        //         referenceColorInDKL = colorManager.referenceColorsRGB[3];
        //         break;
        //     case ReferenceColorState.ReferenceColor5:
        //         referenceColorInDKL = colorManager.referenceColorsRGB[4];
        //         break;
        // }

        Vector3 referenceColorInRGB = colorManager.DKLCarttoRGBbyPsychoPy(referenceColorInDKL);

        // target color 계산
        targetColorInRGB = colorManager.DKLCarttoRGBbyPsychoPy(colorManager.CalculateTargetColor(referenceColorInDKL, currentDimensionState, difficulty));
        // targetColorInRGB = colorManager.CalculateTargetColorTestRGB(referenceColorInRGB, currentDimensionState, difficulty); // DKL이 아니라 RGB로 테스트할 때는 이 메소드 사용할 것.

        // ref mat에 색 입히기22
        colorManager.SetMaterialColor(colorManager.referenceMaterial, new Vector3(referenceColorInRGB.x / 255, referenceColorInRGB.y / 255, referenceColorInRGB.z / 255));
        Debug.Log("(RunSigleTrial)Reference Color: " + referenceColorInRGB + " is applied to material");

        // target mat에 색 입히기
        colorManager.SetMaterialColor(colorManager.targetMaterial, new Vector3(targetColorInRGB.x / 255, targetColorInRGB.y / 255, targetColorInRGB.z / 255));
        Debug.Log("(RunSigleTrial)Target Color: " + targetColorInRGB + " is applied to material");

        // 이심률 지정 및 해당하는 디스크 켜기
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
    }

    // 5초 대기 후 실험 시작 메소드
    IEnumerator ExperimentPreparation()
    {
        // 5초 대기/남은 시간 보여주기
        for (int i = 0; i < 5; i++)
        {
            gameText.text = $"Start in {5 - i}";
            yield return new WaitForSeconds(1f);
        }

        // 대기 후 실험 상태로 setting
        ChangeGameState(GameState.InGame);
        ChangeReferenceColorState(ReferenceColorState.ReferenceColor1);
        ChangeEccentricityState(EccentricityState.Eccentricity_10);
        ChangeDimensionState(DimensionState.xPositive);
    }

    /**
    실험 실행 메소드 끝
    **/





    /**
    State Change Methods 시작
    **/

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
                gameText.gameObject.SetActive(false);

                break;
            case GameState.Pause:
                // 게임 일시 정지로 변경 시 실행 로직

                break;
            case GameState.End:
                // 게임 종료로 변경 시 실행 로직
                gameText.gameObject.SetActive(true);
                gameText.text = "Finished";
                break;
        }
    }
    public void ChangeReferenceColorState(ReferenceColorState referenceColor)
    {
        currentReferenceColorState = referenceColor;

        Debug.Log($"Reference Color Changed to {currentReferenceColorState}");
    }
    public void ChangeReferenceColorToNext()
    {
        switch (currentReferenceColorState)
        {
            case ReferenceColorState.ReferenceColor1:
                ChangeReferenceColorState(ReferenceColorState.ReferenceColor2);
                break;
            case ReferenceColorState.ReferenceColor2:
                ChangeReferenceColorState(ReferenceColorState.ReferenceColor3);
                break;
            case ReferenceColorState.ReferenceColor3:
                ChangeReferenceColorState(ReferenceColorState.ReferenceColor4);
                break;
            case ReferenceColorState.ReferenceColor4:
                ChangeReferenceColorState(ReferenceColorState.ReferenceColor5);
                break;
            case ReferenceColorState.ReferenceColor5:
                ChangeReferenceColorState(ReferenceColorState.ReferenceColor1);
                break;
        }
    }
    public void ChangeEccentricityState(EccentricityState newEccentricityState)
    {
        currentEccentricityState = newEccentricityState;

        Debug.Log($"Eccentricity Changed to {currentEccentricityState}");
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
                ChangeEccentricityState(EccentricityState.Eccentricity_10); // 다시 10으로
                break;
        }

        // Eccentricity 변경 후 관련 변수 초기화 또는 업데이트
        currentConditionTrialCompleted = 0;

        // 현재 실행 중인 코루틴 중지 후 Eccentricity 변경하고 다시 실행(Eccentricity 변경 타이밍에 답변 기다리지 않는 문제 해결 목적)
        StopCoroutine(RunSingleTrial());
        StartCoroutine(RunSingleTrial());
    }

    public void ChangeDimensionState(DimensionState dimensionState)
    {
        currentDimensionState = dimensionState;

        Debug.Log($"Dimension is Changed to {currentDimensionState}");
        StartCoroutine(RunSingleCondition());
    }
    public void ChangeDimensionStateToNext()
    {
        switch (currentDimensionState)
        {
            case DimensionState.xPositive:
                ChangeDimensionState(DimensionState.xNegative);
                break;
            case DimensionState.xNegative:
                ChangeDimensionState(DimensionState.yPositive);
                break;
            case DimensionState.yPositive:
                ChangeDimensionState(DimensionState.yNegative);
                break;
            case DimensionState.yNegative:
                ChangeDimensionState(DimensionState.xPositive); // 다시 처음인 xPositive로
                break;
        }
    }

    public void ChangeConditionToNext()
    {
        // 새로운 condition 시작이라면 isConditionFirst = true;
        isConditionFirst = true;

        if (currentDimensionState == DimensionState.yNegative)
        {
            if (currentEccentricityState == EccentricityState.Eccentricity_35)
            {
                if (currentReferenceColorState == ReferenceColorState.ReferenceColor5)
                {
                    ChangeGameState(GameState.End);
                }
                ChangeReferenceColorToNext();
            }
            ChangeEccentricityStateToNext();
        }
        ChangeDimensionStateToNext();
        Debug.Log($"Condition Changed to {currentReferenceColorState}, {currentEccentricityState}, {currentDimensionState}");
    }

    /** 
    정답 입력 처리 메소드 시작
    **/

    // 키보드 입력에 따른 정답 저장 및 ProcessAnswer 실행
    void HandleDiskSelection()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            Debug.Log("(StateManage.cs/HandleDiskSelection)Disk 1 as answer");
            ProcessAnswer(1);
            ChangeGameState(GameState.InGame);
            isAnswered = true;
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("(StateManage.cs/HandleDiskSelection)Disk 2 as answer");
            ProcessAnswer(2);
            ChangeGameState(GameState.InGame);
            isAnswered = true;

        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            Debug.Log("(StateManage.cs/HandleDiskSelection)Disk 3 as answer");
            ProcessAnswer(3);
            ChangeGameState(GameState.InGame);
            isAnswered = true;

        }
        else if (Input.GetKeyDown(KeyCode.D))
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
            isRight = true;
            difficulty++;
            Debug.Log($"Difficulty is now {difficulty}");
        }
        else
        {
            Debug.Log("(ProcessAnswer)Wrong Answer");
            isRight = false;
            difficulty--;
            Debug.Log($"Difficulty is now {difficulty}");
        }
        // Reversal up or down
        ProcessReversal(isRight);

        // CSV 파일에 저장하는 로직
        SaveToCSV(selectedAnswerNumber, isRight);

        // 다음 시행을 위한 상태 변경
        trialNumber++;
    }
    // reversal process 메소드
    void ProcessReversal(bool currentAnswerCorrect)
    {
        if (isConditionFirst)
        {
            previousAnswerCorrect = currentAnswerCorrect;
            isConditionFirst = false;
            return;
        }
        else if (currentAnswerCorrect != previousAnswerCorrect)
        {
            reversalCount++;
        }

        previousAnswerCorrect = currentAnswerCorrect;
    }


    /**
    정답 처리 메소드 끝
    **/

    // 저장 메소드
    void SaveToCSV(int answer, bool isCorrect)
    {
        // CSV 파일에 데이터 저장 로직
        string newDataLine = $"{trialNumber};{referenceColorInDKL};{currentEccentricityState};{currentDimensionState};{targetColorInRGB};{currentTargetDiskNumber()};{answer};{Convert.ToInt16(isCorrect)};{difficulty};{reversalCount}";

        // 파일이 없으면 새로 만들고, 있으면 데이터 추가
        if (!File.Exists(csvFilePath) || isFirstEntry)
        {
            isFirstEntry = false;
            File.WriteAllText(csvFilePath, "Trial No.;ReferenceColorInRGB;Eccentricity;ChangedDimension;TargetColorInRGB;TargetDisk;Answer;IsCorrect;Difficulty;resersalCount\n");
        }

        File.AppendAllText(csvFilePath, newDataLine + "\n");
    }

    private string GetFormattedDateTime()
    {
        DateTime now = DateTime.Now;

        return now.ToString("MMdd_HH-mm-ss");
    }


    // Disk 처리 메소드
    void SelectRandomTargetDiskNumber()
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

public enum ReferenceColorState
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