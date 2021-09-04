using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum CameraState
{
    Enable,         //рабочее состояние
    Disable,        //управление камерой отколючено
    MoveToCube,     //движется к кубу/у куба
    Return,         //возращается в исходное положение
    Waiting         //состояние ожидания (после 60 сек)
}
public class CameraController : MonoBehaviour
{
    //делегат и событие нажатия на грань куба
    public delegate void OnEdgeClickHandler(string header);
    public static event OnEdgeClickHandler OnEdgeClickEvent;
    //делегат и событие для таймера
    public delegate void OnUseHandler();
    public static event OnUseHandler OnUseEvent;


    private CameraState cameraState = CameraState.Enable;

    private UserActions actions;    

    [SerializeField]
    private Transform target;           //цель
    [SerializeField]
    private float maxHeight = 10;       //верзняя граница
    [SerializeField]
    private float minHeight = 1f;       //нижняя граница

    public float rotationSpeed = 2f;    //макс.скорость вращения

    private float curSpeed;             //текущая скорость вращения

    private Vector3 dir;                            //направление движения камеры
    private Vector3 prevCamPosition;                //буфер сохранения позиции камеры
    private Vector3 startCamPosition;               //начальное положение камеры

    private RaycastHit hit;                         //хит рейкаста

    private void Awake()
    {
        actions = new UserActions();
    }

    private void OnEnable()
    {
        actions.Enable();
        SceneManager.TimerStopEvent += ChangeCameraStateOnWait;
    }

    private void OnDisable()
    {
        actions.Disable();
        SceneManager.TimerStopEvent -= ChangeCameraStateOnWait;
    }

    private void Start()
    {
        cameraState = CameraState.Waiting;
        startCamPosition = transform.position;
        StartCoroutine(CameraWaiting());
    }

    /// <summary>
    /// в fixedupdate описано управление камерой
    /// </summary>
    private void FixedUpdate()
    {
        //если состояние камеры enable
        if (cameraState == CameraState.Enable)
        {
            if (actions.Camera.Hold.phase == InputActionPhase.Started)
            {
                OnUseEvent();                //событие-обнуление счетчика

                curSpeed = rotationSpeed;
                //направление движения мыши
                Vector2 mDelta = actions.Camera.Rotate.ReadValue<Vector2>().normalized;
                //направление движения камеры
                dir = new Vector3(-mDelta.y, mDelta.x, 0);

            }

            //если камера достигает дедзоны, то по У коорд не двигаем
            if (transform.position.y < minHeight + 0.6 && dir.x > 0 || transform.position.y > maxHeight - 0.5 && dir.x < 0)
                dir.x = 0;

            //что-то типа инерции (затухание скорости)
            curSpeed = curSpeed > 0 ? curSpeed - 2f * Time.deltaTime : 0f;

            //transform.RotateAround(target.position, dir, 1f * curSpeed * Time.deltaTime);   

            //вращение вокруг куба по двум осям
            transform.RotateAround(target.position, transform.right, -dir.x * curSpeed);
            transform.RotateAround(target.position, transform.up, -dir.y * curSpeed);

        }

        //поворот камеры на куб
        transform.LookAt(target);
    }

    /// <summary>
    /// в update описано взаимодействие с кубом
    /// </summary>
    private void Update()
    {
        //обработка нажатия на грань куба
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            OnUseEvent();                //событие-обнуление счетчика
            var ray = Camera.main.ScreenPointToRay(SceneManager.inputActions.Camera.HitPress.ReadValue<Vector2>());  //рейкаст курсора мышки

            if (Physics.Raycast(ray, out hit))
            {

                //если рейкаст упал на сторону куба и состояние камеры enable
                if (hit.collider.CompareTag("CubeEdge") && cameraState == CameraState.Enable)
                {
                    Debug.Log("Hit the " + hit.collider.name);

                    prevCamPosition = transform.position;           //сохраняем позицию камеры до приближения к кубу

                    cameraState = CameraState.MoveToCube;                 //смена состояния камеры
                    
                    OnEdgeClickEvent(hit.collider.name);                 //вызов события
                }
                else if (!hit.collider.CompareTag("CubeEdge") && cameraState != CameraState.Waiting)
                {
                    cameraState = CameraState.Return;

                }
            }
        }
        //если состояние камеры onMove, то двигаем камеру к выбранной грани куба
        if (cameraState == CameraState.MoveToCube)
        {
            transform.position = Vector3.Lerp(transform.position, hit.collider.GetComponentsInChildren<Transform>()[1].position, 5f * Time.deltaTime);
            //поворот камеры на куб
            transform.LookAt(target);
        }
        //если состояние OnReturn, то перемещаем камеру от куба к исходной позиции
        if (cameraState == CameraState.Return)
        {
            transform.position = Vector3.Lerp(transform.position,prevCamPosition, 5f * Time.deltaTime);
            //поворот камеры на куб
            transform.LookAt(target);
        }
        //если камера достигла исходной позиции, то меняем состояние на enable
        if((transform.position-prevCamPosition).sqrMagnitude < 0.01f && cameraState == CameraState.Return)
        {
            cameraState = CameraState.Enable;
        }
    }


    private IEnumerator CameraWaiting()
    {
        while ((transform.position - startCamPosition).sqrMagnitude > 0.001f)
        {
            transform.position = Vector3.Slerp(transform.position, startCamPosition, 5f * Time.deltaTime);
            transform.LookAt(target);
            yield return new WaitForSeconds(0.001f);
        }
        while (true)
        {
            yield return new WaitForSeconds(0.01f);
            transform.RotateAround(target.position, transform.up, 5f * Time.deltaTime);
            transform.LookAt(target);
            if (Mouse.current.leftButton.isPressed)
            {
                cameraState = CameraState.Enable;
                OnUseEvent();
                break;
            }
        }
        yield return null;
    }

    /// <summary>
    /// обработчик события остановки счетчика (60 сек),
    /// смена состояния камеры на Waiting
    /// </summary>
    private void ChangeCameraStateOnWait()
    {
        StopAllCoroutines();
        cameraState = CameraState.Waiting;
        StartCoroutine(CameraWaiting());
    }

}
