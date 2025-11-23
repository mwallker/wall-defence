using UnityEngine;

public class Wall : MonoBehaviour
{
    private Animator _wallAnimator;

    private AudioSource _wallAudioSource;

    void Awake()
    {
        _wallAnimator = GetComponent<Animator>();
        _wallAudioSource = GetComponent<AudioSource>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent<IDamageable>(out var target))
        {
            target.HitCore();
        }

        _wallAnimator.SetTrigger("Hit");
        _wallAudioSource.Play();
    }
}
