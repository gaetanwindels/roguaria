using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class EnemySpawner : NetworkBehaviour
{
    [SerializeField] EnemyController enemyToSpawn;
    [SerializeField] float spawnRate;

    private Player[] players = { };

    private float currentTimer = 0;

    // Start is called before the first frame update
    void Start()
    {
        players = FindObjectsOfType<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isServer)
        {
            return;
        }

        currentTimer += Time.deltaTime;

        if (currentTimer >= spawnRate)
        {
            var enemy = Instantiate<EnemyController>(enemyToSpawn, transform.position, Quaternion.identity);
            players = FindObjectsOfType<Player>();
            enemy.SetPlayers(players);
            NetworkServer.Spawn(enemy.gameObject);
            currentTimer = 0;
        }

    }
}
