using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class scr_gameManager : MonoBehaviour
{
    public float spawnEstrelaDelayInicial = 2f;
    public float spawnEstrelaDelay;
    private float spawnEstrelaTempoAtual = 0;

    public GameObject estrelaPrefab;

    private Camera mainCamera;


    private scr_ninetree estrelas;
    public int profundidadeEstrelasTree;

    // X esquerdo, Z direito, Y cima, W baixo
    public Vector4 offsetLateral = new Vector4(0.05f, 0.05f, 0.05f, 0.05f);

    public LineRenderer linhaEstrelas;

    public float linhaFadeDelay = 1.5f;
    private float linhaFadeDuracao = 0f;
    private bool linhaFadeIn = false;
    private bool linhaFadeOut = false;

    private int estrelaCurrPrefix = 0;

    private scr_shop shop;
    private scr_upgradeManager upgradeManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        shop = gameObject.GetComponent<scr_shop>();
        upgradeManager = gameObject.GetComponent<scr_upgradeManager>();

        // cria a Tree
        mainCamera = Camera.main;
        Vector3 sizeViewport = new Vector3(1, 1, 0);
        Vector3 sizeWorld = mainCamera.ViewportToWorldPoint(sizeViewport);
        Vector2 screenSize = new Vector2(sizeWorld.x, sizeWorld.y);

        estrelas = new scr_ninetree(profundidadeEstrelasTree, screenSize);

        // Debug ??
        estrelas.CreateId();
        Debug.Log("screenSize= " + screenSize);
        
        // ajusta o spawn de estrelas
        spawnEstrelaDelay = spawnEstrelaDelayInicial;
    }

    // ------------------ Estrelas
    void Update()
    {
        spawnEstrelaTempoAtual += Time.deltaTime;
        if (spawnEstrelaTempoAtual >= spawnEstrelaDelay)
        {
            SpawnarEstrela();
            // reset o tempo
            spawnEstrelaTempoAtual = 0f;
        }

        // fade a linha de constelacoes
        if (linhaFadeIn || linhaFadeOut)
        {
            linhaFadeDuracao += Time.deltaTime;
            float alpha;
            if (linhaFadeOut)
            {
                alpha = Mathf.Lerp(1f, 0f, linhaFadeDuracao / linhaFadeDelay);
            }
            else
            {
                alpha = Mathf.Lerp(0f, 1f, linhaFadeDuracao * 2);
                // terminou o fade in
                if (Mathf.Approximately(alpha, 1f))
                {
                    linhaFadeIn = false;
                    linhaFadeOut = true;
                    linhaFadeDuracao = 0f;
                }
            }
            // muda a cor da linha
            Color newStart = new Color(linhaEstrelas.startColor.r, linhaEstrelas.startColor.g, linhaEstrelas.startColor.b, alpha);
            Color newEnd = new Color(linhaEstrelas.endColor.r, linhaEstrelas.endColor.g, linhaEstrelas.endColor.b, alpha);
            linhaEstrelas.startColor = newStart;
            linhaEstrelas.endColor = newEnd;
        }

        DrawDebug();
    }

    public void AjustarSpawnEstrelaDelay(float novoTempo)
    {
        spawnEstrelaDelay = novoTempo;
    }

    public void SpawnarEstrela()
    {
        // posicao relativa da camera de [0.0, 1.0]
        float randomX = UnityEngine.Random.Range(offsetLateral.x, 1 - offsetLateral.z);
        float randomY = UnityEngine.Random.Range(offsetLateral.w, 1 - offsetLateral.y);
        float z = -mainCamera.transform.position.z; // retira o Z da camera
        Vector3 randomPoiscaoViewport = new Vector3(randomX, randomY, z);
        // transforma da viewport para do world
        Vector3 randomPoiscaoWorld = mainCamera.ViewportToWorldPoint(randomPoiscaoViewport);

        // instancia
        GameObject estrelaObj = Instantiate(estrelaPrefab, randomPoiscaoWorld, Quaternion.identity);
        scr_estrela estrela = estrelaObj.GetComponent<scr_estrela>();

        // estrela.SetValor(_valor, _prefixIndex);
        int valor = UnityEngine.Random.Range(1, 3);
        estrela.SetValor(valor, estrelaCurrPrefix);

        // adiciona ao estrela criada nas estrelas
        // estrelas.Add(estrelaObj);

        // Debug ???
        int nodeId = estrelas.AddGetId(estrelaObj);
        SpriteRenderer sprite = estrela.GetComponent<SpriteRenderer>();
        sprite.color = spriteColors[nodeId % spriteColors.Length];
    }

    public void PegarEstrela(GameObject estrela)
    {
        int qntdEstrelasExtrasPorClick = upgradeManager.tamConstelacao;
        // array de todas as estrelas que foram pegas
        GameObject[] estrelasPegas = new GameObject[qntdEstrelasExtrasPorClick + 1];

        estrelasPegas[0] = estrela;
        estrelas.Remove(estrela);

        // Se tiver upgrade de pegar estrelas proximas
        if (qntdEstrelasExtrasPorClick > 0)
        {
            EncontrarEstrelasProximas(estrela, qntdEstrelasExtrasPorClick, estrelasPegas);
        }
        
        AdicionarPontos(estrelasPegas);
    }

    private void AdicionarPontos(GameObject[] estrelasPegas)
    {
        foreach (GameObject estrelaObj in estrelasPegas)
        {
            // se n for valido, pule
            if (estrelaObj == null) { continue; }
            // pega o componente scr_estrela
            scr_estrela estrelaComp = estrelaObj.GetComponent<scr_estrela>();
            // pega o valor
            (int valor, int prefix) = estrelaComp.GetValor();
            shop.AddDinheiro(valor, prefix);
            // deleta
            estrelaComp.DiminuirDeletar();
        }
        // Todo somar o valor aqui antes de passar pro shop
    }

    private void EncontrarEstrelasProximas(GameObject estrelaInicial, int qntdExtra, GameObject[] estrelasPegas)
    {
        // distancia minima entre pontos para criar uma linha
        const float minRequiredDistance = 0.5f;
        // lista de estrelas que vao formar os pontos das linhas
        LinkedList<GameObject> estrelasLinhaList = new LinkedList<GameObject>();
        estrelasLinhaList.AddFirst(estrelaInicial);
        // adiciona a estrela inicia nas pegas
        estrelasPegas[0] = estrelaInicial;

        GameObject estrelPrev = estrelaInicial;
        for (int i = 1; i < qntdExtra + 1; i++)
        {
            // remove da lista a estrela, antes de buscar outra estrela proxima
            // Pega a estrela mais proxima da atual
            (GameObject estrelaNext, float dist) = estrelas.EncontrarProximo(estrelPrev);
            // se nao encontrou nenhum perto, acabe o loop
            if (estrelaNext == null) { break; }
            // se tiver o minimo de distancia, add a lista para fazer a linha
            if (dist > minRequiredDistance)
            {
                estrelasLinhaList.AddLast(estrelaNext);
            }
            // remove das estrelas, e marca para deletar
            estrelas.Remove(estrelaNext);
            estrelasPegas[i] = estrelaNext;
            // atualiza
            estrelPrev = estrelaNext;
        }

        // tem mais de 1 estrela para formar a linha entre elas
        if (estrelasLinhaList.Count > 1)
        {
            CriarConstelacao(estrelasLinhaList);
        }
    }

    private void CriarConstelacao(LinkedList<GameObject> estrelasLinhaList)
    {
        // se o angulo for menor do q isso, sao considerados colineares
        const float minAnguloColineares = 20 * (float)Math.PI / 180f; // 20 graus

        // desativado
        if (estrelasLinhaList.Count >= 3 && false)
        {
            // comeca com o segundo elemento da lista
            LinkedListNode<GameObject> nodeEstrelaB = estrelasLinhaList.First.Next;
            while (nodeEstrelaB != estrelasLinhaList.Last)
            {
                // pega as 3 estrelas que serao os 2 segmentos de reta
                GameObject estrelaA = nodeEstrelaB.Previous.Value;
                GameObject estrelaB = nodeEstrelaB.Value;
                GameObject estrelaC = nodeEstrelaB.Next.Value;

                // proximo nodo 
                nodeEstrelaB = nodeEstrelaB.Next;

                // transforma nos vetores AB e BC
                Vector2 dirAB = new Vector2(
                    estrelaB.transform.position.x - estrelaA.transform.position.x,
                    estrelaB.transform.position.y - estrelaA.transform.position.y
                ).normalized;
                Vector2 dirBC = new Vector2(
                    estrelaC.transform.position.x - estrelaB.transform.position.x,
                    estrelaC.transform.position.y - estrelaB.transform.position.y
                ).normalized;
                // produto escalar, como normalizamos, isso nos da o Cos do angulo deles
                float dot = Vector2.Dot(dirAB, dirBC);
                // garantimos que esta entre -1 e 1
                dot = Mathf.Clamp(dot, -1f, 1f);
                // queremos o quao proximo o dot esta de -1, se for menor do q minAngulo sao colineares
                if (Math.Abs(1 + dot) < minAnguloColineares)
                {
                    // criamos um apendice, a linha vai A -> B -> A -> C
                    estrelasLinhaList.AddAfter(nodeEstrelaB, estrelaA);
                }
            }
        }

        // ajusto o LineRenderer para o numero de pontos da minha linha
        linhaEstrelas.positionCount = estrelasLinhaList.Count;
        // para cada estrela na lista, marque como uma posicao da linha
        int contador = 0;
        foreach (GameObject estrela in estrelasLinhaList)
        {
            linhaEstrelas.SetPosition(contador, estrela.transform.position);
            contador++;
        }

        // Fade
        linhaFadeDuracao = 0f;
        linhaFadeIn = true;
        linhaFadeOut = false;
    }

    // ---------- Debug
    
    private void DrawDebug()
    {
        Vector3 sizeViewport = new Vector3(1, 1, 0);
        Vector3 sizeWorld = mainCamera.ViewportToWorldPoint(sizeViewport);
        Vector2 screenSize = new Vector2(sizeWorld.x, sizeWorld.y);

        foreach ((Vector3 _posMin, Vector3 _posMax, int profund, int filhoId) in estrelas.GetBoxes())
        {
            Vector3 posMin = _posMin * screenSize;
            Vector3 posMax = _posMax * screenSize;

            Color color;
            switch (profund)
            {
                case 0:
                    color = Color.green; break;
                case 1:
                    color = Color.blue; break;
                case 2:
                    color = Color.red; break;
                case 3:
                    color = Color.magenta; break;
                default:
                    color = Color.gray; break;
            }
            color.a = 0.2f;

            DrawBox(posMin, posMax, color);

            if (filhoId == 0 || filhoId == 8)
            {
                Debug.DrawLine(posMin, posMax, color);
                if (filhoId == 8)
                {
                    
                    Debug.DrawLine(
                        new Vector3(posMin.x, posMax.y, posMin.z),
                        new Vector3(posMax.x, posMin.y, posMin.z),
                        color);
                }
            }
            
        }
    }

    private Color[] spriteColors = new Color[]
    {
        Color.red,
        Color.green,
        Color.blue,
        Color.cyan,
        Color.magenta,
        Color.yellow,
        Color.black,
        Color.gray,

        new Color(1f, 0.5f, 0f),      // Orange
        new Color(0.5f, 0.25f, 0f),   // Brown
        new Color(0.5f, 0f, 0.5f),    // Purple
        new Color(0.25f, 0.75f, 0.25f), // Soft Green
        new Color(0.75f, 0.25f, 0.25f), // Soft Red
        new Color(0.25f, 0.25f, 0.75f), // Soft Blue
        new Color(0.8f, 0.8f, 0.2f),   // Olive
        new Color(0.2f, 0.8f, 0.8f),   // Teal
        new Color(0.8f, 0.2f, 0.8f),   // Pinkish Purple

        new Color(0.9f, 0.6f, 0.6f),   // Light Red
        new Color(0.6f, 0.9f, 0.6f),   // Light Green
        new Color(0.6f, 0.6f, 0.9f),   // Light Blue
        new Color(0.9f, 0.9f, 0.6f),   // Light Yellow
        new Color(0.6f, 0.9f, 0.9f),   // Light Cyan
        new Color(0.9f, 0.6f, 0.9f),   // Light Magenta
        new Color(0.9f, 0.8f, 0.7f),   // Peach
        new Color(0.7f, 0.9f, 0.8f),   // Mint
        new Color(0.8f, 0.7f, 0.9f),   // Lavender

        new Color(1f, 0.4f, 0.2f),     // Coral
        new Color(0.2f, 1f, 0.4f),     // Bright Green
        new Color(0.4f, 0.2f, 1f),     // Indigo
        new Color(1f, 0.8f, 0.2f),     // Gold
        new Color(0.2f, 1f, 1f),       // Aqua
        new Color(1f, 0.2f, 0.6f),     // Hot Pink
        new Color(0.6f, 0.3f, 0.1f),   // Chestnut
        new Color(0.3f, 0.6f, 0.1f),   // Moss Green
        new Color(0.1f, 0.6f, 0.6f),   // Deep Teal
        new Color(0.6f, 0.1f, 0.6f)    // Deep Purple
    };

    void DrawBox(Vector3 posMin, Vector3 posMax, Color color)
    {
        // 8 corners of the box
        Vector3[] corners = new Vector3[8]
        {
            new Vector3(posMin.x, posMin.y, posMin.z), // 0
            new Vector3(posMax.x, posMin.y, posMin.z), // 1
            new Vector3(posMax.x, posMin.y, posMax.z), // 2
            new Vector3(posMin.x, posMin.y, posMax.z), // 3

            new Vector3(posMin.x, posMax.y, posMin.z), // 4
            new Vector3(posMax.x, posMax.y, posMin.z), // 5
            new Vector3(posMax.x, posMax.y, posMax.z), // 6
            new Vector3(posMin.x, posMax.y, posMax.z)  // 7
        };

        // bottom square
        Debug.DrawLine(corners[0], corners[1], color);
        Debug.DrawLine(corners[1], corners[2], color);
        Debug.DrawLine(corners[2], corners[3], color);
        Debug.DrawLine(corners[3], corners[0], color);

        // top square
        Debug.DrawLine(corners[4], corners[5], color);
        Debug.DrawLine(corners[5], corners[6], color);
        Debug.DrawLine(corners[6], corners[7], color);
        Debug.DrawLine(corners[7], corners[4], color);

        // vertical lines
        Debug.DrawLine(corners[0], corners[4], color);
        Debug.DrawLine(corners[1], corners[5], color);
        Debug.DrawLine(corners[2], corners[6], color);
        Debug.DrawLine(corners[3], corners[7], color);
    }
}
