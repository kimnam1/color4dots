using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DiskManager : MonoBehaviour
{
    public StateManager stateManager;

    // Disk Manager Object 변수
    private GameObject diskManager_Eccentricity10;
    private GameObject diskManager_Eccentricity25;
    private GameObject diskManager_Eccentricity35;

    // Material
    public Material referenceMaterial;
    public Material targetMaterial;

    // Start is called before the first frame update
    void Start()
    {
        // 디스크 매니저 변수 할당
        diskManager_Eccentricity10 = GameObject.Find("DiskManager_Eccentricity10");
        diskManager_Eccentricity25 = GameObject.Find("DiskManager_Eccentricity25");
        diskManager_Eccentricity35 = GameObject.Find("DiskManager_Eccentricity35");

    }

    // Update is called once per frame
    void Update()
    {
        // 이심률 변경
        EccentricityState currentEccentricityState = stateManager.GetCurrentEccentricityState();

        switch (currentEccentricityState)
        {
            case EccentricityState.Eccentricity10:
                ActivateDiskManager(diskManager_Eccentricity10);
                break;
            case EccentricityState.Eccentricity25:
                ActivateDiskManager(diskManager_Eccentricity25);
                break;
            case EccentricityState.Eccentricity35:
                ActivateDiskManager(diskManager_Eccentricity35);
                break;
        }

        // 만약 현재 disk manager object가 활성화 되어있다면 디스크 Material을 업데이트한다.
        if (gameObject.activeInHierarchy)
        {
            UpdateDiskMaterials();
        }
    }

    void ActivateDiskManager(GameObject diskManagerToActivate)
    {
        // 모든 디스크 비활성화
        diskManager_Eccentricity10.SetActive(false);
        diskManager_Eccentricity25.SetActive(false);
        diskManager_Eccentricity35.SetActive(false);

        // 현재 디스크 활성화
        diskManagerToActivate.SetActive(true);
    }

    void UpdateDiskMaterials()
    {
        int targetDiskNumber = (int)stateManager.GetCurrentTargetDisk() + 1; // 열거형 값을 숫자로 변환하고 1을 더함


        // 모든 자식 디스크에 대해 루프
        for (int i = 1; i <= 4; i++)
        {
            GameObject disk = transform.Find("Disk" + i).gameObject;
            if (disk != null)
            {
                Material diskMaterial = (i == targetDiskNumber) ? targetMaterial : referenceMaterial;
                disk.GetComponent<Renderer>().material = diskMaterial;
            }
        }
    }


    Vector3 CalculateTargetDiskColor(Vector3 referenceColor, float adjustmentPercentage)
    {
        Vector3 adjustedColor = new Vector3(
            referenceColor.x + 10,
            referenceColor.y + 10,
            referenceColor.z
        );
        return adjustedColor;
    }



}
