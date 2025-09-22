using UnityEngine;
using System.Collections.Generic;
using System;

public class scr_upgradeManager : MonoBehaviour
{
    // public int tamConstelacao;
    // private float magnitude;
    // private float wavelength;

    
    // public float maxMagnitude { get; private set; } = 31.5f;
    // public float maxWavelength { get; private set; } = 100f;
    // public int maxConstellations { get; private set; } = 9999;
    // public float minMagnitude { get; private set; } = 6.5f;
    // public float minWavelength { get; private set; } = 0f;
    // public int minConstellations { get; private set; } = 0;

    // public UpgradePrecoPair[] precoInicialUpgradesList;
    // private Dictionary<Upgrade, Preco> precoAtualUpgrades = new();

    // private scr_shop shop;
    // private scr_gameManager gameManager;


    // void Start()
    // {
    //     shop = gameObject.GetComponent<scr_shop>();
    //     gameManager = gameObject.GetComponent<scr_gameManager>();

    //     // ajusta os max
    //     // maxWavelength = .prefixos.Length - 1;

    //     // ajusta o preco inicial dos upgrades
    //     foreach (UpgradePrecoPair par in precoInicialUpgradesList)
    //     {
    //         precoAtualUpgrades.Add(par.upgrade, par.preco);
    //     }
    // }

    // public Preco GetPrecoUpgrade(Upgrade upgrade)
    // {
    //     return precoAtualUpgrades[upgrade];
    // }

    // public void AtualizarPrecoUpgrade(Upgrade upgrade)
    // {
    //     switch (upgrade)
    //     {
    //         case Upgrade.Magnitude:
    //             break;
    //         case Upgrade.Wavelength:
    //             break;
    //         case Upgrade.Constellations:
    //             break;
    //     }
    // }

    // private Preco CalcularPrecoNovo(Preco precoAtual)
    // {
    //     // proximo preco
    //     Preco precoNovo = new Preco(precoAtual.valor, precoAtual.prefixId);
    //     // aumenta em 100
    //     precoNovo.valor = precoAtual.valor * 50;
    //     // se for acima de mil, aumenta o prefixId
    //     if (precoNovo.valor >= 1000)
    //     {
    //         precoNovo.valor = precoNovo.valor / 1000;
    //         precoNovo.prefixId = precoAtual.prefixId + 1;
    //     }
    //     return precoNovo;
    // }

    // public void MelhorarConstellations()
    // {
    //     tamConstelacao += 1;
    // }

    // public void MelhorarMagnitude()
    // {
    //     float aumento = 0.5f;
    //     magnitude += aumento;
    //     // calcula quantos aumentos terao no jogo
    //     float qntdAumentos = (maxMagnitude - 6.0f) / aumento;
    //     // diminui o tempo atual
    //     // spawnEstrelaDelay -= (spawnEstrelaDelayInicial - 0.1f) / qntdAumentos;
    //     float novoTempo = (gameManager.spawnEstrelaDelayInicial - 0.1f) / qntdAumentos;
    //     gameManager.AjustarSpawnEstrelaDelay(novoTempo);
    // }
    // public void MelhorarWavelength()
    // {
    //     wavelength += 1;
    //     // estrelaCurrPrefix = wavelength;
    // }


    // public String GetDisplayMagnitude()
    // {
    //     return magnitude.ToString("F1");
    // }

    // public String GetDisplayWavelength()
    // {
    //     return wavelength.ToString() + '%';
    // }
    // public String GetDisplayConstellations()
    // {
    //     return (tamConstelacao + 1).ToString();
    // }

    // public bool IsMaxMagnitude()
    // {
    //     return magnitude >= maxMagnitude;
    // }
    // public bool IsMaxWavelength()
    // {
    //     return wavelength >= maxWavelength;
    // }

    // public bool IsMaxConstellations()
    // {
    //     return tamConstelacao >= maxConstellations;
    // }
}