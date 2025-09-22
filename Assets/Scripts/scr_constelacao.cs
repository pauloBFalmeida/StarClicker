using UnityEngine;
using System.Collections.Generic;

public class scr_constelacao : MonoBehaviour
{
    private LineRenderer linhaRenderer;
    public float linhaFadeDelay = 1.5f;
    private float linhaFadeDuracao = 0f;
    private bool linhaFadeIn = false;
    private bool linhaFadeOut = false;
    private Color linhaColorStart;
    private Color linhaColorEnd;

    void Start()
    {
        linhaRenderer = GetComponent<LineRenderer>();

        linhaColorStart = linhaRenderer.startColor;
        linhaColorEnd = linhaRenderer.endColor;
    }

    void Update()
    {
        // se tiver fade in out em andamento
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
            linhaColorStart.a = alpha;
            linhaColorEnd.a = alpha;
            linhaRenderer.startColor = linhaColorStart;
            linhaRenderer.endColor = linhaColorEnd;
        }
    }

    public void MostrarConstelacao(LinkedList<GameObject> estrelasList)
    {
        // ajusto o LineRenderer para o numero de pontos da minha linha
        linhaRenderer.positionCount = estrelasList.Count;
        // para cada estrela na lista, marque como uma posicao da linha
        int contador = 0;
        foreach (GameObject estrela in estrelasList)
        {
            linhaRenderer.SetPosition(contador, estrela.transform.position);
            contador++;
        }

        // Fade
        linhaFadeDuracao = 0f;
        linhaFadeIn = true;
        linhaFadeOut = false;
    }
}
