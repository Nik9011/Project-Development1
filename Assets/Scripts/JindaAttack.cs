using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JindaAttack : MonoBehaviour
{
    public GameObject magicVFX;
    public Transform magicPos;
   public void spawnMagic()
    {
      GameObject obj=  Instantiate(magicVFX, magicPos.transform.position, transform.rotation);
        Destroy(obj, 3);

    }
}
