using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class SpellBaseScript : MonoBehaviour
{
    public GameObject[] prefab;
    public GameObject[] spellList;
    private PlayerController player;
    private Vector2 spellStartPos;
    private Vector2 spellEndPos;
    private int prefabIndex;

    private void Awake()
    {
        player = FindObjectOfType<PlayerController>();
    }

    private void OnEnable()
    {
        if (player != null)
            PlayerController.castSpellEvent += CastSpell;
    }

    private void OnDisable()
    {
        PlayerController.castSpellEvent -= CastSpell;
    }

    private void Start()
    {
        CreateSpellPool();
    }

    private void CastSpell(Vector2 playerPos, Vector2 mousePos)
    {
        prefabIndex = player.CurrentSpellKey - 1;
        player.spellIsActive = true;
        spellStartPos = playerPos;
        spellEndPos = mousePos;
        StartCoroutine(routine:MoveSpell());
    }

    IEnumerator MoveSpell()
    {
        spellList[prefabIndex].SetActive(true);
        var spellSpeed = 3f;
        spellList[prefabIndex].transform.position = spellStartPos;
        
        while (Vector2.Distance(spellList[prefabIndex].transform.position, spellEndPos) > 0.1f)
        {
            spellList[prefabIndex].transform.position = Vector2.MoveTowards(spellList[prefabIndex].transform.position, spellEndPos, spellSpeed * Time.deltaTime);
            RotateSprite(spellList[prefabIndex], spellEndPos);
            yield return null;
        }
        spellList[prefabIndex].SetActive(false);
        player.spellIsActive = false;
    }
    
    private void RotateSprite(GameObject _spell, Vector2 mouse)
    {
        Vector3 moveDir = (Vector3)mouse - _spell.transform.position;
        moveDir = moveDir.normalized;
        float angle = Mathf.Atan2(moveDir.y, moveDir.x) * Mathf.Rad2Deg;
        Quaternion rotateVal = Quaternion.AngleAxis(angle, Vector3.forward);
        _spell.transform.rotation = Quaternion.Slerp(_spell.transform.rotation, rotateVal, Time.deltaTime * 100f);
    }

    private void CreateSpellPool()
    {
        spellList = new GameObject[4];
        GameObject spell;
        for (int i = 0; i < prefab.Length; i++)
        {
            spellList[i] = Instantiate(prefab[i], Vector2.zero, Quaternion.identity);
            spellList[i].SetActive(false);
        }
    }
}
