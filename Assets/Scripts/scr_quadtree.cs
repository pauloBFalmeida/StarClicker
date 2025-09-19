using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;
using Unity.VisualScripting;

public class scr_quadtree
{
    private QuadtreeNode root;
    public scr_quadtree(int profundidade, Vector2 screenSize)
    {
        Vector2 posMin = new Vector2(-1f, -1f);
        Vector2 posMax = new Vector2(1f, 1f);
        root = new QuadtreeNode(posMin, posMax, profundidade, screenSize);
    }

    public void SetScreenSize(Vector2 screenSize)
    {
        root.SetScreenSize(screenSize);
    }

    public void Add(GameObject objeto)
    {
        root.Add(objeto);
        Debug.Log("Add transf pos" + objeto.transform.position);
    }

    public void Remove(GameObject objeto)
    {
        root.Remove(objeto);
    }

    public (GameObject obj, float dist) EncontrarProximo(GameObject objetoAlvo)
    {
        return root.EncontrarProximo(objetoAlvo);
    }

    public GameObject FindClosestFast(GameObject objetoAlvo)
    {
        return root.FindClosestFast(objetoAlvo);
    }


    public GameObject FindClosest(GameObject objetoAlvo)
    {
        GameObject obj = root.FindClosestFast(objetoAlvo);
        if (obj != null)
        {
            return obj;
        }
        return root.FindClosest(objetoAlvo);

        // Debug.Log("filhos: " + root.QuantidadeObjetosNosFilhos());

        // GameObject obj = root.FindClosest(objetoAlvo);
        // return obj;
    }

    public float Distance(GameObject objetoA, GameObject objetoB)
    {
        return root.DistanciaManhattan(objetoA, objetoB);
    }

    public List<(Vector3 pos, Vector3 size)> GetBoxes()
    {
        List<(Vector3 pos, Vector3 size)> list = new List<(Vector3 pos, Vector3 size)>();
        root.GetBoxes(list);
        return list;
    }

    public void CreateId()
    {
        int id = 0;
        foreach (QuadtreeNode node in GetAllNodesFolha())
        {
            node.id = id;
            id++;
            Debug.Log("node id = " + node.id);
        }
    }
    public List<QuadtreeNode> GetAllNodesFolha()
    {
        List<QuadtreeNode> list = new List<QuadtreeNode>();
        root.GetAllNodesFolha(list);
        return list;

    }
    public int AddGetId(GameObject objeto)
    {
        return root.AddGetId(objeto);
    }
}


public struct Filhos<T>
{
    public T EsqCima;
    public T DirCima;
    public T EsqBaixo;
    public T DirBaixo;

    public Filhos(
        T esqCima,
        T dirCima,
        T esqBaixo,
        T dirBaixo)
    {
        EsqCima = esqCima;
        DirCima = dirCima;
        EsqBaixo = esqBaixo;
        DirBaixo = dirBaixo;
    }

    public T[] GetAll()
    {
        T[] all = {
            EsqCima,
            DirCima,
            EsqBaixo,
            DirBaixo
        };
        return all;
    }
}

public class QuadtreeNode
{
    public Vector2 posMin { get; private set; }
    public Vector2 posMid { get; private set; }
    public Vector2 posMax { get; private set; }

    private Vector2 screenSize;

    /*
        inicial (x, y) - final (x, y)
        caso o valor seja igual o da metade do intervalo, vai no menor x

        filhos EsqCima:  (0, 0) - (0.5, 0.5)
        filhos DirCima:  (0.5, 0) - (1, 0.5)
        filhos EsqBaixo: (0, 0.5) - (0.5, 1)
        filhos DirBaixo: (0.5, 0.5) - (1, 1)
    */
    private Filhos<QuadtreeNode> filhos;

    private List<GameObject> objetos;

    public int quantidadeObjetos { get; private set; }
    public int profundidade { get; private set; }

    public int id { get; set; }

    public int AddGetId(GameObject objeto) {
        quantidadeObjetos += 1;

        if (profundidade == 0)
        {
            objetos.Add(objeto);
            return id;
        }

        QuadtreeNode nodoFilho = GetNodoFilhoContendo(objeto);
        return nodoFilho.AddGetId(objeto);
    }

    public void GetBoxes(List<(Vector3 pos, Vector3 size)> list)
    {
        if (profundidade > 0)
        {
            foreach (QuadtreeNode filho in filhos.GetAll())
            {
                filho.GetBoxes(list);
            }
        }
        else
        {
            Vector3 pos = new Vector3(posMid.x, posMid.y, 0f);
            Vector3 size = new Vector3(posMax.x - posMid.x, posMax.y - posMid.y, 0f);
            list.Add((posMin, posMax));
        }
    }


    public void GetAllNodesFolha(List<QuadtreeNode> list)
    {
        if (profundidade > 0)
        {
            foreach (QuadtreeNode filho in filhos.GetAll())
            {
                filho.GetAllNodesFolha(list);
            }
        }
        else
        {
            list.Add(this);
        }
    }

