﻿using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

/// <summary>
/// Used to store global scene variables
/// </summary>
[NetworkSettings(channel = 3, sendInterval = 0.1f)]
public class SceneOverlord : NetworkBehaviour
{
    //TODO : comment your shit
    [Header("References(Scene)")]
    [SerializeField]
    private EventPanel eventPanel;

    [Header("Global settings")]
    [SyncVar]
    public bool isFrozen;
    [SyncVar]
    public float respawntime;

    [Header("Scoring")]
    [SyncVar]
    public int scoreBLU = 0;
    [SyncVar]
    public int scoreRED = 0;

    [Header("Prefabs")]
    [SerializeField]
    private GameObject ballBody;

    [Server]
    public void Score(Team team)
    {
        switch (team)
        {
            case Team.BLU:
                scoreBLU++;
                StartNewRound(Team.RED);
                break;

            case Team.RED:
                scoreRED++;
                StartNewRound(Team.BLU);
                break;

            default:
                Debug.Log("Grats! You broke everything: this should not have happened!");
                break;
        }

        //TODO : Put score on global UI
    }

    [Server]
    void StartNewRound(Team losingteam)
    {
        respawntime = 0.0f;
        KillAllPlayers();
        isFrozen = true;
        FreezeAllPlayerUpdate();
        InitializeBall(losingteam);
        StartCoroutine(ReadySteadyGO());
    }

    IEnumerator ReadySteadyGO()
    {
        yield return new WaitForSeconds(1.0f);
        eventPanel.Rpc_Ready();
        yield return new WaitForSeconds(1.0f);
        eventPanel.Rpc_Steady();
        yield return new WaitForSeconds(1.0f);
        eventPanel.Rpc_Go();
        respawntime = 2.0f;
        isFrozen = false;
        FreezeAllPlayerUpdate();
    }

    [Server]
    void FreezeAllPlayerUpdate()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            if (player.GetComponent<PlayerStats>().playerTeam != Team.SPE)
            {
                player.GetComponent<PlayerCall>().Call_FreezePlayerUpdate();
            }
        }
    }

    [Server]
    void KillAllPlayers()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            if (player.GetComponent<PlayerStats>().playerTeam != Team.SPE)
            {
                player.GetComponent<PlayerCall>().Call_KillPlayer();
            }
        }
    }

    [Server]
    void InitializeBall(Team losingteam)
    {
        GameObject[] balls = GameObject.FindGameObjectsWithTag("Ball");
        foreach (GameObject ball in balls)
        {
            ball.GetComponent<BallPickUp>().Rpc_DestroyBall();
        }

        Vector3 spawnpos = Vector3.zero;
        GameObject defaultspawn = GameObject.FindGameObjectWithTag(m_Custom.GetBallSpawnTagFromTeam(losingteam));
        if (defaultspawn != null)
        {
            spawnpos = defaultspawn.transform.position;
        }
        else
        {
            Debug.Log("Could not find ball spawn in scene, make sure there is one in the scene");
        }

        GameObject newball = (GameObject)Instantiate(ballBody, spawnpos, Quaternion.identity);     //Creates new ball
        NetworkServer.Spawn(newball);                                                              //Instantiate it on all clients

    }
}
