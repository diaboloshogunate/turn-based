using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Pathfinding;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Unit : MonoBehaviour
{
    public Health Health { get; private set; } = null;
    public Element Element { get; private set; } = null;
    public Attack Attack { get; private set; } = null;
    public Movement Movement { get; private set; } = null;
    public PlayerController PlayerController { get; set; } = null;
    public bool Rested { get; private set; } = false;
    
    [SerializeField] private ActionMenu menu = null;
    
    // target
    private Vector3 adjacentTargetPosition;
    private bool isSelectingTarget = false;
    private Unit target = null;

    [Header("Color")]
    [SerializeField] private Color color = Color.white;
    [SerializeField] private Color usedColor = Color.gray;
    
    [Header("Path finding")]
    [SerializeField] private GameObject walkTile = null;
    [SerializeField] private GameObject attackTile = null;
    [SerializeField] public Seeker WalkSeeker = null;
    [SerializeField] public Seeker AttackSeeker = null;
    [SerializeField] private LayerMask mask;
    private Dictionary<Vector2, GameObject> walkTiles = new Dictionary<Vector2, GameObject>();
    private Dictionary<Vector2, GameObject> attackTiles = new Dictionary<Vector2, GameObject>();

    public bool IsPlayer(PlayerController playerController)
    {
        return playerController == this.PlayerController;
    }

    private void Start()
    {
        this.Attack = GetComponent<Attack>();
        this.Movement = GetComponent<Movement>();
        this.Element = GetComponent<Element>();
        this.Health = GetComponent<Health>();
        this.Health.death += this.Die;
    }

    public bool IsSelectable(PlayerController playerController)
    {
        return this.IsPlayer(playerController);
    }

    public void Select()
    {
        if (this.Rested) return;
        StartCoroutine(this.DisplayWalkPath());
    }
    
    public void Deselect()
    {
        this.menu.Hide();
        this.isSelectingTarget = false;
        this.ClearTiles();
        MouseManager.Instance.ClearSelected();
    }

    public void Die()
    {
        this.Destroy();
    }

    private void Destroy()
    {
        GameManager.GetInstance().RemoveUnit(this.PlayerController, this);
        this.Health.death -= this.Die;
        Destroy(this.gameObject);
    }

    public IEnumerator DisplayWalkPath()
    {
        //change layer an update graphs so the current space does not count
        this.gameObject.layer = LayerMask.NameToLayer("SelectedUnit");
        AstarPath.active.UpdateGraphs (new GraphUpdateObject(this.GetComponent<BoxCollider2D>().bounds));
        
        // find path
        ConstantPath constPath = ConstantPath.Construct(this.transform.position, (this.Movement.GetDistance() + 1) * 1000, null);
        this.WalkSeeker.StartPath(constPath);
        yield return StartCoroutine(constPath.WaitForPath());
        
        // display tiles
        List<Vector3> walkTilePositions = new List<Vector3>();
        List<GraphNode> nodes = constPath.allNodes;
        for (int i = nodes.Count - 1; i >= 0; i--)
        {
            GraphNode node = nodes[i];
            Vector3 position = (Vector3) node.position;
            position.x = Mathf.Round(position.x);
            position.y = Mathf.Round(position.y);
            position.z = 0;
            this.walkTiles.Add(position, Instantiate(this.walkTile, position, Quaternion.identity, this.transform));
        }

        StartCoroutine(this.DisplayAttackPath());
    }
    
    public IEnumerator DisplayAttackPath()
    {
        foreach (Vector3 pos in this.walkTiles.Keys.ToArray())
        {
            // find path
            ConstantPath constPath = ConstantPath.Construct(pos, (this.Attack.GetRange() + 1) * 1000, null);
            this.AttackSeeker.StartPath(constPath);
            yield return StartCoroutine(constPath.WaitForPath());
        
            // display tiles
            List<GraphNode> nodes = constPath.allNodes;
            for (int i = nodes.Count - 1; i >= 0; i--)
            {
                GraphNode node = nodes[i];
                Vector3 position = (Vector3) node.position;
                position.x = Mathf.Round(position.x);
                position.y = Mathf.Round(position.y);
                position.z = 0;
                if(!this.walkTiles.ContainsKey(position) && !this.attackTiles.ContainsKey(position))
                    this.attackTiles.Add(position, Instantiate(this.attackTile, position, Quaternion.identity, this.transform));
            }
        }
    }

    public bool HasAdjacentAttackTiles(Vector2 position)
    {
        foreach(Vector2 direction in new []{Vector2.up, Vector2.right, Vector2.down, Vector2.left})
            if (this.attackTiles.ContainsKey(position + direction) && Physics2D.OverlapCircleAll(position + direction, 0.8f, this.mask).Length > 0)
                return true;
        return false;
    }
    
    public void DisplayAdjacentAttackTiles(Vector2 position)
    {
        foreach(Vector2 direction in new []{Vector2.up, Vector2.right, Vector2.down, Vector2.left})
            if(this.attackTiles.ContainsKey(position + direction) && Physics2D.OverlapCircleAll(position + direction, 0.8f, this.mask).Length > 0)
                this.attackTiles[position + direction].SetActive(true);
    }

    public void ClearTiles()
    {
        foreach (var keyValuePair in this.attackTiles) Destroy(keyValuePair.Value);
        foreach (var keyValuePair in this.walkTiles) Destroy(keyValuePair.Value);
        this.attackTiles.Clear();
        this.walkTiles.Clear();
        this.gameObject.layer = LayerMask.NameToLayer("Unit");
        AstarPath.active.UpdateGraphs (new GraphUpdateObject(this.GetComponent<BoxCollider2D>().bounds));
    }
    
    private void HideTiles()
    {
        foreach (var keyValuePair in this.attackTiles) keyValuePair.Value.SetActive(false);
        foreach (var keyValuePair in this.walkTiles) keyValuePair.Value.SetActive(false);
    }

    public void OnClick(List<Collider2D> collisions)
    {
        if (this.Rested) return;
        if (this.menu.IsActive()) return;
        Vector2 position = MouseManager.Instance.TilePosition;
        
        if (this.isSelectingTarget)
        {
            if (collisions.Count > 0)
            {
                Unit unit = collisions.Find(o => o.GetComponent<Unit>()).GetComponent<Unit>();
                if (unit)
                {
                    this.HideTiles();
                    MouseManager.Instance.Locked = true;
                    this.Movement.MoveThenAttack(this.adjacentTargetPosition, unit);
                    return;
                }
            }
                
            this.Deselect();
        }
        else
        {
            if (this.walkTiles.ContainsKey(position))
            {
                this.HideTiles();
                this.menu.Show(this.HasAdjacentAttackTiles(position), true, true, true);
            }
            else if (this.attackTiles.ContainsKey(position))
            {
                Collider2D collision = collisions.Find(o => o.GetComponent<Unit>());
                if (collision)
                {
                    this.target = collision.GetComponent<Unit>();
                    this.HideTiles();
                    this.menu.Show(true, false, true, false);
                }
            }
            else
                this.Deselect();
        }
    }
    
    private void OnFinishPathCalculation(Path p)
    {
        if (p.error)
        {
            Debug.LogError(p.error.ToString());
            return;
        }
        Debug.Log(p.GetTotalLength());
    }

    public void OnAttackButton()
    {
        this.adjacentTargetPosition = MouseManager.Instance.TilePosition;
        this.menu.Hide();
        
        if (this.target)
        {
            this.HideTiles();
            MouseManager.Instance.Locked = true;
            this.Movement.MoveThenAttack(this.adjacentTargetPosition, this.target, 1);
            this.target = null;
            return;
        }
        
        this.isSelectingTarget = true;
        DisplayAdjacentAttackTiles((Vector2) MouseManager.Instance.TilePosition);
    }

    public void OnCancelButton()
    {
        this.Deselect();
    }
    
    public void OnWaitButton()
    {
        this.ClearTiles();
        if (MouseManager.Instance.TilePosition == this.transform.position)
        {
            this.Rest();
            return;
        }
        this.Movement.MoveTo(MouseManager.Instance.TilePosition);
    }

    public void OnEndTurnButton()
    {
        this.OnWaitButton();
        GameManager.GetInstance().EndTurn();
    }

    public void Rest()
    {
        this.Deselect();
        this.Rested = true;
        GameManager.GetInstance().AutoTurnEnd();
        foreach (SpriteRenderer sprite in GetComponentsInChildren<SpriteRenderer>())
            sprite.color = this.usedColor;
    }
    
    public void Refresh()
    {
        this.Rested = false;
        this.Movement.Refresh();
        this.Attack.Refresh();
        foreach (SpriteRenderer sprite in GetComponentsInChildren<SpriteRenderer>())
            sprite.color = this.color;
    }
}
