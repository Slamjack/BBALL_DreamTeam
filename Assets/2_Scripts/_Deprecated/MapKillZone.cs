﻿using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// This script, when attached to any object with a collider, will kill any player colliding with it
/// </summary>
[NetworkSettings(channel = 3, sendInterval = 0.1f)]
public class MapKillZone : NetworkBehaviour
{

    /// <summary>
    /// Called when something collide
    /// </summary>
    /// <param name="col">Collision information</param>
    void OnCollisionEnter(Collision col)
    {
        GameObject playerRigidBody = col.gameObject;    //Get object related to collision 
        if (playerRigidBody.tag == "Player")            //If it's a player, kills it
        {
            playerRigidBody.GetComponent<PlayerCall>().Call_KillPlayer("");
        }
    }
}
