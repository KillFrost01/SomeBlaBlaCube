using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FreeLookCameraController : MonoBehaviour
{
    public delegate void ShowArticleEventHandler(string articleName);
    public static event ShowArticleEventHandler ShowArticleEvent;
    

    public static RaycastHit hit;
    public GameObject blaBlaCube;

    //референс управления вращением камеры
    public InputActionReference RotateReference;

    private void Update()
    {
        //если нажата левая кнопка мыши
        if(SceneController.inputActions.Camera.Hold.phase == UnityEngine.InputSystem.InputActionPhase.Started)
        {
            //то присваиваем референс вращения к XY осям скрипта CinemachineInputProvider
            GetComponent<CinemachineInputProvider>().XYAxis = RotateReference;
        }
        else
        {
            //иначе удаляем
            GetComponent<CinemachineInputProvider>().XYAxis = null;
        }

        if (GetComponent<CinemachineFreeLook>().Follow == null)
        {
            transform.position = Vector3.Lerp(transform.position, FreeLookCameraController.hit.collider.GetComponentsInChildren<Transform>()[1].transform.position, 0.1f);
            transform.LookAt(FreeLookCameraController.hit.collider.transform.position);

            // FreeLookCameraController.hit.collider.GetComponent<Animator>().SetBool("isPress", true);

        }
    }

    private void LateUpdate()
    {
        //обработка нажатия на сторону куба
        if (Mouse.current.leftButton.wasPressedThisFrame)   //событие нажатия лкм
        {
            var ray = Camera.main.ScreenPointToRay(SceneController.inputActions.Camera.HitPress.ReadValue<Vector2>());  //рейкаст курсора мышки

            if (Physics.Raycast(ray, out hit))
            {
                //
                if (GetComponent<CinemachineFreeLook>().Follow == null && hit.collider.CompareTag("CubeEdge"))
                {
                    Debug.Log("Hit the " + hit.collider.name);

                    GetComponent<CinemachineFreeLook>().Follow = blaBlaCube.transform;
                    GetComponent<CinemachineInputProvider>().XYAxis = RotateReference;

                    hit = new RaycastHit();
                }
                //если рейкаст упал на сторону куба, то отключаем синамашину и двигаем обычную камеру в CameraController.cs
                if (hit.collider.CompareTag("CubeEdge"))
                {
                    Debug.Log("Hit the " + hit.collider.name);
                    GetComponent<CinemachineFreeLook>().Follow = null;
                    ShowArticleEvent(hit.collider.name);
                }

            }
        }
    }
}
