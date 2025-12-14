using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShowCard : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
   
    public float hoverScale = 1.2f;

    [Header("References")]
    public Image cardBackground;
    public Image cardArt;
    public Image nameBackground;
    public TextMeshProUGUI cardNameText;
    public Image snowIcon;
    public Image sunIcon;
    public TextMeshProUGUI snowPoint;
    public TextMeshProUGUI sunPoint;

    private CardScriptableObject cardData;
    private Vector3 originalScale;

    public void CreateTheCard(CardScriptableObject data)
    {
        cardData = data;

        if (cardNameText != null)
        {
            cardNameText.text = data.cardName;
        }
        /*
        if (cardDescription != null)
        {
            cardDescription.text = data.cardDescription;
        }
        */
        if (cardArt != null)
        {
            cardArt.sprite = data.cardPicture;
        }

        // efektler falan filan

        originalScale = transform.localScale;

    }


    // abi mouse la kart üstüne gelince hafif büyüycek sonra týklayýnca o ekranýn merkezine gelip büyüycek sonra kaybolcak bu pointer metodlarý onlarla ilgili
    public void OnPointerEnter(PointerEventData eventData)
    {
        if(originalScale == Vector3.zero) originalScale = Vector3.one;

        transform.localScale = originalScale * hoverScale;

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (originalScale == Vector3.zero) originalScale = Vector3.one;

        transform.localScale = originalScale; ;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        GameManager.Instance.OnCardClicked(cardData, this); 
    }

}
