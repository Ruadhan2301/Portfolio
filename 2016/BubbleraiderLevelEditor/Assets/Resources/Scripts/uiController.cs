using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.IO;

public class uiController {

    private EventSystem eventSystem;

    private List<string> inputData;

    private int levelID = 12000;

    private static int levelWidth = 23;

    private static string fileSavePath = @"C:\Projects\levels.txt";

    // Use this for initialization
    public void init () {
        eventSystem = EventSystem.current;
        buildBalls();
        inputData = readTextFile(fileSavePath);
        //List<string> data = loadLevelData(levelID);
        setBubbleType(""); // initialise the UI object with all the outlines disabled
        //assignBallTypes(data);
    }
	
    private void newLevel() {
        List<string> data = loadLevelData(999999);
        assignBallTypes(data);
    }

    private void load(int levelID) {
        List<string> data = loadLevelData(levelID);
        if (data != null) {
            assignBallTypes(data);
        } else {
            newLevel();
        }
        
    }

    /// <summary>
    /// Retrieve the data on the lines for a given levelID
    /// </summary>
    /// <param name="levelID"></param>
    /// <returns></returns>
    private List<string> loadLevelData(int levelID) {
        
        if(inputData != null) {
            List<string> outputData = new List<string>();
            bool record = false;
            for (int i = 0; i < inputData.Count; i++) {

                if (inputData[i] == "#" + levelID) {
                    record = true;
                }
                if (record) {
                    outputData.Add(inputData[i]);
                }
                if(inputData[i] == "#end") {
                    record = false;
                }

            }
            return outputData;
        }
        return null;
    }
    private bool mirrorMode = true;
	// Update is called once per frame
	public void tick () {
        if (Input.GetMouseButtonUp(0)) {
            buttonPressed();
        }
        if (Input.GetMouseButtonUp(1)) {
            GameObject clickedOn = eventSystem.currentSelectedGameObject;
            if (clickedOn != null) {
                string buttonPath = clickedOn.transform.parent.name + "." + clickedOn.name;
                string[] parts = buttonPath.Split('.');
                if (parts[0].Contains("row_")) {
                    assignBubble(clickedOn, "-", mirrorMode);
                }
            }
        }
	}

    private void buttonPressed() {
        GameObject clickedOn = eventSystem.currentSelectedGameObject;
        if(clickedOn != null) {
            string buttonPath = clickedOn.transform.parent.name + "." + clickedOn.name;
            Debug.Log("Clicked on: " + buttonPath);
            operateButtons(buttonPath, clickedOn);
            clickedOn = null;
        }
    }


    private string bubbleType;
    private void operateButtons(string buttonPath, GameObject obj) {
        string[] parts = buttonPath.Split('.');
        if (parts[0] == "bubble_palette") {
            
            string localBubbleType = parts[1].Replace("type_", "");
            setBubbleType(localBubbleType);
        } else if(parts[0] == "mainControls") {
            int level = getlevelInputField();
            switch (parts[1]) {
                case "save":
                    List<string> saveData = generateSaveData(level);
                    List<string> fullSaveData = buildNewTextData(inputData, saveData, level);
                    writeTextFile(fullSaveData, fileSavePath);
                    inputData = readTextFile(fileSavePath);
                    break;
                case "load":
                    load(level);
                    break;
                case "new":
                    newLevel();
                    break;
            }
        }else if (parts[0].Contains("row_") && bubbleType != "") {
            assignBubble(obj, bubbleType, mirrorMode);
        }
            

    }

    private int getlevelInputField() {
        GameObject buildCore = GameObject.Find("layout_base");
        Transform mainControls = buildCore.transform.Find("mainControls");
        Transform levelInput = mainControls.Find("level_input");
        InputField textinput = levelInput.gameObject.GetComponent<InputField>();

        string text = textinput.text;
        int output = int.Parse(text);
        if(output <= 0) {
            return -1;
        }
        return output;
    }

    private void assignBubble(GameObject bubble, string type, bool mirrormode = false) {
        bubble.GetComponent<bubble_data>().setType(getBubbleType(type));
        if (mirrormode) {
            Transform bubbleParent = bubble.transform.parent;
            int rowID = int.Parse(bubbleParent.name.Replace("row_", ""));
            int bubbleID = int.Parse(bubble.name.Replace("ball_", ""));
            int mirrorID = levelWidth - bubbleID - 1;
            if(rowID % 2 != 0) {
                mirrorID++;
            }
            
            Transform mirrorBubble = bubbleParent.Find("ball_" + mirrorID);
            mirrorBubble.GetComponent<bubble_data>().setType(getBubbleType(type));
        }
    }

