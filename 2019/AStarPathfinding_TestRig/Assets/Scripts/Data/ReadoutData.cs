using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReadoutData
{


    public Dictionary<string, string> FlagDisplayValues = new Dictionary<string, string>
    {
        ["hunger_0"] = "Satiated",
        ["hunger_1"] = "Peckish",
        ["hunger_2"] = "Pretty Hungry",
        ["hunger_3"] = "Really Hungry",
        ["hunger_4"] = "Starving",

        ["energy_0"] = "Energised",
        ["energy_1"] = "A Little Tired",
        ["energy_2"] = "Definitely Tired",
        ["energy_3"] = "Exhausted",
        ["energy_4"] = "Totally Exhausted",
    };

    public static ReadoutData _instance;

    public static ReadoutData Instance
    {
        get {
            if(_instance == null)
            {
                _instance = new ReadoutData();
            }
            return _instance;
        }
    }


    public ReadoutData()
    {

    }

    public string GetDisplayableFlagList(List<string> flags)
    {
        string outputString = "";
        int flagCount = flags.Count;
        for (int i = 0; i < flagCount; i++)
        {
            string newStr = flags[i];
            if (FlagDisplayValues.ContainsKey(newStr))
            {
                if (outputString != "")
                {
                    outputString += ", ";
                }
                outputString += FlagDisplayValues[newStr];
            }
        }

        return outputString;
    }




}
