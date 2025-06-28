using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class BuildSystem : MonoBehaviour
{
    public GridBuilder builder;
    public GameObject building, building2, building3;
    public GameObject hologramObj;
    public float alpha = 0.4f;

    int sizeX = 2;
    int sizeY = 2;
    public a a;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            switch (a)
            {
                case a.uc:
                    a = a.ilk;
                    hologram(building, 1, 1);

                    break;
                case a.iki:

                    a = a.uc;
                    hologram(building3, 2, 1);

                    break;
                case a.ilk:
                    a = a.iki;
                    hologram(building2, 2, 2);

                    break;
            }
            Debug.Log("Þu anki durum: " + a);
        }
        if (a == a.ilk)
        {
            Build(building, 1, 1);
            hologram(building, 1, 1);
        }
        else if (a == a.iki)
        {
            Build(building2, 2, 2);
            hologram(building2, 2, 2);
        }
        else if (a == a.uc)
        {
            Build(building3, 2, 1);
            hologram(building3, 2, 1);
        }

        if (hologramObj != null)
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPos.z = 0;
            builder.GetXY(mouseWorldPos, out int x, out int y);
            Vector3 snappedPos = builder.GetWorldPos(x, y);
            hologramObj.transform.position = snappedPos;
        }


    }
    void hologram(GameObject buildObj,int Sx, int Sy)
    {
        
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0;  // 2D için z sýfýr olmalý
        builder.GetXY(mouseWorldPos, out int x, out int y);
        if (CanPlace(x, y, Sx, Sy)) 
        {
             if(hologramObj == buildObj)
            {
                Destroy(hologramObj);
                Debug.Log("ah");
            }



            if (hologramObj == null )
             {
                Debug.Log("bab");

                hologramObj = Instantiate(buildObj);
                foreach (var comp in hologramObj.GetComponents<Behaviour>())
                {
                    comp.enabled = false;
                }
                hologramObj.GetComponent<SpriteRenderer>().enabled = true;
                SpriteRenderer sr = hologramObj.GetComponent<SpriteRenderer>();
                Color c = sr.color;
                c.a = alpha;
                sr.color = c;
                Debug.Log("gfe");

             }
        }
        else
        {
            Destroy(hologramObj );
        }
    }
    void Build(GameObject buildObj,int Sx,int Sy)
    {

        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPos.z = 0;  // 2D için z sýfýr olmalý
            builder.GetXY(mouseWorldPos, out int x, out int y);
            if (builder.GetGrid(x, y) == null || !builder.GetGrid(x, y).isEmpty)
                return;

            if (CanPlace(x, y, Sx, Sy))
            {
                SetOccupied(x, y, Sx, Sy, true);
                Vector3 cornerPos = new Vector3(
                x + builder.startOffset.x,
                y + builder.startOffset.y,
                0) * builder.cellSize;

                Instantiate(buildObj, cornerPos, Quaternion.identity);
            }
        }
    }
    bool CanPlace(int startX, int startY, int width, int height)
    {
        if (startX + width > builder.Width || startY + height > builder.Height)
            return false;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var gridCell = builder.GetGrid(startX + x, startY + y);
                if (gridCell == null) return false;   // Burada null ise hemen false
                if (gridCell.gameObject == null) return false;  // Ek güvenlik
                if (!gridCell.isEmpty) return false;
            }
        }
        return true;
    }


    void SetOccupied(int startX, int startY, int width, int height, bool occupied)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                builder.GetGrid(startX + x, startY + y).isEmpty = !occupied;
            }
        }
    }
}
public enum a
{
    ilk,
    iki,
    uc
}