using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    private Vector3 pos;

    void Update()
    {
        pos = GameManager.Instance.player.position;
        pos.z = transform.position.z;

        transform.position = pos;
    }
}
