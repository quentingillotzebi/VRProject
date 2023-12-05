using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickToCamera : MonoBehaviour
{
    private Camera cameraVR;
    void Start()
    {
        cameraVR = Camera.main;

        if (cameraVR == null)
        {
            Debug.LogError("La caméra n'a pas été trouvée. Assurez-vous qu'il y a une caméra dans la scène.");
            return;
        }

        // Attacher ce GameObject à la caméra
        transform.SetParent(cameraVR.transform);
    }
}
