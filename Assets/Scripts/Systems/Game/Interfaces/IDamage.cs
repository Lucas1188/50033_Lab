using System;
/// <summary>
/// Defines any defense attributes or special attributes affecting damage to be dealt. 
/// </summary>
public record DamageModificationAttributes
{
    public int type { get; set; } //TODO: Fully define damage and defense stack
    public float value { get; set; }
}
public enum DamageBuffOperator
{
    Add,
    Multiply,
}
#nullable enable
public interface IDamage
{
    public float DealDamage(DamageModificationAttributes[] damageAttributes);
    /// <summary>
    /// Debuff will be called by external systems. Implementations should cache the parameters and expire the effects appropriately.
    /// </summary>
    /// <param name="value">Debuff value</param>
    /// <param name="damageBuffOperator">Operator to implement</param>
    /// <param name="timeSpan">Time of effect</param>
    public void DeBuff(float value, DamageBuffOperator damageBuffOperator, TimeSpan timeSpan);
    /// <summary>
    /// Buff will be called by external systems. Implementations should cache the parameters and expire the effects appropriately.
    /// </summary>
    /// <param name="value">Debuff value</param>
    /// <param name="damageBuffOperator">Operator to implement</param>
    /// <param name="timeSpan">Time of effect</param>
    public void Buff(float value, DamageBuffOperator damageBuffOperator, TimeSpan timeSpan);
    public DamageModificationAttributes[]? GetDamageModificationAttributes();

}