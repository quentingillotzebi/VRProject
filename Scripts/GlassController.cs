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
            if (_liquidChild.transform.localScale.y - _fillStep >= 0)
            {
                _liquidChild.transform.localScale -= new Vector3(0, _fillStep, 0);
            }
        }
        
    }
}
