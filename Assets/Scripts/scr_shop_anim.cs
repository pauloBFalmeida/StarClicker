using UnityEngine;
using UnityEngine.UIElements;

public class scr_shop_anim : MonoBehaviour
{
    private Vector2 showPos;
    public float hidePosY;

    private RectTransform rectTransform;
    private Vector2 targetPos;
    public float speed = 10;

    public GameObject[] buttons;


    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        showPos = new Vector2(rectTransform.anchoredPosition.x, rectTransform.anchoredPosition.y);
        HideMenu();
    }

    // Update is called once per frame
    void Update()
    {
        // Smoothly interpolate from current to target
        rectTransform.anchoredPosition = Vector2.Lerp(
            rectTransform.anchoredPosition,
            targetPos,
            Time.deltaTime * speed
        );
    }

    public void ShowMenu()
    {
        targetPos = showPos;
        AjustButtons(true);
    }

    public void HideMenu()
    {
        targetPos = showPos;
        targetPos.y = hidePosY;
        AjustButtons(false);
    }

    private void AjustButtons(bool visible)
    {
        foreach (GameObject button in buttons)
        {
            button.SetActive(visible);
        }
    }
}
