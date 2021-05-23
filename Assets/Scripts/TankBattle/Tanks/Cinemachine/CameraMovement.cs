using ExtensionMethods;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Cinemachine;

public class CameraMovement : MonoBehaviour
{

    CinemachineVirtualCamera _vcam;

    [SerializeField, FormerlySerializedAs("Follow")]
    Transform _follow;
    [SerializeField, FormerlySerializedAs("LookAt")]
    Transform _lookAt;

    private void Awake()
    {
        _vcam = FindObjectOfType<CinemachineVirtualCamera>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void StartFollowing()
    {
        _vcam.transform.position = _follow.position;
        _vcam.Follow = _follow;
        _vcam.LookAt = _lookAt;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 camPos = _vcam.transform.position;
        Debug.Log(_follow.position);
        _vcam.transform.position = new Vector3(camPos.x,_follow.position.y,camPos.z);
    }

//private void LateUpdate()
//{

//     _turret.UpdateTurret(_xAxis.Value);
//}

}
