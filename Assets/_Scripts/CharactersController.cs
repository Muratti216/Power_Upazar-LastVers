using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 1. Karakter Tiplerini Tanımlıyoruz
public enum CharacterType { Snowman, Enemy }

public class CharactersController : MonoBehaviour
{
    public string characterName;

    [Header("Karakter Ayarı")]
    // 2. Inspector'dan karakterin tipini seçeceğiz
    public CharacterType type;

    public int maxHealth = 10;
    public int currentHealth = 10;

    [Header("Visuals")]
    public SpriteRenderer characterRenderer;
    // Kardan adam için 5, Enemy için 3 görsel atanacak.
    public Sprite[] damageSprites;

    [Header("UI")]
    public Transform healthBarPanel;
    public GameObject healthUnitPrefab;

    private List<Image> healthIcons = new List<Image>();

    void Start()
    {
        if (currentHealth > maxHealth) currentHealth = maxHealth;

        InitializeHealthBar();
        UpdateVisuals();
    }

    private void InitializeHealthBar()
    {
        foreach (Transform child in healthBarPanel) Destroy(child.gameObject);
        healthIcons.Clear();

        for (int i = 0; i < maxHealth; i++)
        {
            GameObject newIcon = Instantiate(healthUnitPrefab, healthBarPanel);
            Image iconImage = newIcon.GetComponent<Image>();
            healthIcons.Add(iconImage);
        }
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0) currentHealth = 0;

        UpdateVisuals(); // Can değişti, görseli güncelle

        if (currentHealth <= 0) Death();
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        if (currentHealth >= maxHealth) currentHealth = maxHealth;

        UpdateVisuals(); // Can değişti, görseli güncelle
    }

    public void Death()
    {
        Debug.Log(characterName + " oyun dışı kaldı!");
    }

    public float GetHealthAmount()
    {
        return currentHealth;
    }

    private void UpdateVisuals()
    {
        // --- 1. Sağlık Barı UI ---
        for (int i = 0; i < healthIcons.Count; i++)
        {
            healthIcons[i].enabled = (i < currentHealth);
        }

        // --- 2. Karakter Sprite Güncellemesi ---
        if (characterRenderer == null || damageSprites.Length == 0) return;

        // Seçilen karaktere göre farklı mantık uygula:
        if (type == CharacterType.Snowman)
        {
            // --- Kardan Adam Mantığı (5 Görsel) ---
            if (damageSprites.Length < 5) return; // Güvenlik

            if (currentHealth >= 9) characterRenderer.sprite = damageSprites[0];
            else if (currentHealth >= 7) characterRenderer.sprite = damageSprites[1];
            else if (currentHealth >= 5) characterRenderer.sprite = damageSprites[2];
            else if (currentHealth >= 3) characterRenderer.sprite = damageSprites[3];
            else if (currentHealth >= 1) characterRenderer.sprite = damageSprites[4];
        }
        else if (type == CharacterType.Enemy)
        {
            // --- Enemy (Büyücü) Mantığı (3 Görsel) ---
            // İstek: 10-6 arası b2, 5-3 arası b1, 2-1 arası ba
            if (damageSprites.Length < 3) return; // Güvenlik

            // Can 6 ve üzeri (10, 9, 8, 7, 6) -> b2 (Dizideki 0. eleman)
            if (currentHealth >= 6)
            {
                characterRenderer.sprite = damageSprites[0];
            }
            // Can 3 ve üzeri (5, 4, 3) -> b1 (Dizideki 1. eleman)
            else if (currentHealth >= 3)
            {
                characterRenderer.sprite = damageSprites[1];
            }
            // Can 1 ve üzeri (2, 1) -> ba (Dizideki 2. eleman)
            else if (currentHealth >= 1)
            {
                characterRenderer.sprite = damageSprites[2];
            }
        }
    }
}