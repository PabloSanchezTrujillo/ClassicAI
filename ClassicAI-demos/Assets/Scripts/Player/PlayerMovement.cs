using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    #region variables

    [SerializeField] private Transform lookAtTarget;
    [SerializeField] private float movementSpeed;
    [SerializeField] private float rotationSpeed;

    private Rigidbody2D rigidbody;

    #endregion variables

    // Start is called before the first frame update
    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        float Y = Input.GetAxis("Vertical");
        float X = Input.GetAxis("Horizontal");

        rigidbody.velocity += new Vector2(X, Y) * movementSpeed * Time.deltaTime;
    }

    public Vector3 GetLookAtPosition()
    {
        return lookAtTarget.position;
    }
}