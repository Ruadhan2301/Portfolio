using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager
{
    Dictionary<string, GameObject> uiElements = new Dictionary<string, GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetTextElement(string elementName,string content)
    {
        GameObject selectedElement = null;
        if (uiElements.ContainsKey(elementName))
        {
            selectedElement = uiElements[elementName];
        }
        else
        {
            GameObject result = GameObject.Find(elementName);
            if(result == null)
            {
                return;
            }
            else
            {
                uiElements.Add(elementName, result);
                selectedElement = result;
            }
        }
        Text textElement = selectedElement.GetComponent<Text>();
        textElement.text = content;
    }
}
