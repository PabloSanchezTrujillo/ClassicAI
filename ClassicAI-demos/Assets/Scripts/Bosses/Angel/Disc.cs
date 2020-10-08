using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Disc : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private bool is_big;
    [HideInInspector] public Vector2 direction;
    [HideInInspector] public bool able_to_move;
    [HideInInspector] public float distance;
    private LerpMovement disc_lerp;

    // Start is called before the first frame update
    void Awake()
    {
        direction = new Vector2();
        disc_lerp = GetComponent<LerpMovement>();
        able_to_move = false;
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (able_to_move)
        {
            able_to_move = false;
            if (is_big)
            {
                Vector3 finalPos = new Vector3(direction.x * 10, direction.y + 23, transform.position.z);

                float dist = Vector3.Distance(transform.position, finalPos);

                disc_lerp.StartLerping(transform.position, finalPos, dist / speed);
            }
            else
            {
                Vector3 finalPos = transform.position + new Vector3(direction.x * 30, direction.y * 30, transform.position.z);

                float dist = Vector3.Distance(transform.position, finalPos);

                disc_lerp.StartLerping(transform.position, finalPos, dist / speed);
            }
        }
        
    }

}
