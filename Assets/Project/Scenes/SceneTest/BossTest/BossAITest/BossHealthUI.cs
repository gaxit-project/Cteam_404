using UnityEngine;
using UnityEngine.UI;

public class BossHealthUI : MonoBehaviour
{
    [Header("HPÉoÅ[ê›íË")]
    [SerializeField] private GameObject healthBarParent;
    [SerializeField] private GameObject healthBarSegmentPrefab;
    private BossHealth bossHealth;

    private void Start()
    {
        bossHealth = GetComponent<BossHealth>();

        if (healthBarParent != null && healthBarSegmentPrefab != null && bossHealth != null)
        {
            for (int i = 0; i < 10; i++)
            {
                GameObject segment = Instantiate(healthBarSegmentPrefab, healthBarParent.transform);
                segment.GetComponent<Image>().color = new Color(1f, 0.647f, 0f);
            }
        }
    }

    private void Update()
    {
        if (healthBarParent != null && bossHealth != null)
        {
            int maxHealth = bossHealth.GetMaxHealth();
            int currentHealth = bossHealth.GetCurrentHealth();
            int segmentsToFill = Mathf.CeilToInt(currentHealth / (float)(maxHealth / 10));

            for (int i = 0; i < 10; i++)
            {
                Image segmentImage = healthBarParent.transform.GetChild(i).GetComponent<Image>();

                if (i < segmentsToFill)
                {
                    segmentImage.color = new Color(1f, 0.647f, 0f);
                }
                else
                {
                    segmentImage.color = Color.black;
                }
            }
        }
    }

    public void DestroyHealthBar()
    {
        if(healthBarParent != null)
        {
            Destroy(healthBarParent);
        }
    }
}
