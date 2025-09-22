using System;
using UnityEngine;

public class scr_statusItem : MonoBehaviour
{
    public Upgrade upgrade;

    private float valorAtual;

    public float valorInicial;
    public float valorMax;
    private float aumentoValor;

    public int qntdCompras;
    private int qntdComprasAtual = 0;

    private Preco precoAtual;

    public Preco precoInicial;
    public Preco precoFinal;
    private double custoInicial;
    private double custoFinal;
    private double custoFinalDivInicial;
    

    void Start()
    {
        // switch (upgrade)
        // {
        //     case Upgrade.Magnitude:
        //         Debug.Log("Magnitude");
        //         break;
        //     case Upgrade.Wavelength:
        //         Debug.Log("Wavelength");
        //         break;
        //     case Upgrade.Constellations:
        //         Debug.Log("Constellations");
        //         break;
        // }

        precoAtual = precoInicial;

        valorAtual = valorInicial;
        aumentoValor = (valorMax - valorInicial) / qntdCompras;

        if (upgrade == Upgrade.Constellations) { aumentoValor = 1f; }

        // 
        custoInicial = MoneyUtils.ToValor(precoInicial);
        custoFinal = MoneyUtils.ToValor(precoFinal);
        custoFinalDivInicial = custoFinal / custoInicial;
    }

    public Preco GetPrecoUpgrade()
    {
        return precoAtual;
    }

    public int GetValorInt()
    {
        return (int)valorAtual;
    }
    public bool IsMax()
    {
        // contelacoes nao tem limite de maximo
        if (upgrade == Upgrade.Constellations) { return false; }

        return qntdComprasAtual >= qntdCompras;
    }

    public float GetPorcentUpgrade()
    {
        return Math.Clamp(qntdComprasAtual / (float)qntdCompras, 0f, 1f);
    }

    public void MelhorarUpgrade()
    {
        // aumenta o quanto subiu o upgrade
        valorAtual += aumentoValor;
        if (valorAtual > valorMax && upgrade != Upgrade.Constellations)
        {
            valorAtual = valorMax;
        }
        // adiciona uma compra ao contador
        qntdComprasAtual = qntdComprasAtual + 1;
        // aumenta o preco
        Debug.Log("valorAtual " + valorAtual);
        AtualizarPrecoUpgrade();
    }

    private void AtualizarPrecoUpgrade()
    {
        // aumenta o preco
        Preco precoNovo = MoneyUtils.CalcularPrecoExp(
            custoInicial,
            custoFinalDivInicial,
            qntdCompras,
            Math.Min(qntdComprasAtual, qntdCompras - 1)
        );
        precoAtual = precoNovo;
        // Debug.Log("atualizar atual " + qntdComprasAtual + " max " + (qntdCompras - 1));
        // Debug.Log("preco v= " + precoNovo.valor + " p= " + precoNovo.prefixId);
    }


    public String GetDisplay()
    {
        // diferente form9atacao para cada tipo
        switch (upgrade)
        {
            case Upgrade.Magnitude:
                return GetDisplayMagnitude();
            case Upgrade.Wavelength:
                return GetDisplayWavelength();
            case Upgrade.Constellations:
                return GetDisplayConstellations();
        }
        return "";
    }

    private String GetDisplayMagnitude()
    {
        return valorAtual.ToString("F1");
    }

    private String GetDisplayWavelength()
    {
        return valorAtual.ToString("F0") + '%';
    }
    private String GetDisplayConstellations()
    {
        return ((int)valorAtual + 1).ToString();
    }
}
