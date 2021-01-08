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
    [SerializeField] private Slider crosswalkDensitySlider;
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
    private int crosswalkDensity;
    private int iterations;
    private Tile[,] tiles;
    private Tile.CardinalPoints turnExit;

    #endregion variables

    /// <summary>
    /// Creates the game object dictionary for the different road types
    /// </summary>
    private void Awake()
    {
        TilesGroup.CreateGroup(crossroadCrosswalk, crossroad, intersectionCrosswalk, intersection, roadEnd, roadCrosswalk, road, roundabout, turn);
        crosswalkDensity = 1;
    }

    /// <summary>
    /// Generates the tiles grid
    /// </summary>
    public void GenerateGrid()
    {
        generationPanel.SetActive(false);
        // Reads the inputs from the generation panel
        rows = int.Parse(rowsInput.text);
        columns = int.Parse(columnsInput.text);
        tiles = new Tile[rows, columns];

        for(int i = 0; i < columns; i++) {
            for(int j = 0; j < rows; j++) {
                // For each position instantiates a new tile object
                GameObject tileObject = Instantiate(tile, transform);
                tiles[j, i] = tileObject.GetComponent<Tile>();

                // Updates the position for the tile
                float posX = i * tileSize;
                float posZ = j * -tileSize;
                tileObject.transform.position = new Vector3(posX, 0, posZ);
            }
        }

        // Adjust the initial position of the grid
        float gridWidth = columns * tileSize;
        float gridHeight = rows * tileSize;
        transform.position = new Vector3(-gridWidth / 2 + tileSize / 2, 0, gridHeight / 2 - tileSize / 2);

        // Sets the iterations for the level generator. The default iteration is one
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

    /// <summary>
    /// Creates the road level on the grid
    /// </summary>
    private void CreateLevel()
    {
        for(int row = 0; row < rows; row++) {
            for(int col = 0; col < columns; col++) {
                // First tile always has a crossroad to expand the level from there
                if(row == 0 && col == 0 && tiles[0, 0].IsEmpty) {
                    tiles[row, col].CreateTile(RoadTypes.RoadType.Crossroad, Tile.CardinalPoints.North, false);
                }
                else if(tiles[row, col].IsEmpty) {
                    // Gets all the possible entries for the actual tile
                    List<Tile.CardinalPoints> possibleEntries = CheckRoadEntryRules(row, col);

                    if(possibleEntries.Count != 0) {
                        // Selects a random entry direction from all the possible entries
                        Tile.CardinalPoints entryDirection = possibleEntries[Random.Range(0, possibleEntries.Count)];
                        turnExit = Tile.CardinalPoints.None;

                        // Selects all the possible road types depending on the exit rules for every road type
                        List<RoadTypes.RoadType> possibleRoadTypes = CheckRoadExitRules(row, col, entryDirection);

                        if(possibleRoadTypes.Count != 0) {
                            // Selects a random road type from all the possible road types
                            RoadTypes.RoadType roadTypeSelected = possibleRoadTypes[Random.Range(0, possibleRoadTypes.Count)];
                            // Checks if the selected road type can have a crosswalk or not
                            RoadTypes.RoadType[] roadTypes = CheckCrosswalkRules(row, col, roadTypeSelected);

                            if(roadTypes.Length == 1) { // No possible crosswalk
                                roadTypeSelected = roadTypes[0];
                            }
                            else { // Possible crosswalk
                                roadTypeSelected = SelectCrosswalk(roadTypes);
                            }

                            if(roadTypeSelected == RoadTypes.RoadType.Turn) {
                                // Creates the turn object on the tile
                                tiles[row, col].CreateTile(roadTypeSelected, entryDirection, turnExit, false);
                            }
                            else {
                                // Creates the road object on the tile
                                tiles[row, col].CreateTile(roadTypeSelected, entryDirection, false);
                            }
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Selects if a road can have a crosswalk depending on the crosswalk density parameter
    /// </summary>
    /// <param name="roadTypes">road type 0 has no crosswalk, road type 1 has a crosswalk</param>
    private RoadTypes.RoadType SelectCrosswalk(RoadTypes.RoadType[] roadTypes)
    {
        switch(crosswalkDensity) {
            // Low density
            case 0:
                return (Random.value < 0.25) ? roadTypes[1] : roadTypes[0];

            // Medium density
            case 1:
                return (Random.value < 0.5) ? roadTypes[1] : roadTypes[0];

            // High density
            case 2:
                return (Random.value < 0.75) ? roadTypes[1] : roadTypes[0];
        }

        return roadTypes[0];
    }

    /// <summary>
    /// Checks the road entry rules for a tile
    /// </summary>
    /// <param name="row">Tile's row</param>
    /// <param name="column">Tile's column</param>
    /// <returns></returns>
    private List<Tile.CardinalPoints> CheckRoadEntryRules(int row, int column)
    {
        List<Tile.CardinalPoints> possibleEntries = new List<Tile.CardinalPoints>();

        // Checks that it is not the first row
        if(row > 0) {
            // If the Northern tile has a South exit, the actual tile can entry from North
            if(tiles[row - 1, column].GetAllExits().Contains(Tile.CardinalPoints.South)) {
                possibleEntries.Add(Tile.CardinalPoints.North);
            }
        }

        // Checks that it is not the last row
        if(row != (rows - 1)) {
            // If the Southern tile has a North exit, the actual tile can entry from South
            if(tiles[row + 1, column].GetAllExits().Contains(Tile.CardinalPoints.North)) {
                possibleEntries.Add(Tile.CardinalPoints.South);
            }
        }

        // Checks that it is not the first column
        if(column > 0) {
            // If the Western tile has an East exit, the actual tile can entry from West
            if(tiles[row, column - 1].GetAllExits().Contains(Tile.CardinalPoints.East)) {
                possibleEntries.Add(Tile.CardinalPoints.West);
            }
        }

        // Checks that it is not the last column
        if(column != (columns - 1)) {
            // If the Eastern tile has a West exit, the actual tle can entry from East
            if(tiles[row, column + 1].GetAllExits().Contains(Tile.CardinalPoints.West)) {
                possibleEntries.Add(Tile.CardinalPoints.East);
            }
        }

        return possibleEntries;
    }

    /// <summary>
    /// Checks the road exit rules for a tile
    /// </summary>
    /// <param name="row">Tile's row</param>
    /// <param name="column">Tile's column</param>
    /// <param name="entryCardinalPoint">Entry direction</param>
    /// <returns></returns>
    private List<RoadTypes.RoadType> CheckRoadExitRules(int row, int column, Tile.CardinalPoints entryCardinalPoint)
    {
        List<RoadTypes.RoadType> possibleRoadTypes = new List<RoadTypes.RoadType>();
        RoadTypes.RoadType[] roadTypes = { RoadTypes.RoadType.Crossroad, RoadTypes.RoadType.Intersection, RoadTypes.RoadType.Road, RoadTypes.RoadType.RoadEnd, RoadTypes.RoadType.Turn };

        // Checks every possible road type
        foreach(RoadTypes.RoadType roadType in roadTypes) {
            bool roadTypePermitted = true;
            tiles[row, column].CreateTile(roadType, entryCardinalPoint, true);

            // Checks that it is not the first row
            if(row > 0) {
                // If the Northen tile has a South exit, the actual tile can exit to North
                if(tiles[row - 1, column].GetAllExits().Contains(Tile.CardinalPoints.South)) {
                    // Checks if the actual tile has an exit or an entry in the North
                    if((tiles[row, column].GetAllExits().Contains(Tile.CardinalPoints.North) || entryCardinalPoint == Tile.CardinalPoints.North)
                        && roadTypePermitted) {
                        roadTypePermitted = true;

                        // Specific rules for turns road types
                        if(roadType == RoadTypes.RoadType.Turn) {
                            try {
                                // Checks if the left tile has an exit or entry in the East or is empty
                                if(tiles[row, column - 1].GetAllExits().Contains(Tile.CardinalPoints.East)
                                    || tiles[row, column - 1].GetEntryDirection() == Tile.CardinalPoints.East
                                    || tiles[row, column - 1].IsEmpty) {
                                    turnExit = Tile.CardinalPoints.West;
                                }
                                // Checks if the right tile has an exit or entry in the West or is empty
                                else if(tiles[row, column + 1].GetAllExits().Contains(Tile.CardinalPoints.West)
                                    || tiles[row, column + 1].GetEntryDirection() == Tile.CardinalPoints.West
                                    || tiles[row, column + 1].IsEmpty) {
                                    turnExit = Tile.CardinalPoints.East;
                                }
                            }
                            catch {
                                //roadTypePermitted = false;
                            }
                        }
                    }
                    else {
                        roadTypePermitted = false;
                    }
                }
                else if(!tiles[row - 1, column].IsEmpty) {
                    if(tiles[row, column].GetAllExits().Contains(Tile.CardinalPoints.North)) {
                        // If the top tile is not empty and the actual tile has an exit in the North then the road type is not permitted
                        roadTypePermitted = false;
                    }
                }
            }

            // Checks that it is not the last row
            if(row != (rows - 1)) {
                // If the South tile has a North exit, the actual tile can exit to South
                if(tiles[row + 1, column].GetAllExits().Contains(Tile.CardinalPoints.North)) {
                    // Checks if the actual tile has an exit or an entry in the South
                    if((tiles[row, column].GetAllExits().Contains(Tile.CardinalPoints.South) || entryCardinalPoint == Tile.CardinalPoints.South)
                        && roadTypePermitted) {
                        roadTypePermitted = true;

                        // Specific rules for turns road types
                        if(roadType == RoadTypes.RoadType.Turn) {
                            try {
                                // Checks if the left tile has an exit or entry in the East or is empty
                                if(tiles[row, column - 1].GetAllExits().Contains(Tile.CardinalPoints.East)
                                    || tiles[row, column - 1].GetEntryDirection() == Tile.CardinalPoints.East
                                    || tiles[row, column - 1].IsEmpty) {
                                    turnExit = Tile.CardinalPoints.West;
                                }
                                // Checks if the right tile has an exit or entry in the West or is empty
                                else if(tiles[row, column + 1].GetAllExits().Contains(Tile.CardinalPoints.West)
                                    || tiles[row, column + 1].GetEntryDirection() == Tile.CardinalPoints.West
                                    || tiles[row, column + 1].IsEmpty) {
                                    turnExit = Tile.CardinalPoints.East;
                                }
                            }
                            catch {
                                //roadTypePermitted = false;
                            }
                        }
                    }
                    else {
                        roadTypePermitted = false;
                    }
                }
                else if(!tiles[row + 1, column].IsEmpty) {
                    if(tiles[row, column].GetAllExits().Contains(Tile.CardinalPoints.South)) {
                        // If the bottom tile is not empty and the actual tile has an exit in the South then the road type is not permitted
                        roadTypePermitted = false;
                    }
                }
            }

            // Checks that it is not the first column
            if(column > 0) {
                // If the West tile has an East exit, the actual tile can exit to West
                if(tiles[row, column - 1].GetAllExits().Contains(Tile.CardinalPoints.East)) {
                    // Checks if the actual tile has an exit or an entry in the West
                    if((tiles[row, column].GetAllExits().Contains(Tile.CardinalPoints.West) || entryCardinalPoint == Tile.CardinalPoints.West)
                        && roadTypePermitted) {
                        roadTypePermitted = true;

                        // Specific rules for turns road types
                        if(roadType == RoadTypes.RoadType.Turn) {
                            try {
                                // Checks if the top tile has an exit or entry in the South or is empty
                                if(tiles[row - 1, column].GetAllExits().Contains(Tile.CardinalPoints.South)
                                    || tiles[row - 1, column].GetEntryDirection() == Tile.CardinalPoints.South
                                    || tiles[row - 1, column].IsEmpty) {
                                    turnExit = Tile.CardinalPoints.North;
                                }
                                // Checks if the bottom tile has an exit or entry in the North or is empty
                                else if(tiles[row + 1, column].GetAllExits().Contains(Tile.CardinalPoints.North)
                                    || tiles[row + 1, column].GetEntryDirection() == Tile.CardinalPoints.North
                                    || tiles[row + 1, column].IsEmpty) {
                                    turnExit = Tile.CardinalPoints.South;
                                }
                            }
                            catch {
                                //roadTypePermitted = false;
                            }
                        }
                    }
                    else {
                        roadTypePermitted = false;
                    }
                }
                else if(!tiles[row, column - 1].IsEmpty) {
                    if(tiles[row, column].GetAllExits().Contains(Tile.CardinalPoints.West)) {
                        // If the left tile is not empty and the actual tile has an exit in the West then the road type is not permitted
                        roadTypePermitted = false;
                    }
                }
            }

            // Checks that it is not the last column
            if(column != (columns - 1)) {
                // If the East tile has a West exit, the actual tle can exit to East
                if(tiles[row, column + 1].GetAllExits().Contains(Tile.CardinalPoints.West)) {
                    // Checks if the actual tile has an exit or an entry in the East
                    if((tiles[row, column].GetAllExits().Contains(Tile.CardinalPoints.East) || entryCardinalPoint == Tile.CardinalPoints.East)
                        && roadTypePermitted) {
                        roadTypePermitted = true;

                        // Specific rules for turns road types
                        if(roadType == RoadTypes.RoadType.Turn) {
                            try {
                                // Checks if the top tile has an exit or entry in the South or is empty
                                if(tiles[row - 1, column].GetAllExits().Contains(Tile.CardinalPoints.South)
                                    || tiles[row - 1, column].GetEntryDirection() == Tile.CardinalPoints.South
                                    || tiles[row - 1, column].IsEmpty) {
                                    turnExit = Tile.CardinalPoints.North;
                                }
                                // Checks if the bottom tile has an exit or entry in the North or is empty
                                else if(tiles[row + 1, column].GetAllExits().Contains(Tile.CardinalPoints.North)
                                    || tiles[row + 1, column].GetEntryDirection() == Tile.CardinalPoints.North
                                    || tiles[row + 1, column].IsEmpty) {
                                    turnExit = Tile.CardinalPoints.South;
                                }
                            }
                            catch {
                                //roadTypePermitted = false;
                            }
                        }
                    }
                    else {
                        roadTypePermitted = false;
                    }
                }
                else if(!tiles[row, column + 1].IsEmpty) {
                    if(tiles[row, column].GetAllExits().Contains(Tile.CardinalPoints.East)) {
                        // If the right tile is not empty and the actual tile has an exit in the East then the road type is not permitted
                        roadTypePermitted = false;
                    }
                }
            }

            // If the turn does not have an exit then the road type is not permitted
            if(roadType == RoadTypes.RoadType.Turn && turnExit == Tile.CardinalPoints.None) {
                roadTypePermitted = false;
            }

            // If the road type is permitted add it to the possible road types
            if(roadTypePermitted) {
                possibleRoadTypes.Add(roadType);
            }
        }

        return possibleRoadTypes;
    }

    /// <summary>
    /// Change the crosswalk density parameter with the slider
    /// </summary>
    public void ChangeCrosswalkDensity()
    {
        crosswalkDensity = (int)crosswalkDensitySlider.value;
    }

    /// <summary>
    /// Checks for the crosswalk rules of the tile
    /// </summary>
    /// <param name="row">Tile's row</param>
    /// <param name="column">Tile's column</param>
    /// <param name="roadType">Tile's road type</param>
    private RoadTypes.RoadType[] CheckCrosswalkRules(int row, int column, RoadTypes.RoadType roadType)
    {
        bool canCrosswalk = true;

        // Turns cannot have crosswalks
        if(roadType == RoadTypes.RoadType.Turn) {
            return new RoadTypes.RoadType[] { RoadTypes.RoadType.Turn };
        }
        // Road ends cannot have crosswalks
        else if(roadType == RoadTypes.RoadType.RoadEnd) {
            return new RoadTypes.RoadType[] { RoadTypes.RoadType.RoadEnd };
        }

        // Crosswalks constraints //

        // Checks that it is not the first row
        if(row > 0) {
            // If the top tile has already a crosswalk then the actual tile cannot have a crosswalk
            canCrosswalk = (!tiles[row - 1, column].HasCrosswalk && canCrosswalk) ? true : false;
        }

        // Checks that it is not the last row
        if(row != (rows - 1)) {
            // If the bottom tile has already a crosswalk then the actual tile cannot have a crosswalk
            canCrosswalk = (!tiles[row + 1, column].HasCrosswalk && canCrosswalk) ? true : false;
        }

        // Checks that it is not the first column
        if(column > 0) {
            // If the left tile has already a crosswalk then the actual tile cannot have a crosswalk
            canCrosswalk = (!tiles[row, column - 1].HasCrosswalk && canCrosswalk) ? true : false;
        }

        // Checks that it is not the last column
        if(column != (columns - 1)) {
            // If the right tile has already a crosswalk then the actual tile cannot have a crosswalk
            canCrosswalk = (!tiles[row, column + 1].HasCrosswalk && canCrosswalk) ? true : false;
        }

        switch(roadType) {
            case RoadTypes.RoadType.Crossroad:
                if(canCrosswalk) {
                    return new RoadTypes.RoadType[] { RoadTypes.RoadType.Crossroad, RoadTypes.RoadType.CrossroadCrosswalk };
                }
                else {
                    return new RoadTypes.RoadType[] { RoadTypes.RoadType.Crossroad };
                }

            case RoadTypes.RoadType.Intersection:
                if(canCrosswalk) {
                    return new RoadTypes.RoadType[] { RoadTypes.RoadType.Intersection, RoadTypes.RoadType.IntersectionCrosswalk };
                }
                else {
                    return new RoadTypes.RoadType[] { RoadTypes.RoadType.Intersection };
                }

            case RoadTypes.RoadType.Road:
                if(canCrosswalk) {
                    return new RoadTypes.RoadType[] { RoadTypes.RoadType.Road, RoadTypes.RoadType.RoadCrosswalk };
                }
                else {
                    return new RoadTypes.RoadType[] { RoadTypes.RoadType.Road };
                }
        }

        return null;
    }
}