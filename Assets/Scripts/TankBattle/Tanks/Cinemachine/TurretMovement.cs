using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class TurretMovement : MonoBehaviour
{

    public void UpdateTurret(float yAngle)
    {
        transform.eulerAngles = new Vector3(0, yAngle, 0);
    }
}
