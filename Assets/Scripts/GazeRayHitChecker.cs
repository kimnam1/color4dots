using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GazeRayHitChecker : MonoBehaviour
{

    [SerializeField] GameObject gazeDestination;

    // 시야가 특정 구역을 벗어나면 띄게 될 경고문구 Text 오브젝트
    [SerializeField] GameObject warningText;
    LayerMask GazeDestination;
    public GameObject GazeTarget;

    // Start is called before the first frame update
    void Start()
    {
        GazeDestination = LayerMask.GetMask("GazeDestination");
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(GazeTarget.transform);

        // Raycast를 실행하고, Sphere와 충돌하는지 확인 (Ray의 최대 거리를 설정할 수 있음)
        if (!Physics.Raycast(transform.position, transform.forward, 500.0f, GazeDestination)) // 100f는 Ray의 최대 거리
        {
            Debug.Log("Gaze Outside!!!!!!!!!!!!");

            // 시야가 특정 구역 바깥으로 나갈 시
            warningText.SetActive(true);
        }
        else
        {
            Debug.Log("Gaze Inside!!!!!");
            // 시야가 구역 안에 있을 시
            warningText.SetActive(false);
        }
    }

}
