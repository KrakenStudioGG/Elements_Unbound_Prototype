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
    public SpellDatabase spellDB;
    public GameObject[] spellList;
    protected PlayerController player;
    private SpellDBManager dbManager;
    private Vector2 spellStartPos;
    private Vector2 spellEndPos;
    private int spellIndex;
    private float spellSpeed;

    private void Awake()
    {
        player = FindObjectOfType<PlayerController>();
        dbManager = FindObjectOfType<SpellDBManager>();
    }

    private void OnEnable()
    {
        dbManager.moveEvent += MoveEvent;
    }

    private void OnDisable()
    {
        //PlayerController.castSpellEvent -= CastSpell;
        dbManager.moveEvent -= MoveEvent;
    }

    private void Start()
    {
        Debug.Log($"Current Spell is : {gameObject.name}");
    }

    private void MoveEvent(bool moveOverride, Vector2 _player, Vector2 _mouse, float _speed)
    {
        if (!moveOverride)
        {
            spellSpeed = _speed;
            spellStartPos = _player;
            spellEndPos = _mouse;
            StartCoroutine(routine: MoveSpell());
        }
        
        if (moveOverride)
        {
            StartCoroutine(routine: MoveSpellOverride());
        }
        
    }

    private IEnumerator MoveSpell()
    {
        transform.position = spellStartPos;
        while (Vector2.Distance(transform.position, spellEndPos) > 0.1f)
        {
            transform.position = Vector2.MoveTowards(transform.position, spellEndPos, spellSpeed * Time.deltaTime);
            RotateSprite(gameObject, spellEndPos);
            yield return null;
        }
        gameObject.SetActive(false);
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

    private IEnumerator MoveSpellOverride()
    {
        yield return new WaitForSeconds(3f);
    }
}
