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
    private SpriteRenderer AttackRangeSpriteReference;

    [SerializeField]
    private TextMeshProUGUI LevelLabelReference;

    [SerializeField]
    private AudioClip BuildSoundEffect;

    [SerializeField]
    private AudioClip ShootSoundEffect;

    private AudioSource GunAudioSource;

    public float AttackDamage { get; private set; }

    public float AttackSpeed { get; private set; }

    public float AttackDistance { get; private set; }

    public float Cost { get; private set; }

    public int Tier { get; private set; }

    private readonly float SHOOT_TIME = 0.1f;

    private readonly float AIMING_TIME = 0.25f;

    public bool IsShooting { get; private set; } = false;

    public GunPlatform Platform { get; private set; }

    private Monster target;

    void Awake()
    {
        GunAudioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        StartCoroutine(Attack());

        Messaging<LevelStateChangedEvent>.Register(HandleLevelStateChange);
    }

    void OnDisable()
    {
        StopAllCoroutines();

        Messaging<LevelStateChangedEvent>.Unregister(HandleLevelStateChange);
    }

    public void Build(GunPlatform platform)
    {
        Platform = platform;
        transform.SetPositionAndRotation(platform.transform.position, platform.transform.rotation);

        PlaySoundEffect(BuildSoundEffect);
    }

    public void Upgrade(GunTierTemplate tier)
    {
        AttackDamage = tier.Damage;
        AttackDistance = tier.Distance;
        AttackSpeed = tier.Speed;
        Tier = tier.Id;
        Cost = tier.Cost;

        if (tier.Base != null)
        {
            BaseSpriteReference.sprite = tier.Base;
        }

        if (tier.Barrel != null)
        {
            BarrelSpriteReference.sprite = tier.Barrel;
        }

        AttackRangeSpriteReference.size = new Vector2(1.1f, tier.Distance); // size.Set(1.1f, tier.Distance);

        GunAnimator.speed = AttackSpeed;
        LevelLabelReference.text = Tier.ToString();

        PlaySoundEffect(BuildSoundEffect);
    }

    private IEnumerator Attack()
    {
        while (true)
        {
            target = FindObjectsByType<Monster>(FindObjectsSortMode.None)
               ?.Where(IsMonsterInRange)
               ?.OrderBy(monster => monster.transform.position.y)
               ?.FirstOrDefault();

            if (target != null && !IsShooting)
            {
                IsShooting = true;
                GunAnimator.SetBool("IsShooting", true);

                PlaySoundEffect(ShootSoundEffect);
                Messaging<GunShootEvent>.Trigger?.Invoke(this);

                yield return new WaitForSeconds(SHOOT_TIME / AttackSpeed);

                IsShooting = false;
                GunAnimator.SetBool("IsShooting", false);
            }

            yield return new WaitForSeconds(AIMING_TIME / AttackSpeed);
        }
    }

    private void PlaySoundEffect(AudioClip clip)
    {
        GunAudioSource.clip = clip;
        GunAudioSource?.Play();
    }

    private void HandleLevelStateChange(LevelState state)
    {
        StopAllCoroutines();
        GunAnimator.SetBool("IsShooting", false);
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
