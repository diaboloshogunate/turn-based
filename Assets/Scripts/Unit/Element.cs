using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Element : MonoBehaviour
{
    [SerializeField] private ELEMENT element;
    public enum ELEMENT {FIRE, WATER, GRASS, PLASMA, ICE, ELECTRC, VOID, NULL}
    
    public ELEMENT GetElement()
    {
        return this.element;
    }

    public void SetElement(ELEMENT element)
    {
        this.element = element;
    }

    public bool IsMagicElement()
    {
        return this.element == ELEMENT.FIRE || this.element == ELEMENT.WATER || this.element == ELEMENT.GRASS || this.element == ELEMENT.VOID;
    }

    public bool IsTechElement()
    {
        return this.element == ELEMENT.PLASMA || this.element == ELEMENT.ICE || this.element == ELEMENT.ELECTRC || this.element == ELEMENT.NULL;
    }
    
    public bool IsStrongAgainst(ELEMENT atk, ELEMENT def)
    {
        if (atk == ELEMENT.FIRE && def == ELEMENT.GRASS) return true;
        if (atk == ELEMENT.WATER && def == ELEMENT.FIRE) return true;
        if (atk == ELEMENT.GRASS && def == ELEMENT.WATER) return true;
        if (atk == ELEMENT.PLASMA && def == ELEMENT.ICE) return true;
        if (atk == ELEMENT.ICE && def == ELEMENT.ELECTRC) return true;
        if (atk == ELEMENT.ELECTRC && def == ELEMENT.PLASMA) return true;

        return false;
    }

    public bool IsWeakAgainst(ELEMENT atk, ELEMENT def)
    {
        return this.IsStrongAgainst(def, atk);
    }
}
