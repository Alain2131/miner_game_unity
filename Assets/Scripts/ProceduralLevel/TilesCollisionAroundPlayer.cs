using System.Collections.Generic;
using UnityEngine;

// Stray collision on the floor
// setup :
// hole in the middle, fly up (~2 tiles), hit the ground, and you might be stopped from moving left or right

[RequireComponent(typeof(PolygonCollider2D))]
public class TilesCollisionAroundPlayer : MonoBehaviour
{
    [SerializeField] private bool manualLutSelection = false;
    //[SerializeField] private bool useLUTTable = false;
	[SerializeField] private bool followPlayer = true;
	[SerializeField] private int  lutSelection = 0;

    private GameManager game_manager;
    private Transform player_transform;

    private PolygonCollider2D polygon_collider_2d;

    void Start()
    {
        game_manager = GameManager.Instance;
        player_transform = game_manager.player.transform;
        polygon_collider_2d = transform.GetComponent<PolygonCollider2D>();
		
		//PrintLUT();
    }

    // Thanks to ChatGPT
    static List<bool> GetBinaryList(int number)
    {
        List<bool> binaryList = new List<bool>();

        // Loop through each bit position, from least significant bit (LSB) to the most significant bit (MSB)
        for (int i = 0; i < 32; i++)  // 32 bits for an integer
        {
            // Check if the i-th bit is turned on
            bool isBitSet = (number & (1 << i)) != 0;
            binaryList.Add(isBitSet);
        }

        //binaryList.Reverse();  // To make the list ordered from MSB to LSB
        return binaryList;
    }

	private Vector2[] ComputeLUTEntry(int lut_number)
	{
		// Detect air tiles
		bool[] air_tiles = GetBinaryList(lut_number).ToArray();

		bool top_left = air_tiles[0];
		bool top_middle = air_tiles[1];
		bool top_right = air_tiles[2];
		bool middle_left = air_tiles[3];
		bool middle_right = air_tiles[4];
		bool bottom_left = air_tiles[5];
		bool bottom_middle = air_tiles[6];
		bool bottom_right = air_tiles[7];


		const float GAP = 0.05f; // should probably make a global variable or something

		// "Algorithm"
		Vector2[] lut_entry = new Vector2[16];
		int count = 0;
		if (top_left == true)
		{
			if (top_middle == false && middle_left == false) // surrounded by air
			{
				// add top_left corner
				lut_entry[0 + count * 4] = new Vector2(0, 0);
				lut_entry[1 + count * 4] = new Vector2(1 - GAP, 0);
				lut_entry[2 + count * 4] = new Vector2(1 - GAP, -1);
				lut_entry[3 + count * 4] = new Vector2(0, -1);
				count++;
			}
			// else, this tile will be merged with one of the middles
		}
		if (top_middle == true)
		{
			float right_edge = 2.0f - GAP;
			float left_edge = 1.0f + GAP;
			if (top_right == true)
				right_edge = 3.0f;
			if (top_left == true && middle_left == false)
				left_edge = 0.0f;
			if (top_left == true && middle_left == true)
				left_edge = 1.0f - GAP;

			// add top_middle
			lut_entry[0 + count * 4] = new Vector2(left_edge, 0);
			lut_entry[1 + count * 4] = new Vector2(right_edge, 0);
			lut_entry[2 + count * 4] = new Vector2(right_edge, -1);
			lut_entry[3 + count * 4] = new Vector2(left_edge, -1);
			count++;
		}
		if (top_right == true)
		{
			if (middle_right == false && top_middle == false) // surrounded by air
			{
				// add top_right corner
				lut_entry[0 + count * 4] = new Vector2(2 + GAP, 0);
				lut_entry[1 + count * 4] = new Vector2(3, 0);
				lut_entry[2 + count * 4] = new Vector2(3, -1);
				lut_entry[3 + count * 4] = new Vector2(2 + GAP, -1);
				count++;
			}
			// else, this tile will be merged with one of the middles
		}
		if (middle_right == true)
		{
			float bottom_edge = -2.0f;
			float top_edge = -1.0f;
			if (bottom_right == true)
				bottom_edge = -3.0f;
			if (top_right == true && top_middle == false)
				top_edge = 0.0f;

			// add middle_right
			lut_entry[0 + count * 4] = new Vector2(2 + GAP, top_edge);
			lut_entry[1 + count * 4] = new Vector2(3, top_edge);
			lut_entry[2 + count * 4] = new Vector2(3, bottom_edge);
			lut_entry[3 + count * 4] = new Vector2(2 + GAP, bottom_edge);
			count++;
		}
		if (bottom_right == true)
		{
			if (bottom_middle == false && middle_right == false) // surrounded by air
			{
				// add bottom_right corner
				lut_entry[0 + count * 4] = new Vector2(2 + GAP, -2);
				lut_entry[1 + count * 4] = new Vector2(3, -2);
				lut_entry[2 + count * 4] = new Vector2(3, -3);
				lut_entry[3 + count * 4] = new Vector2(2 + GAP, -3);
				count++;
			}
			// else, this tile will be merged with one of the middles
		}
		if (bottom_middle == true)
		{
			float left_edge = 1.0f + GAP;
			float right_edge = 2.0f - GAP;
			if (bottom_left == true)
				left_edge = 0.0f;
			if (bottom_right == true && middle_right == false)
				right_edge = 3.0f;
			if (bottom_right == true && middle_right == true)
				right_edge = 2.0f + GAP;

			// add bottom_middle
			lut_entry[0 + count * 4] = new Vector2(left_edge, -2);
			lut_entry[1 + count * 4] = new Vector2(right_edge, -2);
			lut_entry[2 + count * 4] = new Vector2(right_edge, -3);
			lut_entry[3 + count * 4] = new Vector2(left_edge, -3);
			count++;
		}
		if (bottom_left == true)
		{
			if (middle_left == false && bottom_middle == false) // surrounded by air
			{
				// add bottom_left corner
				lut_entry[0 + count * 4] = new Vector2(0, -2);
				lut_entry[1 + count * 4] = new Vector2(1 - GAP, -2);
				lut_entry[2 + count * 4] = new Vector2(1 - GAP, -3);
				lut_entry[3 + count * 4] = new Vector2(0, -3);
				count++;
			}
			// else, this tile will be merged with one of the middles
		}
		if (middle_left == true)
		{
			float top_edge = -1.0f;
			float bottom_edge = -2.0f;
			if (top_left == true)
				top_edge = 0.0f;
			if (bottom_left == true && bottom_middle == false)
				bottom_edge = -3.0f;

			// add middle_left
			lut_entry[0 + count * 4] = new Vector2(0, top_edge);
			lut_entry[1 + count * 4] = new Vector2(1 - GAP, top_edge);
			lut_entry[2 + count * 4] = new Vector2(1 - GAP, bottom_edge);
			lut_entry[3 + count * 4] = new Vector2(0, bottom_edge);
			//count++; // not necessary at the end
		}

		return lut_entry;
	}

