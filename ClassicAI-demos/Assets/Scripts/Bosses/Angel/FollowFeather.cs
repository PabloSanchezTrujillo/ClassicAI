using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowFeather : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float rotation_speed;
    private GameObject target_player;
    private LerpMovement feather_lerp;
    private bool able_to_target;
    [HideInInspector] public bool able_to_move;
    private Dissolver dissolver;

    void Awake()
    {
        target_player = GameObject.FindGameObjectWithTag("Player");
        feather_lerp = GetComponent<LerpMovement>();
        dissolver = GetComponent<Dissolver>();
        able_to_target = true;
        able_to_move = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (able_to_move)
        {
            able_to_target = false;
            able_to_move = false;
            Vector2 dir = transform.up;
            feather_lerp.StartLerping(transform.position, dir, speed);
        }

        if (feather_lerp.hasFinishedLerping)
            Destroy(this.gameObject);
    }

    void FixedUpdate()
    {
        if (able_to_target && this.gameObject && target_player)
            FeatherLookAt();
    }

    private void FeatherLookAt()
    {
        Vector2 direction = target_player.transform.position - transform.position;
        float angle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;

        Quaternion rotation = Quaternion.AngleAxis(-angle, Vector3.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotation_speed * Time.deltaTime);
    }

}
