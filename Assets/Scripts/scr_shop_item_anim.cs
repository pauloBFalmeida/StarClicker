using System;
using UnityEngine;
using UnityEngine.UI;


public class scr_shop_item_anim : MonoBehaviour
{
    private scr_statusItem statusItem;
    // Barra de progresso frente
    public GameObject barraProg;
    RectTransform barraProgRT;

    // TODO mudar as sprites
    public Sprite[] sprites;
    public Image spriteRendererEsq;
    public Image spriteRendererDir;

    // TODO Lerp animation para a barra de prog
    private float barraPorcentPrev;
    private float barraPorcentNext;
    private float barraWidthStart;

    void Start()
    {
        statusItem = gameObject.GetComponent<scr_statusItem>();

        barraProgRT = barraProg.GetComponent<RectTransform>();
        barraWidthStart = barraProgRT.rect.width;

    }

    void Update()
    {
        float width = Mathf.Lerp(0f, barraWidthStart, statusItem.GetPorcentUpgrade());
        barraProgRT.sizeDelta = new Vector2(width, barraProgRT.sizeDelta.y);
    }

}
