using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder;

public class GlassController : MonoBehaviour
{

    public float fillPercentage = 0.01f;
    public GameObject glassParent;
    
    private GameObject _liquidChild;
    private float _maxScale = 0.6868837f;
    private float _fillStep = 0.01f;
    public GameObject player;
    public GameObject lController;
    public GameObject rController;
    private bool isDrinking =false;
    public AudioSource source;
    // Start is called before the first frame update
    void Start()
    {
        _liquidChild = glassParent.transform.GetChild(0).gameObject;
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "BEER_PARTICLE")
        {
            if (_liquidChild.transform.localScale.y < _maxScale + _fillStep)
            {
                _liquidChild.transform.localScale += new Vector3(0, _fillStep, 0);
            }

            Destroy(collider.gameObject);
        }
    }

    private void OnTriggerStay(Collider collider)
    {
        if (collider.tag == "MOUTH")
        {

            Debug.Log("Je bois");
            if (_liquidChild.transform.localScale.y - _fillStep >= 0)
            {
                if (!isDrinking)
                {
                    source.Play();
                    isDrinking = true;
                }

                _liquidChild.transform.localScale -= new Vector3(0, _fillStep, 0);
                Material myMaterial = player.GetComponent<Renderer>().material;
                Color newColor = myMaterial.color; // Obtenez la couleur actuelle
                newColor.a = newColor.a + 0.002f; // Modifiez l'alpha (transparence)

// Assignez la nouvelle couleur modifiée au matériau
                myMaterial.color = newColor;
                //OVRInput.SetControllerVibration(20);
            }
            else
            {
                if (isDrinking)
                {
                    source.Stop();
                    isDrinking = false;
                }
            }
        }
        
    }
}
