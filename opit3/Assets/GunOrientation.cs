using UnityEngine;

public class GunOrientation : MonoBehaviour
{
    private Transform cam;

    private void Start()
    {
        cam = PlayerMovement.Instance.GetPlayerCamTransform();
    }

    private void LateUpdate()
    {
        transform.rotation = cam.rotation;
    }
}
