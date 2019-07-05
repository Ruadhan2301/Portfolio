using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentScript : MonoBehaviour
{
    public List<Vector3> path = new List<Vector3>();
    public Vector3 Destination;
    // Start is called before the first frame update
    void Start()
    {
        PathfindingManager pathfindingManager = new PathfindingManager();
        path = pathfindingManager.GeneratePath(transform.position, Destination);
    }

    // Update is called once per frame
    void Update()
    {
        if(path.Count > 0)
        {
            Vector3 firstNode = path[0];
            firstNode.y = Terrain.activeTerrain.SampleHeight(firstNode) + 1f;
            

            float step = 10 * Time.deltaTime; // calculate distance to move
            transform.position = Vector3.MoveTowards(transform.position, firstNode, step);
            if(transform.position == firstNode)
            {
                path.RemoveAt(0);
            }

        }
    }
}
