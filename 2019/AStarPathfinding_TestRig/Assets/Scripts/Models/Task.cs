using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Task
{
    public string type;
    public object metaData;

    public List<Task> subTasks;

    public Task (string _type, object _metadata)
    {
        type = _type;
        if (_metadata is List<Task>)
        {
            subTasks = (List<Task>)_metadata;
        }
        else
        {
            metaData = _metadata;
        }
    }
}
