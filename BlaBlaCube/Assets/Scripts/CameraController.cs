using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum MoveState
{
    Enable,
    Disable,
    OnMove,
    OnReturn
}
public class CameraController : MonoBehaviour
{
    //делегат и событие нажатия на грань куба
    public delegate void OnEdgeClickHandler(string header);
    public static event OnEdgeClickHandler OnEdgeClick;
    //делегат и событие для таймера
    public delegate void OnUseHandler();
    public static event OnUseHandler OnUse;


    MoveState CameraState = MoveState.Enable;

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
    private RaycastHit hit;                         //хит рейкаста

    private void Awake()
    {
        actions = new UserActions();
    }

    private void OnEnable()
    {
        actions.Enable();
    }

    private void OnDisable()
    {
        actions.Disable();
    }

    //в fixedupdate выполняется движение камерой
    private void FixedUpdate()
    {
        //если состояние камеры enable
        if (CameraState == MoveState.Enable)
        {
            if (actions.Camera.Hold.phase == InputActionPhase.Started)
            {
                OnUse();                //событие-обнуление счетчика

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

    //в update описано взаимодействие с кубом
    private void Update()
    {
        //обработка нажатия на грань куба
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            OnUse();                //событие-обнуление счетчика
            var ray = Camera.main.ScreenPointToRay(SceneManager.inputActions.Camera.HitPress.ReadValue<Vector2>());  //рейкаст курсора мышки

            if (Physics.Raycast(ray, out hit))
            {

                //если рейкаст упал на сторону куба и состояние камеры enable
                if (hit.collider.CompareTag("CubeEdge") && CameraState == MoveState.Enable)
                {
                    Debug.Log("Hit the " + hit.collider.name);

                    prevCamPosition = transform.position;           //сохраняем позицию камеры до приближения к кубу

                    CameraState = MoveState.OnMove;                 //смена состояния камеры
                    
                    OnEdgeClick(hit.collider.name);                 //вызов события
                }
                else if (hit.collider.CompareTag("CubeEdge"))
                {
                    CameraState = MoveState.OnReturn;

                }
            }
        }
        //если состояние камеры onMove, то двигаем камеру к выбранной грани куба
        if (CameraState == MoveState.OnMove)
        {
            transform.position = Vector3.Slerp(transform.position, hit.collider.GetComponentsInChildren<Transform>()[1].position, 0.1f);
            
        }
        //если состояние OnReturn, то перемещаем камеру от куба к исходной позиции
        if (CameraState == MoveState.OnReturn)
        {
            transform.position = Vector3.Slerp(transform.position,prevCamPosition, 0.1f);
        }
        //если камера достигла исходной позиции, то меняем состояние на enable
        if((transform.position-prevCamPosition).sqrMagnitude < 0.01f && CameraState == MoveState.OnReturn)
        {
            CameraState = MoveState.Enable;
        }
    }

}
