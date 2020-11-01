using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Character : MonoBehaviour
{
    #region variables

    public GameObject EnemySelected { get; set; }
    public GameObject AllySelected { get; set; }
    public CharacterStates.States SelfState { get; set; }
    public CharacterStates.States ThirdState { get; set; }
    public bool isEnemy;

    [SerializeField] private int health;
    [SerializeField] private Slider healthBar;
    [SerializeField] private Image healthColor;
    [SerializeField] private Text healthText;
    [SerializeField] private Color[] healthColors;

    private CharactersPool charactersPool;

    #endregion variables

    private void Awake()
    {
        charactersPool = FindObjectOfType<CharactersPool>();
    }

    // Start is called before the first frame update
    private void Start()
    {
        healthBar.value = healthBar.maxValue = health;
        healthColor.color = healthColors[0];
        healthText.text = health.ToString();
        SelfState = CharacterStates.States.Normal;
        ThirdState = CharacterStates.States.Normal;
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

    public void ClickOnCharacter()
    {
        if(isEnemy) {
            foreach(GameObject ally in charactersPool.allies) {
                ally.GetComponent<Character>().EnemySelected = this.gameObject;
            }
        }
        else {
            foreach(GameObject ally in charactersPool.allies) {
                ally.GetComponent<Character>().AllySelected = this.gameObject;
            }
        }
    }

    public void GetDamage(int damage)
    {
        switch(SelfState) {
            case CharacterStates.States.Normal:
                health -= damage;
                break;

            case CharacterStates.States.Shielded:
                health -= (damage / 2);
                break;
        }

        healthBar.value = health;
    }

    public void HealUp(int heal)
    {
        health += heal;
        if(health > 200) {
            health = 200;
        }
        healthBar.value = health;
    }

    public CharactersPool GetCharactersPool()
    {
        return charactersPool;
    }
}