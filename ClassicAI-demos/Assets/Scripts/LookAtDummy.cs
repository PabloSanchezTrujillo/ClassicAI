using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtDummy : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private float distance = 4.0f;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = player.transform.position + Vector3.up * distance;
    }
}
