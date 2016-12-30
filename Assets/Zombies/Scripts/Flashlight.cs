using UnityEngine;
using System.Collections;

public class Flashlight : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
    }
	
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            this.GetComponent<Light>().enabled = !GetComponent<Light>().enabled;
        }

        RaycastHit screenRayInfo;
        Physics.Raycast(Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0)), out screenRayInfo);
        transform.LookAt(screenRayInfo.point);
    }
}
