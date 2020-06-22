using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

public class TimeChamber : MonoBehaviour
{
    public enum TIMELINE { MAGIC, TECH }
    [SerializeField] private int turnsRequired = 4;
    [SerializeField] private Image hourglassFill = null;
    private int turnsRemaining = 4;
    private Unit occupyingUnit = null;

    private void Start()
    {
        this.UpdateTimer(this.turnsRemaining);
    }

    public void BeginTurn()
    {
        if(this.turnsRemaining == 0) 
            this.TimelineChange();
        if(this.occupyingUnit && this.turnsRemaining > 0)
            this.UpdateTimer(this.turnsRemaining - 1);
        else if(turnsRemaining < 4)
            this.UpdateTimer(this.turnsRemaining + 1);
    }

    private void UpdateTimer(int amt)
    {
        this.turnsRemaining = Mathf.Clamp(amt, 0, this.turnsRequired);
        if (this.turnsRemaining == this.turnsRequired)
            this.hourglassFill.fillAmount = 0;
        else
            this.hourglassFill.fillAmount = Mathf.Clamp01((float) (this.turnsRequired - this.turnsRemaining) / this.turnsRequired);
    }

    public void TimelineChange()
    {
        
        this.occupyingUnit.Die();
        //@TODO fade to black and music down
        
        //@TODO have these all inherit an abstract class or interface instead of having multiple loops
        foreach (TimeSwap swap in FindObjectsOfType<TimeSwap>())
            swap.Swap();
        
        this.Relocate();//@TODO 
        this.UpdateTimer(0);
        
        //@TODO fade in and new music
        
    }

    public void Relocate()
    {
    }
    
    public void OnTriggerEnter2D(Collider2D other)
    {
        Unit unit = other.GetComponent<Unit>();
        if (unit)
            this.occupyingUnit = unit;
    }


    public void OnTriggerExit2D(Collider2D other)
    {
        Unit unit = other.GetComponent<Unit>();
        if (unit)
            this.occupyingUnit = null;
    }
}
