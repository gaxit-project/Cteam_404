using UnityEngine;
using UnityEngine.UI;

public class BossHealthDisplay : MonoBehaviour, IAdminAction
{
    [SerializeField] private BossHealth bossHealth;
    [SerializeField] private Text healthText;

    private bool isAdminModeActive = false;

    void Start()
    {
        healthText.enabled = false;
    }
    public void ExecuteAction() 
    { 
        isAdminModeActive = !isAdminModeActive;
        healthText.enabled = isAdminModeActive;
    }

    // Update is called once per frame
    void Update()
    {
        if (isAdminModeActive && bossHealth != null)
        {
            UPdateHealthText();

            if (Input.GetMouseButton(0))
            {
                bossHealth.TakeDamage(10);
            }
        }
    }

    private void UPdateHealthText()
    {
        healthText.text = $"Boss HP: {bossHealth.GetCurrentHealth()} / {bossHealth.GetMaxHealth()}";
    }
}
