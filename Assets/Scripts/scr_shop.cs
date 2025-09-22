using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;


public enum Upgrade
{
    Magnitude,
    Wavelength,
    Constellations
};

[System.Serializable]
public struct UpgradeObjPair
{
    public Upgrade upgrade;
    public GameObject obj;
}

public class scr_shop : MonoBehaviour
{
    public List<UpgradeObjPair> botoesUpgradesList;
    public List<UpgradeObjPair> shopItensList;

    private Dictionary<Upgrade, Button> botoesUpgrades = new();
    private Dictionary<Upgrade, TMP_Text> textoTMPUpgrades = new();
    private Dictionary<Upgrade, scr_shop_item> shopItensUpgrades = new();
    private Dictionary<Upgrade, scr_statusItem> statusItemUpgrades = new();
    public scr_shop_item shopItemColStars;
    private String textoInicialUpgrades = "+\n";

    private scr_moneyManager moneyManager;
    private scr_gameManager gameManager;


    void Start()
    {
        moneyManager = gameObject.GetComponent<scr_moneyManager>();
        gameManager = gameObject.GetComponent<scr_gameManager>();

        // pega as informacoes dos botoes
        foreach (UpgradeObjPair par in botoesUpgradesList)
        {
            GameObject botao = par.obj;
            // coloca o componente botao no dict
            Button button = botao.GetComponent<Button>();
            botoesUpgrades.Add(par.upgrade, button);
            // coloca o componente text tmp no dict
            TMP_Text textoTMP = botao.GetComponentInChildren<TMP_Text>();
            textoTMPUpgrades.Add(par.upgrade, textoTMP);
        }
        // ajusta os itens do shop e os upgrades itens
        foreach (UpgradeObjPair par in shopItensList)
        {
            // shop item
            scr_shop_item shopItem = par.obj.GetComponent<scr_shop_item>();
            shopItensUpgrades.Add(par.upgrade, shopItem);
            // upgrade item
            scr_statusItem statusItem = par.obj.GetComponent<scr_statusItem>();
            statusItemUpgrades.Add(par.upgrade, statusItem);
        }

        UpdateUIShop();
    }

    private void UpdateUIShop()
    {
        TMP_Text textoTMP;
        String textoInicial;
        foreach (Upgrade upgrade in new Upgrade[] { Upgrade.Magnitude, Upgrade.Wavelength, Upgrade.Constellations })
        {
            scr_statusItem statusItem = statusItemUpgrades[upgrade];
            // ----- update botoes de comprar upgrades
            textoTMP = textoTMPUpgrades[upgrade];
            textoInicial = textoInicialUpgrades;
            Preco preco = statusItem.GetPrecoUpgrade();
            // atualiza o texto do botao
            if (statusItem.IsMax())
            {
                textoTMP.text = "Max";
            }
            else
            {
                textoTMP.text = textoInicial + preco.valor + moneyManager.prefixos[preco.prefixId];
            }
            // se pode comprar, se nao esta no maximo ja e tem dinheiro suficente
            bool podeComprar = !statusItem.IsMax() && moneyManager.DinheiroSuficientePreco(preco);
            // se nao pode comprar, bloqueia o botao
            botoesUpgrades[upgrade].interactable = podeComprar;

            // ----- update texto dos status atuais
            // pega o shop item relacionado a esse upgrade
            scr_shop_item shopItem = shopItensUpgrades[upgrade];
            // ajusta o texto, para o valor do upgrade item
            shopItem.AjustarTexto(statusItem.GetDisplay());
        }
        // ----- update texto do dinheiro atual
        shopItemColStars.AjustarTexto(moneyManager.GetDisplayDinheiro());
    }

    public void AddDinheiro(int valor, int prefix)
    {
        moneyManager.AddDinheiro(valor, prefix);
        UpdateUIShop();
    }

    // ------------------------ Comprar upgrade
    public void ClickComprarUpgradeMagnitude()
    {
        ComprarUpgradeBasico(Upgrade.Magnitude);
    }

    public void ClickComprarUpgradeWavelength()
    {
        ComprarUpgradeBasico(Upgrade.Wavelength);
    }
    public void ClickComprarUpgradeContellations()
    {
        ComprarUpgradeBasico(Upgrade.Constellations);
    }
    private void ComprarUpgradeBasico(Upgrade upgrade)
    {
        scr_statusItem statusItem = statusItemUpgrades[upgrade];
        // pagar o preco
        Preco precoAtual = statusItem.GetPrecoUpgrade();
        moneyManager.RemDinheiro(precoAtual.valor, precoAtual.prefixId);
        // atualiza os valores e preco do upgrade
        statusItem.MelhorarUpgrade();
        // atualiza a UI
        UpdateUIShop();
        // melhora acao feita pelo do upgrade
        gameManager.MelhorarAcaoUpgrade(upgrade, statusItem);
    }
}
