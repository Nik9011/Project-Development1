using UnityEngine;

public class MoveVFXForward : MonoBehaviour
{
    public float speed = 10f; // Speed of the VFX movement

    private void Update()
    {
        MoveVFX();
    }

    private void MoveVFX()
    {
        // Move the VFX forward based on its own forward direction
        transform.position += transform.forward * speed * Time.deltaTime;
    }
}
