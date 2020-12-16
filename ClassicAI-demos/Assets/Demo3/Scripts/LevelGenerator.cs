using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    #region variables

    [SerializeField] private int rows;
    [SerializeField] private int columns;
    [SerializeField] private GameObject tile;
    [SerializeField] private float tileSize;

    #endregion variables

    // Start is called before the first frame update
    private void Start()
    {
        GenerateGrid();
    }

    private void GenerateGrid()
    {
        for(int i = 0; i < columns; i++) {
            for(int j = 0; j < rows; j++) {
                GameObject tileObject = Instantiate(tile, transform);

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