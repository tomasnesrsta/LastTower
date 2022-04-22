using UnityEngine;

public class Projectile : MonoBehaviour
{
    private enum SpecialEffects
    {
        None,
        Slowness
    }
    
    private Enemy target;
    private Tower tower;

    [SerializeField]
    private AudioClip audioClip;

    [SerializeField]
    private SpecialEffects specialEffect;

    void Update()
    {
        MoveToTarget();
    }

    public void Init(Tower tower, Enemy target)
    {
        this.tower = tower;
        this.target = target;
        AudioSource.PlayClipAtPoint(audioClip, Camera.main.transform.position);
    }

    private void MoveToTarget()
    {
        if (target)
        {
            transform.position = Vector3.MoveTowards(transform.position, target.transform.position,
                Time.deltaTime * tower.ProjectileSpeed);
            Vector2 direction = target.transform.position - transform.position;
            float angle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            switch (specialEffect)
            {
                case SpecialEffects.None:
                    break;
                case SpecialEffects.Slowness:
                    target.Slow(30f, 5f);
                    break;
            }
            other.GetComponent<Enemy>().Health -= tower.Damage;
            Destroy(gameObject);
        }
    }
}
