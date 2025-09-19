// Rust enum style. Use pattern matching.

abstract record DamageSource
{
    public record Weapon(global::Weapon Source) : DamageSource;
    public record Enemy(global::Enemy Source) : DamageSource;
    public record Unknown() : DamageSource;
}

abstract record Damage(int Amount, DamageSource Source)
{
    public record Physical(int Amount, DamageSource Source) : Damage(Amount, Source);
}
