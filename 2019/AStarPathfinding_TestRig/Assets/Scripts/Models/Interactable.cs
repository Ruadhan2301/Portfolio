using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : Entity
{
    public string type = "";
    public bool hasUser = false;

    void Start()
    {
        AddFlag("type",type);
        TaskManager.Instance.AddEntity(this);
    }   

}
