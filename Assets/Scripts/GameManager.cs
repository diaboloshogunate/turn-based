using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    private static GameManager instance = null;
    private Actions actions = null;

    [Header("Special Objects")] [SerializeField]
    private TimeChamber timeChamber = null;

    [Header("Players")]
    [SerializeField] private PlayerController player1 = null;
    [SerializeField] private PlayerController player2 = null;
    public PlayerController Player { get; private set; } = null;
    private List<Unit> player1Units = new List<Unit>();
    private List<Unit> player2Units = new List<Unit>();

    public static GameManager GetInstance()
    {
        return GameManager.instance;
    }
    
    public void Awake()
    {
        if (GameManager.instance && GameManager.instance != this)
        {
            DestroyImmediate(this.gameObject);
            return;
        }

        GameManager.instance = this;
        this.actions = new Actions();
    }

    private void OnEnable()
    {
        this.RegisterInput();
    }

    private void OnDisable()
    {
        this.UnregisterInput();
    }

    private void RegisterInput()
    {
        this.actions.Player.Exit.performed += this.Exit;
        this.actions.Player.Enable();
    }
    
    private void UnregisterInput()
    {
        this.actions.Player.Exit.performed -= this.Exit;
        this.actions.Player.Disable();
    }

    private void Start()
    {
        this.player1.SpawnUnits();
        this.player2.SpawnUnits();
        this.Player = this.player1;
    }

    public void AddUnit(PlayerController player, Unit unit)
    {
        if(player == this.player1)
            this.player1Units.Add(unit);
        if(player == this.player2)
            this.player2Units.Add(unit);
    }

    public void RemoveUnit(PlayerController player, Unit unit)
    {
        if (player == this.player1)
        {
            this.player1Units.Remove(unit);
            if (this.player1Units.Count == 0)
            {
                TransitionController.GetInstance().Load("GameOver");
            }
        }

        if (player == this.player2)
        {
            this.player2Units.Remove(unit);
            if (this.player2Units.Count == 0)
            {
                TransitionController.GetInstance().Load("GameOver");
            }
        }

    }

    public PlayerController GetOtherPlayer(PlayerController player)
    {
        return this.player1 == player ? this.player2 : this.player1;
    }

    public List<Unit> GetPlayerUnits(PlayerController player)
    {
        return this.player1 == player ? this.player1Units : this.player2Units;
    }

    public void AutoTurnEnd()
    {
        if (!this.GetPlayerUnits(this.Player).Find(x => x.Rested))
            this.EndTurn();
    }

    public void EndTurn()
    {
        this.Player.EndTurn();
        this.Player = this.GetOtherPlayer(this.Player);
        this.Player.BeginTurn();
        this.timeChamber.BeginTurn();
    }
    
    private void Exit(InputAction.CallbackContext context)
    {
        TransitionController.GetInstance().Load("title");
    }

    public bool IsPlayer1(PlayerController player)
    {
        return this.player1 == player;
    }

    public bool isPlayer2(PlayerController player)
    {
        return this.player2 == player;
    }
}
