using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    #region variables

    public enum CardinalPoints
    {
        North,
        East,
        South,
        West
    }

    [SerializeField] private RoadTypes.RoadType roadType;
    [SerializeField] private CardinalPoints entryDirection;
    [SerializeField] private CardinalPoints exitDirection;

    #endregion variables

    public void CreateTile(RoadTypes.RoadType roadType, CardinalPoints entryDirection, CardinalPoints exitDirection, TilesGroup tilesGroup)
    {
        this.roadType = roadType;
        this.entryDirection = entryDirection;
        this.exitDirection = exitDirection;

        if(roadType == RoadTypes.RoadType.Road) {
            GameObject tileObject;
            tilesGroup.TileGroup.TryGetValue(roadType, out tileObject);

            Instantiate(tileObject, transform);
        }
    }
}