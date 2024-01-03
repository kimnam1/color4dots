using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiskManager : MonoBehaviour
{
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


}
