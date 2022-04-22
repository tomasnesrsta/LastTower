using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class TileScript : MonoBehaviour
{
    public Point GridPosition { get; private set; }

    private SpriteRenderer spriteRenderer;

    public bool IsEmpty { get; set; }
    
    private readonly Color32 occupiedTileColor = new Color32(255, 118, 188, 255);
    private readonly Color32 emptyTileColor = new Color32(96, 255, 90, 255);

    private Tower tileTower;

    public void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    public Vector2 WorldPosition
    {
        get
        {
            return new Vector2(transform.position.x + (GetComponent<SpriteRenderer>().bounds.size.x / 2), transform.position.y - (GetComponent<SpriteRenderer>().bounds.size.y / 2));
        }
    }
    
    public void Setup(Point gridPos, Vector3 worldPos, Transform parent)
    {
        IsEmpty = true;
        this.GridPosition = gridPos;
        transform.position = worldPos;
        transform.SetParent(parent);
        LevelManager.Instance.Tiles.Add(gridPos, this);
    }
    
    private void OnMouseOver()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            if (GameManager.Instance.ClickedButton != null)
            {
                if (IsEmpty)
                {
                    ChangeTileColor(emptyTileColor);
                }
                if (!IsEmpty)
                {
                    ChangeTileColor(occupiedTileColor);
                }
                else if (Input.GetMouseButtonDown(0))
                {
                    PlaceTower();
                }
            }
            else if (GameManager.Instance.ClickedButton == null && Input.GetMouseButtonDown(0))
            {
                if (tileTower != null)
                    GameManager.Instance.SelectTower(tileTower);
                else
                    GameManager.Instance.DeselectTower();
            }
            
        }
        
    }

    private void OnMouseExit()
    {
        ChangeTileColor(Color.white);
    }

    private void PlaceTower()
    {
        GameObject tower = Instantiate(GameManager.Instance.ClickedButton.TowerPrefab, transform.position, Quaternion.identity);
        tower.transform.SetParent(transform);

        this.tileTower = tower.transform.GetChild(0).GetComponent<Tower>();
        
        IsEmpty = false;
        ChangeTileColor(Color.white);
        GameManager.Instance.BuyTower();
    }

    private void ChangeTileColor(Color color)
    {
        spriteRenderer.color = color;
    }
}
