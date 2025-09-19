using System.ComponentModel.DataAnnotations;
using UnityEngine;

abstract class Weapon : MonoBehaviour
{
    [SerializeField, Required]
    protected GameObject projectilePrefab;

    public int Tier { get; set; } = 1;

    public abstract float Cooldown { get; }

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
        Instantiate(projectilePrefab, transform.position, direction);
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
