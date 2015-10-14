using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Add PlayerManagement on top of NetworkManager
/// </summary>
public class GameNetworkManager : NetworkManager {

    private static GameNetworkManager instance;
    public static GameNetworkManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GameNetworkManager>();
            }
            return instance;
        }
    }

}
