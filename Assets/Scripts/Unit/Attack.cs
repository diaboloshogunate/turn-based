using System;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;
using Random = UnityEngine.Random;

public class Attack : MonoBehaviour
{
    [SerializeField] private float attack = 10;
    [SerializeField] private int range = 1;
    public bool HasAttacked { get; private set; }= false;
    private Unit unit = null;
    [SerializeField] private Seeker seeker;
    private Element element;

    public int GetRange()
    {
        return this.range;
    }
    
    private void Start()
    {
        this.unit = GetComponent<Unit>();
        this.element = GetComponent<Element>();
    }

    public void Refresh()
    {
        this.HasAttacked = false;
    }

    public void AttackUnit(Unit unit)
    {
        float dmg = this.attack;
        if (this.element.IsWeakAgainst(this.element.GetElement(), unit.Element.GetElement()))
            dmg *= 0.5f;
        else if (this.element.IsStrongAgainst(this.element.GetElement(), unit.Element.GetElement()))
            dmg *= 1.5f;
        
        unit.Health.Damage(dmg);
        this.HasAttacked = true;
        this.unit.Rest();
    }
}
