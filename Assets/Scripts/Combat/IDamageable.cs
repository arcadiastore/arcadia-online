namespace ArcadiaOnline.Combat
{
    /// <summary>
    /// Interface untuk semua entitas yang bisa menerima damage
    /// (Player, Monster, Boss, destructible object).
    /// </summary>
    public interface IDamageable
    {
        float CurrentHP { get; }
        float MaxHP { get; }
        bool IsDead { get; }

        void TakeDamage(DamageResult damage);
        void Heal(float amount);
    }
}
