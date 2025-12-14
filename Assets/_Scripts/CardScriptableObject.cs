using UnityEngine;

public enum Season { Winter, Spring, Summer, Autumn }

[CreateAssetMenu(fileName = "CardScriptableObject", menuName = "ScriptableObjects/CardScriptableObject")]
public class CardScriptableObject : ScriptableObject
{
    [Header("Visuals")]
    public string cardName;
    public string cardDescription;
    public Sprite cardPicture;

    [Header("Stats")]
    public int damageAmount;
    public int healAmount;
    //public int snowCost;

    [Header("CardAppearChanceLogic")]
    public int baseChance;

    public Season preferredSeason;

    public int GetCardAppearChance(float currentHealthAmount, int currentMonth)
    {
        // Crisis card
        int finalChance = baseChance;
        if (healAmount > 0 && currentHealthAmount <= 3)
        {
            return baseChance * 5;
        }

        Season currentSeason = GetSeasonFromMonth(currentMonth);
        if (preferredSeason == currentSeason)
        {
            finalChance *= 3;
        }
        return finalChance;
    }

    private Season GetSeasonFromMonth(int month)
    {
        // Basit oyun döngüsü için: 
        // 1-3: Kış, 4-6: İlkbahar, 7-9: Yaz, 10-12: Sonbahar
        if (month >= 1 && month <= 3) return Season.Winter;
        if (month >= 4 && month <= 6) return Season.Spring;
        if (month >= 7 && month <= 9) return Season.Summer;
        if (month >= 10 && month <= 12) return Season.Autumn;

        return Season.Winter;
    }

}
