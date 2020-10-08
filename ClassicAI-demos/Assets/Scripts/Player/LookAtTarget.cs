using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtTarget : MonoBehaviour
{
    #region variables

    [SerializeField] public Transform target;
    [SerializeField] private float rotationSpeed;

    #endregion variables

    // Update is called once per frame
    private void Update()
    {
        if(target)
            LookAt();
    }

    private void LookAt()
    {
        Vector2 direction = target.position - transform.position;
        float angle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;

        Quaternion rotation = Quaternion.AngleAxis(-angle, Vector3.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotationSpeed * Time.deltaTime);
    }
}