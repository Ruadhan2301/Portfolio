﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapNode
{
    public string id;
    public Vector3 position;
    public float difficulty;
    public MapNode parentNode;
    public float heuristic;
}
