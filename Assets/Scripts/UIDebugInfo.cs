using UnityEngine;
using UnityEngine.UI;

public class UIDebugInfo : MonoBehaviour
{
    public Text FPSCounter;
    public Text depthMeter;

    // Velocity
    // Money Balance
    // Player State (Walking, Falling, Flying, Dig Down/Left/Right, Idle(?))
    // Digging Speed Mult (getting slower to dig as we go down, visualising this is important)
    // isDigging (maybe the playerState will be enough)

    public void Update()
    {
        int current = (int)(1f / Time.unscaledDeltaTime);
        FPSCounter.text = current.ToString() + " FPS";

        int height = -(int)GameManager.Instance.player.position.y;
        depthMeter.text = "Depth : " + height.ToString() + " M";
    }
}
