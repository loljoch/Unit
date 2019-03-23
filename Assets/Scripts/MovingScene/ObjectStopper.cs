using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectStopper : MonoBehaviour
{
    Transform playerObject;
    [SerializeField] private float distanceToPlayer;
    [SerializeField] private string playerTag;
    [SerializeField] private float followDistance;

    private void Awake()
    {
        AssignPlayer();
        //Decides distance between the z position of the player and the wall
        distanceToPlayer = Vector2.Distance(new Vector2(transform.position.z, 0), new Vector2(playerObject.position.z, 0));
        Debug.Log(distanceToPlayer);
    }

    void Update()
    {
        StayWithPlayerObject();
        
    }

    //Makes sure the wall stays in front of the player object so the not-controller objects can't walk further when the player has been stopped by a wall
    private void StayWithPlayerObject()
    {
        Vector3 playerPos = new Vector3(transform.position.x, transform.position.y, playerObject.transform.position.z + distanceToPlayer + followDistance);
        transform.position = playerPos;
    }

    public void AssignPlayer()
    {
        playerObject = GameObject.FindGameObjectWithTag(playerTag).transform;
    }
}
