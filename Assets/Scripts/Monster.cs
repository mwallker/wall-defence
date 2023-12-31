using UnityEngine;

public class Monster : MonoBehaviour, IDamageable
{
    [SerializeField]
    private Animator MonsterAnimator;

    [SerializeField]
    private MonsterHealthBar HealthBarReference;

    [SerializeField]
    public SpriteRenderer BodyReference;

    [SerializeField]
    public SpriteRenderer EyesReference;

    [SerializeField]
    public SpriteRenderer LeftHandReference;

    [SerializeField]
    public SpriteRenderer RightHandReference;

    [SerializeField]
    public SpriteRenderer MouthReference;

    public float MaxHitPoints { get; private set; } = 1f;

    public float HitPoints { get; private set; } = 1f;

    public float Speed { get; private set; } = 1f;
    public float Reward { get; private set; } = 1f;

    public int Score { get; private set; } = 10;

    public int Line { get; private set; } = -1;

    void Awake()
    {
        MonsterAnimator.ResetTrigger("Hit");
    }

    public void TakeDamage(float amount)
    {
        if (gameObject.activeSelf)
        {
            HitPoints -= amount;
            MonsterAnimator.SetTrigger("Hit");
            HealthBarReference.SetHitPoints(HitPoints, MaxHitPoints);

            if (HitPoints <= 0)
            {
                Messaging<MonsterDefeatedEvent>.Trigger?.Invoke(this);

                HitPoints = 0;
                gameObject.SetActive(false);
            }
        }
    }

    public void HitCore()
    {
        MonsterAnimator.ResetTrigger("Hit");
        gameObject.SetActive(false);

        Messaging<MonsterHitCoreEvent>.Trigger?.Invoke(this);
    }

    public Monster SetAttackLine(int value)
    {
        Line = value;
        transform.position = new Vector2(value * 2 - 5, 20f);

        return this;
    }

    public Monster SetParameters(MonsterTemplate parameters)
    {
        Speed = parameters.Speed;
        Reward = parameters.Reward;
        Score = parameters.Score;
        MaxHitPoints = HitPoints = parameters.HitPoints;

        BodyReference.sprite = parameters.Body;
        EyesReference.sprite = parameters.Eyes;
        LeftHandReference.sprite = parameters.LeftHand;
        RightHandReference.sprite = parameters.RightHand;
        MouthReference.sprite = parameters.Mouth;

        HealthBarReference.SetHitPoints(HitPoints, MaxHitPoints);

        return this;
    }
}
