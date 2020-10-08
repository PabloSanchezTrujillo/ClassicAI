using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeatherController : MonoBehaviour
{
    [SerializeField] private Transform[] feather_spawns;
    [SerializeField] private GameObject feather_obj;
    [SerializeField] private GameObject follow_feather_obj;
    [SerializeField] private AudioSource audioHandler;
    private GameObject target_player;

    private List<GameObject> feathers;
    private List<GameObject> follow_feathers;

    // Start is called before the first frame update
    void Start()
    {
        target_player = GameObject.FindGameObjectWithTag("Player");
        feathers = new List<GameObject>();
        follow_feathers = new List<GameObject>();

        if (target_player?.transform.position.x < 0)
        {
            StartCoroutine(FeatherRainAttackLeft());
        }
        else
        {
            StartCoroutine(FeatherRainAttackRight());
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator FeatherRainAttackLeft()
    {
        for (int i = 0; i < feather_spawns.Length; i++)
        {
            yield return new WaitForSeconds(0.2f);
            if(i % 2 == 0)
            {
                audioHandler.Play();
                feathers.Add(Instantiate(feather_obj, feather_spawns[i].position, feather_spawns[i].rotation));
            }
            else
            {
                audioHandler.Play();
                follow_feathers.Add(Instantiate(follow_feather_obj, feather_spawns[i].position, feather_spawns[i].rotation));
            }
        }

        yield return new WaitForSeconds(0.5f);

        for (int i = 0; i < follow_feathers.Count; i++)
        {
            follow_feathers[i].GetComponent<FollowFeather>().able_to_move = true;
        }
        

        yield return new WaitForSeconds(1.0f);

        for (int i = 0; i < feathers.Count; i++)
        {
            feathers[i].GetComponent<Feather>().able_to_move = true;
        }

        Destroy(this.gameObject);

        yield return null;
    }

    private IEnumerator FeatherRainAttackRight()
    {
        for (int i = feather_spawns.Length - 1; i >0; i--)
        {
            yield return new WaitForSeconds(0.2f);

            if (i % 2 == 0)
            {
                audioHandler.Play();
                feathers.Add(Instantiate(feather_obj, feather_spawns[i].position, feather_spawns[i].rotation));
            }
            else
            {
                audioHandler.Play();
                follow_feathers.Add(Instantiate(follow_feather_obj, feather_spawns[i].position, feather_spawns[i].rotation));
            }
        }

        yield return new WaitForSeconds(0.5f);

        for (int i = 0; i < follow_feathers.Count; i++)
        {
            follow_feathers[i].GetComponent<FollowFeather>().able_to_move = true;
        }

        yield return new WaitForSeconds(1.0f);

        for (int i = 0; i < feathers.Count; i++)
        {
            feathers[i].GetComponent<Feather>().able_to_move = true;
        }

        Destroy(this.gameObject);

        yield return null;
    }
}
