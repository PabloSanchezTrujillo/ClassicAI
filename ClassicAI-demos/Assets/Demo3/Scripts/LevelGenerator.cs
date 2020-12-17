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

    private TilesGroup tilesGroup;

    #endregion variables

    private void Awake()
    {
        tilesGroup = GetComponent<TilesGroup>();
    }

    public void GenerateGrid()
    {
        generationPanel.SetActive(false);
        int rows = int.Parse(rowsInput.text);
        int columns = int.Parse(columnsInput.text);

        for(int i = 0; i < columns; i++) {
            for(int j = 0; j < rows; j++) {
                GameObject tileObject = Instantiate(tile, transform);
                tileObject.GetComponent<Tile>().CreateTile(RoadTypes.RoadType.Road, Tile.CardinalPoints.South, Tile.CardinalPoints.North, tilesGroup);

                float posX = i * tileSize;
                float posZ = j * -tileSize;

                tileObject.transform.position = new Vector3(posX, 0, posZ);
            }
        }

        float gridWidth = columns * tileSize;
        float gridHeight = rows * tileSize;
        transform.position = new Vector3(-gridWidth / 2 + tileSize / 2, 0, gridHeight / 2 - tileSize / 2);
    }
}