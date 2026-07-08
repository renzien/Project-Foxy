using UnityEngine;

public class ParallaxEffect : MonoBehaviour
{
    public Camera cam;
    public float parallaxMultiplier;

    private float startPosX;
    private float startPosY;

    void Start()
    {
        if (cam == null)
        {
            cam = Camera.main;
        }

        startPosX = transform.position.x;
        startPosY = transform.position.y;
    }

    void LateUpdate()
    {
        float dist = cam.transform.position.x * parallaxMultiplier;
        transform.position = new Vector3(startPosX + dist, startPosY, transform.position.z);
    }
}
