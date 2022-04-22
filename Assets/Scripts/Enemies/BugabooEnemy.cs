using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BugabooEnemy : Enemy
{
    [SerializeField]
    private int delay;
    [SerializeField]
    private Enemy spawnEnemy;
    
    protected override void Start()
    {
        base.Start();
        StartCoroutine(SpawnEnemy());
    }

    private IEnumerator SpawnEnemy()
    {
        while (true)
        {
            yield return new WaitForSeconds(this.delay);
            base.Slow(10f, 0.5f);
            Enemy enemy = Instantiate(spawnEnemy).GetComponent<Enemy>();
            enemy.transform.parent = GameObject.Find("Enemies").transform;
            enemy.Spawn(transform.position, new Stack<Vector2>(base.path.Reverse()));
        }
    }
}