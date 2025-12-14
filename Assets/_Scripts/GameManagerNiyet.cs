/*
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; // Required for Hover/Click
using TMPro;

public class CardDisplay : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("References")]
    public CardData cardData; // The data this card holds
    public Image artImage;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descText;

    [Header("Settings")]
    public float hoverScale = 1.2f;
    public float moveSpeed = 10f;

    private BattleSystem battleSystem;
    private Vector3 originalPosition;
    private Vector3 originalScale;
    private bool isHovering = false;
    private bool cardPlayed = false;

    // Call this when spawning the card
    public void Setup(CardData data, BattleSystem system)
    {
        cardData = data;
        battleSystem = system;

        // Set visuals
        artImage.sprite = data.artwork;
        nameText.text = data.cardName;
        descText.text = data.description;

        originalScale = transform.localScale;
    }

    // --- INTERACTION EVENTS ---

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (cardPlayed || battleSystem.state != BattleState.PLAYER_TURN) return;

        isHovering = true;
        // Simple scale up
        transform.localScale = originalScale * hoverScale;
        // Optional: Bring to front so it overlaps neighbors
        transform.SetAsLastSibling();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (cardPlayed) return;

        isHovering = false;
        transform.localScale = originalScale;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (cardPlayed || battleSystem.state != BattleState.PLAYER_TURN) return;

        // Play the Card!
        StartCoroutine(PlaySequence());
    }

    // --- ANIMATION SEQUENCE ---

    System.Collections.IEnumerator PlaySequence()
    {
        cardPlayed = true;
        battleSystem.state = BattleState.BUSY; // Lock input

        // 1. Move to Center of Screen and Scale HUGE
        float time = 0;
        Vector3 startPos = transform.position;
        Vector3 centerPos = new Vector3(Screen.width / 2, Screen.height / 2, 0); // Center of Canvas

        while (time < 0.4f)
        {
            transform.position = Vector3.Lerp(startPos, centerPos, time / 0.4f);
            transform.localScale = Vector3.Lerp(originalScale * hoverScale, originalScale * 2f, time / 0.4f);
            time += Time.deltaTime;
            yield return null;
        }

        // 2. Pause to show off the card
        yield return new WaitForSeconds(0.5f);

        // 3. Tell System to Apply Effects
        battleSystem.ApplyCardEffect(cardData);

        // 4. Destroy this card object
        Destroy(gameObject);
    }
}
*/