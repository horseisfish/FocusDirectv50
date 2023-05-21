using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SourceLineController : MonoBehaviour
{
    [SerializeField]
    private LineController linePrefab;


    
    [SerializeField]
    public GameObject Target;

    [SerializeField]
    private float TargetWidth;

    private Transform ModifyTransform;

    // Start is called before the first frame update
    private void Awake()
    {

        

        //Target = GameObject.Find("/nbackquestion/SourcePlane");

        LineController newLine = Instantiate(linePrefab);

        //ModifyTransform.position = Target.transform.position+ new Vector3(0,TargetWidth,0);

        
        
        
        newLine.AssignTarget(transform.position, Target.transform, TargetWidth);
        
    }
    
}
