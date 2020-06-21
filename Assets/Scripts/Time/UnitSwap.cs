using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSwap : TimeSwap
{
    private bool isPlayer1 = true;
    private Element element = null;
    [Header("Player 1")]
    [SerializeField] private GameObject magicPlayer1 = null;
    [SerializeField] private GameObject techPlayer1 = null;
    [Header("Player 2")]
    [SerializeField] private GameObject magicPlayer2 = null;
    [SerializeField] private GameObject techPlayer2 = null;

    private void Start()
    {
        this.element = GetComponent<Element>();
        this.isPlayer1 = GameManager.GetInstance().IsPlayer1(GetComponent<Unit>().PlayerController);
        if (this.isPlayer1)
        {
            this.magicPlayer1.SetActive(true);
            this.techPlayer1.SetActive(false);
            this.magicPlayer2.SetActive(false);
            this.techPlayer2.SetActive(false);   
        }
        else
        {
            this.magicPlayer1.SetActive(false);
            this.techPlayer1.SetActive(false);
            this.magicPlayer2.SetActive(true);
            this.techPlayer2.SetActive(false);  
        }
    }

    public override void Swap()
    {
        this.SwapElement();
        if (this.isPlayer1)
        {
            this.magicPlayer1.SetActive(this.element.IsMagicElement());
            this.techPlayer1.SetActive(this.element.IsTechElement());
        }
        else
        {
            this.magicPlayer2.SetActive(this.element.IsMagicElement());
            this.techPlayer2.SetActive(this.element.IsTechElement());
        }
    }
    
    private void SwapElement()
    {
        Element.ELEMENT currentElement = this.element.GetElement();
        if (currentElement == Element.ELEMENT.FIRE)
            this.element.SetElement(Element.ELEMENT.PLASMA);
        else if (currentElement == Element.ELEMENT.WATER)
            this.element.SetElement(Element.ELEMENT.ICE);
        else if (currentElement == Element.ELEMENT.GRASS)
            this.element.SetElement(Element.ELEMENT.ELECTRC);
        else if (currentElement == Element.ELEMENT.PLASMA)
            this.element.SetElement(Element.ELEMENT.FIRE);
        else if (currentElement == Element.ELEMENT.ICE)
            this.element.SetElement(Element.ELEMENT.WATER);
        else if (currentElement == Element.ELEMENT.ELECTRC)
            this.element.SetElement(Element.ELEMENT.GRASS);
    }
}
