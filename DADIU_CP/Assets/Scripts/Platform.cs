﻿using UnityEngine;
using System.Collections;


public class Platform : MonoBehaviour
{
    public GameObject patrol;
    private Transform[] points;
    public bool autoMove;
    private bool moveBack, left;
    private int destPoint = 0;
    private NavMeshAgent agent;


    void Start()
    {
        agent = GetComponentInParent<NavMeshAgent>();
        agent.updateRotation = false;
        points = new Transform[patrol.transform.childCount];
        for (int i = 0; i < points.Length - 1; i++)
        {
            points[i] = patrol.transform.GetChild(i + 1);
        }
        points[points.Length - 1] = patrol.transform.GetChild(0);
    }


    void GotoNextPoint()
    {
        if (points.Length == 0)
            return;

        agent.destination = points[destPoint].position;      

        if (autoMove)
        {
            if (!left)
            {
                destPoint = (destPoint + 1) % points.Length;
                if (destPoint == points.Length - 1)
                {
                    left = true;
                    destPoint = points.Length - 3;
                }
            }
            else
            {
                destPoint = (int) nfmod((destPoint - 1), points.Length);
                if(destPoint == points.Length - 1)
                {
                    left = false;
                }
            }
        }
        else
        {
            if(destPoint != points.Length - 2)
            {
              destPoint += 1;
            }
        }
    }


    void Update()
    {
        if(moveBack)
        {
            if (!autoMove)
            {
                if (agent.remainingDistance < 0.2f)
                {
                    agent.destination = points[(destPoint + points.Length - 1) % points.Length].position;
                    destPoint--;
                }
                if (destPoint < 0)
                {
                    moveBack = false;
                    destPoint = 0;
                }
            }
            else
            {

                if (agent.remainingDistance < 0.2f)
                {
                    if(!left)
                    {
                        agent.destination = points[(int)nfmod((destPoint - 1), points.Length)].position;
                        destPoint--;
                        if (destPoint < 0)
                        {
                            moveBack = false;
                            destPoint = 0;
                        }
                    }
                    else // IT BREAKS AT SOME POINT.LOOK THE LOGIC.
                    {
                        agent.destination = points[(int)nfmod(destPoint, points.Length)].position;
                        destPoint--;
                        if (destPoint < -1)
                        {
                            moveBack = false;
                            left = false;
                            destPoint = 0;
                        }
                    }
                }
            }
        }
    }


    void OnCollisionEnter(Collision col)
    {
        if (col.collider.tag == "Player")
        {
            CameraController.instance.player.transform.parent = transform;
            GotoNextPoint();
        }
        
    }

    void OnCollisionStay(Collision col)
    {
        if (col.collider.tag == "Player")
        {
            if (agent.remainingDistance < 0.2f)
                GotoNextPoint();
        }
    }
    
    void OnCollisionExit(Collision col)
    {
        if (col.collider.tag == "Player")
        {
            CameraController.instance.player.transform.parent = null;
            moveBack = true;
           
        }
    }

    float nfmod(float a, float b)
    {
        return a - b * Mathf.Floor(a / b);
    }
}