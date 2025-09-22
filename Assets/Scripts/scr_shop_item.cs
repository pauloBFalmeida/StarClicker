using System;
using System.Collections.Generic;
using TMPro;
using TMPro.EditorUtilities;
using Unity.VisualScripting;
using UnityEngine;

public enum ShopItem
{
    Magnitude,
    Wavelength,
    Constellations,
    CollectedStars
}

[System.Serializable]
public struct ShopItemObjPair
{
    public ShopItem item;
    public TMP_Text textComponent;
}

public class scr_shop_item : MonoBehaviour
{
    public GameObject objEsq;
    public GameObject objMid;
    public GameObject objDir;
    public GameObject objText;

    public Vector2 offsetCompactaEsq;
    public Vector2 offsetCompactaMid;
    public Vector2 offsetCompactaDir;

    private TMP_Text textoTMP;
    private String textoInicial;
    private String textoExtraDinamico;

    public GameObject[] objParaEsconderCompacta;

    private Dictionary<GameObject, RectTransform> rectTransformDict = new();
    private Dictionary<GameObject, Vector3> posicaoExtendidaDict = new();
    private Dictionary<GameObject, Vector3> posicaoCompactaDict = new();

    private bool compacto = false;

    void Awake()
    {
        textoTMP = objText.GetComponent<TMP_Text>();
        textoInicial = textoTMP.text;

        foreach (GameObject obj in new GameObject[] { objEsq, objMid, objDir })
        {
            // ajusta a posicao dos objetos validos
            if (obj != null)
            {
                RectTransform rectTransform = obj.GetComponent<RectTransform>();
                rectTransformDict[obj] = rectTransform;
                // posicoes iniciais
                posicaoExtendidaDict[obj] = rectTransform.anchoredPosition;
                posicaoCompactaDict[obj] = rectTransform.anchoredPosition;
            }
        }
        AjustarPosicaoCompacto();
    }

    private void AjustarPosicaoCompacto()
    {
        if (objEsq != null)
        {
            Vector2 posEsq = rectTransformDict[objEsq].anchoredPosition;
            posEsq += offsetCompactaEsq;
            posicaoCompactaDict[objEsq] = posEsq;
        }
        if (objMid != null)
        {
            Vector2 posMid = rectTransformDict[objEsq].anchoredPosition;
            posMid.x += rectTransformDict[objEsq].sizeDelta.x;
            posMid += offsetCompactaMid;
            posicaoCompactaDict[objMid] = posMid;
        }
        if (objDir != null)
        {
            Vector2 posDir = rectTransformDict[objDir].anchoredPosition;
            posDir.x += rectTransformDict[objDir].sizeDelta.x;
            posDir += offsetCompactaDir;
            posicaoCompactaDict[objDir] = posDir;
        }
    }

    public void AjustarTexto(String textoExtra)
    {
        textoExtraDinamico = textoExtra;
        UpdateTexto();
    }

    public void AjustarCompacto(bool _compacto)
    {
        compacto = _compacto;
        // ajusta a posicao dos objetos que mudam de posicao entre modo compacto e extendido
        foreach (GameObject obj in new GameObject[] { objEsq, objMid, objDir })
        {
            if (obj != null)
            {
                rectTransformDict[obj].anchoredPosition = compacto ? posicaoCompactaDict[obj] : posicaoExtendidaDict[obj];
            }
        }
        // esconde os objetos que devem ser escondidos entre modo compacto e extendido
        foreach (GameObject obj in objParaEsconderCompacta)
        {
            obj.SetActive(!compacto);
        }
        UpdateTexto();
    }

    private void UpdateTexto()
    {
        textoTMP.text = compacto ? textoExtraDinamico : textoInicial + textoExtraDinamico;
    }

}
