using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public enum EnemyType
    {
        Pumpkin,
        Tree,
        Bird,
        Bugaboo
    }
    
    
    [SerializeField]
    private float speed;

    [SerializeField]
    private int maxHealth;
    
    [SerializeField]
    private int reward;

    [SerializeField]
    private EnemyType type;
    
    private int health;
    
    private float currentSpeed;

    public EnemyType Type
    { 
        get { return type; }
    }
    
    public float Speed
    {
        get { return speed; }
    }
    
    public float CurrentSpeed
    {
        get { return currentSpeed; }
    }
    public int Health
    {
        get { return health; }
        set
        {
            health = value;
            if (IsDead())
                Destroy(gameObject);

            this.GetComponentInChildren<Bar>().Amount = (float)health / (float)maxHealth;
        }
    }
    
    protected Stack<Vector2> path;
    private Vector3 destination;

    public void Spawn()
    {
        GameManager.Instance.ActiveEnemies++;
        transform.position = LevelManager.Instance.Spawner.transform.position;
        SetPath(LevelManager.Instance.Path);
    }
    
    public void Spawn(Vector2 position, Stack<Vector2> path)
    {
        GameManager.Instance.ActiveEnemies++;
        transform.position = position;
        SetPath(path);
    }

    protected virtual void Start()
    {
        health = maxHealth;
        currentSpeed = speed;
    }

    protected virtual void Update()
    {
        Move();
    }

    private void Move()
    {
        transform.position = Vector2.MoveTowards(transform.position, destination, currentSpeed * Time.deltaTime);
        if (transform.position == destination && path != null)
        {
            if (path.Count > 0)
            {
                destination = path.Pop();
            }
            else if (path.Count == 0)
            {
                GameManager.Instance.Lives--;
                Destroy(gameObject);
            }
        }
    }

    public void Slow(float reducePercentage, float delay)
    {
        StartCoroutine(SlowCoroutine(reducePercentage, delay));
    }
    
    private IEnumerator SlowCoroutine(float speedPercentage, float delay)
    {
        this.currentSpeed = this.speed * (speedPercentage / 100f);
        yield return new WaitForSeconds(delay);
        this.currentSpeed = speed;
    }

    private void SetPath(Stack<Vector2> newPath)
    {
        this.path = newPath;
        destination = path.Pop();
    }

    private bool IsDead()
    {
        return Health <= 0;
    }

    private void OnDestroy()
    {
        try
        {
            GameManager.Instance.ActiveEnemies--;
            GameManager.Instance.Coins += this.reward;
            Destroy(gameObject);
        }
        catch (Exception e)
        {
        }
    }
}
