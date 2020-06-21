using System;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private List<Transform> spawnPoints;
    [SerializeField] private GameObject[] spawnableUnits = null;

    public void SpawnUnits()
    {
        this.spawnPoints.ForEach((Transform t) => {
            GameObject obj = Instantiate(this.GetRandomUnit(), t.position, Quaternion.identity);
            Unit unit = obj.GetComponent<Unit>();
            unit.PlayerController = this;
            GameManager.GetInstance().AddUnit(this, unit);
        });
    }

    private GameObject GetRandomUnit()
    {
        int length = this.spawnableUnits.Length;
        int roll = Random.Range(0, length);
        return this.spawnableUnits[roll];
    }

    public void BeginTurn()
    {
        GameManager.GetInstance().GetPlayerUnits(this).ForEach(u => u.Refresh());
    }

    public void EndTurn()
    {
    }
}
