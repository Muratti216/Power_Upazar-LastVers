using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// to differentiate the effects depending on the card belongs to enemy or player
public enum CharacterType { Snowman, Enemy }

public class CharactersController : MonoBehaviour
{
    public string characterName;
    // to give snowman and sun different sprites ( like they have different card pools )
    public CharacterType type;

    public int maxHealth = 10;
    public int currentHealth = 10;

    [Header("Visuals")]
    public Image characterRenderer;
    public Sprite[] damageSprites;

    [Header("UI")]
    public Transform healthBarPanel;
    public GameObject healthUnitPrefab;

    private List<Image> healthIcons = new List<Image>();

    public bool isInIceMode = false;
    public Color iceModeColor = Color.cyan;


    void Start()
    {
        if (currentHealth > maxHealth) currentHealth = maxHealth;

        InitializeHealthBar();
        UpdateVisuals();
    }


    public void EnterIceMode()
    {
        isInIceMode = true;
        maxHealth = 15;
        currentHealth = 11; // show the limit is surpassed but dont reach to full potential

        characterRenderer.color = iceModeColor;

        characterRenderer.transform.localScale = transform.localScale * 1.2f;

        // create the new health bar now
        InitializeHealthBar();
    }

    private void InitializeHealthBar()
    {
        foreach (Transform child in healthBarPanel)
        {
            Destroy(child.gameObject);
        }
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

        UpdateVisuals(); 

        if (currentHealth <= 0) Death();
    }

    public void Heal(int amount)
    {
        if (type == CharacterType.Snowman && !isInIceMode && (currentHealth + amount) >= 10)
        {
            EnterIceMode(); 
        }

        currentHealth += amount;

        if (currentHealth > maxHealth) currentHealth = maxHealth;

        UpdateVisuals();
    }

    public void Death()
    {
        if (currentHealth > 0) return;

        if (type == CharacterType.Snowman)
        {
            GameManager.Instance.TriggerLose();
        }
        else if (type == CharacterType.Enemy)
        {
            GameManager.Instance.TriggerWin();
        }

    }

    public float GetHealthAmount()
    {
        return currentHealth;
    }

    private void UpdateVisuals()
    {
        for (int i = 0; i < healthIcons.Count; i++)
        {
            if (healthIcons[i] != null)
            {
                // instead of hiding the health icon they are now low opacity / this can be changed temporary
                if (i < currentHealth)
                {
                    healthIcons[i].color = new Color(1f, 1f, 1f, 1f);
                }
                else
                {
                    healthIcons[i].color = new Color(1f, 1f, 1f, 0.2f); 
                }
            }
        }

        if (characterRenderer == null || damageSprites.Length == 0) return;

        if (type == CharacterType.Snowman)
        {
            if (damageSprites.Length < 5) return;

            if (currentHealth >= 9) characterRenderer.sprite = damageSprites[0];
            else if (currentHealth >= 7) characterRenderer.sprite = damageSprites[1];
            else if (currentHealth >= 5) characterRenderer.sprite = damageSprites[2];
            else if (currentHealth >= 3) characterRenderer.sprite = damageSprites[3];
            else if (currentHealth >= 1) characterRenderer.sprite = damageSprites[4];
        }
        else if (type == CharacterType.Enemy)
        {
            if (damageSprites.Length < 3) return;


            if (currentHealth >= 6)
            {
                characterRenderer.sprite = damageSprites[0];
            }
            else if (currentHealth >= 3)
            {
                characterRenderer.sprite = damageSprites[1];
            }
            else if (currentHealth >= 1)
            {
                characterRenderer.sprite = damageSprites[2];
            }
        }
    }
}