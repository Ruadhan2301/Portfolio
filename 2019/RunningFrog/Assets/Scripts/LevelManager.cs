using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Linq;
using SimpleJSON;
using MiniJSON;

public class LevelManager : MonoBehaviour
{
    public GameObject prefab_rock;
    public GameObject prefab_log;

    public static float running_speed_base = 0.05f;

    public float running_speed = 0.0f;
    public float actual_distance = 0f;
    public float current_distance = 0f;
    public int current_score = 0;

    public List<GameObject> obstacles = new List<GameObject>();
    private UIManager uiManager = new UIManager();

    // Start is called before the first frame update
    void Start()
    {
        //createObstacle(prefab_rock);
        readJsonFile("Assets/LevelData/level_0.json");
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.F))
        {
            if (running_speed == 0f)
            {
                running_speed = running_speed_base;
            }
            else
            {
                running_speed = 0f;
            }
        }

        actual_distance += running_speed * Time.deltaTime;
        current_distance = actual_distance * 10f;
        uiManager.SetTextElement("Distance_value", current_distance + "m");

        current_score = (int)(current_distance * 50);
        uiManager.SetTextElement("Score_value", current_score + " points");
    }

    private void readJsonFile(string filePath)
    {
        StreamReader sr = File.OpenText(filePath);
        string content = sr.ReadToEnd();
        Dictionary<string, object> data = null;
        data = Json.Deserialize(content) as Dictionary<string, object>;

        foreach(KeyValuePair<string,object> obstacle in data)
        {
            string obstacleName = obstacle.Key;
            Dictionary<string,object> obstacleData = obstacle.Value as Dictionary<string,object>;
            string distance = obstacleData["distance"] as string;
            int offsetX = 10;
            string type = obstacleData["type"] as string;
            if(int.TryParse(distance, out int result))
            {
                offsetX = result;
                createObstacle(type, offsetX, obstacleName);
            }
            else
            {
                Debug.LogWarning("couldn't parse distance");
            }
        }

    }

    private void createObstacle(string prefabType, int offsetX = 10, string obstacleName = "rock")
    {
        if (prefabType == "rock")
        {
            createObstacle(prefab_rock, offsetX, obstacleName);
        }
        if (prefabType == "log")
        {
            createObstacle(prefab_log, offsetX, obstacleName);
        }
    }

        private void createObstacle(GameObject prefab, int offsetX = 10, string obstacleName = "rock")
    {
        GameObject newObstacle = GameObject.Instantiate<GameObject>(prefab);
        ObstacleController obstacleControllerScript = newObstacle.GetComponent<ObstacleController>();
        obstacleControllerScript.manager = this;
        newObstacle.name = obstacleName;
        newObstacle.transform.position = new Vector3(offsetX, 0, 0);
    }
}
