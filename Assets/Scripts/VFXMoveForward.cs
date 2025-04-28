using UnityEngine;

public class VFXMoveForward : MonoBehaviour
{
    public float speed = 10f;
    public float lifetime = 3f;
    
    public Transform player;         // Assign your player here
    public Transform targetObject;   // The object you want to rotate
 
    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        Destroy(gameObject, lifetime); // Automatically destroy after a few seconds
      
    }

   
    private void Update()
    {
        //targetObject.rotation = Quaternion.LookRotation(player.forward);

        //transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }
}
