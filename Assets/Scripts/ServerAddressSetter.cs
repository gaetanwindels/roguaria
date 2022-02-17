using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class ServerAddressSetter : MonoBehaviour
{
    // Start is called before the first frame update
    private NetworkManager networkManager;
    void Start()
    {
        networkManager = NetworkManager.singleton;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetServerAddress(string address)
    {
        networkManager.networkAddress = address;
    }
}
