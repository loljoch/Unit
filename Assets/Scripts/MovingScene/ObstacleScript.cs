using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleScript : MonoBehaviour
{
    [SerializeField] private string playerTag;
    [SerializeField] private string enemyTag;
    [SerializeField] private string obstacleTag;
    private string ownTag;


    private void Awake()
    {
        ownTag = transform.parent.tag;
    }

    ////Makes sure that not-controlled objects can pass through walls
    //private void OnTriggerEnter(Collider other)
    //{
    //    ownTag = transform.parent.tag;
    //    if (other.transform.tag == obstacleTag && ownTag != enemyTag)
    //    {
    //        other.GetComponent<BoxCollider>().isTrigger = false;
    //    } else if (other.transform.tag == obstacleTag)
    //    {
    //        other.GetComponent<BoxCollider>().isTrigger = true;
    //    }
    //}

    //Makes sure that not-controlled objects can pass through walls
    private void OnTriggerStay(Collider other)
    {
        ownTag = transform.parent.tag;
        //if player is touching
        if (other.transform.tag == obstacleTag)
        {
            if (ownTag != enemyTag)
            {
                other.GetComponent<BoxCollider>().isTrigger = false;
            } else
            {
                other.GetComponent<BoxCollider>().isTrigger = true;
            }
        }
    }

}
