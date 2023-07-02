using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class NetworkManager : NetworkBehaviour
{   
    void Awake()
    {
        GameManager.Instance.networkManager = this;
    }

    public override void Spawned()
    {
        Debug.Log("NetworkManager is Spawned.");
    }
}
