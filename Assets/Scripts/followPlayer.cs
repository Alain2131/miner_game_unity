using UnityEngine;

public class followPlayer : MonoBehaviour
{
    public Transform player;

    private Vector3 pos;

    void Update()
    {
        pos = player.position;
        pos.z = transform.position.z;

        transform.position = pos;
    }
}
