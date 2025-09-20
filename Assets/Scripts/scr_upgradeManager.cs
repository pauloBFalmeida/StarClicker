using UnityEngine;

public class scr_upgradeManager : MonoBehaviour
{
    public int tamConstelacao = 0;
    private float magnitude = 6.5f;
    private int wavelenght = 0;

    public float maxMagnitude { get; private set; } = 31.5f;
    public int maxWavelenght { get; private set; } = 100;
    public int maxConstellations { get; private set; } = 40;

    private scr_shop shop;
    private scr_gameManager gameManager;


    void Start()
    {
        shop = gameObject.GetComponent<scr_shop>();
        gameManager = gameObject.GetComponent<scr_gameManager>();

        // ajusta os max
        // maxWavelenght = .prefixos.Length - 1;

    }


    public void MelhorarConstellations()
    {
        tamConstelacao += 1;
    }

    public void MelhorarMagnitude()
    {
        float aumento = 0.5f;
        magnitude += aumento;
        // calcula quantos aumentos terao no jogo
        float qntdAumentos = (maxMagnitude - 6.0f) / aumento;
        // diminui o tempo atual
        // spawnEstrelaDelay -= (spawnEstrelaDelayInicial - 0.1f) / qntdAumentos;
        float novoTempo = (gameManager.spawnEstrelaDelayInicial - 0.1f) / qntdAumentos;
        gameManager.AjustarSpawnEstrelaDelay(novoTempo);
    }
    public void MelhorarWavelenght()
    {
        wavelenght += 1;
        // estrelaCurrPrefix = wavelenght;
    }


    public float GetDisplayMagnitude()
    {
        return magnitude;
    }

    public int GetDisplayWavelenght()
    {
        return wavelenght * 10;
    }
    public int GetDisplayConstellations()
    {
        return tamConstelacao + 1;
    }

    public bool IsMaxMagnitude()
    {
        return magnitude >= maxMagnitude;
    }
    public bool IsMaxWavelenght()
    {
        return wavelenght >= maxWavelenght - 1;
    }

    public bool IsMaxConstellations()
    {
        return tamConstelacao >= maxConstellations;
    }
}