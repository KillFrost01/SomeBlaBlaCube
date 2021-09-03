using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{

    private UserActions actions;

    public Transform target;
    [SerializeField]
    public float maxHeight = 10;
    [SerializeField]
    public float minHeight = 1f;

    public float rotationSpeed = 2f;
    private float curSpeed;

    private Vector3 dir;
    private Vector3 cubePosition;

    private float distance;

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

    private void Start()
    {
        distance = (transform.position - target.position).sqrMagnitude;
        cubePosition = target.position;
    }

    private void FixedUpdate()
    {
        if (actions.Camera.Hold.phase == UnityEngine.InputSystem.InputActionPhase.Started)
        {
            curSpeed = rotationSpeed;
            //направление движения мыши
            Vector2 mDelta = actions.Camera.Rotate.ReadValue<Vector2>().normalized;

            dir = new Vector3(-mDelta.y, mDelta.x, 0);

            if (transform.position.y < (maxHeight-minHeight)/2f)
            {
                target.position = new Vector3(target.position.x, cubePosition.y + (maxHeight - minHeight) / 2f - transform.position.y, target.position.z);
            }
            
        }

        curSpeed = curSpeed>0? curSpeed - 200f * Time.deltaTime : 0f;   //что-то типа инерции (затухание скорости)
        
        transform.RotateAround(target.position, dir, 1f * curSpeed * Time.deltaTime);   //вращение вокруг куба
        transform.LookAt(target);                                                       //поворот камеры на куб
    }
    

}
