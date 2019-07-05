using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Utility class that helps with type conversion in dictionary classes
/// </summary>
public class DicHelper {

    public static Dictionary<string, object> getDictionaryWithObject(IDictionary<string, object> data, string key, Dictionary<string, object> defaultVal = null) {

        Dictionary<string, object> dataDict = defaultVal;

        if (data.ContainsKey(key)) {
            dataDict = (Dictionary<string, object>)data[key];
        }

        return dataDict;
    }

    public static List<Dictionary<string, object>> getDictionaryListWithObject(IDictionary<string, object> data, string key, List<Dictionary<string, object>> defaultVal = null) {
        List<Dictionary<string, object>> dataDict = defaultVal;
        List<object> dataObjects = new List<object>();
        
        if (data.ContainsKey(key)) {
            dataObjects = (List<object>)data[key];
            dataDict = new List<Dictionary<string, object>>();
        }

        for (int i = 0; i < dataObjects.Count; i++) {
            dataDict.Add((Dictionary<string,object>)dataObjects[i]);
        }

        return dataDict;
    }

    public static Dictionary<string, string> getDictionaryWithString(IDictionary<string, object> data, string key, Dictionary<string, string> defaultVal = null) {

        Dictionary<string, string> extractedData = defaultVal;

        if (data.ContainsKey(key)) {
            extractedData = (Dictionary<string, string>)data[key];
        }

        return extractedData;
    }

    public static Dictionary<string, int> getDictionaryWithInt(IDictionary<string, object> data, string key, Dictionary<string, int> defaultVal = null) {

        Dictionary<string, int> extractedData = defaultVal;

        if (data.ContainsKey(key)) {

            Dictionary<string, object> temp = (Dictionary<string, object>) data[key];
            extractedData = new Dictionary<string,int>();

            foreach (KeyValuePair<string, object> item in temp) {
                extractedData.Add(item.Key, int.Parse(item.Value.ToString()));
            }

        }

        return extractedData;
    }

    public static Dictionary<string, long> getDictionaryWithLong(IDictionary<string, object> data, string key, Dictionary<string, long> defaultVal = null) {

        Dictionary<string, long> extractedData = defaultVal;

        if(data.ContainsKey(key)) {

            Dictionary<string, object> temp = (Dictionary<string, object>) data[key];
            extractedData = new Dictionary<string, long>();

            foreach(KeyValuePair<string, object> item in temp) {
                extractedData.Add(item.Key, long.Parse(item.Value.ToString()));
            }

        }

        return extractedData;
    }

    public static List<string> getStringList(IDictionary<string, object> data, string key, List<string> defaultVal = null, string defaultString = null) {

        List<string> list = defaultVal;

        if (data.ContainsKey(key)) {

            IList<object> temp = (IList<object>)data[key];

            list = new List<string>(temp.Count);

            foreach (object item in temp) {
                if (item != null) {
                    list.Add(item.ToString());
                } else {
                    list.Add(defaultString);
                }
            }

        }

        return list;
    }

    public static List<int> getIntList(IDictionary<string, object> data, string key, List<int> defaultVal = null, int defaultInt = 0) {

        List<int> list = defaultVal;

        if (data.ContainsKey(key)) {

            List<object> temp = (List<object>)data[key];

            list = new List<int>(temp.Count);

            foreach (object item in temp) {
                if (item != null) {
                    list.Add(int.Parse(item.ToString()));
                }
                else {
                    list.Add(defaultInt);
                }
            }

        }

        return list;
    }

    public static List<long> getLongList(IDictionary<string, object> data, string key, List<long> defaultVal = null, long defaultLong = 0L) {

        List<long> list = defaultVal;

        if (data.ContainsKey(key)) {

            List<object> temp = (List<object>)data[key];

            list = new List<long>(temp.Count);

            foreach (object item in temp) {
                if (item != null) {
                    list.Add(long.Parse(item.ToString()));
                }
                else {
                    list.Add(defaultLong);
                }
            }

        }

        return list;
    }

    public static List<float> getFloatList(IDictionary<string, object> data, string key, List<float> defaultVal = null, float defaultFloat = 0f) {

        List<float> list = defaultVal;

        if (data.ContainsKey(key)) {

            List<object> temp = (List<object>)data[key];

            list = new List<float>(temp.Count);

            foreach (object item in temp) {
                if (item != null) {
                    list.Add(long.Parse(item.ToString()));
                }
                else {
                    list.Add(defaultFloat);
                }
            }

        }

        return list;
    }

    public static int getInt(IDictionary<string, object> data, string key, int defaultVal = 0) {
        int dataInt = defaultVal;
        if (data.ContainsKey(key)) {
            dataInt = int.Parse(data[key].ToString());
        }
        return dataInt;
    }


    /// <summary>
    /// Long Dic. Lol.
    /// </summary>
    /// <param name="data"></param>
    /// <param name="key"></param>
    /// <param name="defaultVal"></param>
    /// <returns></returns>
    public static long getLong(IDictionary<string, object> data, string key, long defaultVal = 0) {
        long dataLong = defaultVal;
        if(data.ContainsKey(key)) {
            dataLong = long.Parse(data[key].ToString());
        }
        return dataLong;
    }

    public static bool getBool(IDictionary<string, object> data, string key, bool defaultVal = false) {
        bool dataBool = defaultVal;
        if(data.ContainsKey(key)) {
            string str = data[key].ToString();
            dataBool = str.ToLower().Equals("true");
        }
        return dataBool;
    }

    public static string getString(IDictionary<string, object> data, string key, string defaultVal = null) {
        string dataString = defaultVal;
        if (data.ContainsKey(key)) {
            if (data[key] != null) {
                dataString = data[key].ToString();
            }
        }
        return dataString;
    }


    public static int getIntListIndex(IDictionary<string, object> data, string key, int index, int defaultVal = 0) {
        int dataInt = defaultVal;
        if (data.ContainsKey(key)) {
            List<object> dataList = (List<object>)data[key];
            if (dataList != null) {
                if (dataList.Count > index && index >= 0) {
                    dataInt = int.Parse(dataList[index].ToString());
                }
            }
        }
        return dataInt;
    }

    public static long getLongListIndex(IDictionary<string, object> data, string key, int index, long defaultVal = 0L) {
        long dataFloat = defaultVal;
        if (data.ContainsKey(key)) {
            List<object> dataList = (List<object>)data[key];
            if (dataList != null) {
                dataFloat = long.Parse(dataList[index].ToString());
            }
        }
        return dataFloat;
    }

    public static float getFloatListIndex(IDictionary<string, object> data, string key, int index, float defaultVal = 0f) {
        float dataFloat = defaultVal;
        if (data.ContainsKey(key)) {
            List<object> dataList = (List<object>)data[key];
            if (dataList != null) {
                dataFloat = float.Parse(dataList[index].ToString());
            }
        }
        return dataFloat;
    }

    public static string getStringListIndex(IDictionary<string, object> data, string key, int index, string defaultVal = null) {
        string dataString = defaultVal;
        if (data.ContainsKey(key)) {
            List<object> dataList = (List<object>)data[key];
            if (dataList != null) {
                if (dataList.Count > index) {
                    dataString = dataList[index].ToString();
                }
            }
        }
        return dataString;
    }


}
