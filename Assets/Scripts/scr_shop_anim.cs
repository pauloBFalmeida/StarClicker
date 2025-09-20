using System;
using UnityEngine;
using UnityEngine.UIElements;


public class scr_shop_anim : MonoBehaviour
{
    private Vector2 showPos;
    public float hidePosY;

    private RectTransform rectTransform;
    private Vector2 targetPos;
    public float speed = 10;

    public scr_shop_item[] shopItens;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        showPos = new Vector2(rectTransform.anchoredPosition.x, rectTransform.anchoredPosition.y);
        CompactarMenu();
    }

    void Update()
    {
        // Smoothly interpolate from current to target
        rectTransform.anchoredPosition = Vector2.Lerp(
            rectTransform.anchoredPosition,
            targetPos,
            Time.deltaTime * speed
        );
    }
    
    public void ExtenderMenu()
    {
        targetPos = showPos;
        AjustarMenuCompacto(false);
    }

    // Esconde o menu
    public void CompactarMenu()
    {
        targetPos = showPos;
        targetPos.y = hidePosY;
        AjustarMenuCompacto(true);
    }

    private void AjustarMenuCompacto(bool compacto)
    {
        // ajusta os itens do shop para o modo compacto ou extendido
        foreach (scr_shop_item item in shopItens)
        {
            item.AjustarCompacto(compacto);
        }
    }
}
