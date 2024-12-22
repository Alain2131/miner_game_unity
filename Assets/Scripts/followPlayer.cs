using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public Transform playerTransform;

    private Vector3 pos;

    void Update()
    {
        pos = playerTransform.position;
        pos.z = transform.position.z;

        transform.position = pos;
    }
}
