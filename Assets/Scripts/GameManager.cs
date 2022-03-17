using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GameManager : NetworkBehaviour
{
    private UiManager uiManager;

    private bool isStarted = false;
    private bool isGameOver = false;

    public bool IsStarted { get => isStarted; set => isStarted = value; }
    public bool IsGameOver { get => isGameOver; set => isGameOver = value; }

    // Start is called before the first frame update
    void Start()
    {
        uiManager = FindObjectOfType<UiManager>();

        if (isServer)
        {
            InitGame();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isServer)
        {
            var players = FindObjectsOfType<Player>();
            var countDead = 0;

            foreach (var player in players)
            {
                if (player.Dead)
                {
                    countDead++;
                }
            }

            if (Input.GetKeyDown(KeyCode.P))
            {
                TogglePause();
            }

            if (countDead == players.Length)
            {
                uiManager.DisplayGameOver();
                IsGameOver = true;
                StopGame();
            }
        }

    }

    [ClientRpc]
    public void TogglePause()
    {
        if (Time.timeScale == 1)
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
        }

    }

    [ClientRpc]
    public void StopGame()
    {
        Time.timeScale = 0;
    }

    public void PlayerJoined(Player player)
    {
        var players = FindObjectsOfType<Player>();

        if (isServer)
        {
            var ground = GameObject.FindWithTag(Tags.Ground);
            var startPositions = FindObjectsOfType<StartPosition>();

            StartPosition startPos = null;
            foreach (var pos in startPositions)
            {
                if (pos.Player == null)
                {
                    startPos = pos;
                }
            }

            if (startPos != null)
            {
                startPos.Player = this.gameObject;
                player.InitPosition(new Vector3(startPos.transform.position.x, ground.transform.position.y, startPos.transform.position.z));
            }
        }

        if (players.Length == NetworkManager.singleton.numPlayers)
        {
            InitGame();
        }
    }

    public void InitGame()
    {
        IsStarted = true;
    }
}
