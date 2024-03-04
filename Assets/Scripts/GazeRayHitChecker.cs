using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GazeRayHitChecker : MonoBehaviour
{

    [SerializeField] GameObject gazeDestination;

    // 시야가 특정 구역을 벗어나면 뜨게 될 빨간 Sphere 오브젝트
    [SerializeField] GameObject gazeWarningSphere;
    // 시야가 특정 구역을 벗어나면 뜨게 될 Audio Source
    //[SerializeField] AudioSource gazeWarningSound;

    LayerMask GazeDestination;
    public GameObject GazeTarget;
    public float gazeOutTime = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        GazeDestination = LayerMask.GetMask("GazeDestination");

    }

    // Update is called once per frame
    void Update()
    {
        // 시야가 GazeTarget을 바라보게 함
        transform.LookAt(GazeTarget.transform);
        // 시야가 특정 구역을 벗어나면 뜨게 될 빨간 Sphere 오브젝트를 끄거나 켬
        if (gazeOutTime > 0.5f)
        {
            gazeWarningSphere.SetActive(true);
        }
        else
        {
            gazeWarningSphere.SetActive(false);
        }

        // 시야가 특정 구역을 벗어나면 뜨게 될 Audio Source를 끄거나 켬
        // if (gazeOutTime > 2.5f)
        // {
        //     if (!gazeWarningSound.isPlaying)
        //     {
        //         gazeWarningSound.Play();
        //     }
        // }
        // else
        // {
        //     gazeWarningSound.Stop();
        // }

        // Raycast를 실행하고, Sphere와 충돌하는지 확인 (Ray의 최대 거리를 설정할 수 있음)
        if (!Physics.Raycast(transform.position, transform.forward, 500.0f, GazeDestination)) // 100f는 Ray의 최대 거리
        {
            // 시야가 구역 밖에 있을 시 gazeOutTime 증가
            gazeOutTime += Time.deltaTime;
        }
        else
        {
            // 시야가 구역 안에 있을 시 gazeOutTime 초기화
            gazeOutTime = 0.0f;
            //gazeWarningSound.Stop();
        }
    }
}
