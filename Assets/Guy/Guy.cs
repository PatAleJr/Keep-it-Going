using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guy : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer Head = null;
    [SerializeField]
    private SpriteRenderer[] Hands = { null, null};
    [SerializeField]
    private SpriteRenderer Body = null;
    [SerializeField]
    private SpriteRenderer[] Feet = { null, null};

    public Color[] clothesColors;
    public Color[] skinColors;

    private Color clothesColor;
    private Color skinColor;

    public int baseOrder = 3000;

    public float maxXoffset;

    private void Start()
    {
        //Adds random X
        float xOffset = Random.Range(-maxXoffset, maxXoffset);
        Vector3 offset = new Vector2(xOffset, 0);
        transform.position += offset;

        //Sets random skin and clothes color
        int clothesColorIndex = Random.Range(0, clothesColors.Length);
        int skinColorIndex = Random.Range(0, skinColors.Length);

        clothesColor = clothesColors[clothesColorIndex];
        skinColor = skinColors[skinColorIndex];

        Head.color = skinColor;
        Hands[0].color = skinColor;
        Hands[1].color = skinColor;
        Body.color = clothesColor;
        Feet[0].color = clothesColor;
        Feet[1].color = clothesColor;


        //Set depth based on Y position
        int order = (int)(baseOrder - transform.position.y * 4);    //*4 to be more precise
        Head.sortingOrder = order;
        Body.sortingOrder = order;
        Hands[0].sortingOrder = order+1;    //Hands must be seen above body
        Hands[1].sortingOrder = order+1;
        Feet[0].sortingOrder = order;
        Feet[1].sortingOrder = order;
    }
}
