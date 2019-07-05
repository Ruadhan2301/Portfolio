using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleController : MonoBehaviour
{

    public float speed = 0.05f;
    public LevelManager manager;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 oldPos = transform.position;
        Vector3 newPos = oldPos + new Vector3(-manager.running_speed, 0, 0);
        transform.position = newPos;

        if (transform.position.x < -10)
        {
            Destroy(gameObject);
            //transform.position = new Vector3(10, 0, 0);
            //manager.running_speed += 0.01f;
        }
    }
}
