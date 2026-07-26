using UnityEngine;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour
{
    public int maxHealth = 16;
    public int currentHealth;

    public Image[] heartImages;
    public Sprite[] heartSprites;

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthUI();
    }

    public void Heal(int healAmount)
    {
        currentHealth += healAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthUI();
    }

    private void UpdateHealthUI()
    {
        for (int i = 0; i < heartImages.Length; i++)
        {
            int heartStatus = Mathf.Clamp(currentHealth - (i * 4), 0, 4);
            heartImages[i].sprite = heartSprites[heartStatus];
        }
    }
}
