using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnitDetails : MonoBehaviour
{
    [SerializeField] private Image profile = null;
    [SerializeField] private Image healthFill = null;
    [SerializeField] private TMP_Text attack = null;
    [SerializeField] private TMP_Text range = null;
    [SerializeField] private TMP_Text element = null;
    [SerializeField] private TMP_Text move = null;
    
    public static UnitDetails Instance { get; private set; } = null;
    
    private void Awake()
    {
        if (UnitDetails.Instance && UnitDetails.Instance != this)
        {
            DestroyImmediate(this.gameObject);
            return;
        }

        UnitDetails.Instance = this;
    }

    public void Show(Unit unit)
    {
        string elementText = "";
        if(unit.Element.GetElement() == Element.ELEMENT.ICE)
            elementText = "ICE";
        else if(unit.Element.GetElement() == Element.ELEMENT.FIRE)
            elementText = "FIRE";
        else if(unit.Element.GetElement() == Element.ELEMENT.GRASS)
            elementText = "GRASS";
        else if(unit.Element.GetElement() == Element.ELEMENT.WATER)
            elementText = "WATER";
        else if(unit.Element.GetElement() == Element.ELEMENT.PLASMA)
            elementText = "PLASMA";
        else if(unit.Element.GetElement() == Element.ELEMENT.ELECTRC)
            elementText = "ELECTRIC";
        
        this.healthFill.fillAmount = unit.Health.GetHealthNormal();
        this.attack.text = unit.Attack.GetAttack().ToString();
        this.range.text = unit.Attack.GetRange().ToString();
        this.move.text = unit.Movement.GetDistance().ToString();
        this.element.text = elementText;
        this.profile.sprite = unit.GetComponent<UnitSwap>().GetCurrentSprite();
        this.gameObject.SetActive(true);
    }

    public void Hide()
    {
        this.gameObject.SetActive(false);
    }
}
