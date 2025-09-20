using System;
using UnityEngine;

[System.Serializable]
public struct Preco
{
    public int valor;
    public int prefixId;
    public Preco(int _valor, int _prefixId)
    {
        valor = _valor;
        prefixId = _prefixId;
    }
}

public class scr_moneyManager : MonoBehaviour
{
    public int[] valores = new int[12];
    public char[] prefixos { get; private set; } = {
    //   0    1    2    3    4    5    6    7    8    9    10   11 (inf)
        ' ', 'k', 'M', 'G', 'T', 'P', 'E', 'Z', 'Y', 'R', 'Q', '\u221E'
    };


    public bool DinheiroSuficientePreco(Preco preco)
    {
        if (valores[preco.prefixId] >= preco.valor)
        {
            return true;
        }
        else
        {
            // ve se tem dinheiro acima
            for (int i = preco.prefixId + 1; i < valores.Length; i++)
            {
                if (valores[i] > 0)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public String GetDisplayDinheiro()
    {
        for (int i = valores.Length - 1; i >= 0; i--)
        {
            if (valores[i] > 0 || i == 0)
            {
                return "" + valores[i] + ' ' + prefixos[i];
            }
        }
        return "???";
    }

    public void RemDinheiro(int valor, int prefix) {
        // se for acima da lista, pare a funcao
        if (prefix >= valores.Length) { return; }

        if (valores[prefix] < valor)
        {
            RemDinheiroHelper(prefix + 1);
        }

        // se tem valor suficiente nessa posicao, pegue dessa posicao
        if (valores[prefix] >= valor)
        {
            valores[prefix] -= valor;
            return;
        }
    }

    private void RemDinheiroHelper(int prefix) {
        // se for acima da lista, pare a funcao
        if (prefix >= valores.Length) { return; }
        // se nao tiver nada, vai pro proximo
        if (valores[prefix] <= 0)
        {
            RemDinheiroHelper(prefix + 1);
        }
        // se tiver valor, remove 1 e adiciona mil no abaixo
        if (valores[prefix] > 0)
        {
            valores[prefix] -= 1;
            valores[prefix - 1] += 1000;
        }
        
    }

    public void AddDinheiro(int valor, int prefix)
    {
        // se estiver fora da lista, acabe a func
        if (prefix >= valores.Length) { return; }
        // add dinheiro
        valores[prefix] += valor;
        
        // se for o ultimo elemento, deixe o valor nele, nao continue a func
        if (prefix == valores.Length - 1) { return; }

        // se passar de mil, quebra o valo, add nos de cima
        if (valores[prefix] > 999)
        {
            valores[prefix] -= 1000;
            AddDinheiro(1, prefix + 1);
        }
    }
}
