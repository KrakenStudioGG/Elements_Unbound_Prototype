using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellDBManager : MonoBehaviour
{
    public SpellDatabase spellDB;
    public GameObject[] spellList;
    private PlayerController player;
    public int spellIndex;
    public float spellSpeed;
    public float spellDmg;
    public bool moveOverride;
    
    public delegate void MovementEvent(bool isMoveOverride,Vector2 playerPos, Vector2 mousePos, float speed);
    public event MovementEvent moveEvent;

    private void Awake()
    {
        if(!player)
            player = FindObjectOfType<PlayerController>();
        if(!spellDB)
            spellDB = Resources.Load("SpellDatabase") as SpellDatabase;
    }

    private void OnEnable()
    {
        if(player != null)
            PlayerController.castSpellEvent += CastEvent;
    }

    private void OnDisable()
    {
        PlayerController.castSpellEvent -= CastEvent;
    }

    // Start is called before the first frame update
    void Start()
    {
        spellIndex = 0;
        
        CreateSpellPool();
    }

    private void CastEvent(int _spellIndex, Vector2 _playerPos, Vector2 _mousePos)
    {
        spellIndex = _spellIndex - 1;
        spellList[spellIndex].SetActive(true);
        player.spellIsActive = true;
        spellSpeed = spellDB.spells[spellIndex].speed;
        spellDmg = spellDB.spells[spellIndex].damage;
        moveOverride = spellDB.spells[spellIndex].isMovementOverride;
        moveEvent?.Invoke(moveOverride, _playerPos, _mousePos, spellSpeed);
    }
    
    private void CreateSpellPool()
    {
        spellList = new GameObject[spellDB.spells.Length];
        for (int i = 0; i < spellList.Length; i++)
        {
            spellList[i] = Instantiate(spellDB.spells[i].prefab, Vector2.zero, Quaternion.identity) as GameObject;
            spellList[i].SetActive(false);
        }
    }
}
