using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class ForceAspect : MonoBehaviour
{
    public float aspect = 1.5f;
    
    void OnEnable()
    {
        GetComponent<Camera>().aspect = aspect;
    }
}
