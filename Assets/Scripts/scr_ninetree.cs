using UnityEngine;
using System.Collections.Generic;

public class scr_ninetree
{
    private NinetreeNode root;
    public scr_ninetree(int profundidade, Vector2 screenSize)
    {
        Vector2 posMin = new Vector2(-1f, -1f);
        Vector2 posMax = new Vector2(1f, 1f);
        root = new NinetreeNode(posMin, posMax, profundidade, screenSize);
    }

    public void Remove(GameObject objeto)
    {
        root.Remove(objeto);
    }

    public (GameObject obj, float dist) EncontrarProximo(GameObject objetoAlvo)
    {
        return root.EncontrarProximo(objetoAlvo);
    }

    public List<(Vector3 posInicial, Vector3 posFinal, int profundidade, int filhoId)> GetBoxes()
    {
        List<(Vector3, Vector3, int, int)> list = new List<(Vector3, Vector3, int, int)>();
        root.GetBoxes(list, -1);
        return list;
    }

    public void CreateId()
    {
        int id = 0;
        foreach (NinetreeNode node in GetAllNodesFolha())
        {
            node.id = id;
            id++;
            Debug.Log("node id = " + node.id);
        }
    }
    public List<NinetreeNode> GetAllNodesFolha()
    {
        List<NinetreeNode> list = new List<NinetreeNode>();
        root.GetAllNodesFolha(list);
        return list;

    }
    public int AddGetId(GameObject objeto)
    {
        return root.AddGetId(objeto);
    }
}

public class NinetreeNode
{
    public Vector2 posMin { get; private set; }
    public Vector2 posMax { get; private set; }
    private float posXL; // X left
    private float posXR; // X right
    private float posYT; // Y top
    private float posYB; // Y bottom

    private Vector2 screenSize;
    public int quantidadeObjetos { get; private set; }
    public int profundidade { get; private set; }
    private List<GameObject> objetos;
    /*
        6   7   8
        3   4   5
        0   1   2
    */
    private NinetreeNode[] nodosFilhos;

    public int id { get; set; }

    public int AddGetId(GameObject objeto)
    {
        quantidadeObjetos += 1;

        if (profundidade == 0)
        {
            objetos.Add(objeto);
            return id;
        }

        NinetreeNode nodoFilho = GetNodoFilhoContendo(objeto);
        return nodoFilho.AddGetId(objeto);
    }

    public void GetBoxes(List<(Vector3 posInicial, Vector3 posFinal, int profundidade, int filhoId)> list, int filhoId = -1)
    {
        if (profundidade > 0)
        {
            for (int i = 0; i < 9; i++)
            {
                NinetreeNode filho = nodosFilhos[i];
                filho.GetBoxes(list, i);
            }
        }
        Vector3 posInicial = new Vector3(posMin.x, posMin.y, 0f);
        Vector3 posFinal = new Vector3(posMax.x, posMax.y, 0f);
        list.Add((posInicial, posFinal, profundidade, filhoId));
    }

    public void GetAllNodesFolha(List<NinetreeNode> list)
    {
        if (profundidade > 0)
        {
            foreach (NinetreeNode filho in nodosFilhos)
            {
                filho.GetAllNodesFolha(list);
            }
        }
        else
        {
            list.Add(this);
        }
    }

    public NinetreeNode(Vector2 _posMin, Vector2 _posMax, int _profundidade, Vector2 _screenSize)
    {
        quantidadeObjetos = 0;
        posMin = _posMin;
        posMax = _posMax;
        profundidade = _profundidade;
        screenSize = _screenSize;

        // acha os valores dos intervalos no meio
        Vector2 separacao = (posMax - posMin) / 3;
        posXL = posMin.x + separacao.x;
        posXR = posMin.x + 2 * separacao.x;
        posYT = posMin.y + separacao.y;
        posYB = posMin.y + 2 * separacao.y;

        // se tiver mais niveis de profundidade, cria os filhos
        if (_profundidade > 0)
        {
            // diminui a profundidade pros filhos
            _profundidade -= 1;

            nodosFilhos = new NinetreeNode[9];

            float[] xList = new float[] { posMin.x, posXL, posXR, posMax.x };
            float[] yList = new float[] { posMin.y, posYT, posYB, posMax.y };
            for (int yId = 0; yId < 3; yId++)
            {
                for (int xId = 0; xId < 3; xId++)
                {
                    nodosFilhos[yId * 3 + xId] = new NinetreeNode(
                        new Vector2(xList[xId], yList[yId]),
                        new Vector2(xList[xId + 1], yList[yId + 1]),
                        _profundidade,
                        _screenSize
                    );
                }
            }
        }
        // nodo folha
        else
        {
            objetos = new List<GameObject>();
        }
    }

