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
    public DiskManager diskManager; // Disk Manager Script 불러오기
    private GameState currentGameState; // 현재 게임 상태를 추적하는 변수
    private EccentricityState currentEccentricityState; // 현재 이심률 상태를 추적하는 변수
    private DimensionState currentDimensionState; // 현재 변경할 color space 축 상태를 추적하는 변수
    private ReferenceColor currentReferenceColor; // 현재 reference color
    private int reversalCount; // 현재 reversal count

    private int answerNumber; // 정답 처리 변수
    private TargetDisk currentTargetDisk; // 현재 target(정답) disk
    public int numberOfTrials = 10; // 실험 trial 설정.



    public TextMeshProUGUI startText; // 시작 텍스트

    // Start is called before the first frame update
    void Start()
    {
        ChangeGameState(GameState.Start);

        for (int i = 0; i < numberOfTrials; i++)
        {
            RunSingleTrial();
        }
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
                    SelectRandomTargetDiskNumber();
                }
                if (Input.GetKeyDown(KeyCode.W))
                {
                    ChangeEccentricityState(EccentricityState.Eccentricity25);
                    SelectRandomTargetDiskNumber();

                }
                if (Input.GetKeyDown(KeyCode.E))
                {
                    ChangeEccentricityState(EccentricityState.Eccentricity35);
                    SelectRandomTargetDiskNumber();
                }
                // 피실험자 정답 받기
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
            case GameState.Pause:
                // 게임 일시 정지로 변경 시 실행 로직

                break;
            case GameState.End:
                // 게임 종료로 변경 시 실행 로직

                break;
        }
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
            Debug.Log("Disk 1 as answer");
            ProcessAnswer();
            SelectRandomTargetDiskNumber();
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            answerNumber = 2;
            Debug.Log("Disk 2 as answer");
            ProcessAnswer();
            SelectRandomTargetDiskNumber();
        }
        else if (Input.GetKeyDown(KeyCode.Z))
        {
            answerNumber = 3;
            Debug.Log("Disk 3 as answer");
            ProcessAnswer();
            SelectRandomTargetDiskNumber();
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            answerNumber = 4;
            Debug.Log("Disk 4 as answer");
            ProcessAnswer();
            SelectRandomTargetDiskNumber();
        }
    }

    IEnumerator RunSingleTrial()
    {
        // 디스크 1개 설정
        int randomTargetDiskNumber = SelectRandomTargetDiskNumber();
        Vector3 trialReferenceColor = new Vector3(0, 0, 0);

        // reference color 불러오기
        switch (currentReferenceColor)
        {
            case ReferenceColor.ReferenceColor1:
                trialReferenceColor = colorManager.DKLtoRGB(colorManager.referenceColorsDKL[0]);
                break;
            case ReferenceColor.ReferenceColor2:
                trialReferenceColor = colorManager.DKLtoRGB(colorManager.referenceColorsDKL[1]);
                break;
            case ReferenceColor.ReferenceColor3:
                trialReferenceColor = colorManager.DKLtoRGB(colorManager.referenceColorsDKL[2]);
                break;
            case ReferenceColor.ReferenceColor4:
                trialReferenceColor = colorManager.DKLtoRGB(colorManager.referenceColorsDKL[3]);
                break;
            case ReferenceColor.ReferenceColor5:
                trialReferenceColor = colorManager.DKLtoRGB(colorManager.referenceColorsDKL[4]);
                break;
        }

        // target color 계산
        Vector3 targetColorInRGB = colorManager.XYZtoRGB(colorManager.LMStoXYZ(colorManager.DKLtoLMS(colorManager.CalculateTargetColor(trialReferenceColor))));

        // ref mat에 색 입히기
        colorManager.SetMaterialColor(colorManager.referenceMaterial, trialReferenceColor);

        // target mat에 색 입히기
        colorManager.SetMaterialColor(colorManager.targetMaterial, targetColorInRGB);

        // 500ms 보여주기
        yield return new WaitForSeconds(0.5f);


        // 정답 받기와 정오답 판별은 HandleDiskSelection에서 하고 있음.
        // trial 횟수와 reversal 업데이트 필요.

        // CSV 저장

    }

    void ProcessAnswer()
    {
        bool isCorrect = currentTargetDiskNumber() == answerNumber;
        Debug.Log(isCorrect ? "Correct Answer" : "Wrong Answer");

        // 여기서 필요한 추가 로직을 구현합니다. 예: 점수 업데이트, 다음 시행 준비 등

        // CSV 파일에 저장하는 로직
        SaveToCSV(answerNumber, isCorrect);

        // 다음 타겟 디스크 선택
        SelectRandomTargetDiskNumber();
    }

    void SaveToCSV(int answer, bool isCorrect)
    {
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