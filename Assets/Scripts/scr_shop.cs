using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

[System.Serializable]
public struct UpgradeObjPair
{
    public scr_shop.Upgrade upgrade;
    public GameObject obj;
}


[System.Serializable]
public struct UpgradeIntPair
{
    public scr_shop.Upgrade upgrade;
    public int valor;
}

[System.Serializable]
public struct ValorPrefixo
{
    public int valor;
    public int prefixId;
    public ValorPrefixo(int _valor, int _prefixId)
    {
        valor = _valor;
        prefixId = _prefixId;
    }
}

public class scr_shop : MonoBehaviour
{
    public enum Upgrade
    {
        Magnitude,
        Wavelenght,
        Constellations
    };

    public int[] dinheiros = new int[11];
    public char[] prefixos = { ' ', 'k', 'M', 'G', 'T', 'P', 'E', 'Z', 'Y', 'R', 'Q' };

    public List<UpgradeObjPair> botoesUpgradesList;
    public List<UpgradeIntPair> precoInicialUpgradesList;
    public List<UpgradeObjPair> textosStatusList;

    private Dictionary<Upgrade, Button> botoesUpgrades = new Dictionary<Upgrade, Button>();
    private Dictionary<Upgrade, TMP_Text> textoTMPUpgrades = new Dictionary<Upgrade, TMP_Text>();
    private Dictionary<Upgrade, ValorPrefixo> precoAtualUpgrades = new Dictionary<Upgrade, ValorPrefixo>();
    private Dictionary<Upgrade, TMP_Text> textoTMPStatus = new Dictionary<Upgrade, TMP_Text>();
    private Dictionary<Upgrade, String> textoInicialStatus = new Dictionary<Upgrade, String>();
    private String textoInicialUpgrades = "+\n";
    public TMP_Text textoTMPDinheiro;
    private String textoInicialDinheiro;


    private scr_gameManager gamerManager;

    void Start()
    {
        gamerManager = gameObject.GetComponent<scr_gameManager>();

        foreach (UpgradeIntPair par in precoInicialUpgradesList)
        {
            precoAtualUpgrades.Add(par.upgrade, new ValorPrefixo(par.valor, 0));
        }
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
        foreach (UpgradeObjPair par in textosStatusList)
        {
            TMP_Text textoTMP = par.obj.GetComponent<TMP_Text>();
            textoTMPStatus.Add(par.upgrade, textoTMP);
            textoInicialStatus.Add(par.upgrade, textoTMP.text);
        }
        textoInicialDinheiro = textoTMPDinheiro.text;

        UpdateUIShop();
    }

