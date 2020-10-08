using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Feather : MonoBehaviour
{
    [SerializeField] private float speed;
    [HideInInspector] public bool able_to_move;
    private LerpMovement feather_lerp;
    private Dissolver dissolver;

    // Start is called before the first frame update
    void Awake()
    {
        feather_lerp = GetComponent<LerpMovement>();
        dissolver = GetComponent<Dissolver>();
        able_to_move = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (able_to_move)
        {
            able_to_move = false;
            feather_lerp.StartLerping(transform.position, new Vector2(transform.localPosition.x, transform.localPosition.y - 10), speed);
        }

        if(feather_lerp.hasFinishedLerping)
            Destroy(this.gameObject);
    }
}
