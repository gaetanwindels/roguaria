using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class CustomNetworkManager : NetworkManager
{

    [SerializeField] private bool autoConnect = true;

    public override void OnServerConnect(NetworkConnection conn)
    {
        base.OnServerConnect(conn);
        Debug.Log("HEY s");
        Debug.Log(FindObjectsOfType<Player>().Length);
    }

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        base.OnServerAddPlayer(conn);

        Camera cam = Camera.main;
        float height = 2f * cam.orthographicSize;
        float width = height * cam.aspect;

        var number = FindObjectsOfType<Player>().Length - 1;
        var go = conn.identity.gameObject;
        var sectionSize = width / 4;
        var pos = (number * sectionSize) + (sectionSize / 2) - (width / 2);
        go.transform.position = new Vector3(pos, go.transform.position.y, go.transform.position.z);
    }

    public override void Awake()
    {
        if (!isNetworkActive && autoConnect)
        {
            StartHost();
        }
    }

}
