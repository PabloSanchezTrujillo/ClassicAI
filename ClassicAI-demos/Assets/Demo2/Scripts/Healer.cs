using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Healer : MonoBehaviour
{
    #region variables

    [SerializeField] private GameObject actionsMenu;
    [SerializeField] private GameObject enemyToAttackText;
    [SerializeField] private GameObject allyToHelpText;

    [Header("Action 1")]
    [SerializeField] private string action1Name;

    [SerializeField] private string action1Description;
    [SerializeField] private GameObject action1;
    [SerializeField] private int heals;
    [SerializeField] private GameObject healingParticles;

    [Header("Action 2")]
    [SerializeField] private string action2Name;

    [SerializeField] private string action2Description;
    [SerializeField] private GameObject action2;
    [SerializeField] private int damage;
    [SerializeField] private GameObject damageParticles;

    [Header("Action 3")]
    [SerializeField] private string action3Name;

    [SerializeField] private string action3Description;
    [SerializeField] private GameObject action3;

    private Character character;
    private Text action1TextName;
    private Text action1TextDescription;
    private Button action1Button;
    private Text action2TextName;
    private Text action2TextDescription;
    private Button action2Button;
    private Text action3TextName;
    private Text action3TextDescription;
    private Button action3Button;

    #endregion variables

    private void Awake()
    {
        character = GetComponent<Character>();

        action1TextName = action1.transform.GetChild(0).GetComponent<Text>();
        action1TextDescription = action1.transform.GetChild(1).GetComponent<Text>();
        action1Button = action1.GetComponent<Button>();
        action2TextName = action2.transform.GetChild(0).GetComponent<Text>();
        action2TextDescription = action2.transform.GetChild(1).GetComponent<Text>();
        action2Button = action2.GetComponent<Button>();
        action3TextName = action3.transform.GetChild(0).GetComponent<Text>();
        action3TextDescription = action3.transform.GetChild(1).GetComponent<Text>();
        action3Button = action3.GetComponent<Button>();
    }

    public void SelectCharacter()
    {
        if(!character.isEnemy) {
            actionsMenu.SetActive(true);
            action1TextName.text = action1Name;
            action1TextDescription.text = action1Description;
            action2TextName.text = action2Name;
            action2TextDescription.text = action2Description;
            action3TextName.text = action3Name;
            action3TextDescription.text = action3Description;

            action1Button.onClick.RemoveAllListeners();
            action1Button.onClick.AddListener(() => StartCoroutine(Action1()));
            action2Button.onClick.RemoveAllListeners();
            action2Button.onClick.AddListener(() => StartCoroutine(Action2()));
            action3Button.onClick.RemoveAllListeners();
            action3Button.onClick.AddListener(() => StartCoroutine(Action3()));
        }
    }

    private IEnumerator Action1()
    {
        character.AllySelected = null;
        actionsMenu.SetActive(false);
        allyToHelpText.SetActive(true);

        yield return new WaitUntil(() => character.AllySelected != null);
        character.AllySelected.GetComponent<Character>().HealUp(heals);
        Instantiate(healingParticles, character.AllySelected.transform);
        allyToHelpText.SetActive(false);
    }

    private IEnumerator Action2()
    {
        character.EnemySelected = null;
        actionsMenu.SetActive(false);
        enemyToAttackText.SetActive(true);

        yield return new WaitUntil(() => character.EnemySelected != null);
        if(character.ThirdState == CharacterStates.States.DamageBuffed) {
            int damageExtra = Mathf.RoundToInt(damage * 0.3f);
            character.EnemySelected.GetComponent<Character>().GetDamage(damage + damageExtra);
            character.ThirdState = CharacterStates.States.Normal;
        }
        else {
            character.EnemySelected.GetComponent<Character>().GetDamage(damage);
        }
        Instantiate(damageParticles, character.EnemySelected.transform);
        enemyToAttackText.SetActive(false);
    }

    private IEnumerator Action3()
    {
        character.AllySelected = null;
        actionsMenu.SetActive(false);
        allyToHelpText.SetActive(true);

        yield return new WaitUntil(() => character.AllySelected != null);
        character.AllySelected.GetComponent<Character>().ThirdState = CharacterStates.States.DamageBuffed;
        allyToHelpText.SetActive(false);
    }
}