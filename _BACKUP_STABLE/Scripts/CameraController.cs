using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset = new Vector3(0, 1.0f, -10f);
    [SerializeField] private float smoothSpeed = 5.0f;
    [SerializeField] private bool lockY = false;

    private void Start()
    {
        if (target == null)
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                target = player.transform;
            }
        }
    }

    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 targetPos = target.position + offset;
        if (lockY)
        {
            targetPos.y = transform.position.y;
        }

        transform.position = Vector3.Lerp(transform.position, targetPos, smoothSpeed * Time.deltaTime);
    }
}
