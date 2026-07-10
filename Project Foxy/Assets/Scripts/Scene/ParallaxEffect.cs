using UnityEngine;

public class ParallaxEffect : MonoBehaviour
{
    public Transform target;
    public float parallaxSpeed;

    private Vector3 lastTargetPos;

    void Start()
    {
        if (target == null)
        {
            target = GameObject.FindGameObjectWithTag("Player").transform;
        }

        lastTargetPos = target.position;
    }

    void LateUpdate()
    {
        Vector3 deltaMovement = target.position - lastTargetPos;
        transform.position += new Vector3(deltaMovement.x * parallaxSpeed, 0f, 0f);
        lastTargetPos = target.position;
    }
}
