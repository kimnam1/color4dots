using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Text;
using System.ComponentModel.Design;

public class StateManager : MonoBehaviour
{
    private GameState currentGameState; // 현재 게임 상태를 추적하는 변수
    private EccentricityState currentEccentricityState; // 현재 이심률 상태를 추적하는 변수
    private DimensionState currentDimensionState; // 현재 변경할 color space 축 상태를 추적하는 변수
    private ReferenceColor currentReferenceColor; // 현재 reference color
    private int reversalCount; // 현재 reversal count


    private TargetDisk currentTargetDisk; // 현재 target(정답) disk



    public TextMeshProUGUI startText; // 시작 텍스트
    public GameObject testCube;

    // Start is called before the first frame update
    void Start()
    {
        ChangeGameState(GameState.Start);



        SelectRandomTargetDiskNumber();

        int numberOfTrials = 10; // 실험 횟수 설정.

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
                testCube.gameObject.SetActive(true);
                startText.text = "Press Any Button To Start";
                break;
            case GameState.InGame:
                // 게임 진행으로 변경 시 실행 로직
                Debug.Log("Game State : In Game");
                startText.gameObject.SetActive(false);
                testCube.gameObject.SetActive(false);
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

        if (randomTargetDiskNumber == 1)
        {
            currentTargetDisk = TargetDisk.Disk1;
        }
        if (randomTargetDiskNumber == 2)
        {
            currentTargetDisk = TargetDisk.Disk2;
        }
        if (randomTargetDiskNumber == 3)
        {
            currentTargetDisk = TargetDisk.Disk3;
        }
        if (randomTargetDiskNumber == 4)
        {
            currentTargetDisk = TargetDisk.Disk4;
        }
        return randomTargetDiskNumber;
    }

    void HandleDiskSelection()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            Debug.Log("Disk 1 as answer");
            SelectRandomTargetDiskNumber();
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            Debug.Log("Disk 2 as answer");
            SelectRandomTargetDiskNumber();
        }
        else if (Input.GetKeyDown(KeyCode.Z))
        {
            Debug.Log("Disk 3 as answer");
            SelectRandomTargetDiskNumber();
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            Debug.Log("Disk 4 as answer");
            SelectRandomTargetDiskNumber();
        }
    }

    void RunSingleTrial()
    {

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