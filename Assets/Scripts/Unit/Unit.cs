using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Pathfinding;
using UnityEngine;
using UnityEngine.UI;

public class Unit : MonoBehaviour
{
    private PlayerController playerController = null;
    private Health health = null;
    private Element element = null;
    private Attack attack = null;
    private Movement movement = null;
    private bool isRested = false;
    
    [Header("Selection")]
    [SerializeField] private GameObject select = null;
    [SerializeField] private Color color = Color.white;
    [SerializeField] private Color usedColor = Color.gray;


    public PlayerController GetPlayer()
    {
        return this.playerController;
    }
    
    public void SetPlayer(PlayerController playerController)
    {
        this.playerController = playerController;
    }
    
    public bool IsPlayer(PlayerController playerController)
    {
        return playerController == this.playerController;
    }

    public Element.ELEMENT GetElement() { return this.element.GetElement(); }

    public Health GetHealth() { return this.health; }

    public Movement GetMovement() { return this.movement; }

    public bool HasMoved() { return this.movement.CanMove(); }
    
    public Attack GetAttack() { return this.attack; }
    
    public bool HasAttacked() { return this.attack.HasAttacked(); }
    
    public bool IsRested(){ return this.isRested; }
    
    private void Start()
    {
        this.attack = GetComponent<Attack>();
        this.movement = GetComponent<Movement>();
        this.element = GetComponent<Element>();
        this.health = GetComponent<Health>();
        this.health.death += this.Die;
    }

    public bool IsSelectable(PlayerController playerController)
    {
        return !this.movement.CanMove() && this.IsPlayer(playerController);
    }

    public void Select()
    {
        this.select.SetActive(true);
        this.gameObject.layer = LayerMask.NameToLayer("SelectedUnit");
        AstarPath.active.UpdateGraphs (new GraphUpdateObject(this.GetComponent<BoxCollider2D>().bounds));
        StartCoroutine(this.movement.WalkPath());
    }
    
    public void Deselect()
    {
        this.select.SetActive(false);
        this.gameObject.layer = LayerMask.NameToLayer("Unit");
        AstarPath.active.UpdateGraphs (new GraphUpdateObject(this.GetComponent<BoxCollider2D>().bounds));
        this.movement.RemoveWalkTiles();
        this.attack.RemoveAttackTiles();
    }

    public void Die()
    {
        this.Destroy();
    }

    private void Destroy()
    {
        GameManager.GetInstance().RemoveUnit(this.playerController, this);
        this.health.death -= this.Die;
        Destroy(this.gameObject);
    }
    
    public void Refresh()
    {
        this.movement.Refresh();
        this.attack.Refresh();
        foreach (SpriteRenderer sprite in GetComponentsInChildren<SpriteRenderer>())
            sprite.color = this.color;
    }
    
    public void Rest()
    {
        foreach (SpriteRenderer sprite in GetComponentsInChildren<SpriteRenderer>())
            sprite.color = this.usedColor;
    }
}
