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

    private bool isGameOver = false;

    [Header("Background Settings")]
    public Image backgroundDisplay; 
    public Sprite[] seasonBackgrounds; 

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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }


    public void TriggerWin()
    {
        if (isGameOver) return;
        isGameOver = true;

        if (winPanel != null) winPanel.SetActive(true);
    }

    public void TriggerLose()
    {
        if (isGameOver) return;
        isGameOver = true;

        Debug.Log("You Lost!");
        if (losePanel != null) losePanel.SetActive(true);
    }


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

        // dont draw the same card in the same deck again
        List<CardScriptableObject> drawnCards = new List<CardScriptableObject>();

        int attempts = 0;

        while (drawnCards.Count < count)
        {
            attempts++;
            CardScriptableObject candidate = GetHighChanceCard(playerDeck);

            if (!drawnCards.Contains(candidate))
            {
                drawnCards.Add(candidate);
            }

            if (attempts > 100) break;

        }

        foreach (CardScriptableObject cardData in drawnCards)
        {
            GameObject newCardObject = Instantiate(cardPrefab, deckSlot);
            newCardObject.GetComponent<ShowCard>().CreateTheCard(cardData, false);
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

        int finalDamage = data.damageAmount;

        if (finalDamage > 0 && player.isInIceMode)
        {
            finalDamage += 2;
        }

        // apply math
        if (data.damageAmount > 0) enemy.TakeDamage(finalDamage);
        if (data.healAmount > 0) player.Heal(data.healAmount);

        yield return new WaitForSeconds(1f);

        if (enemy.currentHealth <= 0)
        {
            state = GameState.Won;
            TriggerWin();
        }
        else
        {
            StartCoroutine(EnemyTurn());
        }
    }

    IEnumerator EnemyTurn()
    {
        state = GameState.EnemyTurn;
        // hide player hand on enemy turn
        if (deckSlot != null) deckSlot.gameObject.SetActive(false);
        yield return new WaitForSeconds(1f);

        if (enemyDeck.Count > 0)
        {
            int randomIndex = Random.Range(0, enemyDeck.Count);
            CardScriptableObject enemyMove = enemyDeck[randomIndex];

            GameObject playedCard = Instantiate(cardPrefab, enemyCardSpawnPoint);
            Destroy(playedCard.GetComponent<Button>());
            playedCard.GetComponent<ShowCard>().CreateTheCard(enemyMove, true);

            yield return new WaitForSeconds(2.8f);

            if (enemyMove.damageAmount > 0) player.TakeDamage(enemyMove.damageAmount);
            if (enemyMove.healAmount > 0) enemy.Heal(enemyMove.healAmount);

            Destroy(playedCard);
        }

        if (deckSlot != null) deckSlot.gameObject.SetActive(true);

        if (player.currentHealth <= 0)
        {
            state = GameState.Lost;
            TriggerLose();
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
            TriggerWin();
            return;
        }

        UpdateTimeUI();
    }

    void UpdateTimeUI()
    {
        /* ay kısmı şimdilik yok
        if (monthDisplayImage != null && monthSprites.Length >= 12)
        {
            monthDisplayImage.sprite = monthSprites[currentMonth - 1];
        }
        */
        int seasonIndex = (currentMonth - 1) / 3;

        if (backgroundDisplay != null && seasonBackgrounds.Length > seasonIndex)
        {
            backgroundDisplay.sprite = seasonBackgrounds[seasonIndex];
        }

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

}