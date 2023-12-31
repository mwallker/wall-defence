using UnityEngine;

public class BulletSpawner : MonoBehaviour
{
    [SerializeField]
    private Bullet bulletPrefab;

    private Pool<Bullet> bulletsPool;

    void Awake()
    {
        bulletsPool = new Pool<Bullet>(CreateBullet, GetBullet, ReleaseBullet);

        Messaging<GunShootEvent>.Register(HandleShoot);
        Messaging<LevelStateChangedEvent>.Register(HandleStateChange);
    }

    void OnDisable()
    {
        bulletsPool.Clear();

        Messaging<GunShootEvent>.Unregister(HandleShoot);
        Messaging<LevelStateChangedEvent>.Unregister(HandleStateChange);
    }

    private Bullet CreateBullet()
    {
        return Instantiate(bulletPrefab);
    }

    private void GetBullet(Bullet bullet)
    {
        bullet.gameObject.SetActive(true);
    }

    private void ReleaseBullet(Bullet bullet)
    {
        bullet.gameObject.SetActive(false);

        if (bullet.TryGetComponent<Rigidbody2D>(out var rb))
        {
            rb.Sleep();
        }
    }

    private void HandleShoot(Gun gun)
    {
        Bullet bullet = bulletsPool.Get();

        bullet.SetPool(bulletsPool);
        bullet.ShootFrom(gun);
    }

    private void HandleStateChange(LevelState state)
    {
        bulletsPool.Clear();
    }
}
