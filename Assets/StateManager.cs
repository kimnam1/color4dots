using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateManager : MonoBehaviour
{
    private GameState currentState; // 현재 게임 상태를 추적하는 변수

    // Start is called before the first frame update
    void Start()
    {
        ChangeState(GameState.Start);
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            case GameState.Start:
                // 시작 상태에서의 로직
                break;
            case GameState.InGame:
                // 게임 진행 중 상태에서의 로직
                break;
            case GameState.Pause:
                // 일시 정지 상태에서의 로직
                break;
            case GameState.End:
                // 게임 종료 상태에서의 로직
                break;
        }
    }

    public void ChangeState(GameState newState)
    {
        currentState = newState;

        // 상태 변경 시 필요한 로직을 여기에 추가
        switch (newState)
        {
            case GameState.Start:
                // 게임 시작 로직
                break;
            case GameState.InGame:
                // 게임 진행 로직
                break;
            case GameState.Pause:
                // 게임 일시 정지 로직
                break;
            case GameState.End:
                // 게임 종료 로직
                break;
        }
    }
}

public enum GameState
{
    Start,
    Position10,
    Position25,
    Position35,
    Pause,
    End
}

