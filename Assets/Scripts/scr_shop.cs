using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;


public enum Upgrade
{
    Magnitude,
    Wavelenght,
    Constellations
};

[System.Serializable]
public struct UpgradeObjPair
{
    public Upgrade upgrade;
    public GameObject obj;
}


[System.Serializable]
public struct UpgradePrecoPair
{
    public Upgrade upgrade;
    public Preco preco;
}


public class scr_shop : MonoBehaviour
{
    public List<UpgradeObjPair> botoesUpgradesList;
    public List<UpgradePrecoPair> precoInicialUpgradesList;
    public List<UpgradeObjPair> shopItensList;

    private Dictionary<Upgrade, Button> botoesUpgrades = new();
    private Dictionary<Upgrade, TMP_Text> textoTMPUpgrades = new();
    private Dictionary<Upgrade, Preco> precoAtualUpgrades = new();
    private Dictionary<Upgrade, scr_shop_item> shopItensUpgrades = new();
    public scr_shop_item shopItemColStars;
    private String textoInicialUpgrades = "+\n";

    private scr_moneyManager moneyManager;
    private scr_upgradeManager upgradeManager;
    

    void Start()
    {
        moneyManager = gameObject.GetComponent<scr_moneyManager>();
        upgradeManager = gameObject.GetComponent<scr_upgradeManager>();

        // ajusta o preco inicial dos upgrades
        foreach (UpgradePrecoPair par in precoInicialUpgradesList)
        {
            precoAtualUpgrades.Add(par.upgrade, par.preco);
        }
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
        // ajusta os itens do shop
        foreach (UpgradeObjPair par in shopItensList)
        {
            scr_shop_item shopItem = par.obj.GetComponent<scr_shop_item>();
            shopItensUpgrades.Add(par.upgrade, shopItem);
        }

        UpdateUIShop();
    }

    private void UpdateUIShop()
    {
        TMP_Text textoTMP;
        String textoInicial;
        foreach (Upgrade upgrade in new Upgrade[] { Upgrade.Magnitude, Upgrade.Wavelenght, Upgrade.Constellations })
        {
            // ----- update texto dos botoes de comprar upgrades
            textoTMP = textoTMPUpgrades[upgrade];
            textoInicial = textoInicialUpgrades;
            Preco preco = precoAtualUpgrades[upgrade];

            textoTMP.text = textoInicial + preco.valor + moneyManager.prefixos[preco.prefixId];
            // se pode comprar, se nao esta no maximo ja
            bool podeComprar = true;
            switch (upgrade)
            {
                case Upgrade.Magnitude:
                    podeComprar = !upgradeManager.IsMaxMagnitude();
                    break;
                case Upgrade.Wavelenght:
                    podeComprar = !upgradeManager.IsMaxWavelenght();
                    break;
                case Upgrade.Constellations:
                    podeComprar = !upgradeManager.IsMaxConstellations();
                    break;
            }
            ;
            // se nao tem dinheiro suficente, bloqueia o botao
            podeComprar = podeComprar && moneyManager.DinheiroSuficientePreco(preco);
            botoesUpgrades[upgrade].interactable = podeComprar;
            
            // ----- update texto dos status atuais
            // pega o shop item relacionado a esse upgrade
            scr_shop_item shopItem = shopItensUpgrades[upgrade];
            // ajusta o texto
            String texto = "";
            switch (upgrade)
            {
                case Upgrade.Magnitude:
                    texto += upgradeManager.GetDisplayMagnitude();
                    break;
                case Upgrade.Wavelenght:
                    texto += upgradeManager.GetDisplayWavelenght();
                    texto += '%';
                    break;
                case Upgrade.Constellations:
                    texto += upgradeManager.GetDisplayConstellations();
                    break;
            };
            shopItem.AjustarTexto(texto);
        }
        // ----- update texto do dinheiro atual
        shopItemColStars.AjustarTexto(moneyManager.GetDisplayDinheiro());
    }

    public void AddDinheiro(int valor, int prefix)
    {
        moneyManager.AddDinheiro(valor, prefix);
        UpdateUIShop();
    }

    private Preco CalcularPrecoNovo(Preco precoAtual)
    {
        // proximo preco
        Preco precoNovo = new Preco(precoAtual.valor, precoAtual.prefixId);
        // aumenta em 100
        precoNovo.valor = precoAtual.valor * 50;
        // se for acima de mil, aumenta o prefixId
        if (precoNovo.valor >= 1000)
        {
            precoNovo.valor = precoNovo.valor / 1000;
            precoNovo.prefixId = precoAtual.prefixId + 1;
        }
        return precoNovo;
    }

    private void ComprarUpgradeBasico(Upgrade upgrade)
    {
        // pega o preco
        Preco precoAtual = precoAtualUpgrades[upgrade];
        moneyManager.RemDinheiro(precoAtual.valor, precoAtual.prefixId);
        // atualiza o preco
        precoAtualUpgrades[upgrade] = CalcularPrecoNovo(precoAtual);
        // atualiza a UI
        UpdateUIShop();
    }

    public void ClickComprarUpgradeMagnitude()
    {
        // melhora o upgrade
        upgradeManager.MelhorarMagnitude();
        //
        ComprarUpgradeBasico(Upgrade.Magnitude);
    }

    public void ClickComprarUpgradeWavelenght()
    {
        // melhora o upgrade
        upgradeManager.MelhorarWavelenght();
        //
        ComprarUpgradeBasico(Upgrade.Wavelenght);
    }
    public void ClickComprarUpgradeContellations()
    {
        // melhora o upgrade
        upgradeManager.MelhorarConstellations();
        //
        ComprarUpgradeBasico(Upgrade.Constellations);
    }
}
