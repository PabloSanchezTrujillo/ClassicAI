using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableOnLimits : MonoBehaviour
{

    [SerializeField] private float maxDistance = 100.0f;

    private Vector3 originalPos;
    
    // Start is called before the first frame update
    void Start()
    {
        originalPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if ((transform.position - originalPos).magnitude >= maxDistance)
        {
            gameObject.SetActive(false);
        }
    }
}
