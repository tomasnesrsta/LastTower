using UnityEngine;

public class Hover : Singleton<Hover>
{
    private SpriteRenderer spriteRenderer;
    private SpriteRenderer rangeSpriteRenderer;

    void Start()
    {
        this.spriteRenderer = GetComponent<SpriteRenderer>();
        this.rangeSpriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
    }
    void Update()
    {
        FollowMouse();
    }
    
    private void FollowMouse()
    {
        if (spriteRenderer.sprite != null)
        {
            transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = new Vector2(transform.position.x, transform.position.y);
        }
    }

    public void Enable(Sprite sprite)
    {
        spriteRenderer.enabled = true;
        rangeSpriteRenderer.enabled = true;
        spriteRenderer.sprite = sprite;
    }

    public void Disable()
    {
        spriteRenderer.enabled = false;
        rangeSpriteRenderer.enabled = false;
        GameManager.Instance.ClickedButton = null;
    }
}
