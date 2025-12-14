/*
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum BattleState { START, PLAYER_TURN, ENEMY_TURN, BUSY, WON, LOST }

public class BattleSystem : MonoBehaviour
{
    public BattleState state;

    [Header("Units")]
    public UnitController playerUnit;
    public UnitController enemyUnit;

    [Header("Card System")]
    public GameObject cardPrefab;
    public Transform handArea; // The UI Panel where cards spawn
    public List<CardData> deckPool; // Drag all your ScriptableObjects here
    public List<CardData> enemyDeckPool; // Cards for the AI

    void Start()
    {
        state = BattleState.START;
        StartCoroutine(SetupBattle());
    }

    IEnumerator SetupBattle()
    {
        // Init logic here if needed
        yield return new WaitForSeconds(1f);
        PlayerTurn();
    }

    void PlayerTurn()
    {
        state = BattleState.PLAYER_TURN;
        DrawCards(3); // Give player 3 cards
    }

    // --- DRAWING LOGIC (WEIGHTED) ---
    void DrawCards(int amount)
    {
        // Clear previous hand
        foreach (Transform child in handArea) Destroy(child.gameObject);

        for (int i = 0; i < amount; i++)
        {
            CardData randomCard = GetWeightedCard();

            // Spawn the visual card
            GameObject newCardObj = Instantiate(cardPrefab, handArea);

            // Initialize script
            CardDisplay display = newCardObj.GetComponent<CardDisplay>();
            display.Setup(randomCard, this);
        }
    }

    CardData GetWeightedCard()
    {
        int totalWeight = 0;
        float playerHealthPercent = playerUnit.GetHealthPercentage();

        // 1. Calculate total weight based on current health
        foreach (var card in deckPool)
        {
            totalWeight += card.GetDynamicWeight(playerHealthPercent);
        }

        // 2. Pick a random number
        int randomValue = Random.Range(0, totalWeight);
        int currentSum = 0;

        // 3. Find which card corresponds to that number
        foreach (var card in deckPool)
        {
            currentSum += card.GetDynamicWeight(playerHealthPercent);
            if (randomValue < currentSum)
            {
                return card;
            }
        }

        return deckPool[0]; // Fallback
    }

    // --- PLAYING LOGIC ---

    public void ApplyCardEffect(CardData card)
    {
        // 1. Calculate Power Up (Example: +50% effect if high health)
        int finalDamage = card.damageToEnemy;
        if (playerUnit.GetHealthPercentage() > 0.8f) // 80% Power Up Theme
        {
            finalDamage += 2; // Bonus damage
        }

        // 2. Apply Stats
        enemyUnit.TakeDamage(finalDamage);
        playerUnit.Heal(card.healSelf);
        playerUnit.TakeDamage(card.snowCost); // Pay the cost

        // 3. Check Win Condition
        if (enemyUnit.currentHealth <= 0)
        {
            state = BattleState.WON;
            Debug.Log("YOU WIN!");
        }
        else if (playerUnit.currentHealth <= 0)
        {
            state = BattleState.LOST;
            Debug.Log("YOU MELTED!");
        }
        else
        {
            StartCoroutine(EnemyTurn());
        }
    }

    IEnumerator EnemyTurn()
    {
        state = BattleState.ENEMY_TURN;
        yield return new WaitForSeconds(1.5f); // AI "Thinking" time

        // Simple AI: Pick random card from enemy deck
        CardData aiCard = enemyDeckPool[Random.Range(0, enemyDeckPool.Count)];

        // Visual Feedback for AI
        Debug.Log("Enemy casts: " + aiCard.cardName);

        playerUnit.TakeDamage(aiCard.damageToEnemy);
        enemyUnit.Heal(aiCard.healSelf);

        if (playerUnit.currentHealth <= 0)
        {
            state = BattleState.LOST;
            Debug.Log("GAME OVER");
        }
        else
        {
            PlayerTurn(); // Back to player
        }
    }
}
*/