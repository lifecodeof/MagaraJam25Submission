using R3;
using UnityEngine;

// TODO: Handle game over

[RequireComponent(typeof(Health))]
class PlayerLifecycleManager : MonoBehaviour
{
    void Awake()
    {
        var health = GetComponent<Health>();
        health.IsDead.Where(b => b)
            .Subscribe(_ => Debug.Log("game over"))
            .AddTo(this);
    }
}
