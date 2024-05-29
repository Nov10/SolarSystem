using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Unity.Mathematics;
using UnityEngine.UIElements;

public enum eMode
{
    Two,
    Three
}
public class LagrangePointCalculater : MonoBehaviour
{
    [SerializeField] Solver ThisSolver;
    Body[] Bodies
    {
        get { return ThisSolver.Bodies; }
    }
    [SerializeField] float Scale = 1;
    [SerializeField] eMode Mode;
    private void OnDrawGizmos()
    {
        if(Mode == eMode.Two)
        {
            if (Bodies.Length < 2) return;

            for (int i = 0; i < Bodies.Length; i++)
            {
                Body body1 = Bodies[i].Pivot;
                if (body1 == null)
                    continue;
                Body body2 = Bodies[i];

                Vector3 L1 = CalculateLagrangePointL1(body1, body2);
                Vector3 L2 = CalculateLagrangePointL2(body1, body2);
                Vector3 L3 = CalculateLagrangePointL3(body1, body2);

                Vector3 force = Vector3.zero;

                force += CalculateGravitationalForce(body1, L1);
                force += CalculateGravitationalForce(body2, L1);

                Debug.Log(force.magnitude);

                float distance = (body2.transform.position - SceneView.lastActiveSceneView.camera.transform.position).magnitude;
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(L1, distance / Scale);
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(L2, distance / Scale);
                Gizmos.color = Color.blue;
                Gizmos.DrawSphere(L3, distance / Scale);
            }
        }
        else if(Mode == eMode.Three)
        {
            FindLagrangePoints();
        }
    }

    private void FindLagrangePoints2()
    {
        if (Bodies.Length < 2) return;

        for (int i = 0; i < Bodies.Length; i++)
        {
            for (int j = i + 1; j < Bodies.Length; j++)
            {
                Body body1 = Bodies[i];
                Body body2 = Bodies[j];
                if (Bodies[i].Mass > Bodies[j].Mass)
                {

                    body2 = Bodies[i];
                    body1 = Bodies[j];
                }

                Vector3 L1 = CalculateLagrangePointL1(body1, body2);
                Vector3 L2 = CalculateLagrangePointL2(body1, body2);
                Vector3 L3 = CalculateLagrangePointL3(body1, body2);

                Debug.DrawLine(body1.transform.position, L1, Color.red);
                Debug.DrawLine(body2.transform.position, L2, Color.green);
                Debug.DrawLine(body2.transform.position, L3, Color.blue);


                //Debug.Log($"L1: {L1}, L2: {L2}, L3: {L3}");
            }
        }
    }

    private Vector3 CalculateLagrangePointL1(Body body1, Body body2)
    {
        float r = Vector3.Distance(body1.transform.position, body2.transform.position);
        float mu = body2.Mass / (body1.Mass + body2.Mass);
        float L1 = r * (1 - Mathf.Pow(mu / 3.0f, 1.0f / 3.0f));
        Vector3 direction = (body2.transform.position - body1.transform.position).normalized;
        return body1.transform.position + direction * L1;
    }

    private Vector3 CalculateLagrangePointL2(Body body1, Body body2)
    {
        float r = Vector3.Distance(body1.transform.position, body2.transform.position);
        float mu = body2.Mass / (body1.Mass + body2.Mass);
        float L2 = r * (1 + Mathf.Pow(mu / 3.0f, 1.0f / 3.0f));
        Vector3 direction = (body2.transform.position - body1.transform.position).normalized;
        return body1.transform.position + direction * L2;
    }

    private Vector3 CalculateLagrangePointL3(Body body1, Body body2)
    {
        float r = Vector3.Distance(body1.transform.position, body2.transform.position);
        Vector3 direction = (body2.transform.position - body1.transform.position).normalized;
        return body1.transform.position - direction * r;
    }


    private void FindLagrangePoints()
    {
        if (Bodies.Length < 3) return;

        for (int i = 0; i < Bodies.Length; i++)
        {
            for (int j = i + 1; j < Bodies.Length; j++)
            {
                for (int k = j + 1; k < Bodies.Length; k++)
                {
                    Body body1 = Bodies[i];
                    Body body2 = Bodies[j];
                    Body body3 = Bodies[k];

                    if (body1.transform.name == "지" || body2.transform.name == "지" || body3.transform.name == "지")
                    {

                    }
                    else
                    {
                        continue;
                    }

                    if (body1.transform.name == "Sun" || body2.transform.name == "Sun" || body3.transform.name == "Sun")
                    {

                    }
                    else
                    {
                        continue;
                    }

                    if (body1.transform.name == "달" || body2.transform.name == "달" || body3.transform.name == "달")
                    {

                    }
                    else
                    {
                        continue;
                    }
                    Vector3 L = CalculateLagrangePoint(body1, body2, body3);
                    float distance = (L - SceneView.lastActiveSceneView.camera.transform.position).magnitude;
                    Gizmos.color = Color.red;
                    Gizmos.DrawSphere(L, distance / Scale);
                    //Debug.DrawLine(body1.transform.position, L, Color.red);
                    //Debug.DrawLine(body2.transform.position, L, Color.green);
                    //Debug.DrawLine(body3.transform.position, L, Color.blue);

                    //Debug.Log($"Lagrange Point: {L}");
                }
            }
        }
    }

    private Vector3 CalculateLagrangePoint(Body body1, Body body2, Body body3)
    {
        Vector3 initialGuess = (body1.transform.position + body2.transform.position + body3.transform.position) / 3;
        Vector3 position = initialGuess;
        float tolerance = 1e-4f;
        int maxIterations = 1000;

        for (int iteration = 0; iteration < maxIterations; iteration++)
        {
            Vector3 force = Vector3.zero;
            Vector3 forceDerivative = Vector3.zero;

            force += CalculateGravitationalForce(body1, position);
            force += CalculateGravitationalForce(body2, position);
            force += CalculateGravitationalForce(body3, position);

            forceDerivative += CalculateGravitationalForceDerivative(body1, position);
            forceDerivative += CalculateGravitationalForceDerivative(body2, position);
            forceDerivative += CalculateGravitationalForceDerivative(body3, position);

            Vector3 delta = force / forceDerivative.magnitude / 500f;

            position -= delta;
            Debug.Log(force.magnitude);

            if (delta.magnitude < tolerance)
            {
                break;
            }
        }

        return position;
    }

    private Vector3 CalculateGravitationalForce(Body body, Vector3 position)
    {
        Vector3 direction = body.transform.position - position;
        float distance = direction.magnitude;
        return Universe.GravitationalConstant * body.Mass / (distance * distance) * direction.normalized;
    }

    private Vector3 CalculateGravitationalForceDerivative(Body body, Vector3 position)
    {
        Vector3 direction = body.transform.position - position;
        float distance = direction.magnitude;
        return -2 * Universe.GravitationalConstant * body.Mass / (distance * distance * distance) * direction.normalized;
    }
}