    public QuadtreeNode(Vector2 _posMin, Vector2 _posMax, int _profundidade, Vector2 _screenSize)
    {
        quantidadeObjetos = 0;
        posMin = _posMin;
        posMax = _posMax;
        profundidade = _profundidade;
        screenSize = _screenSize;

        // acha o valor do meio 
        posMid = (posMin + posMax) / 2;

        // se tiver mais niveis de profundidade, cria os filhos
        if (_profundidade > 0)
        {
            // diminui a profundidade pros filhos
            _profundidade -= 1;

            /* Cria os filhos */
            filhos = new Filhos<QuadtreeNode>(
                // EsqCima
                new QuadtreeNode(
                    posMin,
                    new Vector2(posMid.x, posMid.y),
                    _profundidade, _screenSize),
                // DirCima
                new QuadtreeNode(
                    new Vector2(posMid.x, posMin.y),
                    new Vector2(posMax.x, posMid.y),
                    _profundidade, _screenSize),
                // EsqBaixo
                new QuadtreeNode(
                    new Vector2(posMin.x, posMid.y),
                    new Vector2(posMid.x, posMax.y),
                    _profundidade, _screenSize),
                // DirBaixo
                new QuadtreeNode(
                    new Vector2(posMid.x, posMid.y),
                    posMax,
                    _profundidade, _screenSize)
            );
        }
        else
        {
            objetos = new List<GameObject>();
        }
    }

    public void SetScreenSize(Vector2 _screenSize)
    {
        screenSize = _screenSize;
        foreach (QuadtreeNode filho in filhos.GetAll())
        {
            filho.SetScreenSize(_screenSize);
        }
    }

    public int QuantidadeObjetosNosFilhos()
    {
        // se nao tem filhos, retorna a quantidade de objetos que possui
        if (profundidade == 0)
        {
            return objetos.Count;
        }
        // se for pai, retorna a quantidade dos objetos dos filhos
        int soma = 0;
        soma += filhos.EsqCima.QuantidadeObjetosNosFilhos();
        soma += filhos.DirCima.QuantidadeObjetosNosFilhos();
        soma += filhos.EsqBaixo.QuantidadeObjetosNosFilhos();
        soma += filhos.DirBaixo.QuantidadeObjetosNosFilhos();
        return soma;
    }

    public void UpdateQuantidadeObjetos()
    {
        // se nao tem filhos, quantidade de objetos que possui
        if (profundidade == 0)
        {
            quantidadeObjetos = objetos.Count;
        }
        // se for pai, calcula a soma da quantidade dos objetos dos filhos
        else
        {
            filhos.EsqCima.UpdateQuantidadeObjetos();
            filhos.DirCima.UpdateQuantidadeObjetos();
            filhos.EsqBaixo.UpdateQuantidadeObjetos();
            filhos.DirBaixo.UpdateQuantidadeObjetos();
            int soma = 0;
            soma += filhos.EsqCima.quantidadeObjetos;
            soma += filhos.DirCima.quantidadeObjetos;
            soma += filhos.EsqBaixo.quantidadeObjetos;
            soma += filhos.DirBaixo.quantidadeObjetos;

            quantidadeObjetos = soma;
        }
    }

    private QuadtreeNode GetNodoFilhoContendo(GameObject objeto)
    {
        Vector2 posObj = new Vector2(objeto.transform.position.x, objeto.transform.position.y);
        Debug.Log("Contendo " + " posObj " + posObj + " screenSize " + screenSize);
        posObj = posObj / screenSize;

        Debug.Log("Contendo 2 " + " posObj " + posObj + " posMid " + posMid);

        if (posObj.y <= posMid.y)
        {
            if (posObj.x <= posMid.x)
            {
                // x <= 0.5, y <= 0.5
                return filhos.EsqCima;
            }
            else
            {
                // x > 0.5, y <= 0.5
                return filhos.DirCima;
            }
        }
        else
        {
            if (posObj.x <= posMid.x)
            {
                // x <= 0.5, y > 0.5
                return filhos.EsqBaixo;
            }
            else
            {
                // x > 0.5, y > 0.5
                return filhos.DirBaixo;
            }
        }
    }

    public void Add(GameObject objeto)
    {
        quantidadeObjetos += 1;

        if (profundidade == 0)
        {
            objetos.Add(objeto);
            return;
        }

        QuadtreeNode nodoFilho = GetNodoFilhoContendo(objeto);
        nodoFilho.Add(objeto);
    }

    public void Remove(GameObject objeto)
    {
        quantidadeObjetos -= 1;

        if (profundidade == 0)
        {
            objetos.Remove(objeto);
            return;
        }

        QuadtreeNode nodoFilho = GetNodoFilhoContendo(objeto);
        nodoFilho.Remove(objeto);
    }

