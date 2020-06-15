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
    
    [Header("Select")]
    [SerializeField] private GameObject cursor = null;
    [SerializeField] private LayerMask cursorCollisions = 0;
    private Vector2 mousePosition = Vector2.zero;
    private Unit selectedUnit = null;
    
    [Header("Players")]
    [SerializeField] private PlayerController player1 = null;
    [SerializeField] private PlayerController player2 = null;
    private PlayerController currentPlayer = null;
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
        this.actions.Player.Cursor.performed += this.Cursor;
        this.actions.Player.Primary.performed += this.Primary;
        this.actions.Player.Exit.performed += this.Exit;
        this.actions.Player.Enable();
    }
    
    private void UnregisterInput()
    {
        this.actions.Player.Cursor.performed -= this.Cursor;
        this.actions.Player.Primary.performed -= this.Primary;
        this.actions.Player.Exit.performed -= this.Exit;
        this.actions.Player.Disable();
    }

    private void Start()
    {
        this.player1.SpawnUnits();
        this.player2.SpawnUnits();
        this.currentPlayer = this.player1;
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

    private void Cursor(InputAction.CallbackContext context)
    {
        this.mousePosition = Camera.main.ScreenToWorldPoint(context.ReadValue<Vector2>());
        Vector3 position = this.mousePosition;
        this.cursor.transform.position = new Vector3(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y), 0);
    }
    
    private void Primary(InputAction.CallbackContext context)
    {
        // select unit
        Collider2D[] collisions = Physics2D.OverlapPointAll(this.cursor.transform.position, this.cursorCollisions);
        foreach (Collider2D collision in collisions)
        {
            Unit unit = collision.GetComponent<Unit>();
            if (unit.IsSelectable(this.currentPlayer))
            {
                if (this.selectedUnit && this.selectedUnit != unit)
                {
                    this.selectedUnit.Deselect();
                    if(this.selectedUnit.GetMovement().CanMove())
                        this.selectedUnit.Rest();
                }
                else if (this.selectedUnit && this.selectedUnit == unit) 
                    this.selectedUnit.Rest();
                
                this.selectedUnit = unit;
                this.selectedUnit.Select();
                return;
            }
        }
        
        // nothing selected
        if (!this.selectedUnit)
            return;
        
        // attack
        if (!this.selectedUnit.GetAttack().HasAttacked())
        {
            foreach (Collider2D collision in collisions)
            {
                Unit unit = collision.GetComponent<Unit>();
                if (unit.IsPlayer(this.GetOtherPlayer(this.currentPlayer)))
                {
                    this.selectedUnit.GetAttack().AttackUnit(unit);
                    this.AutoTurnEnd();
                    return;
                }
            }
        }
        
        // move
        if (!this.selectedUnit.GetMovement().CanMove())
        {
            this.selectedUnit.GetMovement().MoveTo(new Vector3(Mathf.RoundToInt(this.mousePosition.x), Mathf.RoundToInt(this.mousePosition.y), 0));
            return;
        }
        
        // wait
        this.selectedUnit.Deselect();
        this.selectedUnit.Rest();
        this.selectedUnit = null;
        this.AutoTurnEnd();
    }

    private void AutoTurnEnd()
    {
        if (!this.GetPlayerUnits(this.currentPlayer).Find(x => !x.GetMovement().CanMove()))
            this.EndTurn();
    }

    public void EndTurn()
    {
        this.currentPlayer.EndTurn();
        this.currentPlayer = this.GetOtherPlayer(this.currentPlayer);
        this.currentPlayer.BeginTurn();
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
