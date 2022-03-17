using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class UiManager : NetworkBehaviour
{
    [SerializeField] private GameObject gameOverScreen;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    [ClientRpc]
    public void DisplayGameOver()
    {
        gameOverScreen.SetActive(true);
    }
}
