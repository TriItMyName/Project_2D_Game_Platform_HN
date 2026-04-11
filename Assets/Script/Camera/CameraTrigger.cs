using Cinemachine;
using UnityEngine;

public class CameraTrigger : MonoBehaviour
{
    [SerializeField] CinemachineVirtualCamera newCamera;

    [Header("Parallax (Optional)")]
    public GameObject roomParallax;

    private void OnTriggerEnter2D(Collider2D _other)
    {
        if (!_other.CompareTag("Player")) return;

        CameraManager.instance.SwapCamera(newCamera);

        // 🔥 Bật / tắt Parallax theo room
        if (CameraManager.instance.parallaxController != null)
        {
            CameraManager.instance.parallaxController
                .SetRoomParallax(roomParallax);
        }
    }
}
