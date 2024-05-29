using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[ExecuteAlways]
public class NameCanvas : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    [SerializeField] float InverseScale = 100;
    // Update is called once per frame
    void Update()
    {
        float distance = (transform.position - SceneView.lastActiveSceneView.camera.transform.position).magnitude;
        transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one * distance / InverseScale, Time.deltaTime * 10f);
    }
}
