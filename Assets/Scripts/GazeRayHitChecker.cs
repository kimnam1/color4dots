using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GazeRayHitChecker : MonoBehaviour
{

    [SerializeField] GameObject gazeDestination;

    // 시야가 특정 구역을 벗어나면 뜨게 될 빨간 Sphere 오브젝트
    [SerializeField] GameObject GazeWarningSphere;
    // 시야가 특정 구역을 벗어나면 뜨게 될 Audio Source
    [SerializeField] AudioSource GazeWarningSound;
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
            // 시야가 특정 구역 바깥으로 나갈 시
            GazeWarningSphere.SetActive(true);
            GazeWarningSound.Play();
        }
        else
        {
            // 시야가 구역 안에 있을 시
            GazeWarningSphere.SetActive(false);
            GazeWarningSound.Stop();
        }
    }

}
