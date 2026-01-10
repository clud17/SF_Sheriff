using UnityEngine;

public class ParallaxLayer : MonoBehaviour
{
    [SerializeField] private Transform cam;
    [SerializeField] private float parallaxFactor;
    [SerializeField] private float parallayFactor;

    private Vector3 lastCamPos;

    void Start()
    {
        if (cam == null)
            cam = Camera.main.transform;

        lastCamPos = cam.position;
    }

    void LateUpdate()
    {
        Vector3 delta = cam.position - lastCamPos;
        transform.position -= new Vector3(delta.x * parallaxFactor,
                                          delta.y * parallayFactor,
                                          0f);

        lastCamPos = cam.position;
    }
}