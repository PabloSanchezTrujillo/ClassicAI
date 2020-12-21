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

    private int rows;
    private int columns;
    private TilesGroup tilesGroup;
    private Tile[,] tiles;

    #endregion variables

    private void Awake()
    {
        tilesGroup = GetComponent<TilesGroup>();
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
    }

    private void CreateLevel()
    {
        for(int row = 0; row < rows; row++) {
            for(int col = 0; col < columns; col++) {
                if(row == 0 && col == 0) {
                    tiles[row, col].CreateTile(RoadTypes.RoadType.Turn, Tile.CardinalPoints.East/*(Tile.CardinalPoints)Random.Range(1, 5)*/, tilesGroup);
                }
            }
        }
    }
}