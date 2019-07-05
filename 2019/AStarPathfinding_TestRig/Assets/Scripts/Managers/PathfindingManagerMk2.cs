using System;
using System.Collections.Generic;
using UnityEngine;

public class PathfindingManagerMk2
{
    private static float GridDensity = 1f;
    Dictionary<string, MapNode> OpenNodes = new Dictionary<string, MapNode>();
    Dictionary<string, MapNode> ClosedNodes = new Dictionary<string, MapNode>();

    Vector3 Origin;
    Vector3 Destination;

    List<Vector3> ScanOffsets = new List<Vector3>()
    {
        new Vector3(0,0,GridDensity),
        new Vector3(GridDensity,0,0),
        new Vector3(-GridDensity,0,0),
        new Vector3(0,0,-GridDensity),

        new Vector3(GridDensity,0,GridDensity),
        new Vector3(-GridDensity,0,GridDensity),
        new Vector3(GridDensity,0,-GridDensity),
        new Vector3(-GridDensity,0,-GridDensity)
    };
    
    public PathfindingManagerMk2()
    {

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

    private float CalculateHeuristic(MapNode Node, Vector3 Start, Vector3 End)
    {
        bool IsOriginNode = false;
        float GScore = 0f;
        float HScore = 0f;
        int Breakout = 0;
        MapNode ParentNode = Node;
        while (!IsOriginNode && Breakout < 1000)
        {
            Breakout++;
            ParentNode = Node.parentNode;


            if (ParentNode == null)
            {
                IsOriginNode = true;
            }
            else
            {
                GScore += 1; // number of squares down the list to the Origin
            }
        }
        HScore = 0f;
        double Manhattan = Math.Round(((Node.position - End)).magnitude / GridDensity);
        HScore = (Node.position - End).magnitude / GridDensity;
        float Difficulty = 1f;// Terrain.activeTerrain.SampleHeight(Node.position);
        return GScore + HScore + (Difficulty * 2);
    }

    private MapNode getHeaderNode()
    {

        float BestHeuristic = 99999999;
        MapNode BestNode = null;

        var keys = OpenNodes.Keys;
        var keyCount = keys.Count;

        foreach (string key in keys)
        {
            MapNode Node = OpenNodes[key];
            float Heuristic = Node.heuristic;
            if (Heuristic < BestHeuristic)
            {
                BestHeuristic = Heuristic;
                BestNode = Node;
            }

        }
        return BestNode;
    }


    private bool ScanNodeToNode(Vector3 A, Vector3 B)
    {
        List<Vector2> availableTiles = TraversableReference.Instance.GetTraversableTiles();
        Vector2 testNodeLoc = new Vector2(B.x, B.z);
        if (availableTiles.Contains(testNodeLoc))
        {
            return true;
        }
        return false; // We never hit anything because this system isn't workable yet

    }
    private void CreateNode(Vector3 Coords, MapNode ParentNode)
    {

        Coords = RoundVectorToGrid(Coords);
        string NodeID = "node_" + Coords.x + "_" + Coords.z;
        if (ClosedNodes.ContainsKey(NodeID))
        {
            return;
        }
        if ((ParentNode == null || ScanNodeToNode(Coords, ParentNode.position)))
        {
            if (!OpenNodes.ContainsKey(NodeID))
            {
                MapNode NewNode = new MapNode();
                NewNode.id = NodeID;
                NewNode.position = Coords;
                NewNode.parentNode = ParentNode;
                NewNode.heuristic = CalculateHeuristic(NewNode, Origin, Destination);
                OpenNodes[NodeID] = NewNode;
            }
            else
            {
                MapNode ExistingNode = OpenNodes[NodeID];
                float ASquareHeuristic = ExistingNode.heuristic;
                float PSquareHeuristic = ParentNode.heuristic;
                if (ASquareHeuristic < PSquareHeuristic)
                {
                    ParentNode.parentNode = ExistingNode;
                }
            }
        }
    }



    private void ScanLocalNodes(MapNode ThisNode)
    {
        Vector3 RootSpot = ThisNode.position;
        bool hasNorth = false;
        bool hasSouth = false;
        bool hasWest = false;
        bool hasEast = false;

        /*
        new Vector3(GridDensity,0,GridDensity), East/North
        new Vector3(-GridDensity,0,GridDensity), West/North
        new Vector3(GridDensity,0,-GridDensity), East/South
        new Vector3(-GridDensity,0,-GridDensity) West/South
        */

        for (var i = 0; i < ScanOffsets.Count; i++)
        {
            Vector3 NewPos = RootSpot + ScanOffsets[i];


            bool CanCreateNode = ScanNodeToNode(RootSpot, NewPos);
            switch (i)
            {
                case 4:
                    if(!hasEast || !hasNorth)
                    {
                        CanCreateNode = false;
                    }
                    break;
                case 5:
                    if (!hasWest || !hasNorth)
                    {
                        CanCreateNode = false;
                    }
                    break;
                case 6:
                    if (!hasEast || !hasSouth)
                    {
                        CanCreateNode = false;
                    }
                    break;
                case 7:
                    if (!hasWest || !hasSouth)
                    {
                        CanCreateNode = false;
                    }
                    break;
            }

            if (CanCreateNode)
            {
                switch (i)
                {
                    case 0:
                        hasNorth = true;
                        break;
                    case 1:
                        hasEast = true;
                        break;
                    case 2:
                        hasWest = true;
                        break;
                    case 3:
                        hasSouth = true;
                        break;
                }
                CreateNode(NewPos, ThisNode);
            }

        }
    }

    public List<Vector3> GeneratePath(Vector3 Start, Vector3 End, Boolean forceNew = false)
    {
        List<Vector3> OutputPath = new List<Vector3>();
        
        OpenNodes = new Dictionary<string, MapNode>();
        ClosedNodes = new Dictionary<string, MapNode>();

        Origin = RoundVectorToGrid(Start);
        Destination = RoundVectorToGrid(End);
        Origin.y = 0;
        Destination.y = 0;
        if(Origin == Destination)
        {
            return OutputPath;// return the empty list. We're already there.
        }
        Debug.Log("Navigating from " + Origin + " to " + Destination);
        string DestinationID = "node_" + Destination.x + "_" + Destination.z;
        string pathID = "path_" + Origin.x + "_" + Origin.z + "_to_" + Destination.x + "_" + Destination.z;
        if (TraversableReference.Instance.HistoricalPaths.ContainsKey(pathID))
        {
            return TraversableReference.Instance.GetHistoricPath(pathID);
        }
        CreateNode(Origin, null);
        int iterationCount = 0;
        do
        {
            MapNode HeaderNode = getHeaderNode();
            if(HeaderNode == null)
            {
                Console.WriteLine("wtf");
            }
            ClosedNodes.Add(HeaderNode.id, HeaderNode);
            OpenNodes.Remove(HeaderNode.id);
            ScanLocalNodes(HeaderNode);
            iterationCount++;

        } while (iterationCount < 10000 && !ClosedNodes.ContainsKey(DestinationID) && OpenNodes.Count > 0);
        if (ClosedNodes.ContainsKey(DestinationID))
        {
            MapNode ParentNode = ClosedNodes[DestinationID];
            bool IsOriginNode = false;
            iterationCount = 0;
            do
            {
                OutputPath.Add(ParentNode.position);
                ParentNode = ParentNode.parentNode;
                if (ParentNode == null || ParentNode.parentNode == null)
                {
                    IsOriginNode = true;
                }
            } while (!IsOriginNode && iterationCount < 10000);
            OutputPath.Reverse();
            TraversableReference.Instance.HistoricalPaths.Add(pathID,OutputPath);
        }
        else
        {

            iterationCount = 0;
            Debug.Log("Didn't Find Path: " + DestinationID);
            return new List<Vector3>();
        }
        iterationCount = 0;
        return TraversableReference.Instance.GetHistoricPath(pathID);
    }

}