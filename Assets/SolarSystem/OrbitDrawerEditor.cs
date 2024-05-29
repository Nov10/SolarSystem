using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEditor;
 

//[ExecuteInEditMode]
[ExecuteInEditMode]
public class OrbitDrawerEditor : MonoBehaviour
{
    static OrbitDrawerEditor _Instance;
    public static OrbitDrawerEditor Instance
    {
        get
        {
            if (_Instance == null)
            {
                _Instance = FindObjectOfType<OrbitDrawerEditor>();
            }
            return _Instance;
        }
        set
        {
            _Instance = value;
        }
    }

    Body[] Bodies;
    public bool Draw;

    VirtualBody[] VirtualBodies;
    public int Step = 1000;

    Coroutine Drawer;
    private void Start()
    {
        if(Drawer != null)
            StopCoroutine(Drawer);
        Drawer = StartCoroutine(_StartDraw());
    }

    private void OnApplicationQuit()
    {
        if (Drawer != null)
            StopCoroutine(Drawer);
    }


    IEnumerator _StartDraw()
    {
        Bodies = FindObjectsOfType<Body>();

        VirtualBodies = new VirtualBody[Bodies.Length];
        for (int i = 0; i < Bodies.Length; i++)
        {
            Bodies[i].Initialize();

            float r = Bodies[i].Radius;
            VirtualBodies[i] = new VirtualBody(Bodies[i].Mass, Bodies[i].Radius, Bodies[i].InitialVelocity, Bodies[i].NowVelocity, Bodies[i].ThisRigidbody.position);
            VirtualBodies[i].Mass = Bodies[i].Mass;
            VirtualBodies[i].Radius = Bodies[i].Radius;
            VirtualBodies[i].InitialVelocity = Bodies[i].InitialVelocity;
            VirtualBodies[i].NowVelocity = Bodies[i].NowVelocity;
            VirtualBodies[i].Position = Bodies[i].ThisRigidbody.position;
            VirtualBodies[i].StartPosition = Bodies[i].ThisRigidbody.position;
        }
        while (true)
        {
            if(Draw == true)
            {
                CalculateFuturePositions();
                //StartCoroutine(CalculateFuturePositions());
                DrawLines();
                yield return new WaitForSeconds(0.3f);
            }
            else
            {
                yield return new WaitForSeconds(0.3f);
            }
        }
    }

    VirtualBody Findbody(Body body)
    {
        for(int i = 0; i < Bodies.Length; ++i)
        {
            if (body == Bodies[i])
                return VirtualBodies[i];
        }
        return null;
    }
    Body Findbody(VirtualBody body)
    {
        for (int i = 0; i < VirtualBodies.Length; ++i)
        {
            if (body == VirtualBodies[i])
                return Bodies[i];
        }
        return null;
    }

    public void DrawLines()
    {
        for (int i = 0; i < VirtualBodies.Length; i++)
        {
            List<Vector3> points = new List<Vector3>();

            Vector3 p = Vector3.zero;
            if (Bodies[i].Pivot != null)
            {
                var pivot = Findbody(Bodies[i].Pivot);
                Body body = Findbody(VirtualBodies[i]);
                if(EditorApplication.isPlaying == false)
                {
                    VirtualBodies[i].maxDistance = 0;
                    VirtualBodies[i].minDistance = float.MaxValue;
                }
                //body.E = VirtualBodies[i].CalculateE(pivot);
                //Debug.LogFormat("{0}, {1}", body.transform.name, body.E = VirtualBodies[i].CalculateE(pivot));
                //Debug.Log(Bodies[i].Pivot.name)
                for (int k = 0; k < VirtualBodies[i].FuturePositions.Count; k += 10)
                {
                    points.Add(VirtualBodies[i].FuturePositions[k] - pivot.FuturePositions[k] + pivot.StartPosition);
                }
            }
            else
            {
                for (int k = 0; k < VirtualBodies[i].FuturePositions.Count; k += 10)
                {
                    points.Add(VirtualBodies[i].FuturePositions[k]);
                }
            }

            for (int s = 0; s < points.Count - 1; s++)
            {
                Debug.DrawLine(points[s], points[s + 1], Color.white, 0.3f);
            }
        }
    }


    public void CalculateFuturePositions()
    {
        for (int i = 0; i < VirtualBodies.Length; i++)
        {
            //VirtualBodies[i].InitialVelocity = Bodies[i].InitialVelocity;
            //VirtualBodies[i].Position = Bodies[i].ThisRigidbody.position;

            VirtualBodies[i].Mass = Bodies[i].Mass;
            VirtualBodies[i].Radius = Bodies[i].Radius;
            VirtualBodies[i].InitialVelocity = Bodies[i].InitialVelocity;
            VirtualBodies[i].NowVelocity = Bodies[i].NowVelocity;
            VirtualBodies[i].Position = Bodies[i].ThisRigidbody.position;
            //VirtualBodies[i] = new VirtualBody(Bodies[i].Mass, Bodies[i].Radius, Bodies[i].InitialVelocity, Bodies[i].NowVelocity, Bodies[i].ThisRigidbody.position);
            //VirtualBodies[i].InitialVelocity = VirtualBodies[i].NowVelocity;
            if (EditorApplication.isPlaying == true)
            {
                VirtualBodies[i].InitialVelocity = Bodies[i].NowVelocity;
                VirtualBodies[i].NowVelocity = Bodies[i].NowVelocity;
                VirtualBodies[i].PrePosition = VirtualBodies[i].Position = Bodies[i].ThisRigidbody.position;
                VirtualBodies[i].StartPosition = Bodies[i].ThisRigidbody.position;
            }
            else
            {
                VirtualBodies[i].PrePosition = VirtualBodies[i].StartPosition = VirtualBodies[i].Position = Bodies[i].ThisRigidbody.position;
                VirtualBodies[i].NowVelocity = VirtualBodies[i].InitialVelocity;
            }
            //VirtualBodies[i].NowVelocity = VirtualBodies[i].InitialVelocity;
            VirtualBodies[i].FuturePositions.Clear();
            VirtualBodies[i].FuturePositions.Add(VirtualBodies[i].Position);
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
