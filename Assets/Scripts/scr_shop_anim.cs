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

    private Vector2 touchPosComeco = new();
    private bool manterMenu = false;

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
        // toque no dispositivo mobile
        if (Input.touchCount > 0)
        {
            LidarTouch();
        }
    }

    private void LidarTouch()
    {
        // toque inicial, salva a posicao
        if (Input.GetTouch(0).phase == TouchPhase.Began)
        {
            touchPosComeco = Input.GetTouch(0).position;
        }
        else if (Input.GetTouch(0).phase == TouchPhase.Ended)
        {
            Vector2 touchPosFim = Input.GetTouch(0).position;
            // swipe de uma distancia minima de 10 para evitar miss clicks
            if ((touchPosFim - touchPosComeco).SqrMagnitude() > 100)
            {
                // swipe pra baixo
                if (touchPosFim.y < touchPosComeco.y)
                {
                    ManterCompactarMenu();
                }
                // swipe pra cima
                else
                {
                    ManterExtenderMenu();
                }
            }
        }
    }

    private void ManterExtenderMenu()
    {
        manterMenu = true;
        ExtenderMenu();
    }
    private void ManterCompactarMenu()
    {
        manterMenu = false;
        CompactarMenu();
    }

    public void ExtenderMenu()
    {
        targetPos = showPos;
        AjustarMenuCompacto(false);
    }

    // Esconde o menu
    public void CompactarMenu()
    {
        if (manterMenu) { return; }
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
