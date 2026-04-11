using UnityEngine;

public class ParallaxLayer : MonoBehaviour
{
    [Range(0f, 1f)]
    public float parallaxSpeed = 0.3f;

    private Transform cam;
    private Vector3 lastCamPos;

    void Start()
    {
        cam = Camera.main.transform;
        lastCamPos = cam.position;
    }

    void LateUpdate()
    {
        if (cam == null) return;

        Vector3 delta = cam.position - lastCamPos;
        transform.position += new Vector3(
            delta.x * parallaxSpeed,
            delta.y * parallaxSpeed,
            0f
        );

        lastCamPos = cam.position;
    }

    void OnEnable()
    {
        if (Camera.main != null)
            lastCamPos = Camera.main.transform.position;
    }
}
