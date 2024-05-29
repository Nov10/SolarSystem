using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Solver : MonoBehaviour
{
    public Body[] Bodies;

    private void Awake()
    {
        Bodies = FindObjectsOfType<Body>();
        Time.fixedDeltaTime = Universe.PhysicsTimeStep;
    }


    private void FixedUpdate()
    {
        for(int i = 0; i < Bodies.Length; i++)
        {
            Bodies[i].UpdateVelocity(Bodies, Universe.PhysicsTimeStep);
        }

        for (int i = 0; i < Bodies.Length; i++)
        {
            Bodies[i].UpdatePosition(Universe.PhysicsTimeStep);
        }
        //FindLagrangePoints();
    }
}
