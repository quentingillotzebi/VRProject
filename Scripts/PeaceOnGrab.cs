using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.XR.Interaction.Toolkit;

public class PeaceOnGrab : MonoBehaviour
{

    public GameObject peace;
    public Transform spawnpoint;
    private XRGrabInteractable grabbable;
    private GameObject spawnedPeace;
    public GameObject trigger;


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
			Material myMaterial = trigger.GetComponent<Renderer>().material;
            Color newColor = myMaterial.color; // Obtenez la couleur actuelle
            newColor.a = newColor.a - 0.0005f; // Modifiez l'alpha (transparence)

// Assignez la nouvelle couleur modifiée au matériau
            myMaterial.color = newColor;
        }
    }

    public void OnGrab(XRBaseInteractor arg)
    {
        spawnedPeace = Instantiate(peace);
        trigger.tag = "SUSPICIOUS";
		
    }

     void OnRelease(XRBaseInteractor interactor)
    {
        if (spawnedPeace != null)
        {
            Destroy(spawnedPeace);
            trigger.tag = "Player";
        }
    }
    
}