    public (GameObject, float) EncontrarProximo(GameObject objetoAlvo)
    {
        // -- se nao tem objetos, nem nos filhos
        if (quantidadeObjetos < 1) { return (null, float.PositiveInfinity); }
        // -- se for nodo folha, com filhos, retorne o mais proximo
        if (profundidade == 0) { return ObjetoMaisProximo(objetoAlvo); }
        // -- se nao for nodo folha, e tiver filhos

        QuadtreeNode nodoDireto = GetNodoFilhoContendo(objetoAlvo);
        (GameObject obj, float dist) = nodoDireto.EncontrarProximo(objetoAlvo);

        // se nao tinha filho diretamente abaixo, olha nos lados
        if (obj == null)
        {
            float bestDist = float.PositiveInfinity;
            GameObject bestObj = null;
            foreach (QuadtreeNode nodo in filhos.GetAll())
            {
                (GameObject currObj, float currDist) = nodo.EncontrarProximo(objetoAlvo);
                if (currDist < bestDist)
                {
                    bestDist = currDist;
                    bestObj = currObj;
                }

            }
            return (bestObj, bestDist);
        }
        else {
            return (obj, dist);
        }
    }   

    /*

        Deve ser chamado depois de remove
        Chamado no root
    */
    public GameObject FindClosest(GameObject objetoAlvo)
    {
        // se nao tem objetos, nem nos filhos
        if (quantidadeObjetos < 1)
        {
            return null;
        }

        QuadtreeNode nodoDireto = GetNodoFilhoContendo(objetoAlvo);
        GameObject melhorObj = null;
        float melhorDist = float.PositiveInfinity;
        foreach (QuadtreeNode nodo in filhos.GetAll())
        {
            GameObject obj; float dist;
            if (nodo == nodoDireto)
            {
                (obj, dist) = nodoDireto.FindClosestDirect(objetoAlvo);
            }
            else
            {
                (obj, dist) = nodoDireto.FindClosestHelper(objetoAlvo);
            }
            // compara com o melhor
            if (dist < melhorDist)
            {
                melhorObj = obj;
                melhorDist = dist;
            }
        }
        return melhorObj;
    }

    private (GameObject, float) FindClosestDirect(GameObject objetoAlvo)
    {
        // se nao tem objetos, nem nos filhos
        if (quantidadeObjetos < 1)
        {
            return (null, float.PositiveInfinity);
        }

        // se nodo nao tem filhos, ve quais dos objetos eh o mais proximo
        if (profundidade == 0)
        {
            return ObjetoMaisProximo(objetoAlvo);
        }

        // -- se tiver filhos --

        // se os filhos forem folhas -> pegue todos
        if (profundidade == 1)
        {
            GameObject minObj = null;
            float minDist = float.PositiveInfinity;
            foreach (QuadtreeNode filho in filhos.GetAll())
            {
                (GameObject obj, float dist) = filho.FindClosestDirect(objetoAlvo);
                if (dist < minDist)
                {
                    minDist = dist;
                    minObj = obj;
                }
            }
            return (minObj, minDist);
        }

        QuadtreeNode nodoFilho = GetNodoFilhoContendo(objetoAlvo);
        
        // se o filho direto nao tiver objetos -> pega outro filho
        if (nodoFilho.quantidadeObjetos < 1)
        {
            // pega o primeiro nodo filho, que possua objetos
            foreach (QuadtreeNode filho in filhos.GetAll())
            {
                if (filho.quantidadeObjetos > 0)
                {
                    nodoFilho = filho;
                    break;
                }
            }
        }

        return nodoFilho.FindClosestDirect(objetoAlvo);
    }

    private (GameObject, float) FindClosestHelper(GameObject objetoAlvo)
    {
        // se nao tem objetos, nem nos filhos
        if (quantidadeObjetos < 1)
        {
            return (null, float.PositiveInfinity);
        }

        // se nodo nao tem nodos filhos, ve quais dos objetos eh o mais proximo
        if (profundidade == 0)
        {
            return ObjetoMaisProximo(objetoAlvo);
        }

        // -- se tiver filhos --
        QuadtreeNode nodoFilhoDireto = GetNodoFilhoContendo(objetoAlvo);
        (GameObject objDireto, float distDireto) = nodoFilhoDireto.FindClosestDirect(objetoAlvo);
        // se o gameobj for nulo, vemos se algum outro filho tem um obj valido
        if (objDireto == null)
        {
            GameObject minObj = null;
            float minDist = float.PositiveInfinity;
            foreach (QuadtreeNode filho in filhos.GetAll())
            {
                // se for o filho direto, que ja foi testado, pule
                if (filho == nodoFilhoDireto)
                {
                    continue;
                }
                // pega o melhor obj e dist do filho testado atualmente
                (GameObject obj, float dist) = filho.FindClosestHelper(objetoAlvo);
                if (dist < minDist)
                {
                    minDist = dist;
                    minObj = obj;
                }
            }
            // retorna a melhor opcao que encontramos entre os filhos
            return (minObj, minDist);
        }
        // se o game obj for valido, retornamos ele
        else
        {
            return (objDireto, distDireto);
        }
    }

    public GameObject FindClosestFast(GameObject objetoAlvo)
    {
        (GameObject obj, float dist) = FindClosestDirect(objetoAlvo);
        return obj;
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

    public float DistanciaManhattan(GameObject objetoA, GameObject objetoB)
    {
        Vector3 a = objetoA.transform.position;
        Vector3 b = objetoB.transform.position;
        float dist = Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y) + Mathf.Abs(a.z - b.z);
        return dist;
    }
}
