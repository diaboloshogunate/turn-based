using System;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;

public class Movement : MonoBehaviour
{
    private Rigidbody2D rb = null;
    [SerializeField] private float speed = 2f;
    [SerializeField] private float distance = 4f;
    public bool HasMoved { get; private set; }= false;
    private int currentWaypoint = 0;
    private Unit attacking = null;
    private int stopShort = 0;
    private Path path = null;
    [SerializeField] private Seeker seeker = null;
    private Unit unit = null;

    public int GetDistance() { return (int) this.distance; }
    public void Refresh() { this.HasMoved = false; }
    
    private void Start()
    {
        this.unit = GetComponent<Unit>();
        this.rb = GetComponent<Rigidbody2D>();
        this.UpdateGraphs();
    }
    
    private void FixedUpdate()
    {
        if (this.path == null)
        {
            return;
        }

        if (this.currentWaypoint >= this.path.vectorPath.Count)
        {
            this.gameObject.layer = LayerMask.NameToLayer("Unit");
            AstarPath.active.UpdateGraphs (new GraphUpdateObject(this.GetComponent<BoxCollider2D>().bounds));
            if (this.attacking)
            {
                this.unit.Attack.AttackUnit(this.attacking);
                if (this.stopShort > 0)
                {
                    this.attacking.gameObject.layer = LayerMask.NameToLayer("Unit");
                    AstarPath.active.UpdateGraphs(new GraphUpdateObject(this.attacking.gameObject.GetComponent<BoxCollider2D>().bounds));
                }
            } 
            else this.unit.Rest();
            this.UpdateGraphs();
            this.rb.velocity = Vector2.zero;
            this.path = null;
            return;
        }

        Vector2 direction = (path.vectorPath[this.currentWaypoint] - this.transform.position).normalized;
        this.rb.velocity = direction * this.speed;

        if (Vector2.Distance(this.transform.position , path.vectorPath[this.currentWaypoint]) < 0.1f)
        {
            this.currentWaypoint++;
        }
    }

    private void UpdateGraphs()
    {
        AstarPath.active.Scan();
    }
    
    public void MoveTo(Vector3 target)
    {
        this.attacking = null;
        this.stopShort = 0;
        this.gameObject.layer = LayerMask.NameToLayer("SelectedUnit");
        AstarPath.active.UpdateGraphs (new GraphUpdateObject(this.GetComponent<BoxCollider2D>().bounds));
        seeker.StartPath(this.transform.position, target, OnFinishPathCalculation);
    }

    public void MoveThenAttack(Vector3 target, Unit unit, int stopShort = 0)
    {
        this.attacking = unit;
        this.stopShort = stopShort;
        this.gameObject.layer = LayerMask.NameToLayer("SelectedUnit");
        AstarPath.active.UpdateGraphs (new GraphUpdateObject(this.GetComponent<BoxCollider2D>().bounds));
        if (stopShort > 0)
        {
            unit.gameObject.layer = LayerMask.NameToLayer("SelectedUnit");
            AstarPath.active.UpdateGraphs (new GraphUpdateObject(unit.gameObject.GetComponent<BoxCollider2D>().bounds));
        }
        seeker.StartPath(this.transform.position, target, OnFinishPathCalculation);
    }
    
    private void OnFinishPathCalculation(Path p)
    {
        this.unit.Deselect();
        
        if (p.error)
        {
            Debug.LogError(p.error.ToString());
            return;
        }

        this.HasMoved = true;
        this.path = p;
        this.currentWaypoint = 0;

        if (this.stopShort > 0)
            this.path.vectorPath.RemoveRange(this.path.vectorPath.Count - this.stopShort, this.stopShort);
    }
}
