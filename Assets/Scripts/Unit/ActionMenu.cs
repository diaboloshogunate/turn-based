using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

        Vector3 offset = MouseManager.Instance.TilePosition;
        offset.x += 2;
        offset.y += 1.5f;
        float shownButtons = 0;
        if (attack)
            this.attackButton.transform.position = new Vector3(offset.x, offset.y + shownButtons++ * -this.offsetY, 0);
        if (wait)
            this.waitButton.transform.position = new Vector3(offset.x, offset.y + shownButtons++ * -this.offsetY, 0);
        if (endturn)
            this.endTurnButton.transform.position = new Vector3(offset.x, offset.y + shownButtons++ * -this.offsetY, 0);
        if (cancel)
            this.cancelButton.transform.position = new Vector3(offset.x, offset.y + shownButtons++ * -this.offsetY, 0);
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