    void Update()
    {

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
            ValorPrefixo preco = precoAtualUpgrades[upgrade];
            
            textoTMP.text = textoInicial + preco.valor + prefixos[preco.prefixId];
            // se pode comprar, se nao esta no maximo ja
            bool podeComprar = true;
            switch (upgrade) {
                case Upgrade.Magnitude:
                    podeComprar = !gamerManager.IsMaxMagnitude();
                    break;
                case Upgrade.Wavelenght:
                    podeComprar = !gamerManager.IsMaxWavelenght();
                    break;
                case Upgrade.Constellations:
                    podeComprar = !gamerManager.IsMaxConstellations();
                    break;
            };
            // se nao tem dinheiro suficente, bloqueia o botao
            podeComprar = podeComprar && DinheiroSuficientePreco(preco);
            botoesUpgrades[upgrade].interactable = podeComprar;
            // ----- update texto dos status atuais
            // pega o texto e coloca o inical
            textoTMP = textoTMPStatus[upgrade];
            textoInicial = textoInicialStatus[upgrade];
            textoTMP.text = textoInicial;
            // add o valor
            switch (upgrade)
            {
                case Upgrade.Magnitude:
                    textoTMP.text += gamerManager.GetMagnitude();
                    break;
                case Upgrade.Wavelenght:
                    textoTMP.text += gamerManager.GetWavelenght() * 10;
                    textoTMP.text += '%';
                    break;
                case Upgrade.Constellations:
                    textoTMP.text += gamerManager.GetConstellations();
                    break;
            };
        }
        // ----- update texto do dinheiro atual
        for (int i = dinheiros.Length - 1; i >= 0; i--)
        {
            if (dinheiros[i] > 0 || i == 0)
            {
                textoTMPDinheiro.text = textoInicialDinheiro + dinheiros[i] + ' ' + prefixos[i];
                break;
            }
        }
    }

    private bool DinheiroSuficientePreco(ValorPrefixo preco)
    {
        if (dinheiros[preco.prefixId] >= preco.valor)
        {
            return true;
        }
        else
        {
            // ve se tem dinheiro acima
            for (int i = preco.prefixId + 1; i < dinheiros.Length; i++)
            {
                if (dinheiros[i] > 0)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private void RemDinheiro(int valor, int prefix) {
        if (dinheiros[prefix] >= valor)
        {
            dinheiros[prefix] -= valor;
        }
        else
        {
            // percorre a cima para ver se tem valores
            for (int i = prefix + 1; i < dinheiros.Length; i++)
            {
                // tem valor
                if (dinheiros[i] > 0)
                {
                    // diminui 1 e add 999 no de baixo
                    // TODO arrumar isso para 999 para todos os outros em baixo
                    dinheiros[i] -= 1;
                    AddDinheiro(999, i - 1, false);
                }
            }
        }

        UpdateUIShop();
    }

    public void AddDinheiro(int valor, int prefix, bool atualizarUI = true)
    {
        if (prefix > dinheiros.Length) { return; }
        // add dinheiro
        dinheiros[prefix] += valor;
        // se passar do valor, add nos de cima
        if (dinheiros[prefix] > 999)
        {
            AddDinheiro(1, prefix + 1, false);
        }

        if (atualizarUI) { UpdateUIShop(); }
    }

    private ValorPrefixo CalcularPrecoNovo(ValorPrefixo precoAtual)
    {
        // proximo preco
        ValorPrefixo precoNovo = new ValorPrefixo(precoAtual.valor, precoAtual.prefixId);
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

    public void ClickComprarUpgradeMagnitude()
    {
        Upgrade upgrade = Upgrade.Magnitude;
        // pega o preco
        ValorPrefixo precoAtual = precoAtualUpgrades[upgrade];
        RemDinheiro(precoAtual.valor, precoAtual.prefixId);
        // melhora o upgrade
        gamerManager.MelhorarMagnitude();
        // atualiza o preco
        precoAtualUpgrades[upgrade] = CalcularPrecoNovo(precoAtual);

        // atualiza a UI
        UpdateUIShop();
    }

    public void ClickComprarUpgradeWavelenght()
    {
        Upgrade upgrade = Upgrade.Wavelenght;
        // pega o preco
        ValorPrefixo precoAtual = precoAtualUpgrades[upgrade];
        RemDinheiro(precoAtual.valor, precoAtual.prefixId);
        // melhora o upgrade
        gamerManager.MelhorarWavelenght();
        // atualiza o preco
        precoAtualUpgrades[upgrade] = CalcularPrecoNovo(precoAtual);

        // atualiza a UI
        UpdateUIShop();
    }
    public void ClickComprarUpgradeContellations()
    {
        Upgrade upgrade = Upgrade.Constellations;
        // pega o preco
        ValorPrefixo precoAtual = precoAtualUpgrades[upgrade];
        RemDinheiro(precoAtual.valor, precoAtual.prefixId);
        // melhora o upgrade
        gamerManager.MelhorarConstellations();
        // atualiza o preco
        precoAtualUpgrades[upgrade] = CalcularPrecoNovo(precoAtual);

        // atualiza a UI
        UpdateUIShop();
    }
}
