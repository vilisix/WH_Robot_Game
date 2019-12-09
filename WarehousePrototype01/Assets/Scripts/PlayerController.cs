using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private LevelBuilder lb;
    private Vector3 targetPoint;
    private Quaternion targetRotation;
    private bool canMove = true;


    void Start()
    {        
        lb = GameObject.Find("LevelBuilder").GetComponent<LevelBuilder>();
        targetPoint = transform.position;
        targetRotation = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        if (targetRotation == transform.rotation && targetPoint == transform.position)
        {
            if (Input.GetKeyDown(KeyCode.Space)) lb.PickOrPut();
            if (Input.GetKey(KeyCode.W)) targetPoint = lb.MovePlayer();
            if (Input.GetKey(KeyCode.A)) targetRotation = lb.RotatePlayer(false);
            if (Input.GetKey(KeyCode.D)) targetRotation = lb.RotatePlayer(true);
        }
    }

    private void FixedUpdate()
    {
        if (targetPoint == transform.position) canMove = true;
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPoint, 0.1f);
            canMove = false;
        }
        if (Quaternion.Angle(transform.rotation, targetRotation) < 1)
        {
            transform.rotation = targetRotation;
            canMove = true;
        }
        else
        {
            canMove = false;
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 0.3f);
        }
    }

    public void DisablePlayerMovement() => canMove = false;
    
}
