using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetGenerater : MonoBehaviour
{

    public GameObject objectPrefab; // the prefab of the object to be generated
    public GameObject sourceObject; // the source object from which the generated object will be positioned
    public float radius = 0f;
    public float generateTime;
    public float timer=0;
    public Transform Parent;
    Vector3 direction;
    [SerializeField]
    private LineController linePrefab;
    private GameObject Target;

    [SerializeField]
    private float TargetWidth;

    public SpriteRenderer cautionSignPrefab;

    public Sprite[] cautionSignList;

    public GameObject obj;

    
    

    public void GenerateTarget(int typeOfRegion, int conditionType, int signType)
    {
        obj = Instantiate(objectPrefab, Vector3.zero, Quaternion.identity,Parent);
        Target = obj;
        if(conditionType != 3)
        {
            LineController newLine = Instantiate(linePrefab, obj.transform);
            newLine.AssignTarget(transform.position, Target.transform, TargetWidth);
        }
        
        if(conditionType == 3)
        {
            AudioSource serion = obj.GetComponent<AudioSource>();
            if (!serion.isPlaying)
            {
                serion.Play();
            }
        }

        // generate the object at the random chosen angle area
        float xAngle = Random.Range(-20f, 50f);
        float yAngle = 0f;
       switch (typeOfRegion)
        {
            case 0:
                yAngle = Random.Range(-30f, 30f);
                break;
            case 1:
                while (yAngle == 0)
                {
                    yAngle = Random.Range(-32f, 32f);
                }

                if (yAngle < 0)
                {
                    yAngle -= 30;
                }
                if (yAngle > 0)
                {
                    yAngle += 30;
                }
                break;
            //case 2:

            //    while (yAngle == 0)
            //    {
            //        yAngle = Random.Range(-48f, 48f);
            //    }

            //    if (yAngle < 0)
            //    {
            //        yAngle -= 94;
            //    }
            //    if (yAngle > 0)
            //    {
            //        yAngle += 94;
            //    }
            //    break;
        }
        if (xAngle > 0)
        {
            direction = Quaternion.Euler(xAngle, yAngle, 0) * new Vector3(0, 1, 1);
        }
        if (xAngle < 0)
        {
            direction = Quaternion.Euler(xAngle, yAngle, 0) * new Vector3(0, -1, 1);
        }
        if (xAngle == 0)
        {
            direction = Quaternion.Euler(xAngle, yAngle, 0) * new Vector3(0, 0, 0);
        }
        if (yAngle >= 0)
        {
                SpriteRenderer cautionSign = Instantiate(cautionSignPrefab, new Vector3(2 * TargetWidth, 0, -2 * TargetWidth), new Quaternion(0, 0, 0, 0), obj.transform);
                cautionSign.sprite = cautionSignList[signType];
            
        }
        if (yAngle < 0)
        {
            
                SpriteRenderer cautionSign = Instantiate(cautionSignPrefab, new Vector3(-2 * TargetWidth, 0, -2 * TargetWidth), new Quaternion(0, 0, 0, 0), obj.transform);
                cautionSign.sprite = cautionSignList[signType];
            
        }
        obj.transform.Translate(direction * radius, Space.World);
    }

}
