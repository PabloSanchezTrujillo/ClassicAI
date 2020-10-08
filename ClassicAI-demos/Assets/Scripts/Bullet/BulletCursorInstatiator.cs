using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletCursorInstatiator : MonoBehaviour
{


    [SerializeField] private GameObject target;
    [SerializeField] private GameObject bulletPrefab;

    private Camera mainCamera;
    
    private void Awake()
    {
        mainCamera = Camera.main;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        Vector3 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0.0f;

        Vector3 direction = target.transform.position -  mousePos;
        direction.z = 0;
        direction = direction.normalized;
        
        if (Input.GetMouseButtonDown(0))
        {
            
            GameObject bulletObject = Instantiate(
                bulletPrefab, mousePos, Quaternion.identity);
            Bullet bullet = bulletObject.GetComponent<Bullet>();
            bullet.Init(direction, BulletColor.BLUE);
            
        } else if (Input.GetMouseButtonDown(1))
        {
            GameObject bulletObject = Instantiate(
                bulletPrefab, mousePos, Quaternion.identity);
            Bullet bullet = bulletObject.GetComponent<Bullet>();
            bullet.Init(direction, BulletColor.RED);
        }
    }

}
