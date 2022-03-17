using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBillboard : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void LateUpdate()
    {
        // transform.LookAt(transform.position + Camera.main.transform.forward);
        // transform.eulerAngles = new Vector3(transform.eulerAngles.x, 0f, transform.eulerAngles.z);
        // //transform.forward = new Vector3(Camera.main.transform.forward.x, transform.forward.y, Camera.main.transform.forward.z);
        transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward, Camera.main.transform.rotation * Vector3.up);
        Vector3 eulerAngles = transform.eulerAngles;
        eulerAngles.x = 0;
        transform.eulerAngles = eulerAngles;
    }
}
