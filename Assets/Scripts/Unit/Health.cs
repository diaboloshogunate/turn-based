using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    [SerializeField] private Image healthBar = null;
    [SerializeField] private float maxHealth = 10f;
    public UnityAction death;
    private float currentHealth = 10f;

    private void UpdateHealth()
    {
        this.healthBar.fillAmount = this.GetHealthNormal();
    }

    public void Damage(float dmg)
    {
        this.currentHealth = Mathf.Clamp(this.currentHealth - dmg, 0, this.maxHealth);
        this.UpdateHealth();
        if (this.currentHealth <= 0)
            this.death.Invoke();
    }

    public float GetHealthNormal()
    {
        return Mathf.Clamp01(this.currentHealth / this.maxHealth);
    }
}
