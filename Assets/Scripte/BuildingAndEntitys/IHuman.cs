using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;//new ((movement tut)
using Fusion;//network tut

public abstract class IHuman : IINFO
{
    public bool isSelectet;

    public int Health, currentHealth;
    public int Armor;
    public float Speed;
    public int Damage;
    public float attackSpeed;
    public Color typeColor;

    //new (movement tut)
    public NavMeshAgent agent;
    public bool moveTo, onDestiation;
    Vector3 lastPos, currentPos, finalDestination;

    private void Start()
    {
        lastPos = currentPos;
    }

    protected void removeOnId(Component co)
    {
        if (Runner != null && Runner.IsRunning)
        {
            if (Runner.IsServer)
            {
                if (ID != 1) Destroy(co);
            }
            else if (Runner.IsClient)
            {
                if (ID != 2) Destroy(co);
            }
        }
    }
   
    public void walkTo(Vector3 target, float mindis)
    {

        if (!agent.isActiveAndEnabled)
            agent.enabled = true;

        agent.stoppingDistance = mindis;
        finalDestination = target;
        agent.destination = target;
        moveTo = true;//new
    }


    void isOnDestination(float destBuff)
    {
        if (Vector3.Distance(agent.transform.position, finalDestination) <= destBuff) onDestiation = true;
        else onDestiation = false;
    }

    public void isMoving()
    {
        currentPos = agent.transform.position;

        if (currentPos != lastPos)
        {
            lastPos = currentPos;
            moveTo = true;
        }
        else
        {
            moveTo = false;
        }
    }


    protected  abstract void nextUpdate();

    private void Update()
    {
        if (!moveTo && onDestiation)
            if (agent.isActiveAndEnabled) { agent.enabled = false;}//new
        //else if (!agent.isActiveAndEnabled) agent.enabled = true;
       
        isOnDestination(2f);//change 5 to 1
        isMoving();
        nextUpdate();
    }

    //
    public bool isDead()
    {
        if(currentHealth <= 0)
        {
            return true;
        }
        return false;
    }



    //next movement tut

    public void setWalkTarget(Vector3 walkTarget, float minDistance, int index, int total, float spacing)
    {
        if (!agent.isActiveAndEnabled) agent.enabled = true;
        finalDestination = walkTarget;
        agent.stoppingDistance = minDistance;
        var format = GenerateTrianglePositions(walkTarget, total, spacing);
        agent.destination = format[index];
    }

    Vector3[] GenerateTrianglePositions(Vector3 startPoint, int size, float spacing)
    {
        int numPoints = size * (size + 1) / 2; // Gesamtanzahl der Punkte im Dreieck
        Vector3[] positions = new Vector3[numPoints];

        int index = 0;
        for (int row = 0; row < size; row++)
        {
            for (int col = 0; col <= row; col++)
            {
                float xOffset = col * spacing - row * spacing * 0.5f;
                float zOffset = row * spacing * Mathf.Sqrt(3) / 2;

                Vector3 position = startPoint + new Vector3(xOffset, 0f, -zOffset);
                positions[index] = position;

                index++;
            }
        }
        return positions;
    }

    Vector3 GetTriangleFormationPosition(Vector3 basePosition, int index, int size)
    {
        if (index < 0 || index >= size)
        {
            Debug.LogError("Invalid index for triangle formation.");
            return Vector3.zero;
        }

        Vector3 position = basePosition;

        for (int i = 1; i <= index; i++)
        {
            position.x += (index % 2 == 0) ? 1f : -1f;
            position.z -= 1f;
        }

        return position;
    }
}