	private void PrintLUT()
    {
		string output = "\t// Generated from PrintLUT(), see full log for un-truncated result\n";
		output += "\tprivate Vector2[][] collision_LUT = new Vector2[256][]\n\t{\n";
        for(int lut=0; lut < 256; lut++)
        {
			output += $"\t\t// lut{lut}\n";

			// Algorithm
			Vector2[] lut_entry = ComputeLUTEntry(lut); // would need a better name than that, more evocative of what it contains

			for (int i = 0; i < 4; i++)
			{
				Vector2[] corners = new Vector2[4];

				corners[0] = lut_entry[0 + i * 4];
				corners[1] = lut_entry[1 + i * 4];
				corners[2] = lut_entry[2 + i * 4];
				corners[3] = lut_entry[3 + i * 4];

				string line = $"new Vector2({corners[0][0]}f, {corners[0][1]}f), new Vector2({corners[1][0]}f, {corners[1][1]}f), new Vector2({corners[2][0]}f, {corners[2][1]}f), new Vector2({corners[3][0]}f, {corners[3][1]}f)";

				if (i == 0) // first line
					output += $"\t\tnew Vector2[] {{ {line},\n";
				else if (i == 1 || i == 2) // middle lines
					output += $"\t\t\t\t{line},\n";
				else // last line
					output += $"\t\t\t\t{line} }},\n";
			}
		}
        output += "\t};";
        Debug.Log(output);
    }

	// binary representation of un-dug tiles around the player, see collision_LUT[][]
	// 0 is no tile, 255 is all tiles, 165 is the four corners, 189 is two vertical lines of tiles, 231 is two horizontal lines
	private int GetLUTNumberAroundPixelID(int pixel_ID)
    {
		int lut_number = 0;

		int counter = -1;
		for (int y_offset = -1; y_offset <= 1; y_offset++)
		{
			for (int x_offset = -1; x_offset <= 1; x_offset++)
			{
				// skip center tile, where the player is
				if (x_offset == 0 && y_offset == 0)
					continue;

				counter++;

				int tile_pixel_ID = game_manager.GetPixelIDAtOffset(pixel_ID, x_offset, y_offset);
				if (tile_pixel_ID < 0)
					continue;

				bool is_tile = !game_manager.IsTileDugUp(tile_pixel_ID);
				if (is_tile)
					lut_number += (int)Mathf.Pow(2, counter);
			}
		}

		return lut_number;
    }

	void Update()
    {
        if (manualLutSelection)
        {
			// These random separate ApplyCollision() should probably be removed
			// and entirely handled within ApplyCollision()
			ApplyCollision(lutSelection);
            return;
        }

        int player_pixel_ID = game_manager.PositionToPixelID(player_transform.position);
		
        lutSelection = GetLUTNumberAroundPixelID(player_pixel_ID);
        ApplyCollision(lutSelection);
    }

    void ApplyCollision(int lut_number)
    {
		Vector2 offset = new Vector2(0, 0);
		if (followPlayer)
        {
            Vector2 CENTER_OFFSET = new Vector2(-1.5f, 1.5f);

			int player_pixel_ID = game_manager.PositionToPixelID(player_transform.position);
			offset = game_manager.PixelIDToPosition(player_pixel_ID) + CENTER_OFFSET;
        }
		polygon_collider_2d.offset = offset;


		Vector2[] corners = new Vector2[4];
		Vector2[] data = ComputeLUTEntry(lut_number);
		for (int i = 0; i < 4; i++)
        {
			/*if (useLUTTable)
            {
				corners[0] = collision_LUT[lut_number][0 + i * 4];
				corners[1] = collision_LUT[lut_number][1 + i * 4];
				corners[2] = collision_LUT[lut_number][2 + i * 4];
				corners[3] = collision_LUT[lut_number][3 + i * 4];
			}*/
			//else
            {
				corners[0] = data[0 + i * 4];
				corners[1] = data[1 + i * 4];
				corners[2] = data[2 + i * 4];
				corners[3] = data[3 + i * 4];
			}

            polygon_collider_2d.SetPath(i, corners);
        }
    }
}
