using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShowCard : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
   
    public float hoverScale = 1.2f;
    public Color enemyBackgroundColor = new Color(1f, 0.5f, 0.5f);
    public Color playerBackgroundColor = Color.white;

    public Color normalTextColor = Color.white;
    public Color iceModeTextColor = Color.cyan;


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

    public void CreateTheCard(CardScriptableObject data, bool isEnemy)
    {
        cardData = data;
        originalScale = transform.localScale;

        if (cardNameText != null) cardNameText.text = data.cardName;
        if (cardArt != null) cardArt.sprite = data.cardPicture;

        if (cardBackground != null)
        {
            cardBackground.color = isEnemy ? enemyBackgroundColor : playerBackgroundColor;
        }

        bool isInIceMode = false;
        if (GameManager.Instance != null && GameManager.Instance.player != null)
        {
            isInIceMode = GameManager.Instance.player.isInIceMode;
        }

        if (isEnemy)
        {
            // for enemy sun is + and snow is -
            if (sunPoint != null)
            {
                if (data.healAmount > 0) sunPoint.text = "+" + data.healAmount;
                else sunPoint.text = "0";
            }

            if (snowPoint != null)
            {
                if (data.damageAmount > 0) snowPoint.text = "-" + data.damageAmount;
                else snowPoint.text = "0";
            }
        }
        else
        {
            // for player sun is - and snow is +
            if (snowPoint != null)
            {
                if (data.healAmount > 0)
                {
                    snowPoint.text = "+" + data.healAmount;
                    snowPoint.color = normalTextColor;
                }
                else
                {
                    snowPoint.text = "0";
                }
            }
            // player damage is ice color if in ice mode
            if (sunPoint != null)
            {
                if (data.damageAmount > 0)
                {
                    if (isInIceMode)
                    {
                        int boostedDamage = data.damageAmount + 2;
                        sunPoint.text = "-" + boostedDamage; 
                        sunPoint.color = iceModeTextColor; 
                    }
                    else
                    {
                        sunPoint.text = "-" + data.damageAmount;
                        sunPoint.color = normalTextColor;
                    }
                }
                else
                {
                    sunPoint.text = "0";
                }
            }
        }

    }


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
