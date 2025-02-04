using UnityEngine;
using UnityEngine.UI;

public class BossHealthManager : MonoBehaviour, IAdminAction
{
    [SerializeField] private Text healthText;
    [SerializeField] private int maxHealth = 100;
    private int currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    void Update()
    {
        if (healthText != null)
        {
            healthText.gameObject.SetActive(AdminModeManager.isAdminMode);
        }
    }

    public void ExecuteAction()
    {
        if (AdminModeManager.isAdminMode)
        {
            currentHealth -= 10;
            currentHealth = Mathf.Max(currentHealth, 0);
            UpdateHealthUI();
        }
    }

    public void UpdateHealthUI()
    {
        if (healthText != null)
        {
            healthText.text = "Boss HP: " + currentHealth;
            healthText.gameObject.SetActive(AdminModeManager.isAdminMode);
        }
    }
}
