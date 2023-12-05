using UnityEngine;
using System.Collections;
using Random = System.Random;

public class RobotTestScriptFree : MonoBehaviour {

    private Animator anim;
    public Vector3 targetPosition;
    public float speed=5;

    public Transform player;
    
    RaycastHit hit;
    

    void Start () {
	
        anim = this.gameObject.GetComponent<Animator> ();
        StartCoroutine(RobotMoving());
    }
	
    IEnumerator RobotMoving()
    {
        while (true)
        {
            anim.SetInteger("Speed", 1);

            float elapsedTime = 0f;
            Random random = new Random();
            Vector3 startingPosition = transform.position;
            float x = UnityEngine.Random.Range(-10, 10);
            float z = UnityEngine.Random.Range(-10, 10);
            targetPosition = new Vector3(x, 0.0f, z);
            if (targetPosition != Vector3.zero)
            {
                transform.forward = targetPosition;
            }
            while (elapsedTime < 10.0f)
            {
                 transform.position = Vector3.Lerp(startingPosition, targetPosition, elapsedTime / 10.0f);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            

            anim.SetInteger("Speed", 0);

            yield return new WaitForSeconds(3.0f);
            
        }
    }
    
	
   
    void Update () {

        
        float maxDistance = 100.0f;
        for (int i = -10; i <= 20; i++)
        {
            Vector3 test = transform.TransformDirection(new Vector3((float)i + 5.0f, 4.0f, (float)i + 40)) * 5;
            Ray ray = new Ray(transform.position, test);
            Debug.DrawRay(transform.position,test, Color.green);
            if (Physics.Raycast(ray, out hit, maxDistance))
            {
                if (hit.collider.CompareTag("Player"))
                {
                    Debug.Log("Le joueur a été touché !");
                }
            }
            
        }
       
    }
}