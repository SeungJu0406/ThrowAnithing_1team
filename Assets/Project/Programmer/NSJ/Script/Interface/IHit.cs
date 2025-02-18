
public interface IHit
{
    int MaxHp { get; set; }
    int TakeDamage(int damage, bool isIgnoreDef);

    void TakeCrowdControl(CrowdControlType type, float time);
}
