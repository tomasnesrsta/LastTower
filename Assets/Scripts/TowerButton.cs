using UnityEngine;

public class TowerButton : MonoBehaviour
{
    [SerializeField]
    private GameObject towerPrefab;

    [SerializeField]
    private Sprite sprite;

    [SerializeField]
    private int price;

    public int Price
    {
        get { return this.price; }
    }

    public Sprite Sprite
    { 
        get { return sprite; }
    }
    
    public GameObject TowerPrefab
    {
        get { return towerPrefab; }
    }
}
