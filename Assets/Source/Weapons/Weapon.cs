using System.ComponentModel.DataAnnotations;
using UnityEngine;

abstract class Weapon : MonoBehaviour
{
    [SerializeField, Required]
    protected GameObject projectilePrefab;

    public bool IsEquipped => Tier != 0;

    public int Tier { get; set; } = 0;

    [field: SerializeField]
    public float Cooldown { get; private set; } = 1f;

    public float LastFireTime { get; private set; } = 0f;

    public void Fire()
    {
        LastFireTime = Time.time;
        OnFire(transform.rotation);
    }

    // based on cooldown
    public virtual bool CanFire() => Time.time - LastFireTime >= Cooldown;

    protected virtual void OnFire(Quaternion direction)
    {
        Instantiate(projectilePrefab, transform.position, direction * Quaternion.Euler(0, 0, 90));
    }

    // Calculation helpers for tiered stats
    protected float TieredMultiplicative(float baseValue, float perTier) =>
        baseValue * Mathf.Pow(perTier, Tier - 1);

    protected float TieredAdditive(float baseValue, float perTier) =>
        baseValue + perTier * (Tier - 1);

    protected float TieredLogarithmic(float baseValue, float perTier) =>
        baseValue + perTier * Mathf.Log(Tier);

    protected float TieredExponential(float baseValue, float perTier) =>
        baseValue * Mathf.Exp(perTier * (Tier - 1));
}
