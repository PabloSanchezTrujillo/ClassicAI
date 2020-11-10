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
    [SerializeField] private GameObject inspirationParticles;

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

        FindObjects();
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

    private void FindObjects()
    {
        GameObject actionsUI = GameObject.FindGameObjectWithTag("ActionsMenu");
        actionsMenu = actionsUI.transform.GetChild(0).gameObject;
        enemyToAttackText = actionsUI.transform.GetChild(1).gameObject;
        allyToHelpText = actionsUI.transform.GetChild(2).gameObject;
        action1 = actionsMenu.transform.GetChild(0).gameObject;
        action2 = actionsMenu.transform.GetChild(1).gameObject;
        action3 = actionsMenu.transform.GetChild(2).gameObject;
    }

    public void SelectCharacter()
    {
        if(!character.isEnemy && character.GetCharactersPool().Turn < 3 && character.CanAttack) {
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

    public IEnumerator Action1()
    {
        character.AllySelected = null;
        actionsMenu.SetActive(false);

        if(!character.isEnemy) {
            allyToHelpText.SetActive(true);
            yield return new WaitUntil(() => character.AllySelected != null);
            allyToHelpText.SetActive(false);
        }
        else {
            // TODO: Quitar el random poniendo la opción (seleccionar al 0 o al 1) en el MCTS
            int randomAlly = Random.Range(0, 2);
            character.AllySelected = character.GetCharactersPool().enemies[randomAlly];
        }
        character.AllySelected.GetComponent<Character>().HealUp(heals);
        Instantiate(healingParticles, character.AllySelected.transform);
        EndTurn();
    }

    public IEnumerator Action2()
    {
        character.EnemySelected = null;
        actionsMenu.SetActive(false);

        if(!character.isEnemy) {
            enemyToAttackText.SetActive(true);
            yield return new WaitUntil(() => character.EnemySelected != null);
            enemyToAttackText.SetActive(false);
        }
        else {
            int randomEnemy = Random.Range(0, 2);
            character.EnemySelected = character.GetCharactersPool().allies[randomEnemy];
        }

        if(character.AttackingState == CharacterStates.States.DamageBuffed) {
            int damageExtra = Mathf.RoundToInt(damage * 0.3f);
            character.EnemySelected.GetComponent<Character>().GetDamage(damage + damageExtra);
            character.AttackingState = CharacterStates.States.Normal;
        }
        else {
            character.EnemySelected.GetComponent<Character>().GetDamage(damage);
        }
        Instantiate(damageParticles, character.EnemySelected.transform);
        EndTurn();
    }

    public IEnumerator Action3()
    {
        character.AllySelected = null;
        actionsMenu.SetActive(false);

        if(!character.isEnemy) {
            allyToHelpText.SetActive(true);
            yield return new WaitUntil(() => character.AllySelected != null);
            allyToHelpText.SetActive(false);
        }
        else {
            int randomAlly = Random.Range(0, 2);
            character.AllySelected = character.GetCharactersPool().enemies[randomAlly];
        }
        character.AllySelected.GetComponent<Character>().AttackingState = CharacterStates.States.DamageBuffed;
        Instantiate(inspirationParticles, character.AllySelected.transform);
        EndTurn();
    }

    private void EndTurn()
    {
        CharactersPool charactersPool = character.GetCharactersPool();
        character.CanAttack = false;
        charactersPool.Turn++;

        if(charactersPool.Turn == 3) {
            print("Enemies turn");
            charactersPool.EnemiesTurn();
        }
        else if(charactersPool.Turn == 5) {
            print("Allies turn");
            charactersPool.AlliesTurn();
        }
    }

    // Actions for NO Monobehaviours
    public void FireAction1()
    {
        StartCoroutine(Action1());
    }

    public void FireAction2()
    {
        StartCoroutine(Action2());
    }

    public void FireAction3()
    {
        StartCoroutine(Action3());
    }
}