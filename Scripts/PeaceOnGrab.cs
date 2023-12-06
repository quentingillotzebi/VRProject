using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.XR.Interaction.Toolkit;

public class PeaceOnClick : MonoBehaviour
{

    public GameObject peace;
    public Transform spawnpoint;
    private XRGrabInteractable grabbable;
    private GameObject spawnedPeace;
    private bool isGrabbed = false;
    public XRController leftController;
    public XRController rightController;
    public Transform blockedArea;

    // Start is called before the first frame update
    void Start()
    {
        grabbable = GetComponent<XRGrabInteractable>();
        grabbable.onSelectEntered.AddListener(OnGrab);
        grabbable.onSelectExited.AddListener(OnRelease);
    }

    // Update is called once per frame
    void Update()
    {
        spawnpoint.transform.SetParent(gameObject.transform);
         if (grabbable.isSelected)
        {
            UpdateWhileGrabbed();
        }
    }

     void UpdateWhileGrabbed()
    {
         if (spawnedPeace != null)
        {
            spawnedPeace.transform.rotation = spawnpoint.transform.rotation;
            spawnedPeace.transform.position = spawnpoint.transform.position;
            BlockControllerPosition(leftController);
            BlockControllerPosition(rightController);
        }
    }

    void BlockControllerPosition(XRController controller)
    {
        if (controller != null)
        {
            // Limitez la position du contrôleur à l'intérieur de la zone bloquée
            Vector3 clampedPosition = blockedArea.InverseTransformPoint(controller.transform.position);
            clampedPosition.x = Mathf.Clamp(clampedPosition.x, -1f, 1f);
            clampedPosition.y = Mathf.Clamp(clampedPosition.y, -1f, 1f);
            clampedPosition.z = Mathf.Clamp(clampedPosition.z, -1f, 1f);

            controller.transform.position = blockedArea.TransformPoint(clampedPosition);
        }
    }

    public void OnGrab(XRBaseInteractor arg)
    {
        spawnedPeace = Instantiate(peace);
        
        Rigidbody spawnedRigidbody = spawnedPeace.GetComponent<Rigidbody>();
        if (spawnedRigidbody != null)
        {
            spawnedRigidbody.isKinematic = true;
        }

        isGrabbed = true;
    }

     void OnRelease(XRBaseInteractor interactor)
    {
        if (spawnedPeace != null)
        {
            Rigidbody spawnedRigidbody = spawnedPeace.GetComponent<Rigidbody>();
            if (spawnedRigidbody != null)
            {
                spawnedRigidbody.isKinematic = false;
            }

            // Détruire l'objet spawnedPeace
            Destroy(spawnedPeace);

            // Marquer l'objet comme non saisi
            isGrabbed = false;
        }
    }
    
}
