using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Character : MonoBehaviour
{
    #region variables

    [SerializeField] private int health;
    [SerializeField] private Slider healthBar;
    [SerializeField] private Image healthColor;
    [SerializeField] private Text healthText;
    [SerializeField] private Color[] healthColors;
    [SerializeField] private GameObject actionsMenu;
    [SerializeField] private bool isEnemy;

    #endregion variables

    // Start is called before the first frame update
    private void Start()
    {
        healthBar.value = healthBar.maxValue = health;
        healthColor.color = healthColors[0];
        healthText.text = health.ToString();
    }

    public void UpdateHealth()
    {
        //healthBar.value = health;
        healthText.text = health.ToString();

        if(health >= (healthBar.maxValue * 0.5)) {
            healthColor.color = healthColors[0];
        }
        else if(health > (healthBar.maxValue * 0.15) && health < (healthBar.maxValue * 0.5)) {
            healthColor.color = healthColors[1];
        }
        else {
            healthColor.color = healthColors[2];
        }
    }

    public void SelectCharacter()
    {
        if(!isEnemy) {
            actionsMenu.SetActive(true);
        }
    }

    public void GetDamage(int damage)
    {
        health -= damage;
        healthBar.value = health;
    }
}