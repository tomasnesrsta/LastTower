using UnityEngine;

public class Bar : MonoBehaviour
{
    private float amount;
    
    public float Amount
    {
        get { return amount; }
        set
        {
            amount = value;
            DisplayValue(value);
        }
    }
    
    private void Start()
    {
        Amount = amount;
    }

    private void DisplayValue(float value)
    {
        transform.localScale = new Vector3(value * 1.81f, transform.localScale.y, 0);
    }
    
}
