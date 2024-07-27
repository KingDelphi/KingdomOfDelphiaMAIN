using Fusion;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public static class AuthorityHelper
{
    public static NetworkObject FindNetworkObjectWithStateAuthority()
    {
        var networkObjects = Object.FindObjectsOfType<NetworkObject>();
        return networkObjects.FirstOrDefault(no => no.HasStateAuthority);
    }

    public static List<NetworkObject> FindAllNetworkObjectsWithStateAuthority()
    {
        var networkObjects = Object.FindObjectsOfType<NetworkObject>();
        return networkObjects.Where(no => no.HasStateAuthority).ToList();
    }
}