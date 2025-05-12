interface ICharacter
{
    void TakeDamage(int amount);
    void Die();
    bool IsDead { get; }
}