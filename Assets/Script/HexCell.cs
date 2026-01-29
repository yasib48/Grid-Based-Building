
using UnityEngine;

public class HexCell : MonoBehaviour
{
    [Header("State")]
    public bool isActive = false; // is activ or not activ

    [Header("Visuals")]
    public SpriteRenderer spriteRenderer; // renderer for displaing sprite
    public Sprite activeSprite;           // sprite when cell is activ
    public Sprite inactiveSprite;         // sprite when cell is inactiv

    [Header("Colors")]
    public Color activeColor = Color.white;                       // color for activ state
    public Color inactiveColor = new Color(0.3f, 0.3f, 0.3f, 0.5f); // color for inactiv state

    [HideInInspector] public int gridX; // x position in grid sytem
    [HideInInspector] public int gridY; // y position in grid sytem

    void Start()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>(); // get renderer from curent object

        UpdateVisual(); // update visual at start
    }

    public void Setup(int x, int y, bool active = false)
    {
        gridX = x; // set x value
        gridY = y; // set y value
        isActive = active; // set activ state

        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>(); // get renderer if not exist

        UpdateVisual(); // refresh visual
    }

    public void SetActive(bool value)
    {
        if (isActive == value) return; // if same value do nothing

        isActive = value; // change activ state
        UpdateVisual();   // update sprite and color
    }

    public void Toggle()
    {
        SetActive(!isActive); // toggle activ and inactiv
    }

    void UpdateVisual()
    {
        if (spriteRenderer == null) return; // if no renderer then return

        if (isActive)
        {
            spriteRenderer.sprite = activeSprite; // show activ sprite
            spriteRenderer.color = activeColor;   // apply activ color
        }
        else
        {
            spriteRenderer.sprite = inactiveSprite; // show inactiv sprite
            spriteRenderer.color = inactiveColor;   // apply inactiv color
        }
    }
}
