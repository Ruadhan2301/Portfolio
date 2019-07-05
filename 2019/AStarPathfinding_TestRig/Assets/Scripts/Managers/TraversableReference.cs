using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TraversableReference
{

    private static float GridDensity = 1f;
    public static TraversableReference _instance;

    public static TraversableReference Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new TraversableReference();
            }
            return _instance;
        }
    }
    private List<Vector2> availableTiles = new List<Vector2>();
    private Dictionary<Vector3, DoorScript> mapDoors = new Dictionary<Vector3, DoorScript>();

    public Dictionary<string, List<Vector3>> HistoricalPaths = new Dictionary<string, List<Vector3>>();

    public List<Vector3> GetHistoricPath(string pathId)
    {
        List<Vector3> OldPath = TraversableReference.Instance.HistoricalPaths[pathId];
        int pathLength = OldPath.Count;
        List<Vector3> outputPath = new List<Vector3>();
        for (int i = 0; i < pathLength; i++)
        {
            Vector3 pathNode = OldPath[i];
            outputPath.Add(new Vector3(pathNode.x, pathNode.y, pathNode.z));
        }
        return outputPath;
    }

    private Vector3 RoundVectorToGrid(Vector3 input)
    {
        Vector3 output = input / GridDensity;
        double X = Math.Round(output.x);
        double Y = Math.Round(output.y);
        double Z = Math.Round(output.z);
        output = new Vector3((float)X, (float)Y, (float)Z) * GridDensity;
        return output;
    }
    public void RestrictTile(float x, float z)
    {
        Vector3 roundedVec = RoundVectorToGrid(new Vector3(x, 0, z));
        //Debug.Log("Adding traversable tile: " + roundedVec.x  + "," + roundedVec.z);
        availableTiles.Add(new Vector2(roundedVec.x, roundedVec.z));
    }

    public void AddDoorToTile(Vector3 tile, DoorScript door)
    {
        if (!mapDoors.ContainsKey(tile))
        {
            mapDoors.Add(tile, door);
        }
    }

    public List<Vector2> GetTraversableTiles()
    {
        return availableTiles;
    }

    public DoorScript GetDoorAtTile(Vector3 tile)
    {
        if (mapDoors.ContainsKey(tile))
        {
            return mapDoors[tile];
        }
        return null;
    }

    public bool GetTileContainsDoor(Vector3 tile)
    {
        if (mapDoors.ContainsKey(tile))
        {
            return true;
        }
        return false;
    }
}
