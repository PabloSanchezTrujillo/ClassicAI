using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    #region variables

    public bool IsEmpty { get; set; } = true;

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

    public void CreateTile(RoadTypes.RoadType roadType, CardinalPoints entryDirection, bool simulate)
    {
        this.roadType = roadType;
        this.entryDirection = entryDirection;
        exitDirection_1 = CardinalPoints.None;
        exitDirection_2 = CardinalPoints.None;
        exitDirection_3 = CardinalPoints.None;

        TilesGroup.TileGroup.TryGetValue(roadType, out GameObject tileObject);
        CreatePiece(tileObject, simulate);
        IsEmpty = simulate;
    }

    public void CreateTile(RoadTypes.RoadType roadType, CardinalPoints entryDirection, CardinalPoints exitDirection, bool simulate)
    {
        this.roadType = roadType;
        this.entryDirection = entryDirection;
        exitDirection_1 = exitDirection;
        exitDirection_2 = CardinalPoints.None;
        exitDirection_3 = CardinalPoints.None;

        TilesGroup.TileGroup.TryGetValue(roadType, out GameObject tileObject);
        CreatePiece(tileObject, simulate);
        IsEmpty = simulate;
    }

    private void CreatePiece(GameObject tileObject, bool simulate)
    {
        switch(roadType) {
            case RoadTypes.RoadType.Crossroad:
                AdjustCrossroad();
                break;

            case RoadTypes.RoadType.CrossroadCrosswalk:
                AdjustCrossroad();
                break;

            case RoadTypes.RoadType.Intersection:
                AdjustIntersection();
                break;

            case RoadTypes.RoadType.IntersectionCrosswalk:
                AdjustIntersection();
                break;

            case RoadTypes.RoadType.RoadEnd:
                AdjustRoadEnd();
                break;

            case RoadTypes.RoadType.Road:
                AdjustRoad();
                break;

            case RoadTypes.RoadType.RoadCrosswalk:
                AdjustRoad();
                break;

            case RoadTypes.RoadType.Turn:
                AdjustTurn();
                break;
        }

        if(!simulate) {
            Instantiate(tileObject, transform);
        }
    }

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