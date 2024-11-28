using UnityEngine;
using UnityEngine.Tilemaps;

public class MapGenerator : MonoBehaviour
{
    [Header("Tilemap and Bounds")]
    public Tilemap tilemap; // The target Tilemap where tiles will be placed
    public Vector2Int gridSize = new Vector2Int(10, 10); // Size of the area to modify

    [Header("Tile Palette Settings")]
    public TileBase[] detailTiles; // Array of tiles (e.g., rocks, trees) from your Tile Palette
    public float placementChance = 0.3f; // Chance (0 to 1) to place a tile on a position

    private void Start()
    {
        PlaceRandomTiles();
    }

    private void PlaceRandomTiles()
    {
        if (tilemap == null || detailTiles.Length == 0)
        {
            Debug.LogWarning("Tilemap or detail tiles are not assigned!");
            return;
        }

        // Get the starting tilemap position based on the GameObject's world position
        Vector3Int startTilePosition = tilemap.WorldToCell(transform.position);

        // Loop through the defined grid size
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                // Skip placement based on the placement chance
                if (Random.value > placementChance)
                    continue;

                // Calculate the tile position relative to the starting point
                Vector3Int tilePosition = new Vector3Int(startTilePosition.x + x, startTilePosition.y + y, 0);

                // Skip placement if there's no base tile here
                if (!tilemap.HasTile(tilePosition))
                    continue;

                // Pick a random tile from the detailTiles array
                TileBase randomTile = detailTiles[Random.Range(0, detailTiles.Length)];

                // Place the tile on the Tilemap
                tilemap.SetTile(tilePosition, randomTile);
            }
        }
    }
}
