using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelGenerator : MonoBehaviour
{
    #region variables

    [SerializeField] private InputField rowsInput;
    [SerializeField] private InputField columnsInput;
    [SerializeField] private InputField iterationsInput;
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
    private int iterations;
    private Tile[,] tiles;
    private Tile.CardinalPoints turnExit;

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

        if(iterationsInput.text.Length == 0) {
            iterations = 1;
        }
        else {
            iterations = int.Parse(iterationsInput.text);
        }
        for(int i = 0; i < iterations; i++) {
            CreateLevel();
        }
    }

    private void CreateLevel()
    {
        for(int row = 0; row < rows; row++) {
            for(int col = 0; col < columns; col++) {
                // First tile
                if(row == 0 && col == 0 && tiles[0, 0].IsEmpty) {
                    tiles[row, col].CreateTile(RoadTypes.RoadType.Intersection, (Tile.CardinalPoints)Random.Range(1, 5), false);
                }
                else if(tiles[row, col].IsEmpty) {
                    List<Tile.CardinalPoints> possibleEntries = CheckRoadEntryRules(row, col);
                    if(possibleEntries.Count != 0) {
                        Tile.CardinalPoints entryDirection = possibleEntries[Random.Range(0, possibleEntries.Count)];

                        List<RoadTypes.RoadType> possibleRoadTypes = new List<RoadTypes.RoadType>();
                        possibleRoadTypes = CheckRoadExitRules(row, col, entryDirection);

                        if(possibleRoadTypes.Count != 0) {
                            RoadTypes.RoadType roadTypeSelected = possibleRoadTypes[Random.Range(0, possibleRoadTypes.Count)];

                            if(roadTypeSelected == RoadTypes.RoadType.Turn) {
                                tiles[row, col].CreateTile(roadTypeSelected, entryDirection, turnExit, false);
                            }
                            else {
                                tiles[row, col].CreateTile(roadTypeSelected, entryDirection, false);
                            }
                        }
                    }
                }
            }
        }
    }

    private List<Tile.CardinalPoints> CheckRoadEntryRules(int row, int column)
    {
        List<Tile.CardinalPoints> possibleEntries = new List<Tile.CardinalPoints>();

        // Checks that it is not the first row
        if(row > 0) {
            // If the North tile has a South exit, the actual tile can entry from North
            if(tiles[row - 1, column].GetAllExits().Contains(Tile.CardinalPoints.South)) {
                possibleEntries.Add(Tile.CardinalPoints.North);
            }
        }

        // Checks that it is not the last row
        if(row != (rows - 1)) {
            // If the South tile has a North exit, the actual tile can entry from South
            if(tiles[row + 1, column].GetAllExits().Contains(Tile.CardinalPoints.North)) {
                possibleEntries.Add(Tile.CardinalPoints.South);
            }
        }

        // Checks that it is not the first column
        if(column > 0) {
            // If the West tile has an East exit, the actual tile can entry from West
            if(tiles[row, column - 1].GetAllExits().Contains(Tile.CardinalPoints.East)) {
                possibleEntries.Add(Tile.CardinalPoints.West);
            }
        }

        // Checks that it is not the last column
        if(column != (columns - 1)) {
            // If the East tile has a West exit, the actual tle can entry from East
            if(tiles[row, column + 1].GetAllExits().Contains(Tile.CardinalPoints.West)) {
                possibleEntries.Add(Tile.CardinalPoints.East);
            }
        }

        return possibleEntries;
    }

    private List<RoadTypes.RoadType> CheckRoadExitRules(int row, int column, Tile.CardinalPoints entryCardinalPoint)
    {
        // Comprobar las salidas de mi tile con TODAS las salidas de las tiles de alrededor
        // y también comprobar que la entrada de mi tile coincide con alguna de las salidas de alrededor
        // Descartar aquellas RoadTypes que no coincidan

        List<RoadTypes.RoadType> possibleRoadTypes = new List<RoadTypes.RoadType>();
        RoadTypes.RoadType[] roadTypes = { RoadTypes.RoadType.Crossroad, RoadTypes.RoadType.Intersection, RoadTypes.RoadType.Road, RoadTypes.RoadType.RoadEnd, RoadTypes.RoadType.Turn };

        foreach(RoadTypes.RoadType roadType in roadTypes) {
            bool roadTypePermitted = true;
            tiles[row, column].CreateTile(roadType, entryCardinalPoint, true);

            // Checks that it is not the first row
            if(row > 0) {
                // If the North tile has a South exit, the actual tile can exit to North
                if(tiles[row - 1, column].GetAllExits().Contains(Tile.CardinalPoints.South)) {
                    if((tiles[row, column].GetAllExits().Contains(Tile.CardinalPoints.North) || entryCardinalPoint == Tile.CardinalPoints.North)
                        && roadTypePermitted) {
                        roadTypePermitted = true;

                        if(roadType == RoadTypes.RoadType.Turn) {
                            if(column - 1 < 0) {
                                turnExit = Tile.CardinalPoints.East;
                            }
                            else {
                                if(tiles[row, column - 1].GetAllExits().Contains(Tile.CardinalPoints.East)) {
                                    turnExit = Tile.CardinalPoints.West;
                                }
                                else {
                                    turnExit = Tile.CardinalPoints.East;
                                }
                            }
                        }
                    }
                    else {
                        roadTypePermitted = false;
                    }
                }
                else if(!tiles[row - 1, column].IsEmpty) {
                    if(tiles[row, column].GetAllExits().Contains(Tile.CardinalPoints.North)) {
                        roadTypePermitted = false;
                    }
                }
            }

            // Checks that it is not the last row
            if(row != (rows - 1)) {
                // If the South tile has a North exit, the actual tile can exit to South
                if(tiles[row + 1, column].GetAllExits().Contains(Tile.CardinalPoints.North)) {
                    if((tiles[row, column].GetAllExits().Contains(Tile.CardinalPoints.South) || entryCardinalPoint == Tile.CardinalPoints.South)
                        && roadTypePermitted) {
                        roadTypePermitted = true;

                        if(roadType == RoadTypes.RoadType.Turn) {
                            if(column - 1 < 0) {
                                turnExit = Tile.CardinalPoints.East;
                            }
                            else {
                                if(tiles[row, column - 1].GetAllExits().Contains(Tile.CardinalPoints.East)) {
                                    turnExit = Tile.CardinalPoints.West;
                                }
                                else {
                                    turnExit = Tile.CardinalPoints.East;
                                }
                            }
                        }
                    }
                    else {
                        roadTypePermitted = false;
                    }
                }
                else if(!tiles[row + 1, column].IsEmpty) {
                    if(tiles[row, column].GetAllExits().Contains(Tile.CardinalPoints.South)) {
                        roadTypePermitted = false;
                    }
                }
            }

            // Checks that it is not the first column
            if(column > 0) {
                // If the West tile has an East exit, the actual tile can exit to West
                if(tiles[row, column - 1].GetAllExits().Contains(Tile.CardinalPoints.East)) {
                    if((tiles[row, column].GetAllExits().Contains(Tile.CardinalPoints.West) || entryCardinalPoint == Tile.CardinalPoints.West)
                        && roadTypePermitted) {
                        roadTypePermitted = true;

                        if(roadType == RoadTypes.RoadType.Turn) {
                            if(row - 1 < 0) {
                                turnExit = Tile.CardinalPoints.South;
                            }
                            else {
                                if(tiles[row - 1, column].GetAllExits().Contains(Tile.CardinalPoints.South)) {
                                    turnExit = Tile.CardinalPoints.North;
                                }
                                else {
                                    turnExit = Tile.CardinalPoints.South;
                                }
                            }
                        }
                    }
                    else {
                        roadTypePermitted = false;
                    }
                }
                else if(!tiles[row, column - 1].IsEmpty) {
                    if(tiles[row, column].GetAllExits().Contains(Tile.CardinalPoints.West)) {
                        roadTypePermitted = false;
                    }
                }
            }

            // Checks that it is not the last column
            if(column != (columns - 1)) {
                // If the East tile has a West exit, the actual tle can exit to East
                if(tiles[row, column + 1].GetAllExits().Contains(Tile.CardinalPoints.West)) {
                    if((tiles[row, column].GetAllExits().Contains(Tile.CardinalPoints.East) || entryCardinalPoint == Tile.CardinalPoints.East)
                        && roadTypePermitted) {
                        roadTypePermitted = true;

                        if(roadType == RoadTypes.RoadType.Turn) {
                            if(row - 1 < 0) {
                                turnExit = Tile.CardinalPoints.South;
                            }
                            else {
                                if(tiles[row - 1, column].GetAllExits().Contains(Tile.CardinalPoints.South)) {
                                    turnExit = Tile.CardinalPoints.North;
                                }
                                else {
                                    turnExit = Tile.CardinalPoints.South;
                                }
                            }
                        }
                    }
                    else {
                        roadTypePermitted = false;
                    }
                }
                else if(!tiles[row, column + 1].IsEmpty) {
                    if(tiles[row, column].GetAllExits().Contains(Tile.CardinalPoints.East)) {
                        roadTypePermitted = false;
                    }
                }
            }

            if(roadTypePermitted) {
                possibleRoadTypes.Add(roadType);
            }
        }

        return possibleRoadTypes;
    }
}