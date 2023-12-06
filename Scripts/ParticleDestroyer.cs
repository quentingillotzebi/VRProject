using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleDestroyer : MonoBehaviour
{
    private void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "BEER_PARTICLE")
        {
            Destroy(collider.gameObject);
        }

    }
}
