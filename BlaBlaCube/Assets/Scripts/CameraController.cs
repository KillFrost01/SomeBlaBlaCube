using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    public CinemachineFreeLook CMFreeLook;

    private void Update()
    {

        /*if (CMFreeLook.Follow == null)
        {
            CMFreeLook.transform.position = Vector3.Lerp(transform.position, FreeLookCameraController.hit.collider.GetComponentsInChildren<Transform>()[1].transform.position, 0.1f);
            CMFreeLook.transform.LookAt(FreeLookCameraController.hit.collider.transform.position);

           // FreeLookCameraController.hit.collider.GetComponent<Animator>().SetBool("isPress", true);

        }*/

    }

}
