using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DiskManager : MonoBehaviour
{
    public GameObject[] disks; // 디스크 오브젝트 배열
    public Color referenceColor = Color.green; // 참조 색상 (녹색)
    public Color targetColor = Color.red; // 타겟 색상 (빨간색)

    // Start is called before the first frame update
    void Start()
    {

    }


    public int viewingAngle;

    // Update is called once per frame
    void Update()
    {
        // 1 = 10도, 2=25도, 3=35도.
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            viewingAngle = 10;
            SetObjectsWithTag("position10", true);
            SetObjectsWithTag("position25", false);
            SetObjectsWithTag("position35", false);
            Debug.Log("Viewing Angle : " + viewingAngle);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            viewingAngle = 25;
            SetObjectsWithTag("position10", false);
            SetObjectsWithTag("position25", true);
            SetObjectsWithTag("position35", false);
            Debug.Log("Viewing Angle : " + viewingAngle);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            viewingAngle = 35;
            SetObjectsWithTag("position10", false);
            SetObjectsWithTag("position25", false);
            SetObjectsWithTag("position35", true);
            Debug.Log("Viewing Angle : " + viewingAngle);
        }


    }
    void SetObjectsWithTag(string tag, bool isActive)
    {
        Transform[] childObjects = GetComponentsInChildren<Transform>(true);

        foreach (Transform child in childObjects)
        {
            if (child.CompareTag(tag))
            {
                child.gameObject.SetActive(isActive);
            }
        }
    }

    void SetRandomDiskColor()
    {
        // 모든 디스크를 참조 색상으로 설정
        foreach (GameObject disk in disks)
        {
            SetDiskColor(disk, referenceColor);
        }

        // 랜덤하게 하나의 디스크를 선택
        int randomIndex = Random.Range(0, disks.Length);

        // 선택된 디스크를 타겟 색상으로 설정
        SetDiskColor(disks[randomIndex], targetColor);
    }

    void SetDiskColor(GameObject disk, Color color)
    {
        Renderer diskRenderer = disk.GetComponent<Renderer>();
        if (diskRenderer != null)
        {
            diskRenderer.material.color = color;
        }
    }


}
