using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class BulletSkelletonSpawner : MonoBehaviour
{

    public float bulletPerSecond = 20.0f;

    public float lifeTime = 5.0f;

    private bool canShot = true;

    public float delayStart = 0.3f;
    
    // Start is called before the first frame update

    private void Start()
    {
        StartCoroutine(StartShooting());
        StartCoroutine(StopShooting());
    }
    

    IEnumerator StartShooting()
    {
        yield return new WaitForSeconds(delayStart);
        
        while (canShot)
        {
            GameObject bullet = ObjectPooler.Instance.SpawnFromPool("BulletSke", transform.position, Quaternion.identity);
            //bullet.transform.parent = transform;
            yield return new WaitForSeconds(bulletPerSecond/60.0f);
        }
        
        Destroy(gameObject);
    }

    IEnumerator StopShooting()
    {
        yield return new WaitForSeconds(lifeTime);
        canShot = false;
        
    }
}
