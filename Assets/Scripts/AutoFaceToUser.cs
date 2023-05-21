using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoFaceToUser : MonoBehaviour
{
    [SerializeField]
    public GameObject Player;

    private void Start()
    {
        Player = GameObject.FindWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        //transform.LookAt(Player.transform);
        transform.rotation = Quaternion.LookRotation(transform.position - Player.transform.position);
    }
}
