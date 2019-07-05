using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : Entity
{
    public float energy = 100f;
    public float hunger = 100f;
    public float happiness = 100f;
    public float loyalty = 100f;
    
    public string statusReadout = "";
    public enum ActionStatus
    {
        Idle,
        Moving,
        Interacting,
        Sleeping,
        Eating,
        Attacking
    }

    public ActionStatus currentStatus = ActionStatus.Idle;
    public List<Task> TaskQueue = new List<Task>();

    private void Start()
    {
        AddFlag("type","character");
        TaskManager.Instance.AddEntity(this);
    }

    void LateUpdate()
    {
        if(energy > 0)
        {
            if (currentStatus != ActionStatus.Sleeping)
            {
                energy -= Time.deltaTime * 5f;
            }
            int energyIdx = 0;
            if (energy < 20) { energyIdx = 4;}
            else if (energy < 40) { energyIdx = 3;}
            else if (energy < 60) { energyIdx = 2;}
            else if (energy < 80) { energyIdx = 1;}
            else if (energy >= 80){ energyIdx = 0;}
            AddFlag("energy_status", "energy_" + energyIdx);
        }
        if(hunger > 0)
        {
            if (currentStatus != ActionStatus.Eating)
            {
                hunger -= Time.deltaTime * 5f;
            }
            int hungerIdx = 0;
            if (hunger < 20) { hungerIdx = 4; }
            else if (hunger < 40) { hungerIdx = 3; }
            else if (hunger < 60) { hungerIdx = 2; }
            else if (hunger < 80) { hungerIdx = 1; }
            else if (hunger >= 80) { hungerIdx = 0; }
            AddFlag("hunger_status", "hunger_" + hungerIdx);
        }
        statusReadout = ReadoutData.Instance.GetDisplayableFlagList(getFlagList());
        EvaluateNeeds();
        OperateTasks();
        OperateMotor();
    }


    private void EvaluateNeeds()
    {
        string energyFlag = GetFlag("energy_status");
        string hungerFlag = GetFlag("hunger_status");

        string eIDXStr = energyFlag.Substring(7);
        int eIDX = int.Parse(eIDXStr);
        if (eIDX > 0)
        {
            TaskManager.Instance.RequestTask("sleep", this);
            
        }
        string hIDXStr = hungerFlag.Substring(7);
        int hIDX = int.Parse(hIDXStr);
        if (hIDX > 0)
        {
            TaskManager.Instance.RequestTask("eat", this);
            
        }
        if (currentStatus == ActionStatus.Idle)
        {
            TaskManager.Instance.RequestTask("work", this);
        }

    }

    public void ClearTasks()
    {
        TaskQueue.Clear();
    }
    public void EndCurrentTask()
    {

    }

    public void AddTask(Task newTask)
    {
        TaskQueue.Add(newTask);
    }

    public Task GetCurrentTask()
    {
        if (TaskQueue.Count > 0)
        {
            return TaskQueue[0];
        }
        return null;
    }

    public void CompleteTask(Task task)
    {
        TaskQueue.Remove(task);
    }

    public List<Task> GetTasks()
    {
        return TaskQueue;
    }

    /// <summary>
    /// This is not the front of the task-queue, this is the subtask we're currently working with.
    /// </summary>
    private Task lastTask;
    private Task activeTask;
    private List<Vector3> currentNavPath;
    private float worktime = 10f;

    /// <summary>
    /// Enact the current task
    /// </summary>
    private void OperateTasks()
    {
        Task currentTask = GetCurrentTask();
        if(currentTask == null)
        {
            return;
        }
        string taskType = currentTask.type;
        List<Task> subTasks = currentTask.subTasks;
        if (subTasks != null && subTasks.Count > 0)
        {
            activeTask = subTasks[0];
        }
        else
        {
            CompleteTask(currentTask); // we finished this tasking
            return;
        }
        if (activeTask != null) {
            if (activeTask != lastTask) {
                switch (activeTask.type)
                {
                    case "move":
                        object metadata = activeTask.metaData; // this will be the coordinates we're moving to
                        Vector3 destination = (Vector3)metadata;
                        Vector3 myPosition = GetPosition();
                        PathfindingManagerMk2 pathfinder = new PathfindingManagerMk2();
                        currentNavPath = pathfinder.GeneratePath(myPosition, destination);
                        if(currentNavPath.Count == 0)
                        {

                            for (int i = 0; i < subTasks.Count; i++)
                            {
                                if(subTasks[i].metaData is Interactable)
                                {
                                    ((Interactable)(subTasks[i].metaData)).hasUser = false;
                                }
                            }
                            currentStatus = ActionStatus.Idle;
                            activeTask = null;
                            ClearTasks();
                            return;
                        }
                        currentStatus = ActionStatus.Moving;
                        break;

                    case "sleep":
                        currentStatus = ActionStatus.Sleeping;
                        break;

                    case "eat":
                        currentStatus = ActionStatus.Eating;
                        break;

                    case "work":
                        currentStatus = ActionStatus.Interacting;
                        worktime = 5f;
                        break;

                }
                lastTask = activeTask;
            }
            else
            { // continuous checking
                if (activeTask != null)
                    switch (activeTask.type)
                    {
                        case "move":
                            if(currentNavPath == null || currentNavPath.Count == 0)
                            {
                                subTasks.Remove(activeTask);
                                currentStatus = ActionStatus.Idle;
                                activeTask = null;
                            }
                            break;
                        case "sleep":
                            energy += Time.deltaTime * 5;
                            if (energy >= 100)
                            {
                                subTasks.Remove(activeTask);
                                currentStatus = ActionStatus.Idle;
                                ((Interactable)activeTask.metaData).hasUser = false;
                                activeTask = null;
                            }
                            break;
                        case "eat":
                            hunger += Time.deltaTime * 5;
                            if (hunger >= 100)
                            {
                                subTasks.Remove(activeTask);
                                currentStatus = ActionStatus.Idle;
                                ((Interactable)activeTask.metaData).hasUser = false;
                                activeTask = null;
                            }
                            break;
                        case "work":
                            if(worktime > 0)
                            {
                                worktime -= Time.deltaTime;
                            }
                            else
                            {
                                subTasks.Remove(activeTask);
                                currentStatus = ActionStatus.Idle;
                                ((Interactable)activeTask.metaData).hasUser = false;
                                activeTask = null;
                            }
                            break;
                    }
            }
        }

    }

    private void OperateMotor()
    {
        if (currentNavPath != null && currentNavPath.Count > 0)
        {
            Vector3 firstNode = currentNavPath[0];
            if (TraversableReference.Instance.GetTileContainsDoor(firstNode))
            {
                DoorScript door = TraversableReference.Instance.GetDoorAtTile(firstNode);
                if(door.doorState != DoorScript.Status.Open)
                {
                    door.TriggerOpen();
                    return;
                }
                //Operate the door

            }
                firstNode.y = 1f;// Terrain.activeTerrain.SampleHeight(firstNode) + 1f;


                float step = 2 * Time.deltaTime; // calculate distance to move
                transform.position = Vector3.MoveTowards(transform.position, firstNode, step);
                if (transform.position == firstNode)
                {
                    currentNavPath.RemoveAt(0);
                }
            

        }
    }

    
}
