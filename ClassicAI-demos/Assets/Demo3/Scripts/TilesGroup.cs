using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TilesGroup : MonoBehaviour
{
    #region variables

    public Dictionary<RoadTypes.RoadType, GameObject> TileGroup { get; set; }

    [SerializeField] private GameObject crossroadCrosswalk;
    [SerializeField] private GameObject crossroad;
    [SerializeField] private GameObject intersectionCrosswalk;
    [SerializeField] private GameObject intersection;
    [SerializeField] private GameObject roadEnd;
    [SerializeField] private GameObject roadCrosswalk;
    [SerializeField] private GameObject road;
    [SerializeField] private GameObject roundabout;
    [SerializeField] private GameObject turn;

    #endregion variables

    // Start is called before the first frame update
    private void Start()
    {
        TileGroup = new Dictionary<RoadTypes.RoadType, GameObject>();

        TileGroup.Add(RoadTypes.RoadType.CrossroadCrosswalk, crossroadCrosswalk);
        TileGroup.Add(RoadTypes.RoadType.Crossroad, crossroad);
        TileGroup.Add(RoadTypes.RoadType.IntersectionCrosswalk, intersectionCrosswalk);
        TileGroup.Add(RoadTypes.RoadType.Intersection, intersection);
        TileGroup.Add(RoadTypes.RoadType.RoadEnd, roadEnd);
        TileGroup.Add(RoadTypes.RoadType.RoadCrosswalk, roadCrosswalk);
        TileGroup.Add(RoadTypes.RoadType.Road, road);
        TileGroup.Add(RoadTypes.RoadType.Roundabout, roundabout);
        TileGroup.Add(RoadTypes.RoadType.Turn, turn);
    }
}