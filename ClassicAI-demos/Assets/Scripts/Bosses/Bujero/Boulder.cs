using System;
using System.Collections;
using UnityEngine;

public class Boulder : MonoBehaviour
{
    [SerializeField] private AudioSource spinningSource;

    public float speed = 3;

    public Transform sucker;

    public float bounceAngleMaxVariation = 20;

    private float angle;

    private LerpMovement lerper;

    private bool isBeingSucked;

    private bool isOrbit;

    private bool isBouncer;

    public void InitSuck(Transform suckPoint, float speed)
    {
        this.sucker = suckPoint;
        this.speed = speed;
        this.isBeingSucked = true;
    }

    public void InitOrbit(Transform center, float orbitSpeed)
    {
        this.sucker = center;
        this.speed = orbitSpeed;
        this.isOrbit = true;
    }

    public void InitBouncer(Transform center, float angle, float bouncerSpeed)
    {
        this.sucker = center;
        this.speed = bouncerSpeed;
        this.angle = angle;
        this.isBouncer = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        lerper = GetComponent<LerpMovement>();

        if (isBeingSucked)
        {
            float dist = Vector3.Distance(this.transform.position, sucker.position);
            float time = speed == 0 ? 20 : Mathf.Clamp(dist / speed, -20, 20);

            lerper.StartLerping(this.transform.position, sucker.position, time);
        }

        if (isBouncer)
        {
            spinningSource.Play();

            StartCoroutine(ThrowAtAngle(angle));
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isOrbit)
        {
            this.transform.RotateAround(sucker.position, Vector3.forward, Time.deltaTime * speed * 60);
        }

        if (isBeingSucked && lerper.hasFinishedLerping)
        {
            Destroy(this.gameObject);
        }
    }

    public void DestroyWithFade()
    {
        StartCoroutine(DestroyWithFadeCoroutine());
    }

    private IEnumerator DestroyWithFadeCoroutine()
    {
        if (this is null)
            yield return null;

        this.speed = 0;
        this.lerper.enabled = false;

        // Activate Dissolve
        foreach (Dissolver dis in GetComponentsInChildren<Dissolver>())
        {
            dis.Dissolve();
        }

        // Wait for boulder to fade out
        yield return new WaitForSeconds(GetComponentInChildren<Dissolver>().dissolveTime);

        // Destroy
        Destroy(this.gameObject);
    }

    private IEnumerator ThrowAtAngle(float ang)
    {
        Vector3 direction = new Vector3(Mathf.Cos(Mathf.Deg2Rad * ang), Mathf.Sin(Mathf.Deg2Rad * ang), 0);

        // Check destination with raycast
        RaycastHit2D[] hits = new RaycastHit2D[10];
        RaycastHit2D wallHit = new RaycastHit2D();
        ContactFilter2D filter = new ContactFilter2D();

        Physics2D.Raycast(this.transform.position, direction, filter, hits, Mathf.Infinity);

        foreach (RaycastHit2D hit in hits)
        {
            if (hit.transform is null)
                continue;

            if (hit.collider.gameObject.CompareTag("Wall"))
            {
                wallHit = hit;
            }
        }

        // Calculate time of travel
        float dist = Vector3.Distance(this.transform.position, wallHit.point);
        float time = speed == 0 ? 20 : Mathf.Clamp(dist / speed, -20, 20);

        // Throw at destination and wait for it to hit
        lerper.StartLerping(this.transform.position, wallHit.point, time);
        yield return new WaitUntil(() => lerper.hasFinishedLerping);

        // Calculate new angle
        Vector2 newDir = Vector2.Reflect(direction, wallHit.normal);

        float nextAngle = Vector2.SignedAngle(Vector2.right, newDir);
        nextAngle += UnityEngine.Random.Range(-bounceAngleMaxVariation, bounceAngleMaxVariation);

        // Throw again
        StartCoroutine(ThrowAtAngle(nextAngle));
    }
}
