using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorScript : MonoBehaviour
{
    public enum Status
    {
        Closed,
        Open,
        Opening,
        Closing
    }
    public Status doorState = Status.Closed;
    public Transform doorLeft;
    public Transform doorRight;
    private bool toggleOpen = false;

    // Start is called before the first frame update
    void Start()
    {
        int childCount = transform.childCount;
        for (int i = 0; i < childCount; i++)
        {
            Transform child = transform.GetChild(i);
            if (child.name == "door_left")
            {
                doorLeft = child;
            }else if (child.name == "door_right")
            {
                doorRight = child;
            }
        }

        Vector3 scale = transform.parent.localScale;
        Vector3 position = transform.position;
        float width = 1f * scale.x;
        float length = 1f * scale.z;
        if (transform.parent.localEulerAngles.y == 90)
        {
            width = 1f * scale.z;
            length = 1f * scale.x;
        }
        float tileWidth = Mathf.Round(width);
        float tileLength = Mathf.Round(length);
        for (int i = 0; i < tileWidth*2; i++)
        {
            for (int j = 0; j < tileLength*2; j++)
            {
                TraversableReference.Instance.AddDoorToTile(new Vector3(position.x + i-tileWidth,0, position.z + j-tileLength),this);
            }

        }

        //PathfindingManagerMk2.Instance.RestrictTile();
    }
    private float doorOutput = 0f;
    private void Update()
    {
        if (toggleOpen)
        {
            if(doorState == Status.Closed || doorState == Status.Closing)
            {
                doorState = Status.Opening;
            }
            if(doorState == Status.Opening)
            {
                if (doorOutput < 0.74f)
                {
                    doorOutput += Time.deltaTime*2;
                    if(doorOutput > 0.74f)
                    {
                        doorOutput = 0.74f;
                    }
                    doorLeft.localPosition = new Vector3(doorOutput, 1.5f, doorLeft.localPosition.z);
                    doorRight.localPosition = new Vector3(-doorOutput, 1.5f, doorRight.localPosition.z);
                }
                else
                {
                    doorState = Status.Open;
                    Invoke("TriggerClosed", 4f);
                }
                    
            }
        }
        else
        {
            if(doorState == Status.Open || doorState == Status.Opening)
            {
                doorState = Status.Closing;
            }
            if (doorState == Status.Closing)
            {
                if (doorOutput > 0.25f)
                {
                    doorOutput -= Time.deltaTime*2;
                    if(doorOutput < 0.25f)
                    {
                        doorOutput = 0.25f;
                    }
                    doorLeft.localPosition = new Vector3(doorOutput, 1.5f, doorLeft.localPosition.z);
                    doorRight.localPosition = new Vector3(-doorOutput, 1.5f, doorRight.localPosition.z);
                }
                else
                {
                    doorState = Status.Closed;
                }

            }

        }
    }

    public void TriggerOpen()
    {
        toggleOpen = true;
    }
    public void TriggerClosed()
    {
        toggleOpen = false;
    }

}
