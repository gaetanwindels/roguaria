using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;

public class LobbyManager : NetworkBehaviour
{
    [SerializeField] TMP_InputField ipAddress;

    [SerializeField] GameObject startButton;

    void Start()
    {
        ipAddress.text = GetPublicIP() + ":7777";
    }

    // Update is called once per frame
    void Update()
    {
        startButton.SetActive(isServer);
    }

    public static string GetPublicIP()
    {
        string url = "http://checkip.dyndns.org";
        System.Net.WebRequest req = System.Net.WebRequest.Create(url);
        System.Net.WebResponse resp = req.GetResponse();
        System.IO.StreamReader sr = new System.IO.StreamReader(resp.GetResponseStream());
        string response = sr.ReadToEnd().Trim();
        string[] a = response.Split(':');
        string a2 = a[1].Substring(1);
        string[] a3 = a2.Split('<');
        string a4 = a3[0];
        return a4;
    }

    public void StartGame()
    {
        NetworkManager.singleton.ServerChangeScene("Game Scene");
    }
}
