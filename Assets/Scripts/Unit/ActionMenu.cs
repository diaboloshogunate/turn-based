using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionMenu : MonoBehaviour
{
    [SerializeField] private float offsetY = 1f;
    [SerializeField] private GameObject attackButton = null;
    [SerializeField] private GameObject waitButton = null;
    [SerializeField] private GameObject endTurnButton = null;
    [SerializeField] private GameObject cancelButton = null;

    public void Show(bool attack, bool wait, bool cancel, bool endturn)
    {
        MouseManager.Instance.Locked = true;
        
        this.gameObject.SetActive(true);
        this.attackButton.SetActive(attack);
        this.waitButton.SetActive(wait);
        this.endTurnButton.SetActive(endturn);
        this.cancelButton.SetActive(cancel);

        float shownButtons = 0;
        if (attack)
            this.attackButton.transform.localPosition = new Vector3(0, shownButtons++ * -this.offsetY, 0);
        if (wait)
            this.waitButton.transform.localPosition = new Vector3(0, shownButtons++ * -this.offsetY, 0);
        if (endturn)
            this.endTurnButton.transform.localPosition = new Vector3(0, shownButtons++ * -this.offsetY, 0);
        if (cancel)
            this.cancelButton.transform.localPosition = new Vector3(0, shownButtons++ * -this.offsetY, 0);
    }

    public void Hide()
    {
        MouseManager.Instance.Locked = false;
        this.gameObject.SetActive(false);
    }

    public bool IsActive()
    {
        return gameObject.activeSelf;
    }
}
