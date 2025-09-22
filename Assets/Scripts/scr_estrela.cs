using UnityEngine;

public class scr_estrela : MonoBehaviour
{
    public Sprite[] sprites;
    public SpriteRenderer spriteRenderer;

    private float destroyDelay = 2f;

    public scr_gameManager gameManagerInstance;
    // duracao em segundos de diminuir
    private bool diminuindo = false;
    public float diminuirDuracao = 2f;
    private float tempoPassado = 0f;
    private Vector3 scaleOriginal;

    private int valor;
    private int prefixIndex;

    public void SetGameManagerInstace(scr_gameManager _gameManagerInstance)
    {
        gameManagerInstance = _gameManagerInstance;
    }

    void Start()
    {
        gameManagerInstance = FindFirstObjectByType<scr_gameManager>();
        // pega um dos sprites em aleatorio
        int randomIndex = Random.Range(0, sprites.Length); 
        spriteRenderer.sprite = sprites[randomIndex];
    }

    void OnMouseDown()
    {
        gameManagerInstance.PegarEstrela(this.gameObject);
    }

    public void SetValor(int _valor, int _prefixIndex)
    {
        valor = _valor;
        prefixIndex = _prefixIndex;
    }
    public void SetValor(Preco preco)
    {
        valor = preco.valor;
        prefixIndex = preco.prefixId;
    }

    public (int _valor, int _prefixIndex) GetValor()
    {
        return (valor, prefixIndex);
    }

    public void DiminuirDeletar()
    {
        diminuindo = true;
        tempoPassado = 0f;
        scaleOriginal = transform.localScale; 
    }

    void Update()
    {
        if (diminuindo)
        {
            tempoPassado += Time.deltaTime;
            float weight = tempoPassado / diminuirDuracao;

            transform.localScale = Vector3.Lerp(scaleOriginal, Vector3.zero, weight);

            if (tempoPassado >= diminuirDuracao)
            {
                Destroy(this.gameObject, destroyDelay);
            }
        }
    }

}
