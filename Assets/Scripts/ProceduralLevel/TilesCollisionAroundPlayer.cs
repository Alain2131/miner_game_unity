using UnityEngine;

[RequireComponent(typeof(PolygonCollider2D))]
public class TilesCollisionAroundPlayer : MonoBehaviour
{
    [SerializeField] private bool manualLutSelection = false;
    [SerializeField] private int lutSelection = 0;

    private GameManager game_manager;
    private Transform player_transform;

    private PolygonCollider2D polygon_collider_2d;

    void Start()
    {
        game_manager = GameManager.Instance;
        player_transform = game_manager.player.transform;
        polygon_collider_2d = transform.GetComponent<PolygonCollider2D>();
    }

    // Should probably be defined in a text file or something
    // .. this was written by hand. If you can change this into an algorithm, it'd be great.
    private Vector2[][] collision_LUT = new Vector2[256][]
    {
        // lut0
        new Vector2[] { new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut1
        new Vector2[] { new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, -1), new Vector2(0, -1),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut2
        new Vector2[] { new Vector2(1, 0), new Vector2(2, 0), new Vector2(2, -1), new Vector2(1, -1),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut3
        new Vector2[] { new Vector2(0, 0), new Vector2(2, 0), new Vector2(2, -1), new Vector2(0, -1),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut4
        new Vector2[] { new Vector2(2, 0), new Vector2(3, 0), new Vector2(3, -1), new Vector2(2, -1),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut5
        new Vector2[] { new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, -1), new Vector2(0, -1),
                    new Vector2(2, 0), new Vector2(3, 0), new Vector2(3, -1), new Vector2(2, -1),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut6
        new Vector2[] { new Vector2(1, 0), new Vector2(3, 0), new Vector2(3, -1), new Vector2(1, -1),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut7
        new Vector2[] { new Vector2(0, 0), new Vector2(3, 0), new Vector2(3, -1), new Vector2(0, -1),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut8
        new Vector2[] { new Vector2(0, -1), new Vector2(1, -1), new Vector2(1, -2), new Vector2(0, -2),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut9
        new Vector2[] { new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, -2), new Vector2(0, -2),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut10
        new Vector2[] { new Vector2(1, 0), new Vector2(2, 0), new Vector2(2, -1), new Vector2(1, -1),
                    new Vector2(0, -1), new Vector2(1, -1), new Vector2(1, -2), new Vector2(0, -2),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut11
        new Vector2[] { new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, -2), new Vector2(0, -2),
                    new Vector2(1, 0), new Vector2(2, 0), new Vector2(2, -1), new Vector2(1, -1),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut12
        new Vector2[] { new Vector2(2, 0), new Vector2(3, 0), new Vector2(3, -1), new Vector2(2, -1),
                    new Vector2(0, -1), new Vector2(1, -1), new Vector2(1, -2), new Vector2(0, -2),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut13
        new Vector2[] { new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, -2), new Vector2(0, -2),
                    new Vector2(2, 0), new Vector2(3, 0), new Vector2(3, -1), new Vector2(2, -1),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut14
        new Vector2[] { new Vector2(1, 0), new Vector2(3, 0), new Vector2(3, -1), new Vector2(1, -1),
                    new Vector2(0, -1), new Vector2(1, -1), new Vector2(1, -2), new Vector2(0, -2),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut15
        new Vector2[] { new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, -2), new Vector2(0, -2),
                    new Vector2(1, 0), new Vector2(3, 0), new Vector2(3, -1), new Vector2(1, -1),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut16
        new Vector2[] { new Vector2(2, -1), new Vector2(3, -1), new Vector2(3, -2), new Vector2(2, -2),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut17
        new Vector2[] { new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, -1), new Vector2(0, -1),
                    new Vector2(2, -1), new Vector2(3, -1), new Vector2(3, -2), new Vector2(2, -2),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut18
        new Vector2[] { new Vector2(1, 0), new Vector2(2, 0), new Vector2(2, -1), new Vector2(1, -1),
                    new Vector2(2, -1), new Vector2(3, -1), new Vector2(3, -2), new Vector2(2, -2),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut19
        new Vector2[] { new Vector2(0, 0), new Vector2(2, 0), new Vector2(2, -1), new Vector2(0, -1),
                    new Vector2(2, -1), new Vector2(3, -1), new Vector2(3, -2), new Vector2(2, -2),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut20
        new Vector2[] { new Vector2(2, 0), new Vector2(3, 0), new Vector2(3, -2), new Vector2(2, -2),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut21
        new Vector2[] { new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, -1), new Vector2(0, -1),
                    new Vector2(2, 0), new Vector2(3, 0), new Vector2(3, -2), new Vector2(2, -2),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut22
        new Vector2[] { new Vector2(1, 0), new Vector2(3, 0), new Vector2(3, -1), new Vector2(1, -1),
                    new Vector2(2, -1), new Vector2(3, -1), new Vector2(3, -2), new Vector2(2, -2),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut23
        new Vector2[] { new Vector2(0, 0), new Vector2(3, 0), new Vector2(3, -1), new Vector2(0, -1),
                    new Vector2(2, -1), new Vector2(3, -1), new Vector2(3, -2), new Vector2(2, -2),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut24
        new Vector2[] { new Vector2(0, -1), new Vector2(1, -1), new Vector2(1, -2), new Vector2(0, -2),
                    new Vector2(2, -1), new Vector2(3, -1), new Vector2(3, -2), new Vector2(2, -2),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut25
        new Vector2[] { new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, -2), new Vector2(0, -2),
                    new Vector2(2, -1), new Vector2(3, -1), new Vector2(3, -2), new Vector2(2, -2),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut26
        new Vector2[] { new Vector2(1, 0), new Vector2(2, 0), new Vector2(2, -1), new Vector2(1, -1),
                    new Vector2(0, -1), new Vector2(1, -1), new Vector2(1, -2), new Vector2(0, -2),
                    new Vector2(2, -1), new Vector2(3, -1), new Vector2(3, -2), new Vector2(2, -2),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut27
        new Vector2[] { new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, -2), new Vector2(0, -2),
                    new Vector2(1, 0), new Vector2(2, 0), new Vector2(2, -1), new Vector2(1, -1),
                    new Vector2(2, -1), new Vector2(3, -1), new Vector2(3, -2), new Vector2(2, -2),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut28
        new Vector2[] { new Vector2(2, 0), new Vector2(3, 0), new Vector2(3, -2), new Vector2(2, -2),
                    new Vector2(0, -1), new Vector2(1, -1), new Vector2(1, -2), new Vector2(0, -2),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut29
        new Vector2[] { new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, -2), new Vector2(0, -2),
                    new Vector2(2, 0), new Vector2(3, 0), new Vector2(3, -2), new Vector2(2, -2),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut30
        new Vector2[] { new Vector2(1, 0), new Vector2(3, 0), new Vector2(3, -1), new Vector2(1, -1),
                    new Vector2(0, -1), new Vector2(1, -1), new Vector2(1, -2), new Vector2(0, -2),
                    new Vector2(2, -1), new Vector2(3, -1), new Vector2(3, -2), new Vector2(2, -2),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut31
        new Vector2[] { new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, -2), new Vector2(0, -2),
                    new Vector2(1, 0), new Vector2(3, 0), new Vector2(3, -1), new Vector2(1, -1),
                    new Vector2(2, -1), new Vector2(3, -1), new Vector2(3, -2), new Vector2(2, -2),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut32
        new Vector2[] { new Vector2(0, -2), new Vector2(1, -2), new Vector2(1, -3), new Vector2(0, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut33
        new Vector2[] { new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, -1), new Vector2(0, -1),
                    new Vector2(0, -2), new Vector2(1, -2), new Vector2(1, -3), new Vector2(0, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut34
        new Vector2[] { new Vector2(1, 0), new Vector2(2, 0), new Vector2(2, -1), new Vector2(1, -1),
                    new Vector2(0, -2), new Vector2(1, -2), new Vector2(1, -3), new Vector2(0, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut35
        new Vector2[] { new Vector2(0, 0), new Vector2(2, 0), new Vector2(2, -1), new Vector2(0, -1),
                    new Vector2(0, -2), new Vector2(1, -2), new Vector2(1, -3), new Vector2(0, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut36
        new Vector2[] { new Vector2(2, 0), new Vector2(3, 0), new Vector2(3, -1), new Vector2(2, -1),
                    new Vector2(0, -2), new Vector2(1, -2), new Vector2(1, -3), new Vector2(0, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut37
        new Vector2[] {new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, -1), new Vector2(0, -1),
                    new Vector2(2, 0), new Vector2(3, 0), new Vector2(3, -1), new Vector2(2, -1),
                    new Vector2(0, -2), new Vector2(1, -2), new Vector2(1, -3), new Vector2(0, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut38
        new Vector2[] {new Vector2(1, 0), new Vector2(3, 0), new Vector2(3, -1), new Vector2(1, -1),
                    new Vector2(0, -2), new Vector2(1, -2), new Vector2(1, -3), new Vector2(0, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut39
        new Vector2[] {new Vector2(0, 0), new Vector2(3, 0), new Vector2(3, -1), new Vector2(0, -1),
                    new Vector2(0, -2), new Vector2(1, -2), new Vector2(1, -3), new Vector2(0, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut40
        new Vector2[] {new Vector2(0, -1), new Vector2(1, -1), new Vector2(1, -3), new Vector2(0, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut41
        new Vector2[] {new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, -3), new Vector2(0, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut42
        new Vector2[] {new Vector2(1, 0), new Vector2(2, 0), new Vector2(2, -1), new Vector2(1, -1),
                    new Vector2(0, -1), new Vector2(1, -1), new Vector2(1, -3), new Vector2(0, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut43
        new Vector2[] {new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, -3), new Vector2(0, -3),
                    new Vector2(1, 0), new Vector2(2, 0), new Vector2(2, -1), new Vector2(1, -1),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut44
        new Vector2[] {new Vector2(2, 0), new Vector2(3, 0), new Vector2(3, -1), new Vector2(2, -1),
                    new Vector2(0, -1), new Vector2(1, -1), new Vector2(1, -3), new Vector2(0, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut45
        new Vector2[] {new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, -3), new Vector2(0, -3),
                    new Vector2(2, 0), new Vector2(3, 0), new Vector2(3, -1), new Vector2(2, -1),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut46
        new Vector2[] {new Vector2(1, 0), new Vector2(3, 0), new Vector2(3, -1), new Vector2(1, -1),
                    new Vector2(0, -1), new Vector2(1, -1), new Vector2(1, -3), new Vector2(0, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut47
        new Vector2[] {new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, -3), new Vector2(0, -3),
                    new Vector2(1, 0), new Vector2(3, 0), new Vector2(3, -1), new Vector2(1, -1),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut48
        new Vector2[] {new Vector2(2, -1), new Vector2(3, -1), new Vector2(3, -2), new Vector2(2, -2),
                    new Vector2(0, -2), new Vector2(1, -2), new Vector2(1, -3), new Vector2(0, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut49
        new Vector2[] {new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, -1), new Vector2(0, -1),
                    new Vector2(2, -1), new Vector2(3, -1), new Vector2(3, -2), new Vector2(2, -2),
                    new Vector2(0, -2), new Vector2(1, -2), new Vector2(1, -3), new Vector2(0, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut50
        new Vector2[] {new Vector2(1, 0), new Vector2(2, 0), new Vector2(2, -1), new Vector2(1, -1),
                    new Vector2(2, -1), new Vector2(3, -1), new Vector2(3, -2), new Vector2(2, -2),
                    new Vector2(0, -2), new Vector2(1, -2), new Vector2(1, -3), new Vector2(0, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut51
        new Vector2[] {new Vector2(0, 0), new Vector2(2, 0), new Vector2(2, -1), new Vector2(0, -1),
                    new Vector2(2, -1), new Vector2(3, -1), new Vector2(3, -2), new Vector2(2, -2),
                    new Vector2(0, -2), new Vector2(1, -2), new Vector2(1, -3), new Vector2(0, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut52
        new Vector2[] {new Vector2(2, 0), new Vector2(3, 0), new Vector2(3, -2), new Vector2(2, -2),
                    new Vector2(0, -2), new Vector2(1, -2), new Vector2(1, -3), new Vector2(0, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut53
        new Vector2[] {new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, -1), new Vector2(0, -1),
                    new Vector2(2, 0), new Vector2(3, 0), new Vector2(3, -2), new Vector2(2, -2),
                    new Vector2(0, -2), new Vector2(1, -2), new Vector2(1, -3), new Vector2(0, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut54
        new Vector2[] {new Vector2(1, 0), new Vector2(3, 0), new Vector2(3, -1), new Vector2(1, -1),
                    new Vector2(2, -1), new Vector2(3, -1), new Vector2(3, -2), new Vector2(2, -2),
                    new Vector2(0, -2), new Vector2(1, -2), new Vector2(1, -3), new Vector2(0, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut55
        new Vector2[] {new Vector2(0, 0), new Vector2(3, 0), new Vector2(3, -1), new Vector2(0, -1),
                    new Vector2(2, -1), new Vector2(3, -1), new Vector2(3, -2), new Vector2(2, -2),
                    new Vector2(0, -2), new Vector2(1, -2), new Vector2(1, -3), new Vector2(0, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut56
        new Vector2[] {new Vector2(0, -1), new Vector2(1, -1), new Vector2(1, -3), new Vector2(0, -3),
                    new Vector2(2, -1), new Vector2(3, -1), new Vector2(3, -2), new Vector2(2, -2),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut57
        new Vector2[] {new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, -3), new Vector2(0, -3),
                    new Vector2(2, -1), new Vector2(3, -1), new Vector2(3, -2), new Vector2(2, -2),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut58
        new Vector2[] {new Vector2(1, 0), new Vector2(2, 0), new Vector2(2, -1), new Vector2(1, -1),
                    new Vector2(0, -1), new Vector2(1, -1), new Vector2(1, -3), new Vector2(0, -3),
                    new Vector2(2, -1), new Vector2(3, -1), new Vector2(3, -2), new Vector2(2, -2),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut59
        new Vector2[] {new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, -3), new Vector2(0, -3),
                    new Vector2(1, 0), new Vector2(2, 0), new Vector2(2, -1), new Vector2(1, -1),
                    new Vector2(2, -1), new Vector2(3, -1), new Vector2(3, -2), new Vector2(2, -2),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut60
        new Vector2[] {new Vector2(2, 0), new Vector2(3, 0), new Vector2(3, -2), new Vector2(2, -2),
                    new Vector2(0, -1), new Vector2(1, -1), new Vector2(1, -3), new Vector2(0, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut61
        new Vector2[] {new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, -3), new Vector2(0, -3),
                    new Vector2(2, 0), new Vector2(3, 0), new Vector2(3, -2), new Vector2(2, -2),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut62
        new Vector2[] {new Vector2(1, 0), new Vector2(3, 0), new Vector2(3, -1), new Vector2(1, -1),
                    new Vector2(0, -1), new Vector2(1, -1), new Vector2(1, -3), new Vector2(0, -3),
                    new Vector2(2, -1), new Vector2(3, -1), new Vector2(3, -2), new Vector2(2, -2),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut63
        new Vector2[] {new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, -3), new Vector2(0, -3),
                    new Vector2(1, 0), new Vector2(3, 0), new Vector2(3, -1), new Vector2(1, -1),
                    new Vector2(2, -1), new Vector2(3, -1), new Vector2(3, -2), new Vector2(2, -2),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut64
        new Vector2[] {new Vector2(1, -2), new Vector2(2, -2), new Vector2(2, -3), new Vector2(1, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut65
        new Vector2[] { new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, -1), new Vector2(0, -1),
                    new Vector2(1, -2), new Vector2(2, -2), new Vector2(2, -3), new Vector2(1, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut66
        new Vector2[] { new Vector2(1, 0), new Vector2(2, 0), new Vector2(2, -1), new Vector2(1, -1),
                    new Vector2(1, -2), new Vector2(2, -2), new Vector2(2, -3), new Vector2(1, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut67
        new Vector2[] { new Vector2(0, 0), new Vector2(2, 0), new Vector2(2, -1), new Vector2(0, -1),
                    new Vector2(1, -2), new Vector2(2, -2), new Vector2(2, -3), new Vector2(1, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut68
        new Vector2[] { new Vector2(2, 0), new Vector2(3, 0), new Vector2(3, -1), new Vector2(2, -1),
                    new Vector2(1, -2), new Vector2(2, -2), new Vector2(2, -3), new Vector2(1, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut69
        new Vector2[] { new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, -1), new Vector2(0, -1),
                    new Vector2(2, 0), new Vector2(3, 0), new Vector2(3, -1), new Vector2(2, -1),
                    new Vector2(1, -2), new Vector2(2, -2), new Vector2(2, -3), new Vector2(1, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut70
        new Vector2[] { new Vector2(1, 0), new Vector2(3, 0), new Vector2(3, -1), new Vector2(1, -1),
                    new Vector2(1, -2), new Vector2(2, -2), new Vector2(2, -3), new Vector2(1, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut71
        new Vector2[] { new Vector2(0, 0), new Vector2(3, 0), new Vector2(3, -1), new Vector2(0, -1),
                    new Vector2(1, -2), new Vector2(2, -2), new Vector2(2, -3), new Vector2(1, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut72
        new Vector2[] { new Vector2(0, -1), new Vector2(1, -1), new Vector2(1, -2), new Vector2(0, -2),
                    new Vector2(1, -2), new Vector2(2, -2), new Vector2(2, -3), new Vector2(1, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut73
        new Vector2[] { new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, -2), new Vector2(0, -2),
                    new Vector2(1, -2), new Vector2(2, -2), new Vector2(2, -3), new Vector2(1, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut74
        new Vector2[] { new Vector2(1, 0), new Vector2(2, 0), new Vector2(2, -1), new Vector2(1, -1),
                    new Vector2(0, -1), new Vector2(1, -1), new Vector2(1, -2), new Vector2(0, -2),
                    new Vector2(1, -2), new Vector2(2, -2), new Vector2(2, -3), new Vector2(1, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut75
        new Vector2[] { new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, -2), new Vector2(0, -2),
                    new Vector2(1, 0), new Vector2(2, 0), new Vector2(2, -1), new Vector2(1, -1),
                    new Vector2(1, -2), new Vector2(2, -2), new Vector2(2, -3), new Vector2(1, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut76
        new Vector2[] { new Vector2(2, 0), new Vector2(3, 0), new Vector2(3, -1), new Vector2(2, -1),
                    new Vector2(0, -1), new Vector2(1, -1), new Vector2(1, -2), new Vector2(0, -2),
                    new Vector2(1, -2), new Vector2(2, -2), new Vector2(2, -3), new Vector2(1, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut77
        new Vector2[] { new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, -2), new Vector2(0, -2),
                    new Vector2(2, 0), new Vector2(3, 0), new Vector2(3, -1), new Vector2(2, -1),
                    new Vector2(1, -2), new Vector2(2, -2), new Vector2(2, -3), new Vector2(1, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut78
        new Vector2[] { new Vector2(1, 0), new Vector2(3, 0), new Vector2(3, -1), new Vector2(1, -1),
                    new Vector2(0, -1), new Vector2(1, -1), new Vector2(1, -2), new Vector2(0, -2),
                    new Vector2(1, -2), new Vector2(2, -2), new Vector2(2, -3), new Vector2(1, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut79
        new Vector2[] { new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, -2), new Vector2(0, -2),
                    new Vector2(1, 0), new Vector2(3, 0), new Vector2(3, -1), new Vector2(1, -1),
                    new Vector2(1, -2), new Vector2(2, -2), new Vector2(2, -3), new Vector2(1, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut80
        new Vector2[] { new Vector2(2, -1), new Vector2(3, -1), new Vector2(3, -2), new Vector2(2, -2),
                    new Vector2(1, -2), new Vector2(2, -2), new Vector2(2, -3), new Vector2(1, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut81
        new Vector2[] { new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, -1), new Vector2(0, -1),
                    new Vector2(2, -1), new Vector2(3, -1), new Vector2(3, -2), new Vector2(2, -2),
                    new Vector2(1, -2), new Vector2(2, -2), new Vector2(2, -3), new Vector2(1, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut82
        new Vector2[] { new Vector2(1, 0), new Vector2(2, 0), new Vector2(2, -1), new Vector2(1, -1),
                    new Vector2(2, -1), new Vector2(3, -1), new Vector2(3, -2), new Vector2(2, -2),
                    new Vector2(1, -2), new Vector2(2, -2), new Vector2(2, -3), new Vector2(1, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut83
        new Vector2[] { new Vector2(0, 0), new Vector2(2, 0), new Vector2(2, -1), new Vector2(0, -1),
                    new Vector2(2, -1), new Vector2(3, -1), new Vector2(3, -2), new Vector2(2, -2),
                    new Vector2(1, -2), new Vector2(2, -2), new Vector2(2, -3), new Vector2(1, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut84
        new Vector2[] { new Vector2(2, 0), new Vector2(3, 0), new Vector2(3, -2), new Vector2(2, -2),
                    new Vector2(1, -2), new Vector2(2, -2), new Vector2(2, -3), new Vector2(1, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut85
        new Vector2[] { new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, -1), new Vector2(0, -1),
                    new Vector2(2, 0), new Vector2(3, 0), new Vector2(3, -2), new Vector2(2, -2),
                    new Vector2(1, -2), new Vector2(2, -2), new Vector2(2, -3), new Vector2(1, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut86
        new Vector2[] { new Vector2(1, 0), new Vector2(3, 0), new Vector2(3, -1), new Vector2(1, -1),
                    new Vector2(2, -1), new Vector2(3, -1), new Vector2(3, -2), new Vector2(2, -2),
                    new Vector2(1, -2), new Vector2(2, -2), new Vector2(2, -3), new Vector2(1, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut87
        new Vector2[] { new Vector2(0, 0), new Vector2(3, 0), new Vector2(3, -1), new Vector2(0, -1),
                    new Vector2(2, -1), new Vector2(3, -1), new Vector2(3, -2), new Vector2(2, -2),
                    new Vector2(1, -2), new Vector2(2, -2), new Vector2(2, -3), new Vector2(1, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut88
        new Vector2[] { new Vector2(0, -1), new Vector2(1, -1), new Vector2(1, -2), new Vector2(0, -2),
                    new Vector2(2, -1), new Vector2(3, -1), new Vector2(3, -2), new Vector2(2, -2),
                    new Vector2(1, -2), new Vector2(2, -2), new Vector2(2, -3), new Vector2(1, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut89
        new Vector2[] { new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, -2), new Vector2(0, -2),
                    new Vector2(2, -1), new Vector2(3, -1), new Vector2(3, -2), new Vector2(2, -2),
                    new Vector2(1, -2), new Vector2(2, -2), new Vector2(2, -3), new Vector2(1, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut90
        new Vector2[] { new Vector2(1, 0), new Vector2(2, 0), new Vector2(2, -1), new Vector2(1, -1),
                    new Vector2(0, -1), new Vector2(1, -1), new Vector2(1, -2), new Vector2(0, -2),
                    new Vector2(2, -1), new Vector2(3, -1), new Vector2(3, -2), new Vector2(2, -2),
                    new Vector2(1, -2), new Vector2(2, -2), new Vector2(2, -3), new Vector2(1, -3) },
        // lut91
        new Vector2[] { new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, -2), new Vector2(0, -2),
                    new Vector2(1, 0), new Vector2(2, 0), new Vector2(2, -1), new Vector2(1, -1),
                    new Vector2(2, -1), new Vector2(3, -1), new Vector2(3, -2), new Vector2(2, -2),
                    new Vector2(1, -2), new Vector2(2, -2), new Vector2(2, -3), new Vector2(1, -3) },
        // lut92
        new Vector2[] { new Vector2(2, 0), new Vector2(3, 0), new Vector2(3, -2), new Vector2(2, -2),
                    new Vector2(0, -1), new Vector2(1, -1), new Vector2(1, -2), new Vector2(0, -2),
                    new Vector2(1, -2), new Vector2(2, -2), new Vector2(2, -3), new Vector2(1, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut93
        new Vector2[] { new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, -2), new Vector2(0, -2),
                    new Vector2(2, 0), new Vector2(3, 0), new Vector2(3, -2), new Vector2(2, -2),
                    new Vector2(1, -2), new Vector2(2, -2), new Vector2(2, -3), new Vector2(1, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut94
        new Vector2[] { new Vector2(1, 0), new Vector2(3, 0), new Vector2(3, -1), new Vector2(1, -1),
                    new Vector2(2, -1), new Vector2(3, -1), new Vector2(3, -2), new Vector2(2, -2),
                    new Vector2(0, -1), new Vector2(1, -1), new Vector2(1, -2), new Vector2(0, -2),
                    new Vector2(1, -2), new Vector2(2, -2), new Vector2(2, -3), new Vector2(1, -3) },
        // lut95
        new Vector2[] { new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, -2), new Vector2(0, -2),
                    new Vector2(1, 0), new Vector2(3, 0), new Vector2(3, -1), new Vector2(1, -1),
                    new Vector2(2, -1), new Vector2(3, -1), new Vector2(3, -2), new Vector2(2, -2),
                    new Vector2(1, -2), new Vector2(2, -2), new Vector2(2, -3), new Vector2(1, -3) },
        // lut96
        new Vector2[] { new Vector2(0, -2), new Vector2(2, -2), new Vector2(2, -3), new Vector2(0, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut97
        new Vector2[] { new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, -1), new Vector2(0, -1),
                    new Vector2(0, -2), new Vector2(2, -2), new Vector2(2, -3), new Vector2(0, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut98
        new Vector2[] { new Vector2(1, 0), new Vector2(2, 0), new Vector2(2, -1), new Vector2(1, -1),
                    new Vector2(0, -2), new Vector2(2, -2), new Vector2(2, -3), new Vector2(0, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut99
        new Vector2[] { new Vector2(0, 0), new Vector2(2, 0), new Vector2(2, -1), new Vector2(0, -1),
                    new Vector2(0, -2), new Vector2(2, -2), new Vector2(2, -3), new Vector2(0, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut100
        new Vector2[] { new Vector2(2, 0), new Vector2(3, 0), new Vector2(3, -1), new Vector2(2, -1),
                    new Vector2(0, -2), new Vector2(2, -2), new Vector2(2, -3), new Vector2(0, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut101
        new Vector2[] {new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, -1), new Vector2(0, -1),
                    new Vector2(2, 0), new Vector2(3, 0), new Vector2(3, -1), new Vector2(2, -1),
                    new Vector2(0, -2), new Vector2(2, -2), new Vector2(2, -3), new Vector2(0, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut102
        new Vector2[] {new Vector2(1, 0), new Vector2(3, 0), new Vector2(3, -1), new Vector2(1, -1),
                    new Vector2(0, -2), new Vector2(2, -2), new Vector2(2, -3), new Vector2(0, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut103
        new Vector2[] {new Vector2(0, 0), new Vector2(3, 0), new Vector2(3, -1), new Vector2(0, -1),
                    new Vector2(0, -2), new Vector2(2, -2), new Vector2(2, -3), new Vector2(0, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut104
        new Vector2[] {new Vector2(0, -1), new Vector2(1, -1), new Vector2(1, -2), new Vector2(0, -2),
                    new Vector2(0, -2), new Vector2(2, -2), new Vector2(2, -3), new Vector2(0, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut105
        new Vector2[] {new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, -2), new Vector2(0, -2),
                    new Vector2(0, -2), new Vector2(2, -2), new Vector2(2, -3), new Vector2(0, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut106
        new Vector2[] {new Vector2(1, 0), new Vector2(2, 0), new Vector2(2, -1), new Vector2(1, -1),
                    new Vector2(0, -1), new Vector2(1, -1), new Vector2(1, -2), new Vector2(0, -2),
                    new Vector2(0, -2), new Vector2(2, -2), new Vector2(2, -3), new Vector2(0, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut107
        new Vector2[] {new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, -2), new Vector2(0, -2),
                    new Vector2(1, 0), new Vector2(2, 0), new Vector2(2, -1), new Vector2(1, -1),
                    new Vector2(0, -2), new Vector2(2, -2), new Vector2(2, -3), new Vector2(0, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut108
        new Vector2[] {new Vector2(2, 0), new Vector2(3, 0), new Vector2(3, -1), new Vector2(2, -1),
                    new Vector2(0, -1), new Vector2(1, -1), new Vector2(1, -2), new Vector2(0, -2),
                    new Vector2(0, -2), new Vector2(2, -2), new Vector2(2, -3), new Vector2(0, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut109
        new Vector2[] {new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, -2), new Vector2(0, -2),
                    new Vector2(2, 0), new Vector2(3, 0), new Vector2(3, -1), new Vector2(2, -1),
                    new Vector2(0, -2), new Vector2(2, -2), new Vector2(2, -3), new Vector2(0, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut110
        new Vector2[] {new Vector2(1, 0), new Vector2(3, 0), new Vector2(3, -1), new Vector2(1, -1),
                    new Vector2(0, -1), new Vector2(1, -1), new Vector2(1, -2), new Vector2(0, -2),
                    new Vector2(0, -2), new Vector2(2, -2), new Vector2(2, -3), new Vector2(0, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut111
        new Vector2[] {new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, -2), new Vector2(0, -2),
                    new Vector2(1, 0), new Vector2(3, 0), new Vector2(3, -1), new Vector2(1, -1),
                    new Vector2(0, -2), new Vector2(2, -2), new Vector2(2, -3), new Vector2(0, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut112
        new Vector2[] {new Vector2(2, -1), new Vector2(3, -1), new Vector2(3, -2), new Vector2(2, -2),
                    new Vector2(0, -2), new Vector2(2, -2), new Vector2(2, -3), new Vector2(0, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut113
        new Vector2[] {new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, -1), new Vector2(0, -1),
                    new Vector2(2, -1), new Vector2(3, -1), new Vector2(3, -2), new Vector2(2, -2),
                    new Vector2(0, -2), new Vector2(2, -2), new Vector2(2, -3), new Vector2(0, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut114
        new Vector2[] {new Vector2(1, 0), new Vector2(2, 0), new Vector2(2, -1), new Vector2(1, -1),
                    new Vector2(2, -1), new Vector2(3, -1), new Vector2(3, -2), new Vector2(2, -2),
                    new Vector2(0, -2), new Vector2(2, -2), new Vector2(2, -3), new Vector2(0, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut115
        new Vector2[] {new Vector2(0, 0), new Vector2(2, 0), new Vector2(2, -1), new Vector2(0, -1),
                    new Vector2(2, -1), new Vector2(3, -1), new Vector2(3, -2), new Vector2(2, -2),
                    new Vector2(0, -2), new Vector2(2, -2), new Vector2(2, -3), new Vector2(0, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut116
        new Vector2[] {new Vector2(2, 0), new Vector2(3, 0), new Vector2(3, -2), new Vector2(2, -2),
                    new Vector2(0, -2), new Vector2(2, -2), new Vector2(2, -3), new Vector2(0, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut117
        new Vector2[] {new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, -1), new Vector2(0, -1),
                    new Vector2(2, 0), new Vector2(3, 0), new Vector2(3, -2), new Vector2(2, -2),
                    new Vector2(0, -2), new Vector2(2, -2), new Vector2(2, -3), new Vector2(0, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut118
        new Vector2[] {new Vector2(1, 0), new Vector2(3, 0), new Vector2(3, -1), new Vector2(1, -1),
                    new Vector2(2, -1), new Vector2(3, -1), new Vector2(3, -2), new Vector2(2, -2),
                    new Vector2(0, -2), new Vector2(2, -2), new Vector2(2, -3), new Vector2(0, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut119
        new Vector2[] {new Vector2(0, 0), new Vector2(3, 0), new Vector2(3, -1), new Vector2(0, -1),
                    new Vector2(2, -1), new Vector2(3, -1), new Vector2(3, -2), new Vector2(2, -2),
                    new Vector2(0, -2), new Vector2(2, -2), new Vector2(2, -3), new Vector2(0, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut120
        new Vector2[] {new Vector2(0, -1), new Vector2(1, -1), new Vector2(1, -2), new Vector2(0, -2),
                    new Vector2(2, -1), new Vector2(3, -1), new Vector2(3, -2), new Vector2(2, -2),
                    new Vector2(0, -2), new Vector2(2, -2), new Vector2(2, -3), new Vector2(0, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut121
        new Vector2[] {new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, -2), new Vector2(0, -2),
                    new Vector2(2, -1), new Vector2(3, -1), new Vector2(3, -2), new Vector2(2, -2),
                    new Vector2(0, -2), new Vector2(2, -2), new Vector2(2, -3), new Vector2(0, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut122
        new Vector2[] {new Vector2(1, 0), new Vector2(2, 0), new Vector2(2, -1), new Vector2(1, -1),
                    new Vector2(0, -1), new Vector2(1, -1), new Vector2(1, -2), new Vector2(0, -2),
                    new Vector2(2, -1), new Vector2(3, -1), new Vector2(3, -2), new Vector2(2, -2),
                    new Vector2(0, -2), new Vector2(2, -2), new Vector2(2, -3), new Vector2(0, -3) },
        // lut123
        new Vector2[] {new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, -2), new Vector2(0, -2),
                    new Vector2(1, 0), new Vector2(2, 0), new Vector2(2, -1), new Vector2(1, -1),
                    new Vector2(2, -1), new Vector2(3, -1), new Vector2(3, -2), new Vector2(2, -2),
                    new Vector2(0, -2), new Vector2(2, -2), new Vector2(2, -3), new Vector2(0, -3) },
        // lut124
        new Vector2[] {new Vector2(2, 0), new Vector2(3, 0), new Vector2(3, -2), new Vector2(2, -2),
                    new Vector2(0, -1), new Vector2(1, -1), new Vector2(1, -2), new Vector2(0, -2),
                    new Vector2(0, -2), new Vector2(2, -2), new Vector2(2, -3), new Vector2(0, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut125
        new Vector2[] {new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, -2), new Vector2(0, -2),
                    new Vector2(2, 0), new Vector2(3, 0), new Vector2(3, -2), new Vector2(2, -2),
                    new Vector2(0, -2), new Vector2(2, -2), new Vector2(2, -3), new Vector2(0, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut126
        new Vector2[] {new Vector2(1, 0), new Vector2(3, 0), new Vector2(3, -1), new Vector2(1, -1),
                    new Vector2(0, -1), new Vector2(1, -1), new Vector2(1, -2), new Vector2(0, -2),
                    new Vector2(2, -1), new Vector2(3, -1), new Vector2(3, -2), new Vector2(2, -2),
                    new Vector2(0, -2), new Vector2(2, -2), new Vector2(2, -3), new Vector2(0, -3) },
        // lut127
        new Vector2[] {new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, -2), new Vector2(0, -2),
                    new Vector2(1, 0), new Vector2(3, 0), new Vector2(3, -1), new Vector2(1, -1),
                    new Vector2(2, -1), new Vector2(3, -1), new Vector2(3, -2), new Vector2(2, -2),
                    new Vector2(0, -2), new Vector2(2, -2), new Vector2(2, -3), new Vector2(0, -3) },
        // lut128
        new Vector2[] {new Vector2(2, -2), new Vector2(3, -2), new Vector2(3, -3), new Vector2(2, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut129
        new Vector2[] { new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, -1), new Vector2(0, -1),
                    new Vector2(2, -2), new Vector2(3, -2), new Vector2(3, -3), new Vector2(2, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut130
        new Vector2[] { new Vector2(1, 0), new Vector2(2, 0), new Vector2(2, -1), new Vector2(1, -1),
                    new Vector2(2, -2), new Vector2(3, -2), new Vector2(3, -3), new Vector2(2, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut131
        new Vector2[] { new Vector2(0, 0), new Vector2(2, 0), new Vector2(2, -1), new Vector2(0, -1),
                    new Vector2(2, -2), new Vector2(3, -2), new Vector2(3, -3), new Vector2(2, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut132
        new Vector2[] { new Vector2(2, 0), new Vector2(3, 0), new Vector2(3, -1), new Vector2(2, -1),
                    new Vector2(2, -2), new Vector2(3, -2), new Vector2(3, -3), new Vector2(2, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut133
        new Vector2[] { new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, -1), new Vector2(0, -1),
                    new Vector2(2, 0), new Vector2(3, 0), new Vector2(3, -1), new Vector2(2, -1),
                    new Vector2(2, -2), new Vector2(3, -2), new Vector2(3, -3), new Vector2(2, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut134
        new Vector2[] { new Vector2(1, 0), new Vector2(3, 0), new Vector2(3, -1), new Vector2(1, -1),
                    new Vector2(2, -2), new Vector2(3, -2), new Vector2(3, -3), new Vector2(2, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut135
        new Vector2[] { new Vector2(0, 0), new Vector2(3, 0), new Vector2(3, -1), new Vector2(0, -1),
                    new Vector2(2, -2), new Vector2(3, -2), new Vector2(3, -3), new Vector2(2, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut136
        new Vector2[] { new Vector2(0, -1), new Vector2(1, -1), new Vector2(1, -2), new Vector2(0, -2),
                    new Vector2(2, -2), new Vector2(3, -2), new Vector2(3, -3), new Vector2(2, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut137
        new Vector2[] { new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, -2), new Vector2(0, -2),
                    new Vector2(2, -2), new Vector2(3, -2), new Vector2(3, -3), new Vector2(2, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut138
        new Vector2[] { new Vector2(1, 0), new Vector2(2, 0), new Vector2(2, -1), new Vector2(1, -1),
                    new Vector2(0, -1), new Vector2(1, -1), new Vector2(1, -2), new Vector2(0, -2),
                    new Vector2(2, -2), new Vector2(3, -2), new Vector2(3, -3), new Vector2(2, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut139
        new Vector2[] { new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, -2), new Vector2(0, -2),
                    new Vector2(1, 0), new Vector2(2, 0), new Vector2(2, -1), new Vector2(1, -1),
                    new Vector2(2, -2), new Vector2(3, -2), new Vector2(3, -3), new Vector2(2, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut140
        new Vector2[] { new Vector2(2, 0), new Vector2(3, 0), new Vector2(3, -1), new Vector2(2, -1),
                    new Vector2(0, -1), new Vector2(1, -1), new Vector2(1, -2), new Vector2(0, -2),
                    new Vector2(2, -2), new Vector2(3, -2), new Vector2(3, -3), new Vector2(2, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut141
        new Vector2[] { new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, -2), new Vector2(0, -2),
                    new Vector2(2, 0), new Vector2(3, 0), new Vector2(3, -1), new Vector2(2, -1),
                    new Vector2(2, -2), new Vector2(3, -2), new Vector2(3, -3), new Vector2(2, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut142
        new Vector2[] { new Vector2(1, 0), new Vector2(3, 0), new Vector2(3, -1), new Vector2(1, -1),
                    new Vector2(0, -1), new Vector2(1, -1), new Vector2(1, -2), new Vector2(0, -2),
                    new Vector2(2, -2), new Vector2(3, -2), new Vector2(3, -3), new Vector2(2, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut143
        new Vector2[] { new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, -2), new Vector2(0, -2),
                    new Vector2(1, 0), new Vector2(3, 0), new Vector2(3, -1), new Vector2(1, -1),
                    new Vector2(2, -2), new Vector2(3, -2), new Vector2(3, -3), new Vector2(2, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut144
        new Vector2[] { new Vector2(2, -1), new Vector2(3, -1), new Vector2(3, -3), new Vector2(2, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut145
        new Vector2[] { new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, -1), new Vector2(0, -1),
                    new Vector2(2, -1), new Vector2(3, -1), new Vector2(3, -3), new Vector2(2, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut146
        new Vector2[] { new Vector2(1, 0), new Vector2(2, 0), new Vector2(2, -1), new Vector2(1, -1),
                    new Vector2(2, -1), new Vector2(3, -1), new Vector2(3, -3), new Vector2(2, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut147
        new Vector2[] { new Vector2(0, 0), new Vector2(2, 0), new Vector2(2, -1), new Vector2(0, -1),
                    new Vector2(2, -1), new Vector2(3, -1), new Vector2(3, -3), new Vector2(2, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut148
        new Vector2[] { new Vector2(2, 0), new Vector2(3, 0), new Vector2(3, -3), new Vector2(2, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut149
        new Vector2[] { new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, -1), new Vector2(0, -1),
                    new Vector2(2, 0), new Vector2(3, 0), new Vector2(3, -3), new Vector2(2, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut150
        new Vector2[] { new Vector2(1, 0), new Vector2(3, 0), new Vector2(3, -1), new Vector2(1, -1),
                    new Vector2(2, -1), new Vector2(3, -1), new Vector2(3, -3), new Vector2(2, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut151
        new Vector2[] { new Vector2(0, 0), new Vector2(3, 0), new Vector2(3, -1), new Vector2(0, -1),
                    new Vector2(2, -1), new Vector2(3, -1), new Vector2(3, -3), new Vector2(2, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut152
        new Vector2[] { new Vector2(0, -1), new Vector2(1, -1), new Vector2(1, -2), new Vector2(0, -2),
                    new Vector2(2, -1), new Vector2(3, -1), new Vector2(3, -3), new Vector2(2, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut153
        new Vector2[] { new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, -2), new Vector2(0, -2),
                    new Vector2(2, -1), new Vector2(3, -1), new Vector2(3, -3), new Vector2(2, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut154
        new Vector2[] { new Vector2(1, 0), new Vector2(2, 0), new Vector2(2, -1), new Vector2(1, -1),
                    new Vector2(0, -1), new Vector2(1, -1), new Vector2(1, -2), new Vector2(0, -2),
                    new Vector2(2, -1), new Vector2(3, -1), new Vector2(3, -3), new Vector2(2, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut155
        new Vector2[] { new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, -2), new Vector2(0, -2),
                    new Vector2(1, 0), new Vector2(2, 0), new Vector2(2, -1), new Vector2(1, -1),
                    new Vector2(2, -1), new Vector2(3, -1), new Vector2(3, -3), new Vector2(2, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut156
        new Vector2[] { new Vector2(2, 0), new Vector2(3, 0), new Vector2(3, -3), new Vector2(2, -3),
                    new Vector2(0, -1), new Vector2(1, -1), new Vector2(1, -2), new Vector2(0, -2),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut157
        new Vector2[] { new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, -2), new Vector2(0, -2),
                    new Vector2(2, 0), new Vector2(3, 0), new Vector2(3, -3), new Vector2(2, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut158
        new Vector2[] {new Vector2(1, 0), new Vector2(3, 0), new Vector2(3, -1), new Vector2(1, -1),
                    new Vector2(0, -1), new Vector2(1, -1), new Vector2(1, -2), new Vector2(0, -2),
                    new Vector2(2, -1), new Vector2(3, -1), new Vector2(3, -3), new Vector2(2, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut159
        new Vector2[] { new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, -2), new Vector2(0, -2),
                    new Vector2(1, 0), new Vector2(3, 0), new Vector2(3, -1), new Vector2(1, -1),
                    new Vector2(2, -1), new Vector2(3, -1), new Vector2(3, -3), new Vector2(2, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut160
        new Vector2[] { new Vector2(0, -2), new Vector2(1, -2), new Vector2(1, -3), new Vector2(0, -3),
                    new Vector2(2, -2), new Vector2(3, -2), new Vector2(3, -3), new Vector2(2, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut161
        new Vector2[] { new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, -1), new Vector2(0, -1),
                    new Vector2(0, -2), new Vector2(1, -2), new Vector2(1, -3), new Vector2(0, -3),
                    new Vector2(2, -2), new Vector2(3, -2), new Vector2(3, -3), new Vector2(2, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut162
        new Vector2[] { new Vector2(1, 0), new Vector2(2, 0), new Vector2(2, -1), new Vector2(1, -1),
                    new Vector2(0, -2), new Vector2(1, -2), new Vector2(1, -3), new Vector2(0, -3),
                    new Vector2(2, -2), new Vector2(3, -2), new Vector2(3, -3), new Vector2(2, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut163
        new Vector2[] { new Vector2(0, 0), new Vector2(2, 0), new Vector2(2, -1), new Vector2(0, -1),
                    new Vector2(0, -2), new Vector2(1, -2), new Vector2(1, -3), new Vector2(0, -3),
                    new Vector2(2, -2), new Vector2(3, -2), new Vector2(3, -3), new Vector2(2, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut164
        new Vector2[] { new Vector2(2, 0), new Vector2(3, 0), new Vector2(3, -1), new Vector2(2, -1),
                    new Vector2(0, -2), new Vector2(1, -2), new Vector2(1, -3), new Vector2(0, -3),
                    new Vector2(2, -2), new Vector2(3, -2), new Vector2(3, -3), new Vector2(2, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut165
        new Vector2[] {new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, -1), new Vector2(0, -1),
                    new Vector2(2, 0), new Vector2(3, 0), new Vector2(3, -1), new Vector2(2, -1),
                    new Vector2(0, -2), new Vector2(1, -2), new Vector2(1, -3), new Vector2(0, -3),
                    new Vector2(2, -2), new Vector2(3, -2), new Vector2(3, -3), new Vector2(2, -3) },
        // lut166
        new Vector2[] {new Vector2(1, 0), new Vector2(3, 0), new Vector2(3, -1), new Vector2(1, -1),
                    new Vector2(0, -2), new Vector2(1, -2), new Vector2(1, -3), new Vector2(0, -3),
                    new Vector2(2, -2), new Vector2(3, -2), new Vector2(3, -3), new Vector2(2, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut167
        new Vector2[] {new Vector2(0, 0), new Vector2(3, 0), new Vector2(3, -1), new Vector2(0, -1),
                    new Vector2(0, -2), new Vector2(1, -2), new Vector2(1, -3), new Vector2(0, -3),
                    new Vector2(2, -2), new Vector2(3, -2), new Vector2(3, -3), new Vector2(2, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut168
        new Vector2[] {new Vector2(0, -1), new Vector2(1, -1), new Vector2(1, -3), new Vector2(0, -3),
                    new Vector2(2, -2), new Vector2(3, -2), new Vector2(3, -3), new Vector2(2, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut169
        new Vector2[] {new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, -3), new Vector2(0, -3),
                    new Vector2(2, -2), new Vector2(3, -2), new Vector2(3, -3), new Vector2(2, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut170
        new Vector2[] {new Vector2(1, 0), new Vector2(2, 0), new Vector2(2, -1), new Vector2(1, -1),
                    new Vector2(0, -1), new Vector2(1, -1), new Vector2(1, -3), new Vector2(0, -3),
                    new Vector2(2, -2), new Vector2(3, -2), new Vector2(3, -3), new Vector2(2, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut171
        new Vector2[] {new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, -3), new Vector2(0, -3),
                    new Vector2(1, 0), new Vector2(2, 0), new Vector2(2, -1), new Vector2(1, -1),
                    new Vector2(2, -2), new Vector2(3, -2), new Vector2(3, -3), new Vector2(2, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut172
        new Vector2[] {new Vector2(2, 0), new Vector2(3, 0), new Vector2(3, -1), new Vector2(2, -1),
                    new Vector2(0, -1), new Vector2(1, -1), new Vector2(1, -3), new Vector2(0, -3),
                    new Vector2(2, -2), new Vector2(3, -2), new Vector2(3, -3), new Vector2(2, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut173
        new Vector2[] {new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, -3), new Vector2(0, -3),
                    new Vector2(2, 0), new Vector2(3, 0), new Vector2(3, -1), new Vector2(2, -1),
                    new Vector2(2, -2), new Vector2(3, -2), new Vector2(3, -3), new Vector2(2, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut174
        new Vector2[] {new Vector2(1, 0), new Vector2(3, 0), new Vector2(3, -1), new Vector2(1, -1),
                    new Vector2(0, -1), new Vector2(1, -1), new Vector2(1, -3), new Vector2(0, -3),
                    new Vector2(2, -2), new Vector2(3, -2), new Vector2(3, -3), new Vector2(2, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut175
        new Vector2[] {new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, -3), new Vector2(0, -3),
                    new Vector2(1, 0), new Vector2(3, 0), new Vector2(3, -1), new Vector2(1, -1),
                    new Vector2(2, -2), new Vector2(3, -2), new Vector2(3, -3), new Vector2(2, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut176
        new Vector2[] {new Vector2(2, -1), new Vector2(3, -1), new Vector2(3, -3), new Vector2(2, -3),
                    new Vector2(0, -2), new Vector2(1, -2), new Vector2(1, -3), new Vector2(0, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut177
        new Vector2[] {new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, -1), new Vector2(0, -1),
                    new Vector2(2, -1), new Vector2(3, -1), new Vector2(3, -3), new Vector2(2, -3),
                    new Vector2(0, -2), new Vector2(1, -2), new Vector2(1, -3), new Vector2(0, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut178
        new Vector2[] {new Vector2(1, 0), new Vector2(2, 0), new Vector2(2, -1), new Vector2(1, -1),
                    new Vector2(2, -1), new Vector2(3, -1), new Vector2(3, -3), new Vector2(2, -3),
                    new Vector2(0, -2), new Vector2(1, -2), new Vector2(1, -3), new Vector2(0, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut179
        new Vector2[] {new Vector2(0, 0), new Vector2(2, 0), new Vector2(2, -1), new Vector2(0, -1),
                    new Vector2(2, -1), new Vector2(3, -1), new Vector2(3, -3), new Vector2(2, -3),
                    new Vector2(0, -2), new Vector2(1, -2), new Vector2(1, -3), new Vector2(0, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut180
        new Vector2[] {new Vector2(2, 0), new Vector2(3, 0), new Vector2(3, -3), new Vector2(2, -3),
                    new Vector2(0, -2), new Vector2(1, -2), new Vector2(1, -3), new Vector2(0, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut181
        new Vector2[] {new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, -1), new Vector2(0, -1),
                    new Vector2(2, 0), new Vector2(3, 0), new Vector2(3, -3), new Vector2(2, -3),
                    new Vector2(0, -2), new Vector2(1, -2), new Vector2(1, -3), new Vector2(0, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut182
        new Vector2[] {new Vector2(1, 0), new Vector2(3, 0), new Vector2(3, -1), new Vector2(1, -1),
                    new Vector2(2, -1), new Vector2(3, -1), new Vector2(3, -3), new Vector2(2, -3),
                    new Vector2(0, -2), new Vector2(1, -2), new Vector2(1, -3), new Vector2(0, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut183
        new Vector2[] {new Vector2(0, 0), new Vector2(3, 0), new Vector2(3, -1), new Vector2(0, -1),
                    new Vector2(2, -1), new Vector2(3, -1), new Vector2(3, -3), new Vector2(2, -3),
                    new Vector2(0, -2), new Vector2(1, -2), new Vector2(1, -3), new Vector2(0, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut184
        new Vector2[] {new Vector2(0, -1), new Vector2(1, -1), new Vector2(1, -3), new Vector2(0, -3),
                    new Vector2(2, -1), new Vector2(3, -1), new Vector2(3, -3), new Vector2(2, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut185
        new Vector2[] {new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, -3), new Vector2(0, -3),
                    new Vector2(2, -1), new Vector2(3, -1), new Vector2(3, -3), new Vector2(2, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut186
        new Vector2[] {new Vector2(1, 0), new Vector2(2, 0), new Vector2(2, -1), new Vector2(1, -1),
                    new Vector2(0, -1), new Vector2(1, -1), new Vector2(1, -3), new Vector2(0, -3),
                    new Vector2(2, -1), new Vector2(3, -1), new Vector2(3, -3), new Vector2(2, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut187
        new Vector2[] {new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, -3), new Vector2(0, -3),
                    new Vector2(1, 0), new Vector2(2, 0), new Vector2(2, -1), new Vector2(1, -1),
                    new Vector2(2, -1), new Vector2(3, -1), new Vector2(3, -3), new Vector2(2, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut188
        new Vector2[] {new Vector2(2, 0), new Vector2(3, 0), new Vector2(3, -3), new Vector2(2, -3),
                    new Vector2(0, -1), new Vector2(1, -1), new Vector2(1, -3), new Vector2(0, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut189
        new Vector2[] {new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, -3), new Vector2(0, -3),
                    new Vector2(2, 0), new Vector2(3, 0), new Vector2(3, -3), new Vector2(2, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut190
        new Vector2[] {new Vector2(1, 0), new Vector2(3, 0), new Vector2(3, -1), new Vector2(1, -1),
                    new Vector2(0, -1), new Vector2(1, -1), new Vector2(1, -3), new Vector2(0, -3),
                    new Vector2(2, -1), new Vector2(3, -1), new Vector2(3, -3), new Vector2(2, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut191
        new Vector2[] {new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, -3), new Vector2(0, -3),
                    new Vector2(1, 0), new Vector2(3, 0), new Vector2(3, -1), new Vector2(1, -1),
                    new Vector2(2, -1), new Vector2(3, -1), new Vector2(3, -3), new Vector2(2, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut192
        new Vector2[] {new Vector2(1, -2), new Vector2(3, -2), new Vector2(3, -3), new Vector2(1, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut193
        new Vector2[] { new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, -1), new Vector2(0, -1),
                    new Vector2(1, -2), new Vector2(3, -2), new Vector2(3, -3), new Vector2(1, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut194
        new Vector2[] { new Vector2(1, 0), new Vector2(2, 0), new Vector2(2, -1), new Vector2(1, -1),
                    new Vector2(1, -2), new Vector2(3, -2), new Vector2(3, -3), new Vector2(1, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut195
        new Vector2[] { new Vector2(0, 0), new Vector2(2, 0), new Vector2(2, -1), new Vector2(0, -1),
                    new Vector2(1, -2), new Vector2(3, -2), new Vector2(3, -3), new Vector2(1, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut196
        new Vector2[] { new Vector2(2, 0), new Vector2(3, 0), new Vector2(3, -1), new Vector2(2, -1),
                    new Vector2(1, -2), new Vector2(3, -2), new Vector2(3, -3), new Vector2(1, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut197
        new Vector2[] { new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, -1), new Vector2(0, -1),
                    new Vector2(2, 0), new Vector2(3, 0), new Vector2(3, -1), new Vector2(2, -1),
                    new Vector2(1, -2), new Vector2(3, -2), new Vector2(3, -3), new Vector2(1, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut198
        new Vector2[] { new Vector2(1, 0), new Vector2(3, 0), new Vector2(3, -1), new Vector2(1, -1),
                    new Vector2(1, -2), new Vector2(3, -2), new Vector2(3, -3), new Vector2(1, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut199
        new Vector2[] { new Vector2(0, 0), new Vector2(3, 0), new Vector2(3, -1), new Vector2(0, -1),
                    new Vector2(1, -2), new Vector2(3, -2), new Vector2(3, -3), new Vector2(1, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut200
        new Vector2[] { new Vector2(0, -1), new Vector2(1, -1), new Vector2(1, -2), new Vector2(0, -2),
                    new Vector2(1, -2), new Vector2(3, -2), new Vector2(3, -3), new Vector2(1, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut201
        new Vector2[] { new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, -2), new Vector2(0, -2),
                    new Vector2(1, -2), new Vector2(3, -2), new Vector2(3, -3), new Vector2(1, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut202
        new Vector2[] { new Vector2(1, 0), new Vector2(2, 0), new Vector2(2, -1), new Vector2(1, -1),
                    new Vector2(0, -1), new Vector2(1, -1), new Vector2(1, -2), new Vector2(0, -2),
                    new Vector2(1, -2), new Vector2(3, -2), new Vector2(3, -3), new Vector2(1, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut203
        new Vector2[] { new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, -2), new Vector2(0, -2),
                    new Vector2(1, 0), new Vector2(2, 0), new Vector2(2, -1), new Vector2(1, -1),
                    new Vector2(1, -2), new Vector2(3, -2), new Vector2(3, -3), new Vector2(1, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut204
        new Vector2[] { new Vector2(2, 0), new Vector2(3, 0), new Vector2(3, -1), new Vector2(2, -1),
                    new Vector2(0, -1), new Vector2(1, -1), new Vector2(1, -2), new Vector2(0, -2),
                    new Vector2(1, -2), new Vector2(3, -2), new Vector2(3, -3), new Vector2(1, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut205
        new Vector2[] { new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, -2), new Vector2(0, -2),
                    new Vector2(2, 0), new Vector2(3, 0), new Vector2(3, -1), new Vector2(2, -1),
                    new Vector2(1, -2), new Vector2(3, -2), new Vector2(3, -3), new Vector2(1, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut206
        new Vector2[] { new Vector2(1, 0), new Vector2(3, 0), new Vector2(3, -1), new Vector2(1, -1),
                    new Vector2(0, -1), new Vector2(1, -1), new Vector2(1, -2), new Vector2(0, -2),
                    new Vector2(1, -2), new Vector2(3, -2), new Vector2(3, -3), new Vector2(1, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut207
        new Vector2[] { new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, -2), new Vector2(0, -2),
                    new Vector2(1, 0), new Vector2(3, 0), new Vector2(3, -1), new Vector2(1, -1),
                    new Vector2(1, -2), new Vector2(3, -2), new Vector2(3, -3), new Vector2(1, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut208
        new Vector2[] { new Vector2(2, -1), new Vector2(3, -1), new Vector2(3, -3), new Vector2(2, -3),
                    new Vector2(1, -2), new Vector2(2, -2), new Vector2(2, -3), new Vector2(1, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut209
        new Vector2[] { new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, -1), new Vector2(0, -1),
                    new Vector2(2, -1), new Vector2(3, -1), new Vector2(3, -3), new Vector2(2, -3),
                    new Vector2(1, -2), new Vector2(2, -2), new Vector2(2, -3), new Vector2(1, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut210
        new Vector2[] { new Vector2(1, 0), new Vector2(2, 0), new Vector2(2, -1), new Vector2(1, -1),
                    new Vector2(2, -1), new Vector2(3, -1), new Vector2(3, -3), new Vector2(2, -3),
                    new Vector2(1, -2), new Vector2(2, -2), new Vector2(2, -3), new Vector2(1, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut211
        new Vector2[] { new Vector2(0, 0), new Vector2(2, 0), new Vector2(2, -1), new Vector2(0, -1),
                    new Vector2(2, -1), new Vector2(3, -1), new Vector2(3, -3), new Vector2(2, -3),
                    new Vector2(1, -2), new Vector2(2, -2), new Vector2(2, -3), new Vector2(1, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut212
        new Vector2[] { new Vector2(2, 0), new Vector2(3, 0), new Vector2(3, -3), new Vector2(2, -3),
                    new Vector2(1, -2), new Vector2(2, -2), new Vector2(2, -3), new Vector2(1, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut213
        new Vector2[] { new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, -1), new Vector2(0, -1),
                    new Vector2(2, 0), new Vector2(3, 0), new Vector2(3, -3), new Vector2(2, -3),
                    new Vector2(1, -2), new Vector2(2, -2), new Vector2(2, -3), new Vector2(1, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut214
        new Vector2[] { new Vector2(1, 0), new Vector2(3, 0), new Vector2(3, -1), new Vector2(1, -1),
                    new Vector2(2, -1), new Vector2(3, -1), new Vector2(3, -3), new Vector2(2, -3),
                    new Vector2(1, -2), new Vector2(2, -2), new Vector2(2, -3), new Vector2(1, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut215
        new Vector2[] { new Vector2(0, 0), new Vector2(3, 0), new Vector2(3, -1), new Vector2(0, -1),
                    new Vector2(2, -1), new Vector2(3, -1), new Vector2(3, -3), new Vector2(2, -3),
                    new Vector2(1, -2), new Vector2(2, -2), new Vector2(2, -3), new Vector2(1, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut216
        new Vector2[] { new Vector2(0, -1), new Vector2(1, -1), new Vector2(1, -2), new Vector2(0, -2),
                    new Vector2(2, -1), new Vector2(3, -1), new Vector2(3, -3), new Vector2(2, -3),
                    new Vector2(1, -2), new Vector2(2, -2), new Vector2(2, -3), new Vector2(1, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut217
        new Vector2[] { new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, -2), new Vector2(0, -2),
                    new Vector2(2, -1), new Vector2(3, -1), new Vector2(3, -3), new Vector2(2, -3),
                    new Vector2(1, -2), new Vector2(2, -2), new Vector2(2, -3), new Vector2(1, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut218
        new Vector2[] { new Vector2(1, 0), new Vector2(2, 0), new Vector2(2, -1), new Vector2(1, -1),
                    new Vector2(0, -1), new Vector2(1, -1), new Vector2(1, -2), new Vector2(0, -2),
                    new Vector2(2, -1), new Vector2(3, -1), new Vector2(3, -3), new Vector2(2, -3),
                    new Vector2(1, -2), new Vector2(2, -2), new Vector2(2, -3), new Vector2(1, -3) },
        // lut219
        new Vector2[] { new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, -2), new Vector2(0, -2),
                    new Vector2(1, 0), new Vector2(2, 0), new Vector2(2, -1), new Vector2(1, -1),
                    new Vector2(2, -1), new Vector2(3, -1), new Vector2(3, -3), new Vector2(2, -3),
                    new Vector2(1, -2), new Vector2(2, -2), new Vector2(2, -3), new Vector2(1, -3) },
        // lut220
        new Vector2[] { new Vector2(2, 0), new Vector2(3, 0), new Vector2(3, -3), new Vector2(2, -3),
                    new Vector2(0, -1), new Vector2(1, -1), new Vector2(1, -2), new Vector2(0, -2),
                    new Vector2(1, -2), new Vector2(2, -2), new Vector2(2, -3), new Vector2(1, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut221
        new Vector2[] { new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, -2), new Vector2(0, -2),
                    new Vector2(2, 0), new Vector2(3, 0), new Vector2(3, -3), new Vector2(2, -3),
                    new Vector2(1, -2), new Vector2(2, -2), new Vector2(2, -3), new Vector2(1, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut222
        new Vector2[] { new Vector2(1, 0), new Vector2(3, 0), new Vector2(3, -1), new Vector2(1, -1),
                    new Vector2(0, -1), new Vector2(1, -1), new Vector2(1, -2), new Vector2(0, -2),
                    new Vector2(2, -1), new Vector2(3, -1), new Vector2(3, -3), new Vector2(2, -3),
                    new Vector2(1, -2), new Vector2(2, -2), new Vector2(2, -3), new Vector2(1, -3) },
        // lut223
        new Vector2[] { new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, -2), new Vector2(0, -2),
                    new Vector2(1, 0), new Vector2(3, 0), new Vector2(3, -1), new Vector2(1, -1),
                    new Vector2(2, -1), new Vector2(3, -1), new Vector2(3, -3), new Vector2(2, -3),
                    new Vector2(1, -2), new Vector2(2, -2), new Vector2(2, -3), new Vector2(1, -3) },
        // lut224
        new Vector2[] { new Vector2(0, -2), new Vector2(3, -2), new Vector2(3, -3), new Vector2(0, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut225
        new Vector2[] { new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, -1), new Vector2(0, -1),
                    new Vector2(0, -2), new Vector2(3, -2), new Vector2(3, -3), new Vector2(0, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut226
        new Vector2[] { new Vector2(1, 0), new Vector2(2, 0), new Vector2(2, -1), new Vector2(1, -1),
                    new Vector2(0, -2), new Vector2(3, -2), new Vector2(3, -3), new Vector2(0, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut227
        new Vector2[] { new Vector2(0, 0), new Vector2(2, 0), new Vector2(2, -1), new Vector2(0, -1),
                    new Vector2(0, -2), new Vector2(3, -2), new Vector2(3, -3), new Vector2(0, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut228
        new Vector2[] { new Vector2(2, 0), new Vector2(3, 0), new Vector2(3, -1), new Vector2(2, -1),
                    new Vector2(0, -2), new Vector2(3, -2), new Vector2(3, -3), new Vector2(0, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut229
        new Vector2[] {new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, -1), new Vector2(0, -1),
                    new Vector2(2, 0), new Vector2(3, 0), new Vector2(3, -1), new Vector2(2, -1),
                    new Vector2(0, -2), new Vector2(3, -2), new Vector2(3, -3), new Vector2(0, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut230
        new Vector2[] {new Vector2(1, 0), new Vector2(3, 0), new Vector2(3, -1), new Vector2(1, -1),
                    new Vector2(0, -2), new Vector2(3, -2), new Vector2(3, -3), new Vector2(0, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut231
        new Vector2[] {new Vector2(0, 0), new Vector2(3, 0), new Vector2(3, -1), new Vector2(0, -1),
                    new Vector2(0, -2), new Vector2(3, -2), new Vector2(3, -3), new Vector2(0, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut232
        new Vector2[] {new Vector2(0, -1), new Vector2(1, -1), new Vector2(1, -2), new Vector2(0, -2),
                    new Vector2(0, -2), new Vector2(3, -2), new Vector2(3, -3), new Vector2(0, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut233
        new Vector2[] {new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, -2), new Vector2(0, -2),
                    new Vector2(0, -2), new Vector2(3, -2), new Vector2(3, -3), new Vector2(0, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut234
        new Vector2[] {new Vector2(1, 0), new Vector2(2, 0), new Vector2(2, -1), new Vector2(1, -1),
                    new Vector2(0, -1), new Vector2(1, -1), new Vector2(1, -2), new Vector2(0, -2),
                    new Vector2(0, -2), new Vector2(3, -2), new Vector2(3, -3), new Vector2(0, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut235
        new Vector2[] {new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, -2), new Vector2(0, -2),
                    new Vector2(1, 0), new Vector2(2, 0), new Vector2(2, -1), new Vector2(1, -1),
                    new Vector2(0, -2), new Vector2(3, -2), new Vector2(3, -3), new Vector2(0, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut236
        new Vector2[] {new Vector2(2, 0), new Vector2(3, 0), new Vector2(3, -1), new Vector2(2, -1),
                    new Vector2(0, -1), new Vector2(1, -1), new Vector2(1, -2), new Vector2(0, -2),
                    new Vector2(0, -2), new Vector2(3, -2), new Vector2(3, -3), new Vector2(0, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut237
        new Vector2[] {new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, -2), new Vector2(0, -2),
                    new Vector2(2, 0), new Vector2(3, 0), new Vector2(3, -1), new Vector2(2, -1),
                    new Vector2(0, -2), new Vector2(3, -2), new Vector2(3, -3), new Vector2(0, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut238
        new Vector2[] {new Vector2(1, 0), new Vector2(3, 0), new Vector2(3, -1), new Vector2(1, -1),
                    new Vector2(0, -1), new Vector2(1, -1), new Vector2(1, -2), new Vector2(0, -2),
                    new Vector2(0, -2), new Vector2(3, -2), new Vector2(3, -3), new Vector2(0, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut239
        new Vector2[] {new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, -2), new Vector2(0, -2),
                    new Vector2(1, 0), new Vector2(3, 0), new Vector2(3, -1), new Vector2(1, -1),
                    new Vector2(0, -2), new Vector2(3, -2), new Vector2(3, -3), new Vector2(0, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut240
        new Vector2[] {new Vector2(2, -1), new Vector2(3, -1), new Vector2(3, -3), new Vector2(2, -3),
                    new Vector2(0, -2), new Vector2(2, -2), new Vector2(2, -3), new Vector2(0, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut241
        new Vector2[] {new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, -1), new Vector2(0, -1),
                    new Vector2(2, -1), new Vector2(3, -1), new Vector2(3, -3), new Vector2(2, -3),
                    new Vector2(0, -2), new Vector2(2, -2), new Vector2(2, -3), new Vector2(0, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut242
        new Vector2[] {new Vector2(1, 0), new Vector2(2, 0), new Vector2(2, -1), new Vector2(1, -1),
                    new Vector2(2, -1), new Vector2(3, -1), new Vector2(3, -3), new Vector2(2, -3),
                    new Vector2(0, -2), new Vector2(2, -2), new Vector2(2, -3), new Vector2(0, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut243
        new Vector2[] {new Vector2(0, 0), new Vector2(2, 0), new Vector2(2, -1), new Vector2(0, -1),
                    new Vector2(2, -1), new Vector2(3, -1), new Vector2(3, -3), new Vector2(2, -3),
                    new Vector2(0, -2), new Vector2(2, -2), new Vector2(2, -3), new Vector2(0, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut244
        new Vector2[] {new Vector2(2, 0), new Vector2(3, 0), new Vector2(3, -3), new Vector2(2, -3),
                    new Vector2(0, -2), new Vector2(2, -2), new Vector2(2, -3), new Vector2(0, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut245
        new Vector2[] {new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, -1), new Vector2(0, -1),
                    new Vector2(2, 0), new Vector2(3, 0), new Vector2(3, -3), new Vector2(2, -3),
                    new Vector2(0, -2), new Vector2(2, -2), new Vector2(2, -3), new Vector2(0, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut246
        new Vector2[] {new Vector2(1, 0), new Vector2(3, 0), new Vector2(3, -1), new Vector2(1, -1),
                    new Vector2(2, -1), new Vector2(3, -1), new Vector2(3, -3), new Vector2(2, -3),
                    new Vector2(0, -2), new Vector2(2, -2), new Vector2(2, -3), new Vector2(0, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut247
        new Vector2[] {new Vector2(0, 0), new Vector2(3, 0), new Vector2(3, -1), new Vector2(0, -1),
                    new Vector2(2, -1), new Vector2(3, -1), new Vector2(3, -3), new Vector2(2, -3),
                    new Vector2(0, -2), new Vector2(2, -2), new Vector2(2, -3), new Vector2(0, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut248
        new Vector2[] {new Vector2(0, -1), new Vector2(1, -1), new Vector2(1, -2), new Vector2(0, -2),
                    new Vector2(2, -1), new Vector2(3, -1), new Vector2(3, -3), new Vector2(2, -3),
                    new Vector2(0, -2), new Vector2(2, -2), new Vector2(2, -3), new Vector2(0, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut249
        new Vector2[] {new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, -2), new Vector2(0, -2),
                    new Vector2(2, -1), new Vector2(3, -1), new Vector2(3, -3), new Vector2(2, -3),
                    new Vector2(0, -2), new Vector2(2, -2), new Vector2(2, -3), new Vector2(0, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut250
        new Vector2[] {new Vector2(1, 0), new Vector2(2, 0), new Vector2(2, -1), new Vector2(1, -1),
                    new Vector2(0, -1), new Vector2(1, -1), new Vector2(1, -2), new Vector2(0, -2),
                    new Vector2(2, -1), new Vector2(3, -1), new Vector2(3, -3), new Vector2(2, -3),
                    new Vector2(0, -2), new Vector2(2, -2), new Vector2(2, -3), new Vector2(0, -3) },
        // lut251
        new Vector2[] {new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, -2), new Vector2(0, -2),
                    new Vector2(1, 0), new Vector2(2, 0), new Vector2(2, -1), new Vector2(1, -1),
                    new Vector2(2, -1), new Vector2(3, -1), new Vector2(3, -3), new Vector2(2, -3),
                    new Vector2(0, -2), new Vector2(2, -2), new Vector2(2, -3), new Vector2(0, -3) },
        // lut252
        new Vector2[] {new Vector2(2, 0), new Vector2(3, 0), new Vector2(3, -3), new Vector2(2, -3),
                    new Vector2(0, -1), new Vector2(1, -1), new Vector2(1, -2), new Vector2(0, -2),
                    new Vector2(0, -2), new Vector2(2, -2), new Vector2(2, -3), new Vector2(0, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut253
        new Vector2[] {new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, -2), new Vector2(0, -2),
                    new Vector2(2, 0), new Vector2(3, 0), new Vector2(3, -3), new Vector2(2, -3),
                    new Vector2(0, -2), new Vector2(2, -2), new Vector2(2, -3), new Vector2(0, -3),
                    new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0) },
        // lut254
        new Vector2[] {new Vector2(1, 0), new Vector2(3, 0), new Vector2(3, -1), new Vector2(1, -1),
                    new Vector2(0, -1), new Vector2(1, -1), new Vector2(1, -2), new Vector2(0, -2),
                    new Vector2(2, -1), new Vector2(3, -1), new Vector2(3, -3), new Vector2(2, -3),
                    new Vector2(0, -2), new Vector2(2, -2), new Vector2(2, -3), new Vector2(0, -3) },
        // lut255
        new Vector2[] {new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, -2), new Vector2(0, -2),
                    new Vector2(1, 0), new Vector2(3, 0), new Vector2(3, -1), new Vector2(1, -1),
                    new Vector2(2, -1), new Vector2(3, -1), new Vector2(3, -3), new Vector2(2, -3),
                    new Vector2(0, -2), new Vector2(2, -2), new Vector2(2, -3), new Vector2(0, -3) },
    };

    void Update()
    {
        if (manualLutSelection)
        {
            ApplyCollision(false);
            return;
        }


        int player_pixel_ID = game_manager.PositionToPixelID(player_transform.position);

        // dirty hack when over ground
        // will have to find a better solution
        // the issue is then when above ground, pixel_ID is negative
        if(player_pixel_ID < 0)
        {
            lutSelection = 7;
            ApplyCollision();
            return;
        }

        // binary representation of the non-air tiles around the player, see luts[][]
        // 0 is no tile, 255 is all tiles, 165 is the four corners, 189 is two vertical lines of tiles, 231 is two horizontal lines
        lutSelection = 0;

        int path_counter = -1;
        for (int y_offset = -1; y_offset <= 1; y_offset++)
        {
            for (int x_offset = -1; x_offset <= 1; x_offset++)
            {
                // skip center tile, where the player is
                if (x_offset == 0 && y_offset == 0)
                    continue;

                path_counter++;

                int pixel_ID = game_manager.GetPixelIDAtOffset(player_pixel_ID, x_offset, y_offset);
                if (pixel_ID < 0)
                    continue;

                bool is_tile = !game_manager.IsTileDugUp(pixel_ID);
                if (is_tile)
                    lutSelection += (int)Mathf.Pow(2, path_counter);
            }
        }

        ApplyCollision();
    }

    void ApplyCollision(bool follow_player=true)
    {
        Vector2[] corners_ = new Vector2[4];

        if (follow_player)
        {
            int player_pixel_ID = game_manager.PositionToPixelID(player_transform.position);
            player_pixel_ID = Mathf.Abs(player_pixel_ID); // dirty hack for above ground

            Vector3 OFFSET = new Vector3(-1.5f, 1.5f, 0.0f);
            polygon_collider_2d.offset = game_manager.PixelIDToPosition(player_pixel_ID) + OFFSET;
        }
        else
        {
            polygon_collider_2d.offset = new Vector3(0, 0, 0);
        }
        

        for (int i = 0; i < 4; i++)
        {
            corners_[0] = collision_LUT[lutSelection][0 + i * 4];
            corners_[1] = collision_LUT[lutSelection][1 + i * 4];
            corners_[2] = collision_LUT[lutSelection][2 + i * 4];
            corners_[3] = collision_LUT[lutSelection][3 + i * 4];

            polygon_collider_2d.SetPath(i, corners_);
        }
    }
}
