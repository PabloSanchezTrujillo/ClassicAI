using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TilesGroup
{
    #region variables

    public static Dictionary<RoadTypes.RoadType, GameObject> TileGroup { get; set; }

    #endregion variables

    // Start is called before the first frame update
    public static void CreateGroup(GameObject crossroadCrosswalk, GameObject crossroad, GameObject intersectionCrosswalk, GameObject intersection,
        GameObject roadEnd, GameObject roadCrosswalk, GameObject road, GameObject roundabout, GameObject turn)
    {
        TileGroup = new Dictionary<RoadTypes.RoadType, GameObject>();

        TileGroup.Add(RoadTypes.RoadType.CrossroadCrosswalk, crossroadCrosswalk);
        TileGroup.Add(RoadTypes.RoadType.Crossroad, crossroad);
        TileGroup.Add(RoadTypes.RoadType.IntersectionCrosswalk, intersectionCrosswalk);
        TileGroup.Add(RoadTypes.RoadType.Intersection, intersection);
        TileGroup.Add(RoadTypes.RoadType.RoadEnd, roadEnd);
        TileGroup.Add(RoadTypes.RoadType.RoadCrosswalk, roadCrosswalk);
        TileGroup.Add(RoadTypes.RoadType.Road, road);
        //TileGroup.Add(RoadTypes.RoadType.Roundabout, roundabout);
        TileGroup.Add(RoadTypes.RoadType.Turn, turn);
    }
}