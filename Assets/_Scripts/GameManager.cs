using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum GameState { Start, PlayerTurn, Busy, EnemyTurn, Won, Lost }

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public static GameState state;

    public int currentMonth = 1;

    [Header("Time UI")]
    public Image monthDisplayImage;
    public Sprite[] monthSprites;
    public TextMeshProUGUI seasonInfoText;

    // --- YENİ: Arka Plan Ayarları ---
    [Header("Background Settings")]
    public Image backgroundDisplay; // Sahnedeki Background objesini buraya ata
    public Sprite[] seasonBackgrounds; // 4 Mevsim arka planını buraya ata
    // --------------------------------

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(Instance);
        else Instance = this;
    }

    [Header("UI Panels")]
    public GameObject winPanel;
    public GameObject losePanel;

    [Header("UI References")]
    public GameObject cardPrefab;
    public Transform deckSlot;
    public Transform enemyCardSpawnPoint;

    [Header("Game References")]
    public CharactersController player;
    public CharactersController enemy;
    public List<CardScriptableObject> playerDeck;
    public List<CardScriptableObject> enemyDeck;

    private void Start()
    {
        state = GameState.Start;
        if (winPanel != null) winPanel.SetActive(false);
        if (losePanel != null) losePanel.SetActive(false);

        UpdateTimeUI(); // Başlangıçta arka planı ve zamanı ayarla

        StartCoroutine(SetupTheDeck());
    }

    // --- EKSİK OLAN FONKSİYONLAR BURADA ---
    IEnumerator SetupTheDeck()
    {
        yield return new WaitForSeconds(1f);
        StartPlayerTurn();
    }

    private void StartPlayerTurn()
    {
        if (state == GameState.Won || state == GameState.Lost) return;

        state = GameState.PlayerTurn;
        DrawCard(3);
    }
    // ----------------------------------------

    public void DrawCard(int count)
    {
        foreach (Transform child in deckSlot) Destroy(child.gameObject);

        for (int i = 0; i < count; i++)
        {
            GameObject newCardObject = Instantiate(cardPrefab, deckSlot);
            CardScriptableObject randomCardData = GetHighChanceCard(playerDeck);
            newCardObject.GetComponent<ShowCard>().CreateTheCard(randomCardData);
        }
    }

    CardScriptableObject GetHighChanceCard(List<CardScriptableObject> targetDeck)
    {
        if (targetDeck.Count == 0) return null;

        int totalChance = 0;
        float playerHealth = player.GetHealthAmount();

        foreach (var card in targetDeck)
            totalChance += card.GetCardAppearChance(playerHealth, currentMonth);

        int randomValue = Random.Range(0, totalChance);
        int currentChance = 0;

        foreach (var card in targetDeck)
        {
            currentChance += card.GetCardAppearChance(playerHealth, currentMonth);
            if (randomValue < currentChance) return card;
        }
        return targetDeck[0];
    }

    public void OnCardClicked(CardScriptableObject cardData, ShowCard cardVisual)
    {
        if (state != GameState.PlayerTurn) return;
        StartCoroutine(PlayCardSequence(cardData, cardVisual));
    }

    IEnumerator PlayCardSequence(CardScriptableObject data, ShowCard visualCard)
    {
        state = GameState.Busy;
        Destroy(visualCard.gameObject);

        if (data.damageAmount > 0) enemy.TakeDamage(data.damageAmount);
        if (data.healAmount > 0) player.Heal(data.healAmount);

        yield return new WaitForSeconds(1f);

        if (enemy.currentHealth <= 0)
        {
            state = GameState.Won;
            Win();
        }
        else
        {
            StartCoroutine(EnemyTurn());
        }
    }

    IEnumerator EnemyTurn()
    {
        state = GameState.EnemyTurn;
        yield return new WaitForSeconds(1f);

        if (enemyDeck.Count > 0)
        {
            int randomIndex = Random.Range(0, enemyDeck.Count);
            CardScriptableObject enemyMove = enemyDeck[randomIndex];

            GameObject playedCard = Instantiate(cardPrefab, enemyCardSpawnPoint);
            Destroy(playedCard.GetComponent<Button>());
            playedCard.GetComponent<ShowCard>().CreateTheCard(enemyMove);

            yield return new WaitForSeconds(2f);

            if (enemyMove.damageAmount > 0) player.TakeDamage(enemyMove.damageAmount);
            if (enemyMove.healAmount > 0) enemy.Heal(enemyMove.healAmount);

            Destroy(playedCard);
        }

        if (player.currentHealth <= 0)
        {
            state = GameState.Lost;
            Lose();
        }
        else
        {
            AdvanceTime();

            if (state != GameState.Won && state != GameState.Lost)
            {
                StartPlayerTurn();
            }
        }
    }

    void AdvanceTime()
    {
        currentMonth++;

        if (currentMonth > 12)
        {
            state = GameState.Won;
            Win();
            return;
        }

        UpdateTimeUI();
    }

    void UpdateTimeUI()
    {
        // 1. Ay görselini güncelle
        if (monthDisplayImage != null && monthSprites.Length >= 12)
        {
            monthDisplayImage.sprite = monthSprites[currentMonth - 1];
        }

        int seasonIndex = (currentMonth - 1) / 3;

        // 2. Arka planı değiştir (YENİ KISIM)
        if (backgroundDisplay != null && seasonBackgrounds.Length > seasonIndex)
        {
            backgroundDisplay.sprite = seasonBackgrounds[seasonIndex];
        }

        // 3. Mevsim yazısını güncelle
        if (seasonInfoText != null)
        {
            string seasonName = "";
            int turnInSeason = ((currentMonth - 1) % 3) + 1;

            switch (seasonIndex)
            {
                case 0: seasonName = "WINTER"; break;
                case 1: seasonName = "SPRING"; break;
                case 2: seasonName = "SUMMER"; break;
                case 3: seasonName = "AUTUMN"; break;
            }

            seasonInfoText.text = $"{seasonName}\nMonth {turnInSeason}/3";
        }
    }

    private void Win()
    {
        if (winPanel != null) winPanel.SetActive(true);
    }

    private void Lose()
    {
        if (losePanel != null) losePanel.SetActive(true);
    }
}