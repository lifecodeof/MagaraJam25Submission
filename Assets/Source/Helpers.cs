using UnityEngine;
using UnityEngine.AddressableAssets;

public static class Helpers
{
    public static T FindRequired<T>() where T : Object
    {
        var obj = Object.FindFirstObjectByType<T>();

#pragma warning disable IDE0270 // Use coalesce expression: Unity object shouldn't use null-coalescing operator
        if (obj == null)
#pragma warning restore IDE0270
            throw new System.Exception($"Required object of type {typeof(T)} not found in scene.");

        return obj;
    }

    public static T GetAddressable<T>(string address)
    {
        return Addressables.LoadAssetAsync<T>(address).WaitForCompletion();
    }
}
