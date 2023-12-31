using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class TireuseController : MonoBehaviour
{
    public GameObject lever;
    public GameObject particuleBiere;
    public GameObject faucet;
    public AudioSource beerSound;

    private float _leverActivateRangeInf = 50f;
    private float _leverActivateRangeSup = 80f;
    private bool _beerIsRunning = false;

    private IEnumerator coroutine;

    public GameObject player;

	public XRGrabInteractable grabbable;



    // Start is called before the first frame update
    void Start()
    {
        coroutine = BeerFlow();
		//grabbable = GetComponent<XRGrabInteractable>();
    }

    // Update is called once per frame
    void Update()
    {
        if (lever.transform.eulerAngles.x > _leverActivateRangeInf && lever.transform.eulerAngles.x <= _leverActivateRangeSup)
        {
            if (!_beerIsRunning )
            {
                _beerIsRunning = true;
                StartCoroutine(coroutine);
                beerSound.Play();
               player.tag = "SUSPICIOUS";
            }

        } 
        else
        {
            _beerIsRunning = false;
            StopCoroutine(coroutine);
            beerSound.Stop();
			if (!grabbable.isSelected){
            	player.tag = "Player";
			}
        }
    }

    IEnumerator BeerFlow()
    {
        while(true)
        {
            yield return new WaitForSeconds(0.07f);
            Instantiate(particuleBiere, new Vector3(faucet.transform.position.x, faucet.transform.position.y - 0.02f, faucet.transform.position.z), Quaternion.identity);
        }
    }
}
