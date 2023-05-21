using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineController : MonoBehaviour
{
    private LineRenderer lineRenderer;

    private Transform target;

    private float targetWitdth;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    public void AssignTarget(Vector3 startPosition, Transform newTarget, float newTargetWidth)
    {
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, startPosition);
        target = newTarget;
        targetWitdth = newTargetWidth;
    }

    // Update is called once per frame
    void Update()
    {
        lineRenderer.SetPosition(1, target.position+ new Vector3(0, 0, targetWitdth));
    }
}
