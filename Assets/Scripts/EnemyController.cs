using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class EnemyController : NetworkBehaviour
{
    // Config parameters
    [SerializeField] private float moveSpeed = 10;
    [SerializeField] private float turnSpeed = 10;
    private Player[] players;

    void Start()
    {
    }

    void OnCollisionEnter(Collision other)
    {
        if (!isServer)
        {
            return;
        }

        if (other.gameObject.tag == "bullet")
        {
            NetworkServer.Destroy(this.gameObject);
            NetworkServer.Destroy(other.gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!isServer)
        {
            return;
        }

        var closest = FindClosestPlayer();
        if (closest != null)
        {
            var targetPos = new Vector3(closest.transform.position.x, this.transform.position.y, closest.transform.position.y);
            transform.position = Vector3.MoveTowards(transform.position, targetPos, Time.deltaTime * moveSpeed);
            transform.LookAt(targetPos);
        }
    }

    public void SetPlayers(Player[] players)
    {
        this.players = players;
    }

    private Player FindClosestPlayer()
    {
        float minDistance = 100000000;
        Player minPlayer = null;
        foreach (var player in players)
        {
            var distance = Vector3.Distance(transform.position, player.transform.position);
            if (distance < minDistance)
            {
                minPlayer = player;
                minDistance = distance;
            }
        }

        return minPlayer;
    }
}
