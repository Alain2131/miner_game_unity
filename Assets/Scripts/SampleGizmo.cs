using UnityEngine;
using UnityEngine.UI;

public class SampleGizmo : MonoBehaviour
{
    public Text infoText;
    private GameManager game_manager;

    void Start()
    {
        game_manager = GameManager.Instance;
    }

    void Update()
    {
        int pixel_ID = game_manager.PositionToPixelID(transform.position);
        TileInfo selected_tile = game_manager.PixelIDToTileInfo(pixel_ID, 0);

        string info = $"ID {pixel_ID} : {selected_tile.type}";
        infoText.text = info;
    }
}
