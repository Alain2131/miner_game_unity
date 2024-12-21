using UnityEngine;
using UnityEngine.UI;

public class UIDebugInfo : MonoBehaviour
{
    public Text FPS_counter;
    public Text depth_meter;
    public Text pixel_ID_info;

    // Velocity
    // Money Balance
    // Player State (Walking, Falling, Flying, Dig Down/Left/Right, Idle(?))
    // Digging Speed Mult (getting slower to dig as we go down, visualising this is important)
    // isDigging (maybe the playerState will be enough)

    public void Update()
    {
        Transform player = GameManager.Instance.player;

        int FPS = (int)(1f / Time.unscaledDeltaTime);
        FPS_counter.text = FPS.ToString() + " FPS";

        int height = -(int)player.position.y;
        depth_meter.text = "Depth : " + height.ToString() + " M";

        int pixel_ID = (int)GameManager.Instance.PositionToPixelID(player.position);
        pixel_ID_info.text = "Pixel ID : " + pixel_ID.ToString();
    }
}