    private void setBubbleType(string type) {
        bubbleType = type;
        GameObject buildCore = GameObject.Find("bubble_palette");
        foreach(Transform child in buildCore.transform) {
            if(child.gameObject.name == "type_" + type) {
                child.GetComponent<Image>().enabled = true;
            } else {
                child.GetComponent<Image>().enabled = false;
            }
        }
    }
    
    private void assignBallTypes(List<string> data) {
        GameObject buildCore = GameObject.Find("bubbles");
        Transform rowObj = buildCore.transform.Find("row_0");
        for (int row = 0; row < 18; row++) {
            rowObj = buildCore.transform.Find("row_" + row);
            if (data.Count > row + 1) {
                string dataStr = data[row + 1].Replace(" ", "");
                foreach (Transform child in rowObj) {
                    int id = int.Parse(child.gameObject.name.Replace("ball_", ""));
                    string type = dataStr.Substring(id, 1);
                    //if (type != "-") {
                    //Sprite spr = Resources.Load<Sprite>("Textures/bubble_" + type + "");
                    //child.GetComponent<Image>().sprite = spr;
                    child.GetComponent<bubble_data>().setType(getBubbleType(type));
                    //}
                }
            }
        }

    }

    private string getBubbleType(string inputType) {
        switch (inputType) {
            case ">":
                return "q";
            case "<":
                return "c";
            default:
                return inputType;
            
        }
    }

    
    /// <summary>
    /// take our existing level design and establish the pattern of data to apply to the original file, eg: bookend with #1234 and #end
    /// </summary>
    private List<string> generateSaveData(int level) {
        List<string> output = new List<string>();
        output.Add("#" + level);
        GameObject buildCore = GameObject.Find("bubbles");
        Transform rowObj = buildCore.transform.Find("row_0");
        //Transform bubbleTemplate = rowObj.Find("ball_0");
        for (int row = 0; row < 18; row++) {
            rowObj = buildCore.transform.Find("row_" + row);
            string rowData = "";
            if (row % 2 == 0) {
                rowData += " ";
            }
            for (int column = 0; column < 23; column++) {

                
                Transform ballTransform = rowObj.Find("ball_" + column);
                string type = ballTransform.gameObject.GetComponent<bubble_data>().getType();

                rowData += type;
                rowData += " ";
            }
            output.Add(rowData);
        }
        output.Add("#end");
        return output;
    }

    private List<string> readTextFile(string file) {
        List<string> lineData = new List<string>();
        if (File.Exists(file)){
            StreamReader sr = File.OpenText(file);
            string line = sr.ReadLine();
            while(line != null){
                lineData.Add(line);
                //Debug.Log(line); // prints each line of the file
                line = sr.ReadLine();
            }  
        } else {
            Debug.Log("Could not Open the file: " + file + " for reading.");
            return null;
        }
        return lineData;
    }

    private List<string> buildNewTextData(List<string> oldData, List<string> appendData, int level) {
        bool updating = false;
        if(oldData == null || appendData == null) {
            return null;
        }
        bool writing = false;
        int j = 0;
        for (int i = 0; i < oldData.Count; i++) {
            if(oldData[i] == "#" + level) {
                updating = true;
                writing = true;
            }
            if (writing) {
                oldData[i] = appendData[j];
                j++;
            }
            if(oldData[i] == "#end") {
                writing = false;
            }
        }
        if (!updating) { // we weren't able to find an existing level with this ID, make a new one!
            oldData.Add("");
            for (int i = 0; i < appendData.Count; i++) {
                oldData.Add(appendData[i]);
            }
        }
        return oldData;
    }

    private void writeTextFile(List<string> data, string fileName) {
        StreamWriter sr = File.CreateText(fileName);
        for (int i = 0; i < data.Count; i++) {
            sr.WriteLine(data[i]);
        }
        sr.Close();
    }


    private void buildBalls() {
        GameObject buildCore = GameObject.Find("bubbles");
        Transform rowObj = buildCore.transform.Find("row_0");
        Transform bubbleTemplate = rowObj.Find("ball_0");
        for (int row = 0; row < 18; row++) {
            rowObj = buildCore.transform.Find("row_" + row);
            for (int column = 0; column < 23; column++) {
                if (column == 0 && row == 0) {
                    bubbleTemplate.GetComponent<bubble_data>().setType("-");
                } else {
                    GameObject newBall = GameObject.Instantiate(bubbleTemplate.gameObject) as GameObject;
                    newBall.transform.SetParent(rowObj);
                    //newBall.name = "ball_" + (column + (row * 23));
                    newBall.name = "ball_" + column;
                    newBall.GetComponent<bubble_data>().setType("-");
                }
            }
        }
    }

}
