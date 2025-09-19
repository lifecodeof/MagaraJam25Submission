using R3;


static class Events
{
    public static readonly Subject<Unit> EnemyKilled = new();
    public static readonly Subject<SkillTreeSkillButton> SkillUnlocked = new();
}
