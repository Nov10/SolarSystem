using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;
using static UnityEngine.Rendering.HableCurve;


[System.Serializable]  
public class VirtualBody
{
    public float Mass;
    public float Radius;

    public Vector3 InitialVelocity;
    public Vector3 NowVelocity;

    public Vector3 PrePosition;
    public Vector3 Position;
    public Vector3 StartPosition;


    public List<Vector3> FuturePositions = new List<Vector3>();

    public void Reset(Vector3 position, Vector3 nowVelocity)
    {
        StartPosition = Position = PrePosition = position;
        InitialVelocity = NowVelocity = nowVelocity;
    }

    public VirtualBody(float mass, float radius, Vector3 initialVelocity, Vector3 nowVelocity, Vector3 position)
    { 
        Mass = mass;
        Radius = radius;
        InitialVelocity = initialVelocity;
        NowVelocity = nowVelocity;
        PrePosition = Position = position;
        StartPosition = position;
    }

    public void UpdateVelocity(VirtualBody[] bodies, float dt)
    {
        for (int i = 0; i < bodies.Length; i++)
        {
            VirtualBody other = bodies[i];
            if (other == this)
                continue;
            float sqrDistance = (other.Position - Position).sqrMagnitude;
            Vector3 forceDirection = (other.Position - Position).normalized;
            if (sqrDistance < float.Epsilon)
                continue;
            Vector3 force = forceDirection * Universe.GravitationalConstant * Mass * other.Mass / sqrDistance;

            Vector3 acceleration = force / Mass;
            NowVelocity += acceleration * dt;
        }
    }

    public void UpdatePosition(float dt)
    {
        PrePosition = Position;
        Position += NowVelocity * dt;
        FuturePositions.Add(Position);
    }

       public float maxDistance = 0;
    public float minDistance = float.MaxValue;
    public float CalculateE(VirtualBody pivot)
    {

        for(int i = 0; i < FuturePositions.Count; i++)
        {
            float sqrDistance = (FuturePositions[i] - pivot.FuturePositions[i] + pivot.StartPosition).sqrMagnitude;
            if(sqrDistance > maxDistance)
                maxDistance = sqrDistance;
            if(sqrDistance < minDistance)
                minDistance = sqrDistance;
        }
        return Mathf.Sqrt(1 - minDistance / maxDistance);
    }
}

public class Body : MonoBehaviour
{
    public float E;
    private void OnValidate()
    {
        //OrbitDrawer.Instance.DrawT();
    }
    //[SerializeField] public VirtualBody ThisBody;

    //public float SurfaceGravity = 10;
    public float Mass;
    public float Radius;

    public Vector3 InitialVelocity;
    Vector3 prePosition;
    [HideInInspector] public Vector3 NowVelocity;
    [HideInInspector] public Rigidbody ThisRigidbody;

    [SerializeField] public Body Pivot;

    public void Initialize()
    { 
        //Mass = SurfaceGravity * Radius * Radius / Universe.GravitationalConstant;
        ThisRigidbody = GetComponent<Rigidbody>();
        NowVelocity = InitialVelocity;
        ThisRigidbody.mass = Mass; 
        transform.localScale = Vector3.one * Radius;
    }
    private void Awake()
    {
        Initialize();
    }

    public void UpdateVelocity(Body[] bodies, float dt)
    {
        for(int i = 0; i < bodies.Length; i++) { 
            Body other = bodies[i];
            if (other == this)
                continue;
            float sqrDistance = (other.ThisRigidbody.position - ThisRigidbody.position).sqrMagnitude;
            Vector3 forceDirection = (other.ThisRigidbody.position - ThisRigidbody.position).normalized;

            Vector3 force = forceDirection * Universe.GravitationalConstant * Mass * other.Mass / sqrDistance;

            Vector3 acceleration = force / Mass;
            NowVelocity += acceleration * dt;
        }
    }

    float preAngle;
    Vector3 prePivot;
    public void UpdatePosition(float dt)
    {
        ThisRigidbody.position += NowVelocity * dt;
    }
    void SaveValueToFile(float value)
    {
        string filePath = Path.Combine(Application.dataPath, "SValues.txt");
        using (StreamWriter writer = new StreamWriter(filePath, true))
        {
            writer.WriteLine(value);
        }
    }
    void InitializeFile()
    {
        string filePath = Path.Combine(Application.dataPath, "SValues.txt");
        // 파일 초기화
        using (StreamWriter writer = new StreamWriter(filePath, false))
        {
            writer.WriteLine("");
        }
    }
    IEnumerator Kapler()
    {
        Vector3 prePos = ThisRigidbody.position;
        Vector3 prePivot = Pivot.ThisRigidbody.position;
        Debug.Log(Path.Combine(Application.dataPath, "SValues.txt"));
        while (true)
        {
            float angle = Vector3.Angle((prePos - prePivot), (ThisRigidbody.position - Pivot.ThisRigidbody.position));
            float preD = Vector3.Magnitude(prePos - prePivot);
            float nowD = Vector3.Magnitude(ThisRigidbody.position - Pivot.ThisRigidbody.position);
            float S = 0.5f * Mathf.Pow((preD + nowD) / 2, 2) * angle;
            float t = 1f;
            if(transform.name == "금")
            {
                SaveValueToFile(S);
                Debug.LogFormat("{0} : {1}", transform.name, S);

                int segments =(int)(nowD/30f);
                for (int i = 0; i <= segments; i++)
                {
                    Vector3 a = (prePivot + Pivot.ThisRigidbody.position) / 2f;
                    Vector3 start = Vector3.Lerp(a, prePos, i / (float)segments);
                    Vector3 end = Vector3.Lerp(a, ThisRigidbody.position, i / (float)segments);

                    Debug.DrawLine(start, end, Color.yellow, t);
                }

                //Debug.DrawLine(prePivot, prePos, Color.yellow, 5f);
                //Debug.DrawLine(Pivot.ThisRigidbody.position, ThisRigidbody.position, Color.yellow, 5f);
            }
            prePivot = Pivot.ThisRigidbody.position;
            prePos = ThisRigidbody.position;
            yield return new WaitForSeconds(t);
        }
    }

    private void Update()
    {
        Debug.DrawLine(prePosition, ThisRigidbody.position, Color.yellow, 100f);
        //Debug.LogFormat("theta {0}, t {1}, {2}", angle - preAngle, Time.deltaTime, transform.name);
        //prePivot = Pivot.ThisRigidbody.position;
        prePosition = ThisRigidbody.position;
        //preAngle = angle;
    }

}
