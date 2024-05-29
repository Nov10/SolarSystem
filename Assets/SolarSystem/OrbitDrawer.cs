using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEditor;


//[ExecuteInEditMode]
public class OrbitDrawer : MonoBehaviour
{
    static OrbitDrawer _Instance;
    public static OrbitDrawer Instance
    {
        get
        {
            if(_Instance == null)
            {
                _Instance = FindObjectOfType<OrbitDrawer>();
            }
            return _Instance;
        }
        set
        {
            _Instance = value;
        }
    }

    Body[] Bodies;

    VirtualBody[] VirtualBodies;
    public int Step = 1000;

    private void Awake()
    {
        Bodies = FindObjectsOfType<Body>();
        Time.fixedDeltaTime = Universe.PhysicsTimeStep;

        VirtualBodies = new VirtualBody[Bodies.Length];
        for (int i = 0; i < Bodies.Length; i++)
        { 
            Bodies[i].Initialize();

            VirtualBodies[i] = new VirtualBody(Bodies[i].Mass, Bodies[i].Radius, Bodies[i].InitialVelocity, Bodies[i].NowVelocity, Bodies[i].ThisRigidbody.position);
         }

        StartCoroutine(_StartDraw());
    }
    IEnumerator _StartDraw()
    {
        while(true)
        {
            //CalculateFuturePositions();
            //DrawLines();
            //yield return new WaitForSeconds(0.5f);
            break;
        }
        yield return null;
    }
    private void FixedUpdate()
    {
    }
    public void DrawLines()
    {
        for (int i = 0; i < VirtualBodies.Length; i++)
        {
            List<Vector3> points = new List<Vector3>();

            for (int k = 0; k < VirtualBodies[i].FuturePositions.Count; k += 10)
            {
                points.Add(VirtualBodies[i].FuturePositions[k]);
            }

            for(int s = 0; s < points.Count-1; s++)
            {
                Debug.DrawLine(points[s], points[s + 1], Color.white, 0.5f);
            }
        }
    }


    public void CalculateFuturePositions()
    {
        for (int i = 0; i < VirtualBodies.Length; i++)
        {
            //VirtualBodies[i].Mass = Bodies[i].Mass;
            //VirtualBodies[i].Radius = Bodies[i].Radius;
            //VirtualBodies[i].InitialVelocity = Bodies[i].InitialVelocity;
            //VirtualBodies[i].Position = Bodies[i].ThisRigidbody.position;
            VirtualBodies[i] = new VirtualBody(Bodies[i].Mass, Bodies[i].Radius, Bodies[i].InitialVelocity, Bodies[i].NowVelocity, Bodies[i].ThisRigidbody.position);
            VirtualBodies[i].InitialVelocity = VirtualBodies[i].NowVelocity;
            //VirtualBodies[i].FuturePositions.Clear();
        }
        for (int step = 0; step < Step; step++)
        {
            for (int i = 0; i < VirtualBodies.Length; i++)
            {
                VirtualBodies[i].UpdateVelocity(VirtualBodies, Universe.PhysicsTimeStep);
            }

            for (int i = 0; i < VirtualBodies.Length; i++)
            {
                VirtualBodies[i].UpdatePosition(Universe.PhysicsTimeStep);
            }
        }
    }
}
