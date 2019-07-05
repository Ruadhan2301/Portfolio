using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfindingManager
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

    public PathfindingManager()
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


                if (ParentNode == null){
                    IsOriginNode = true;
                }else{
                    GScore += 1; // number of squares down the list to the Origin
                }
            }
        HScore = 0f;
        double Manhattan = Math.Round(((Node.position - End)).magnitude / GridDensity);
        HScore = (Node.position - End).magnitude / GridDensity;
        float Difficulty = Terrain.activeTerrain.SampleHeight(Node.position);
        return GScore + HScore + (Difficulty*2);
    }

    private MapNode getHeaderNode()
    {

        float BestHeuristic = 99999999;
        MapNode BestNode = null;

        var keys = OpenNodes.Keys;
        var keyCount = keys.Count;

        foreach(string key in keys){
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
        /*RaycastHit hit;
        if (Physics.Raycast(A, A-B, out hit, (A-B).magnitude))
        {
            //Debug.DrawRay(A, (A-B).normalized * hit.distance, Color.yellow);
            //Debug.Log("Did Hit");
            return false;
        }
        else
        {
            //Debug.DrawRay(A, (A - B).normalized * hit.distance, Color.white);
            //Debug.Log("Did not Hit");
            return true;
        }*/
        return true; // We never hit anything because this system isn't workable yet

    }
    private void CreateNode(Vector3 Coords, MapNode ParentNode)
    {

        Coords = RoundVectorToGrid(Coords);
        string NodeID = "node_" + Coords.x + "_" + Coords.z;
        if (ClosedNodes.ContainsKey(NodeID)){
            return;
        }
        if ((ParentNode == null || ScanNodeToNode(Coords, ParentNode.position)))
        {
            if (!OpenNodes.ContainsKey(NodeID)){
                MapNode NewNode = new MapNode();
                NewNode.id = NodeID;
                NewNode.position = Coords;
                NewNode.parentNode = ParentNode;
                NewNode.heuristic = CalculateHeuristic(NewNode, Origin, Destination);
                OpenNodes[NodeID] = NewNode;
            }else{
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
                
        for (var i = 0; i< ScanOffsets.Count; i++)
        {
            Vector3 NewPos = RootSpot + ScanOffsets[i];


            bool CanCreateNode = ScanNodeToNode(RootSpot, NewPos);
            if (CanCreateNode)
            {
                CreateNode(NewPos, ThisNode);
            }

        }
    }

    public List<Vector3> GeneratePath(Vector3 Start, Vector3 End)
    {
        Debug.Log("starting path generation");
        List<Vector3> OutputPath = new List<Vector3>();
        OpenNodes = new Dictionary<string, MapNode>();
        ClosedNodes = new Dictionary<string, MapNode>();

        Origin = RoundVectorToGrid(Start);
        Destination = RoundVectorToGrid(End);
        Origin.y = 0;
        Destination.y = 0;
        Debug.Log("Navigating from " + Origin + " to " + Destination);
        string DestinationID = "node_" + Destination.x + "_" + Destination.z;
        CreateNode(Origin, null);
        int iterationCount = 0;
        do
        {
            MapNode HeaderNode = getHeaderNode();
            ClosedNodes.Add(HeaderNode.id, HeaderNode);
            OpenNodes.Remove(HeaderNode.id);              
            ScanLocalNodes(HeaderNode);
            iterationCount++;
             
        } while (iterationCount < 10000 && !ClosedNodes.ContainsKey(DestinationID));
        if (ClosedNodes.ContainsKey(DestinationID))
        {
            Debug.Log("Found path");
            MapNode ParentNode = ClosedNodes[DestinationID];
            bool IsOriginNode = false;
            iterationCount = 0;
            do
            {
                OutputPath.Add(ParentNode.position);
                ParentNode = ParentNode.parentNode;
                if (ParentNode.parentNode == null)
                {
                    IsOriginNode = true;
                }
            } while (!IsOriginNode && iterationCount < 10000);
            if (IsOriginNode)
            {
                Debug.Log("Pathing Complete");
            }
        }
        else
        {

            Debug.Log("Didn't Find Path: " + DestinationID);
        }
        iterationCount = 0;
        OutputPath.Reverse();
        return OutputPath;
    }

}