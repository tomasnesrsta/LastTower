using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    private enum TowerType
    {
        Arrow,
        Cannon,
        Magic
    }
    
    private SpriteRenderer spriteRenderer;

    public int Damage
    {
        get { return damage; }
    }
    
    [SerializeField]
    private int damage;

    [SerializeField]
    private float projectileSpeed;

    [SerializeField]
    private Projectile projectile;

    [SerializeField]
    private TowerType towerType;
    public float ProjectileSpeed
    {
        get
        { return projectileSpeed; }
    }

    private List<Enemy> enemies = new List<Enemy>();

    [SerializeField]
    private float attackSpeed;

    private bool started = false;
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    
    public void Select()
    {
        spriteRenderer.enabled = !spriteRenderer.enabled;
    }

    private IEnumerator Attack()
    {
        if (enemies.Count > 0)
        {
            foreach (Enemy enemy in enemies)
            {
                if (towerType == TowerType.Magic)
                {
                    if (enemy.CurrentSpeed == enemy.Speed)
                    {
                        Shoot(enemy);
                        break;
                    }
                }
                else
                {
                    Shoot(enemy);
                    break;
                }
            }
        }

        yield return new WaitForSeconds(attackSpeed);
        StartCoroutine(nameof(Attack));
    }

    private void Shoot(Enemy enemy)
    {
        Projectile proj = Instantiate(projectile, transform.position, transform.rotation);
        proj.transform.parent = GameObject.Find("Projectiles").transform;
        proj.Init(this, enemy);
        projectile.transform.position = transform.position;
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            enemies.Add(other.GetComponent<Enemy>());
            if (!started)
            {
                StartCoroutine("Attack");
                started = true;
            }
        }
    }

    public void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            enemies.Remove(other.GetComponent<Enemy>());
            if (enemies.Count == 0)
            {
                started = false;
                StopCoroutine("Attack");
            }
        }
    }
}
