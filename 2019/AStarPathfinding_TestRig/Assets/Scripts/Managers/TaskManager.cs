using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskManager
{
    public TaskManager () {

    }

    private static TaskManager _instance;
    public static TaskManager Instance
    {
        get {
            if (_instance == null)
            {
                _instance = new TaskManager();
            }
            return _instance; }
    }

    private Dictionary<string, Entity> EntityMap = new Dictionary<string, Entity>();
    private int entityIDIncrement = 0;

    public void AddEntity(Entity newEntity)
    {
        if (!EntityMap.ContainsKey("entity_" + entityIDIncrement))
        {
            EntityMap.Add("entity_" + entityIDIncrement, newEntity);
            entityIDIncrement++;
        }
    }

    public Entity GetEntity(string id)
    {
        if (!EntityMap.ContainsKey(id))
        {
            return null;
        }
        return EntityMap[id];
    }

    public Entity FindEntityByAvailableFlag(string flag)
    {
        foreach (var ent in EntityMap)
        {
            Entity e = ent.Value;
            if (e.ContainsFlag(flag))
            {
                return e;
            }
        }
        return null;
    }
    public List<Entity> GetEntitiesByAvailableFlag(string flag)
    {
        List<Entity> outputList = new List<Entity>();
        foreach (var ent in EntityMap)
        {
            Entity e = ent.Value;
            if (e.ContainsFlag(flag))
            {
                outputList.Add(e);
            }
        }
        return outputList;
    }
    public List<Entity> GetAvailableEntitiesByType(string type)
    {
        List<Entity> outputList = new List<Entity>();
        foreach (var ent in EntityMap)
        {
            Entity e = ent.Value;
            if (e.ContainsFlagAndValue("type", type))
            {
                if (e is Interactable)
                {
                    Interactable interactable = (Interactable)e;
                    if (interactable.hasUser == false)
                    {
                        outputList.Add(interactable);
                    }
                }
            }
        }
        return outputList;
    }

    private List<TaskRequest> requests = new List<TaskRequest>();

    public void RunUpdate()
    {
        if (requests.Count > 0) {
            TaskRequest firstTask = requests[0];
            ProcessTask(firstTask);
            requests.Remove(firstTask);
        }
    }
    public void RequestTask(string task, Agent requestingAgent = null)
    {
        var requestCount = requests.Count;
        for (int i = 0; i < requestCount; i++)
        {
            TaskRequest request = requests[i];
            if(request.requestKey == task && request.requestingAgent == requestingAgent)
            {
                return;
            }
        }
        TaskRequest newRequest = new TaskRequest();
        newRequest.requestKey = task;
        newRequest.requestingAgent = requestingAgent;
        requests.Add(newRequest);
    }

    public void ProcessTask(TaskRequest taskRequest)
    {
        var requestingAgent = taskRequest.requestingAgent;
        var task = taskRequest.requestKey;

        if (requestingAgent == null)
        {
            return;
        }
            List<Task> tasks = requestingAgent.GetTasks();
            foreach (var item in tasks)
            {
                if(item.type == task)
                {
                    return; // we're already doing it!
                }
            }
            Task currentTask = requestingAgent.GetCurrentTask();
        
        Vector3 agentPos = requestingAgent.GetPosition();
        float shortestDistance = 99999f;
        Entity nearestEnt = null;
        switch (task)
        {
            case "sleep":
                List<Entity> beds = GetAvailableEntitiesByType("bed");
                shortestDistance = 99999f;
                nearestEnt = null;
                foreach (var item in beds)
                {
                    if (((Interactable)item).hasUser)
                    {
                        continue;
                    }
                    Vector3 itemPos = item.GetPosition();
                    float distance = (itemPos - agentPos).sqrMagnitude;
                    if (distance < shortestDistance)
                    {
                        nearestEnt = item;
                        shortestDistance = distance;
                    }

                }
                if (nearestEnt != null)
                {
                    ((Interactable)nearestEnt).hasUser = true;
                    
                    Task moveTask = new Task("move", nearestEnt.GetPosition());
                    Task sleepTask = new Task("sleep", nearestEnt);
                    Task metaTask = new Task(task, new List<Task> { moveTask, sleepTask });
                    requestingAgent.AddTask(metaTask);
                }
                break;

            case "eat":
                List<Entity> foodMachines = GetAvailableEntitiesByType("food");
                shortestDistance = 99999f;
                nearestEnt = null;
                foreach (var item in foodMachines)
                {
                    if (((Interactable)item).hasUser)
                    {
                        continue;
                    }
                    Vector3 itemPos = item.GetPosition();
                    float distance = (itemPos - agentPos).sqrMagnitude;
                    if (distance < shortestDistance)
                    {
                        nearestEnt = item;
                        shortestDistance = distance;
                    }

                }
                if (nearestEnt != null)
                {
                    ((Interactable)nearestEnt).hasUser = true;
                    Task moveToFoodTask = new Task("move", nearestEnt.GetPosition());
                    Task eatTask = new Task("eat", nearestEnt);
                    Task metaEatingTask = new Task(task, new List<Task> { moveToFoodTask, eatTask });
                    requestingAgent.AddTask(metaEatingTask);
                }
                break;

            case "work":
                List<Entity> availableMachines = GetAvailableEntitiesByType("work");
                shortestDistance = 99999f;
                nearestEnt = null;
                foreach (var item in availableMachines)
                {
                    if (((Interactable)item).hasUser)
                    {
                        continue;
                    }
                    Vector3 itemPos = item.GetPosition();
                    float distance = (itemPos - agentPos).sqrMagnitude;
                    if (distance < shortestDistance)
                    {
                        nearestEnt = item;
                        shortestDistance = distance;
                    }

                }
                if (nearestEnt != null)
                {
                    ((Interactable)nearestEnt).hasUser = true;
                    Task moveToWorkTask = new Task("move", nearestEnt.GetPosition());
                    Task workTask = new Task("work", nearestEnt);
                    Task metaWorkTask = new Task(task, new List<Task> { moveToWorkTask, workTask });
                    requestingAgent.AddTask(metaWorkTask);
                }
                break;
        }
    }

}
