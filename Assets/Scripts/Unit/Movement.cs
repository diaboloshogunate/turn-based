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
    [SerializeField] private GameObject walkTile;
    private List<GameObject> movementTiles = new List<GameObject>();
    private bool hasMoved = false;
    private int currentWaypoint = 0;
    private Path path = null;
    [SerializeField] private Seeker seeker = null;
    private Attack attack = null;

    public bool CanMove() { return this.hasMoved; }

    public int GetDistance() { return (int) this.distance; }
    public void Refresh() { this.hasMoved = false; }
    
    private void Start()
    {
        this.rb = GetComponent<Rigidbody2D>();
        this.attack = GetComponent<Attack>();
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
            this.UpdateGraphs();
            this.rb.velocity = Vector2.zero;
            this.path = null;
            StartCoroutine(this.attack.AttackPath());
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

    public IEnumerator WalkPath()
    {
        int maxGScore = (this.GetDistance() + 1) * 1000;
        ConstantPath constPath = ConstantPath.Construct(this.transform.position, maxGScore, null);
        this.seeker.StartPath(constPath);
        yield return StartCoroutine(constPath.WaitForPath());
        List<GraphNode> nodes = constPath.allNodes;
        for (int i = nodes.Count - 1; i >= 0; i--)
        {
            Vector3 pos = (Vector3) nodes[i].position;
            GameObject tile =Instantiate(this.walkTile, pos, Quaternion.identity, this.transform);
            this.movementTiles.Add(tile);
        }
    }
    
    public void RemoveWalkTiles() { this.movementTiles.ForEach(o => Destroy(o)); }
    
    public void MoveTo(Vector3 target)
    {
        seeker.StartPath(this.transform.position, target, OnFinishPathCalculation);
    }
    
    private void OnFinishPathCalculation(Path p)
    {
        if (p.error)
        {
            Debug.LogError(p.error.ToString());
            return;
        }

        Debug.Log(p.GetTotalLength());
        if (p.GetTotalLength() <= this.distance + 0.2f)// sometimes the distance is off by 0.1
        {
            this.hasMoved = true;
            this.path = p;
            this.currentWaypoint = 0;
            this.RemoveWalkTiles();
        }
    }
}
