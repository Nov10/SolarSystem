using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Kepler : MonoBehaviour
{
    [SerializeField] Body TargetBody;
    [SerializeField] float Time = 1f;

    private void Start()
    {
        InitializeFile();
        StartCoroutine(Kapler2ndLaw());
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

    IEnumerator Kapler2ndLaw()
    {
        Vector3 prePos = TargetBody.ThisRigidbody.position;
        Vector3 prePivot = TargetBody.Pivot.ThisRigidbody.position;
        Debug.Log(Path.Combine(Application.dataPath, "SValues.txt"));
        while (true)
        {
            if (TargetBody == null)
            {
                yield return null;
                continue;
            }

            float angle = Vector3.Angle((prePos - prePivot), (TargetBody.ThisRigidbody.position - TargetBody.Pivot.ThisRigidbody.position));
            float preD = Vector3.Magnitude(prePos - prePivot);
            float nowD = Vector3.Magnitude(TargetBody.ThisRigidbody.position - TargetBody.Pivot.ThisRigidbody.position);
            float S = 0.5f * Mathf.Pow((preD + nowD) / 2, 2) * angle;
            float t = Time;
            SaveValueToFile(S);
            //Debug.LogFormat("{0} : {1}", transform.name, S);

            int segments = (int)(nowD / 30f);
            for (int i = 0; i <= segments; i++)
            {
                Vector3 a = (prePivot + TargetBody.Pivot.ThisRigidbody.position) / 2f;
                Vector3 start = Vector3.Lerp(a, prePos, i / (float)segments);
                Vector3 end = Vector3.Lerp(a, TargetBody.ThisRigidbody.position, i / (float)segments);

                Debug.DrawLine(start, end, Color.yellow, t);
            }

            prePivot = TargetBody.Pivot.ThisRigidbody.position;
            prePos = TargetBody.ThisRigidbody.position;
            yield return new WaitForSeconds(t);
        }
    }
}
