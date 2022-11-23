using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;

[ExecuteInEditMode]
public class Solidify : MonoBehaviour
{
    public Shader flatShader;
    Camera cam;

    void OnEnable()
    {
        cam = GetComponent<Camera>();
        cam.SetReplacementShader(flatShader, "");
    }

}
