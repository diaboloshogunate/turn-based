using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class MouseManager : MonoBehaviour
{
    private Actions actions = null;
    private Unit selectedUnit = null;

    [SerializeField] private GameObject cameraTarget = null;
    [SerializeField] private GameObject cursor = null;
    [SerializeField] private LayerMask mask = 0;

    public static MouseManager Instance { get; private set; } = null;
    public bool Locked { get; set; } = false;
    public Vector2 ScreenPosition {get; private set; }
    public Vector3 WorldPosition { get; private set; }
    public Vector3 TilePosition { get; private set; }

    private void Awake()
    {
        if (MouseManager.Instance && MouseManager.Instance != this)
        {
            DestroyImmediate(this.gameObject);
            return;
        }

        MouseManager.Instance = this;
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
    
    void Update()
    {
        this.WorldPosition = Camera.main.ScreenToWorldPoint(this.ScreenPosition); 
        Vector3 targetPosition = this.WorldPosition;
        targetPosition.z = 0;
        this.cameraTarget.transform.position = targetPosition;
        
        if (this.Locked) return;
        this.TilePosition = new Vector3(Mathf.RoundToInt(this.WorldPosition.x), Mathf.RoundToInt(this.WorldPosition.y), 0);
        this.cursor.transform.position = this.TilePosition;

        List<Collider2D> collisions = GetCollisions();
        Collider2D collision = collisions.Find(o => o.GetComponent<Unit>());
        if (collision) UnitDetails.Instance.Show(collision.GetComponent<Unit>());
        else UnitDetails.Instance.Hide();
    }

    private void RegisterInput()
    {
        this.actions.Player.Cursor.performed += this.OnCursorMove;
        this.actions.Player.Primary.performed += this.Primary;
        this.actions.Player.Secondary.performed += this.Secondary;
        this.actions.Player.Enable();
    }
    
    private void UnregisterInput()
    {
        this.actions.Player.Cursor.performed -= this.OnCursorMove;
        this.actions.Player.Primary.performed -= this.Primary;
        this.actions.Player.Secondary.performed -= this.Secondary;
        this.actions.Player.Disable();
    }

    private void OnCursorMove(InputAction.CallbackContext context)
    {
        this.ScreenPosition = context.ReadValue<Vector2>();
        // world/tile position set in update because camera can move while the cursor doesn't
    }
    
    private void Primary(InputAction.CallbackContext context)
    {
        List<Collider2D> collisions = MouseManager.Instance.GetCollisions();
        if (this.selectedUnit)
        {
            this.selectedUnit.OnClick(collisions);
            return;
        }
        Collider2D collision = collisions.Find(col => col.GetComponent<Unit>().IsSelectable(GameManager.GetInstance().Player) == true);
        if (!collision) return;
        this.selectedUnit = collision.GetComponent<Unit>();
        this.selectedUnit.Select();
    }
    
    private void Secondary(InputAction.CallbackContext context)
    {
        if (!this.selectedUnit) return;
        
        this.selectedUnit.Deselect();
        this.ClearSelected();
    }

    public List<Collider2D> GetCollisions()
    {
        return Physics2D.OverlapPointAll(this.TilePosition, this.mask).ToList();
    }

    public void ClearSelected() { this.selectedUnit = null; }
}
