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
    [SerializeField] private GameObject attackTile;
    private List<GameObject> attackTiles = new List<GameObject>();
    private Unit AttackTarget = null;
    private bool hasAttacked = false;
    private Unit unit = null;
    [SerializeField] private Seeker seeker;
    private Element element;

    public bool HasAttacked() { return this.hasAttacked; }
    
    private void Start()
    {
        this.unit = GetComponent<Unit>();
        this.element = GetComponent<Element>();
    }

    public void Refresh()
    {
        this.hasAttacked = false;
    }

    public void AttackUnit(Unit unit)
    {
        this.AttackTarget = unit;
        seeker.StartPath(this.transform.position, unit.transform.position, OnFinishAttackPathCalculation);
    }
    
    private void OnFinishAttackPathCalculation(Path p)
    {
        if (p.error)
        {
            Debug.LogError(p.error.ToString());
            return;
        }

        if (p.GetTotalLength() > this.range)
            return;
        
        float dmg = this.attack;
        if (this.element.IsWeakAgainst(this.element.GetElement(), this.AttackTarget.GetElement()))
        {
            dmg *= 0.5f;
        }
        else
        {
            if (this.element.IsStrongAgainst(this.element.GetElement(), this.AttackTarget.GetElement()))
                dmg *= 1.5f;
        }
        
        this.AttackTarget.GetHealth().Damage(dmg);
        this.hasAttacked = true;
        this.unit.Rest();
        this.RemoveAttackTiles();
    }
    
    
    public IEnumerator AttackPath()
    {
        int maxGScore = (this.range + 1) * 1000;
        ConstantPath constPath = ConstantPath.Construct(this.transform.position, maxGScore, null);
        this.seeker.StartPath(constPath);
        yield return StartCoroutine(constPath.WaitForPath());
        List<GraphNode> nodes = constPath.allNodes;
        for (int i = nodes.Count - 1; i >= 0; i--)
        {
            Vector3 pos = (Vector3) nodes[i].position;
            if((this.transform.position - pos).sqrMagnitude < 1f) continue;
            GameObject tile =Instantiate(this.attackTile, pos, Quaternion.identity, this.transform);
            this.attackTiles.Add(tile);
        }
    }
    
    public void RemoveAttackTiles() { this.attackTiles.ForEach(o => Destroy(o)); }
}
