using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    private Dictionary<string,string> _flags = new Dictionary<string, string>();

    /// <summary>
    /// returns the values within all the flags available.
    /// </summary>
    /// <returns></returns>
    public List<string> getFlagList()
    {
        List<string> outputList = new List<string>();
        foreach (var item in _flags)
        {
            outputList.Add(item.Value);
        }

        return outputList;
    }

    public bool ContainsFlag(string key) 
    {
        if (_flags.ContainsKey(key))
        {
            return true;
        }
        return false;
    }
    public bool ContainsFlagAndValue(string key, string value)
    {
        if (_flags.ContainsKey(key))
        {
            if (_flags[key] == value)
            {
                return true;
            }
        }
        return false;
    }

    public void AddFlag(string key, string value)
    {
        if (!_flags.ContainsKey(key))
        {
            _flags.Add(key, value);
        }
        else
        {
            if(_flags[key] != value)
            {
                _flags[key] = value;
            }
        }
    }

    public string GetFlag(string key)
    {
        if (_flags.ContainsKey(key))
        {
            return _flags[key];
        }
        return null;
    }

    public void RemoveFlag(string key)
    {
        if (_flags.ContainsKey(key))
        {
            _flags.Remove(key);
        }
    }

    public Vector3 GetPosition()
    {
        return gameObject.transform.position;
    }
}