    private (int x, int y) GetIndexContendo(GameObject objeto)
    {
        Vector2 posObj = new Vector2(objeto.transform.position.x, objeto.transform.position.y);
        posObj = posObj / screenSize;

        int yId = 0;
        if (posObj.y <= posYT)
        { yId = 0; }
        else if (posObj.y <= posYB)
        { yId = 1; }
        else
        { yId = 2; }

        int xId = 0;
        if (posObj.x <= posXL)
        { xId = 0; }
        else if (posObj.x <= posXR)
        { xId = 1; }
        else
        { xId = 2; }

        return (xId, yId);
    }

    private NinetreeNode GetNodoFilhoContendo(GameObject objeto)
    {
        (int xId, int yId) = GetIndexContendo(objeto);
        return nodosFilhos[yId * 3 + xId];
    }

    private NinetreeNode[] GetNodosOrdenadosContendo(GameObject objeto)
    {
        NinetreeNode[] listaOrdenada = new NinetreeNode[9];
        (int xId, int yId) = GetIndexContendo(objeto);

        int[] intOrdenados = GetIntOrdenados(xId, yId);
        for (int i = 0; i < 9; i++)
        {
            listaOrdenada[i] = nodosFilhos[intOrdenados[i]];
        }
        return listaOrdenada;
    }
    private int[] GetIntOrdenados(int x, int y) {
        /*
            0   1   2
            3   4   5
            6   7   8
        */
        int index = y * 3 + x;
        return index switch
        {
            0 => new int[] { 0, 1, 3, 4, 2, 6, 5, 7, 8 },
            1 => new int[] { 1, 0, 2, 4, 3, 5, 6, 7, 8 },
            2 => new int[] { 2, 1, 5, 4, 0, 8, 3, 7, 6 },
            3 => new int[] { 3, 0, 4, 6, 1, 7, 2, 5, 8 },
            4 => new int[] { 4, 1, 3, 5, 7, 0, 2, 6, 8 },
            5 => new int[] { 5, 2, 4, 8, 1, 7, 0, 3, 6 },
            6 => new int[] { 6, 3, 7, 0, 4, 8, 1, 5, 2 },
            7 => new int[] { 7, 4, 6, 8, 3, 5, 0, 1, 2 },
            8 => new int[] { 8, 5, 7, 2, 4, 6, 1, 3, 0 },
            _ => new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8 }, // nao deve cair aqui
        };
    }

    public void Remove(GameObject objeto)
    {
        quantidadeObjetos -= 1;

        if (profundidade == 0)
        {
            objetos.Remove(objeto);
            return;
        }

        NinetreeNode nodoFilho = GetNodoFilhoContendo(objeto);
        nodoFilho.Remove(objeto);
    }

    // Tem que ser usado remover antes
    public (GameObject, float) EncontrarProximo(GameObject objetoAlvo)
    {
        // -- se nao tem objetos, nem nos filhos
        if (quantidadeObjetos < 1) { return (null, float.PositiveInfinity); }
        // -- se for nodo folha, com filhos, retorne o mais proximo
        if (profundidade == 0) { return ObjetoMaisProximo(objetoAlvo); }
        // -- se nao for nodo folha, e tiver filhos

        // NinetreeNode nodoDireto = GetNodoFilhoContendo(objetoAlvo);
        // (GameObject obj, float dist) = nodoDireto.EncontrarProximo(objetoAlvo);

        // se nao tinha filho diretamente abaixo, olha nos lados
        // if (obj == null)
        if (true)
        {
            float bestDist = float.PositiveInfinity;
            GameObject bestObj = null;

            int contador = 1;
            foreach (NinetreeNode nodo in GetNodosOrdenadosContendo(objetoAlvo))
            {
                // pule o nodo direto, que ja foi testado
                // if (nodo == nodoDireto) { continue; }
                // verifica se o nodo atual tem um objeto melhor do que os encontrados anteriormente
                (GameObject currObj, float currDist) = nodo.EncontrarProximo(objetoAlvo);
                if (currDist < bestDist)
                {
                    bestDist = currDist;
                    bestObj = currObj;
                }
                // a cada 4 ve se ja encontrou um objeto valido, se nao continue 
                if (contador % 4 == 0)
                {
                    // se ja encontrou um objeto valido, retorne ele
                    if (bestObj != null)
                    {
                        break;
                    }
                    contador = 0;
                }
                contador++;
            }
            return (bestObj, bestDist);
        }
        // else
        // {
        //     return (obj, dist);
        // }
    }
    
    private (GameObject, float) ObjetoMaisProximo(GameObject objetoAlvo)
    {
        GameObject minObj = null;
        float minDist = float.PositiveInfinity;
        foreach (GameObject obj in objetos)
        {
            // calcula a distancia do elemento atual pro objeto alvo
            float dist = DistanciaManhattan(obj, objetoAlvo);
            // se for a menor que a ate agora, salve como a menor
            if (dist < minDist)
            {
                minDist = dist;
                minObj = obj;
            }
        }
        return (minObj, minDist);
    }

    private float DistanciaManhattan(GameObject objetoA, GameObject objetoB)
    {
        Vector3 a = objetoA.transform.position;
        Vector3 b = objetoB.transform.position;
        float dist = Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y) + Mathf.Abs(a.z - b.z);
        return dist;
    }
    
}