using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class EnemySpawner : NetworkBehaviour
{
    [SerializeField] GameObject enemyToSpawn;
    [SerializeField] float spawnRate;

    private Player[] players = { };
    private GameManager gameManager;

    private float currentTimer = 0;

    // Start is called before the first frame update
    void Start()
    {
        players = FindObjectsOfType<Player>();
        gameManager = FindObjectOfType<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isServer || !gameManager.IsStarted)
        {
            return;
        }

        if (currentTimer == 0 || currentTimer >= spawnRate)
        {
            var enemy = Instantiate(enemyToSpawn, transform.position, Quaternion.identity);
            players = FindObjectsOfType<Player>();
            //enemy.SetPlayers(players);
            NetworkServer.Spawn(enemy.gameObject);
            currentTimer = 0;
        }

        currentTimer += Time.deltaTime;

    }
}
