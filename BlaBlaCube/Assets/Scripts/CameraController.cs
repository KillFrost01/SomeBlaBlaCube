using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    private UserActions inputActions;               
    //референс управления вращением камеры
    public InputActionReference RotateReference;

    private void Awake()
    {
        inputActions = new UserActions();
    }

    private void OnEnable()
    {
        inputActions.Camera.Press.Enable();
    }
    private void OnDisable()
    {
        inputActions.Camera.Press.Disable();
    }

    private void Update()
    {
        //если нажата левая кнопка мыши
        if(inputActions.Camera.Press.phase == UnityEngine.InputSystem.InputActionPhase.Started)
        {
            //то присваиваем референс вращения к XY осям скрипта CinemachineInputProvider
            GetComponent<CinemachineInputProvider>().XYAxis = RotateReference;
        }
        else
        {
            //иначе удаляем
            GetComponent<CinemachineInputProvider>().XYAxis = null;
        }
    }


    //наш бла-бла-кубик
    /*public Transform target;
    public float rotationSpeed;
    private Vector3 mousePosition;

    // Start is called before the first frame update
    void Start()
    {
        mousePosition = Input.mousePosition;
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(target);
        Vector3 rotDirection = (mousePosition - (Vector3)Input.mousePosition).normalized;
        
        //если зажата лкм
        if (Input.GetMouseButton(0))
        {
            transform.RotateAround(target.position, rotDirection, rotationSpeed*Time.deltaTime);
        }
    }*/
}
