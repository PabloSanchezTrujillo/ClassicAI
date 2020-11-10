using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharactersPool : MonoBehaviour
{
    #region variables

    public int Turn { get; set; }
    public GameObject[] allies;
    public GameObject[] enemies;

    [SerializeField] private Transform[] charactersPosition;
    [SerializeField] private GameObject selectionMenu;
    [SerializeField] private Button knightButton;
    [SerializeField] private Button healerButton;
    [SerializeField] private Button guardButton;
    [SerializeField] private Button necromancerButton;
    [SerializeField] private GameObject knight;
    [SerializeField] private GameObject healer;
    [SerializeField] private GameObject guard;
    [SerializeField] private GameObject necromancer;

    private int selection;
    private Vector3 positionUI;
    private Quaternion rotationUI;

    #endregion variables

    private void Start()
    {
        Turn = 1;
        selection = 0;
        positionUI = new Vector3(0, 0, 0);
        rotationUI = Quaternion.Euler(0, 140, 0);
    }

    public void AlliesTurn()
    {
        Turn = 1;
        foreach(GameObject ally in allies) {
            ally.GetComponent<Character>().CanAttack = true;
        }
    }

    public void EnemiesTurn()
    {
        foreach(GameObject enemy in enemies) {
            enemy.GetComponent<Character>().CanAttack = true;
            // TODO: Crear el estado de cada turno
            enemy.GetComponent<MonteCarloTreeSearch>().RunMonteCarloTreeSearch(null, 500);
        }
    }

    public void SelectKnight()
    {
        GameObject knightObject = Instantiate(knight, charactersPosition[selection].position, charactersPosition[selection].rotation);
        switch(selection) {
            case 0:
                allies[0] = knightObject;
                knightObject.GetComponent<MonteCarloTreeSearch>().enabled = false;
                break;

            case 1:
                allies[1] = knightObject;
                knightObject.GetComponent<MonteCarloTreeSearch>().enabled = false;
                break;

            case 2:
                enemies[0] = knightObject;
                knightObject.GetComponent<Character>().isEnemy = true;
                knightObject.transform.GetChild(2).localRotation = rotationUI;
                break;

            case 3:
                enemies[1] = knightObject;
                knightObject.GetComponent<Character>().isEnemy = true;
                knightObject.transform.GetChild(2).localRotation = rotationUI;
                break;
        }
        knightButton.interactable = false;
        selection++;

        if(selection == 2) {
            selectionMenu.SetActive(false);
            AIChoose();
        }
    }

    public void SelectHealer()
    {
        GameObject healerObject = Instantiate(healer, charactersPosition[selection].position, charactersPosition[selection].rotation);
        switch(selection) {
            case 0:
                allies[0] = healerObject;
                healerObject.GetComponent<MonteCarloTreeSearch>().enabled = false;
                break;

            case 1:
                allies[1] = healerObject;
                healerObject.GetComponent<MonteCarloTreeSearch>().enabled = false;
                break;

            case 2:
                enemies[0] = healerObject;
                healerObject.GetComponent<Character>().isEnemy = true;
                healerObject.transform.GetChild(2).localRotation = rotationUI;
                break;

            case 3:
                enemies[1] = healerObject;
                healerObject.GetComponent<Character>().isEnemy = true;
                healerObject.transform.GetChild(2).localRotation = rotationUI;
                break;
        }
        healerButton.interactable = false;
        selection++;

        if(selection == 2) {
            selectionMenu.SetActive(false);
            AIChoose();
        }
    }

    public void SelectGuard()
    {
        GameObject guardObject = Instantiate(guard, charactersPosition[selection].position, charactersPosition[selection].rotation);
        switch(selection) {
            case 0:
                allies[0] = guardObject;
                guardObject.GetComponent<MonteCarloTreeSearch>().enabled = false;
                break;

            case 1:
                allies[1] = guardObject;
                guardObject.GetComponent<MonteCarloTreeSearch>().enabled = false;
                break;

            case 2:
                enemies[0] = guardObject;
                guardObject.GetComponent<Character>().isEnemy = true;
                guardObject.transform.GetChild(2).localRotation = rotationUI;
                break;

            case 3:
                enemies[1] = guardObject;
                guardObject.GetComponent<Character>().isEnemy = true;
                guardObject.transform.GetChild(2).localRotation = rotationUI;
                break;
        }
        guardButton.interactable = false;
        selection++;

        if(selection == 2) {
            selectionMenu.SetActive(false);
            AIChoose();
        }
    }

    public void SelectNecromancer()
    {
        GameObject necromancerObject = Instantiate(necromancer, charactersPosition[selection].position, charactersPosition[selection].rotation);
        switch(selection) {
            case 0:
                allies[0] = necromancerObject;
                necromancerObject.GetComponent<MonteCarloTreeSearch>().enabled = false;
                break;

            case 1:
                allies[1] = necromancerObject;
                necromancerObject.GetComponent<MonteCarloTreeSearch>().enabled = false;
                break;

            case 2:
                enemies[0] = necromancerObject;
                necromancerObject.GetComponent<Character>().isEnemy = true;
                necromancerObject.transform.GetChild(2).localRotation = rotationUI;
                break;

            case 3:
                enemies[1] = necromancerObject;
                necromancerObject.GetComponent<Character>().isEnemy = true;
                necromancerObject.transform.GetChild(2).localRotation = rotationUI;
                break;
        }
        necromancerButton.interactable = false;
        selection++;

        if(selection == 2) {
            selectionMenu.SetActive(false);
            AIChoose();
        }
    }

    private void AIChoose()
    {
        int previousRandom = -1;
        int random;

        for(int i = 0; i < 2; i++) {
            do {
                random = UnityEngine.Random.Range(0, 4);
            } while(random == previousRandom);

            switch(random) {
                case 0:
                    SelectKnight();
                    break;

                case 1:
                    SelectHealer();
                    break;

                case 2:
                    SelectGuard();
                    break;

                case 3:
                    SelectNecromancer();
                    break;
            }
            previousRandom = random;
        }
    }
}