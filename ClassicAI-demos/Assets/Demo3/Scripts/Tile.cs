using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    #region variables

    public bool IsEmpty { get; set; } = true;
    public bool HasCrosswalk { get; set; }

    public enum CardinalPoints
    {
        None,
        North,
        East,
        South,
        West
    }

    [SerializeField] private RoadTypes.RoadType roadType;
    [SerializeField] private CardinalPoints entryDirection;
    [SerializeField] private CardinalPoints exitDirection_1;
    [SerializeField] private CardinalPoints exitDirection_2;
    [SerializeField] private CardinalPoints exitDirection_3;

    #endregion variables

    /// <summary>
    /// Tile constructor for all the road types but the turns
    /// </summary>
    /// <param name="roadType">Road type contained in the tile</param>
    /// <param name="entryDirection">Entry direction of the road</param>
    /// <param name="simulate">Is the tile being simulated or not</param>
    public void CreateTile(RoadTypes.RoadType roadType, CardinalPoints entryDirection, bool simulate)
    {
        this.roadType = roadType;
        this.entryDirection = entryDirection;
        exitDirection_1 = CardinalPoints.None;
        exitDirection_2 = CardinalPoints.None;
        exitDirection_3 = CardinalPoints.None;

        TilesGroup.TileGroup.TryGetValue(roadType, out GameObject tileObject); // Gets the instantiable object of the road type
        CreatePiece(tileObject, simulate);
        IsEmpty = simulate;
    }

    /// <summary>
    /// Tile constructor for the turns
    /// </summary>
    /// <param name="roadType">Road type contained in the tile</param>
    /// <param name="entryDirection">Entry direction of the road</param>
    /// <param name="exitDirection">Exit direction of the road</param>
    /// <param name="simulate">Is the tile being simulated?</param>
    public void CreateTile(RoadTypes.RoadType roadType, CardinalPoints entryDirection, CardinalPoints exitDirection, bool simulate)
    {
        this.roadType = roadType;
        this.entryDirection = entryDirection;
        exitDirection_1 = exitDirection;
        exitDirection_2 = CardinalPoints.None;
        exitDirection_3 = CardinalPoints.None;

        TilesGroup.TileGroup.TryGetValue(roadType, out GameObject tileObject); // Gets the instantiable object of the road type
        CreatePiece(tileObject, simulate);
        IsEmpty = simulate;
    }

    /// <summary>
    /// Creates the object of the tile
    /// </summary>
    /// <param name="tileObject">Instantiable road object</param>
    /// <param name="simulate">Is the tile being simulated?</param>
    private void CreatePiece(GameObject tileObject, bool simulate)
    {
        switch(roadType) {
            case RoadTypes.RoadType.Crossroad:
                HasCrosswalk = false;
                AdjustCrossroad();
                break;

            case RoadTypes.RoadType.CrossroadCrosswalk:
                HasCrosswalk = true;
                AdjustCrossroad();
                break;

            case RoadTypes.RoadType.Intersection:
                HasCrosswalk = false;
                AdjustIntersection();
                break;

            case RoadTypes.RoadType.IntersectionCrosswalk:
                HasCrosswalk = true;
                AdjustIntersection();
                break;

            case RoadTypes.RoadType.RoadEnd:
                HasCrosswalk = false;
                AdjustRoadEnd();
                break;

            case RoadTypes.RoadType.Road:
                HasCrosswalk = false;
                AdjustRoad();
                break;

            case RoadTypes.RoadType.RoadCrosswalk:
                HasCrosswalk = true;
                AdjustRoad();
                break;

            case RoadTypes.RoadType.Turn:
                HasCrosswalk = false;
                AdjustTurn();
                break;
        }

        // If the tile is not being simulated instantiates the road object on the tile
        if(!simulate) {
            Instantiate(tileObject, transform);
        }
    }

    /// <summary>
    /// Adjust the exits of the crossroad object depending on the entry direction
    /// </summary>
    private void AdjustCrossroad()
    {
        switch(entryDirection) {
            case CardinalPoints.South:
                exitDirection_1 = CardinalPoints.West;
                exitDirection_2 = CardinalPoints.North;
                exitDirection_3 = CardinalPoints.East;
                break;

            case CardinalPoints.West:
                exitDirection_1 = CardinalPoints.North;
                exitDirection_2 = CardinalPoints.East;
                exitDirection_3 = CardinalPoints.South;
                break;

            case CardinalPoints.North:
                exitDirection_1 = CardinalPoints.East;
                exitDirection_2 = CardinalPoints.South;
                exitDirection_3 = CardinalPoints.West;
                break;

            case CardinalPoints.East:
                exitDirection_1 = CardinalPoints.South;
                exitDirection_2 = CardinalPoints.West;
                exitDirection_3 = CardinalPoints.North;
                break;
        }
    }

    /// <summary>
    /// Adjust the rotation and exits of the intersection object depending on the entry direction
    /// </summary>
    private void AdjustIntersection()
    {
        switch(entryDirection) {
            case CardinalPoints.South:
                exitDirection_1 = CardinalPoints.West;
                exitDirection_2 = CardinalPoints.East;
                transform.localRotation = Quaternion.Euler(0, 180, 0);
                break;

            case CardinalPoints.West:
                exitDirection_1 = CardinalPoints.North;
                exitDirection_2 = CardinalPoints.South;
                transform.localRotation = Quaternion.Euler(0, 270, 0);
                break;

            case CardinalPoints.North:
                exitDirection_1 = CardinalPoints.West;
                exitDirection_2 = CardinalPoints.East;
                transform.localRotation = Quaternion.Euler(0, 0, 0);
                break;

            case CardinalPoints.East:
                exitDirection_1 = CardinalPoints.North;
                exitDirection_2 = CardinalPoints.South;
                transform.localRotation = Quaternion.Euler(0, 90, 0);
                break;
        }
    }

    /// <summary>
    /// Adjust the rotation and exits of the road end object depending on the entry direction
    /// </summary>
    private void AdjustRoadEnd()
    {
        switch(entryDirection) {
            case CardinalPoints.South:
                exitDirection_1 = CardinalPoints.None;
                transform.localRotation = Quaternion.Euler(0, 270, 0);
                break;

            case CardinalPoints.West:
                exitDirection_1 = CardinalPoints.None;
                transform.localRotation = Quaternion.Euler(0, 0, 0);
                break;

            case CardinalPoints.North:
                exitDirection_1 = CardinalPoints.None;
                transform.localRotation = Quaternion.Euler(0, 90, 0);
                break;

            case CardinalPoints.East:
                exitDirection_1 = CardinalPoints.None;
                transform.localRotation = Quaternion.Euler(0, 180, 0);
                break;
        }
    }

    /// <summary>
    /// Adjust the rotation and exits of the road object depending on the entry direction
    /// </summary>
    private void AdjustRoad()
    {
        switch(entryDirection) {
            case CardinalPoints.South:
                exitDirection_1 = CardinalPoints.North;
                transform.localRotation = Quaternion.Euler(0, 90, 0);
                break;

            case CardinalPoints.West:
                exitDirection_1 = CardinalPoints.East;
                transform.localRotation = Quaternion.Euler(0, 0, 0);
                break;

            case CardinalPoints.North:
                exitDirection_1 = CardinalPoints.South;
                transform.localRotation = Quaternion.Euler(0, 90, 0);
                break;

            case CardinalPoints.East:
                exitDirection_1 = CardinalPoints.West;
                transform.localRotation = Quaternion.Euler(0, 0, 0);
                break;
        }
    }

    /// <summary>
    /// Adjust the roation of the turn depending on the entry direction and the exit direction
    /// </summary>
    private void AdjustTurn()
    {
        switch(entryDirection) {
            case CardinalPoints.South:
                if(exitDirection_1 == CardinalPoints.East) {
                    transform.localRotation = Quaternion.Euler(0, 90, 0);
                }
                else if(exitDirection_1 == CardinalPoints.West) {
                    transform.localRotation = Quaternion.Euler(0, 180, 0);
                }
                break;

            case CardinalPoints.West:
                if(exitDirection_1 == CardinalPoints.South) {
                    transform.localRotation = Quaternion.Euler(0, 180, 0);
                }
                else if(exitDirection_1 == CardinalPoints.North) {
                    transform.localRotation = Quaternion.Euler(0, 270, 0);
                }
                break;

            case CardinalPoints.North:
                if(exitDirection_1 == CardinalPoints.West) {
                    transform.localRotation = Quaternion.Euler(0, 270, 0);
                }
                else if(exitDirection_1 == CardinalPoints.East) {
                    transform.localRotation = Quaternion.Euler(0, 0, 0);
                }
                break;

            case CardinalPoints.East:
                if(exitDirection_1 == CardinalPoints.North) {
                    transform.localRotation = Quaternion.Euler(0, 0, 0);
                }
                else if(exitDirection_1 == CardinalPoints.South) {
                    transform.localRotation = Quaternion.Euler(0, 90, 0);
                }
                break;
        }
    }

    /// <summary>
    /// Returns the entry direction of the tile
    /// </summary>
    public CardinalPoints GetEntryDirection()
    {
        return entryDirection;
    }

    /// <summary>
    /// Returns all possible exits of the tile
    /// </summary>
    public List<CardinalPoints> GetAllExits()
    {
        List<CardinalPoints> exits = new List<CardinalPoints>();

        if(exitDirection_1 != CardinalPoints.None) {
            exits.Add(exitDirection_1);
        }
        if(exitDirection_2 != CardinalPoints.None) {
            exits.Add(exitDirection_2);
        }
        if(exitDirection_3 != CardinalPoints.None) {
            exits.Add(exitDirection_3);
        }

        return exits;
    }
}