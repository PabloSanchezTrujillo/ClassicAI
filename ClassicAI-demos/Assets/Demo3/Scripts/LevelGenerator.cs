using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelGenerator : MonoBehaviour
{
    #region variables

    [SerializeField] private InputField rowsInput;
    [SerializeField] private InputField columnsInput;
    [SerializeField] private GameObject generationPanel;
    [SerializeField] private GameObject tile;
    [SerializeField] private float tileSize;

    [Header("Tiles")]
    [SerializeField] private GameObject crossroadCrosswalk;

    [SerializeField] private GameObject crossroad;
    [SerializeField] private GameObject intersectionCrosswalk;
    [SerializeField] private GameObject intersection;
    [SerializeField] private GameObject roadEnd;
    [SerializeField] private GameObject roadCrosswalk;
    [SerializeField] private GameObject road;
    [SerializeField] private GameObject roundabout;
    [SerializeField] private GameObject turn;

    private int rows;
    private int columns;
    private Tile[,] tiles;

    #endregion variables

    private void Awake()
    {
        TilesGroup.CreateGroup(crossroadCrosswalk, crossroad, intersectionCrosswalk, intersection, roadEnd, roadCrosswalk, road, roundabout, turn);
    }

    public void GenerateGrid()
    {
        generationPanel.SetActive(false);
        rows = int.Parse(rowsInput.text);
        columns = int.Parse(columnsInput.text);
        tiles = new Tile[rows, columns];

        for(int i = 0; i < columns; i++) {
            for(int j = 0; j < rows; j++) {
                GameObject tileObject = Instantiate(tile, transform);
                tiles[j, i] = tileObject.GetComponent<Tile>();

                float posX = i * tileSize;
                float posZ = j * -tileSize;

                tileObject.transform.position = new Vector3(posX, 0, posZ);
            }
        }

        float gridWidth = columns * tileSize;
        float gridHeight = rows * tileSize;
        transform.position = new Vector3(-gridWidth / 2 + tileSize / 2, 0, gridHeight / 2 - tileSize / 2);

        CreateLevel();
        // TODO: Hacer una segunda iteración para completar nuevas posibilidades
    }

    private void CreateLevel()
    {
        for(int row = 0; row < rows; row++) {
            for(int col = 0; col < columns; col++) {
                // First tile
                if(row == 0 && col == 0) {
                    tiles[row, col].CreateTile(RoadTypes.RoadType.Intersection, (Tile.CardinalPoints)Random.Range(1, 5));
                }
                else {
                    List<Tile.CardinalPoints> possibleEntries = CheckRoadEntryRules(row, col);
                    if(possibleEntries.Count != 0) {
                        tiles[row, col].CreateTile((RoadTypes.RoadType)Random.Range(0, 7), possibleEntries[Random.Range(0, possibleEntries.Count)]);
                    }
                }
            }
        }
    }

    private List<Tile.CardinalPoints> CheckRoadEntryRules(int row, int column)
    {
        List<Tile.CardinalPoints> possibleEntries = new List<Tile.CardinalPoints>();

        // Checks it is not the first row
        if(row > 0) {
            // If the North tile has a South exit, the actual tile can entry from North
            if(tiles[row - 1, column].GetAllExits().Contains(Tile.CardinalPoints.South)) {
                possibleEntries.Add(Tile.CardinalPoints.North);
            }
        }

        // Checks it is not the last row
        if(row != (rows - 1)) {
            // If the South tile has a North exit, the actual tile can entry from South
            if(tiles[row + 1, column].GetAllExits().Contains(Tile.CardinalPoints.North)) {
                possibleEntries.Add(Tile.CardinalPoints.South);
            }
        }

        // Checks it is not the first column
        if(column > 0) {
            // If the West tile has an East exit, the actual tile can entry from West
            if(tiles[row, column - 1].GetAllExits().Contains(Tile.CardinalPoints.East)) {
                possibleEntries.Add(Tile.CardinalPoints.West);
            }
        }

        // Checks it is not the last column
        if(column != (columns - 1)) {
            // If the East tile has a West exit, the actual tle can entry from East
            if(tiles[row, column + 1].GetAllExits().Contains(Tile.CardinalPoints.West)) {
                possibleEntries.Add(Tile.CardinalPoints.East);
            }
        }

        return possibleEntries;
    }

    private void CheckRoadExitRules()
    {
        // Comprobar las salidas de mi tile con las salidas de las tiles de alrededor
        // Descartar aquellas RoadTypes que no coincidan
    }
}