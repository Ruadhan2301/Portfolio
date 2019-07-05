using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkableTile : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Vector3 scale = transform.localScale;
        Vector3 position = transform.position;
        float width = 5f * scale.x;
        float length = 5f * scale.z;
        float tileWidth = width*2;
        float tileLength = length*2;// Mathf.Round(length);
        Vector3 rootPos = position - new Vector3(Mathf.Floor(width), 0, Mathf.Floor(length));
        for (int i = 0; i < tileWidth; i++)
        {
            for (int j = 0; j < tileLength; j++)
            {
                TraversableReference.Instance.RestrictTile(rootPos.x + i, rootPos.z + j);
            }

        }

        //PathfindingManagerMk2.Instance.RestrictTile();
    }

}
