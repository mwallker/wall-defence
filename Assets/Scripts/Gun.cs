using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;

public class Gun : MonoBehaviour
{
    [SerializeField]
    private Animator GunAnimator;

    [SerializeField]
    private SpriteRenderer BaseSpriteReference;

    [SerializeField]
    private SpriteRenderer BarrelSpriteReference;

    [SerializeField]
    private TextMeshProUGUI LevelLabelReference;

    public float AttackDamage { get; private set; }

    public float AttackSpeed { get; private set; }

    public float AttackDistance { get; private set; }

    public int Tier { get; private set; }

    private readonly float SHOOT_TIME = 0.1f;

    private readonly float AIMING_TIME = 0.25f;

    public bool IsShooting { get; private set; } = false;

    public GunPlatform Platform { get; private set; }

    private Monster target;

    void Start()
    {
        StartCoroutine(Attack());

        Messaging<LevelStateChangedEvent>.Register((state) =>
        {
            StopAllCoroutines();
            GunAnimator.SetBool("IsShooting", false);
        });
    }

    void OnDisable()
    {
        StopAllCoroutines();
    }

    public void Build(GunPlatform platform)
    {
        Platform = platform;
        transform.SetPositionAndRotation(platform.transform.position, platform.transform.rotation);
    }

    public void Upgrade(GunTierTemplate tier)
    {
        AttackDamage = tier.Damage;
        AttackDistance = tier.Distance;
        AttackSpeed = tier.Speed;
        Tier = tier.Id;

        if (tier.Base != null)
        {
            BaseSpriteReference.sprite = tier.Base;
        }

        if (tier.Barrel != null)
        {
            BarrelSpriteReference.sprite = tier.Barrel;
        }

        GunAnimator.speed = AttackSpeed;
        LevelLabelReference.text = (Tier + 1).ToString();
    }

    private IEnumerator Attack()
    {
        while (true)
        {
            target = FindObjectsOfType<Monster>()
               ?.Where(IsMonsterInRange)
               ?.OrderBy(monster => monster.transform.position.y)
               ?.FirstOrDefault();

            if (target != null && !IsShooting)
            {
                IsShooting = true;
                GunAnimator.SetBool("IsShooting", true);

                Messaging<GunShootEvent>.Trigger?.Invoke(this);

                yield return new WaitForSeconds(SHOOT_TIME / AttackSpeed);

                IsShooting = false;
                GunAnimator.SetBool("IsShooting", false);
            }

            yield return new WaitForSeconds(AIMING_TIME / AttackSpeed);
        }
    }

    private bool IsMonsterInRange(Monster monster)
    {
        if (monster.Line != Platform.Line || !monster.gameObject.activeSelf)
        {
            return false;
        }

        float distance = monster.transform.position.y - transform.position.y;

        return distance > 0 && distance <= AttackDistance;
    }
}
