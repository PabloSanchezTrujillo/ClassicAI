using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Necromancer : MonoBehaviour
{
    #region variables

    [SerializeField] private GameObject actionsMenu;
    [SerializeField] private GameObject enemyToAttackText;
    [SerializeField] private GameObject allyToHelpText;

    [Header("Action 1")]
    [SerializeField] private string action1Name;

    [SerializeField] private string action1Description;
    [SerializeField] private GameObject action1;
    [SerializeField] private int action1Damage;
    [SerializeField] private GameObject magicalDamage;

    [Header("Action 2")]
    [SerializeField] private string action2Name;

    [SerializeField] private string action2Description;
    [SerializeField] private GameObject action2;
    [SerializeField] private GameObject reviveParticles;

    [Header("Action 3")]
    [SerializeField] private string action3Name;

    [SerializeField] private string action3Description;
    [SerializeField] private GameObject action3;
    [SerializeField] private int action3Damage;
    [SerializeField] private GameObject deathParticles;

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
            action1Button.onClick.AddListener(Action1);
            action2Button.onClick.RemoveAllListeners();
            action2Button.onClick.AddListener(Action2);
            action3Button.onClick.RemoveAllListeners();
            action3Button.onClick.AddListener(Action3);
        }
    }

    public void Action1()
    {
        actionsMenu.SetActive(false);

        if(!character.isEnemy) {
            foreach(GameObject enemy in character.GetCharactersPool().enemies) {
                if(character.AttackingState == CharacterStates.States.DamageBuffed) {
                    int damageExtra = Mathf.RoundToInt(action1Damage * 0.3f);
                    enemy.GetComponent<Character>().GetDamage(action1Damage + damageExtra);
                    character.AttackingState = CharacterStates.States.Normal;
                }
                else {
                    enemy.GetComponent<Character>().GetDamage(action1Damage);
                }
                Instantiate(magicalDamage, enemy.transform);
            }
        }
        else {
            foreach(GameObject ally in character.GetCharactersPool().allies) {
                if(character.AttackingState == CharacterStates.States.DamageBuffed) {
                    int damageExtra = Mathf.RoundToInt(action1Damage * 0.3f);
                    ally.GetComponent<Character>().GetDamage(action1Damage + damageExtra);
                    character.AttackingState = CharacterStates.States.Normal;
                }
                else {
                    ally.GetComponent<Character>().GetDamage(action1Damage);
                }
                Instantiate(magicalDamage, ally.transform);
            }
        }
        EndTurn();
    }

    public void Action2()
    {
        actionsMenu.SetActive(false);

        if(!character.isEnemy) {
            foreach(GameObject ally in character.GetCharactersPool().allies) {
                if(ally.GetComponent<Character>().GetHealth() <= 0) {
                    ally.GetComponent<Character>().Revive();
                    Instantiate(reviveParticles, ally.transform);
                }
            }
        }
        else {
            foreach(GameObject enemy in character.GetCharactersPool().enemies) {
                if(enemy.GetComponent<Character>().GetHealth() <= 0) {
                    enemy.GetComponent<Character>().Revive();
                    Instantiate(reviveParticles, enemy.transform);
                }
            }
        }
        EndTurn();
    }

    public void Action3()
    {
        actionsMenu.SetActive(false);
        character.DefensiveState = CharacterStates.States.DeathExplosive;
        EndTurn();
    }

    public void DeathExplosion()
    {
        if(!character.isEnemy) {
            foreach(GameObject enemy in character.GetCharactersPool().enemies) {
                enemy.GetComponent<Character>().GetDamage(action3Damage);
                Instantiate(deathParticles, enemy.transform);
            }
        }
        else {
            foreach(GameObject ally in character.GetCharactersPool().allies) {
                ally.GetComponent<Character>().GetDamage(action3Damage);
                Instantiate(deathParticles, ally.transform);
            }
        }
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
        Action1();
    }

    public void FireAction2()
    {
        Action2();
    }

    public void FireAction3()
    {
        Action3();
    }
}