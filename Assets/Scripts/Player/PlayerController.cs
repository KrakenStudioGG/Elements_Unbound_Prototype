using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class PlayerController : MonoBehaviour
{
    
    //private PlayerInput inputs;
    public Transform playerTransform;
    private InputControls inputControls;
    private Rigidbody2D playerRB;
    private Vector2 moveInput;
    private Vector2 lookInput;
    private float fireInput;
    private Vector3 mousePos;
    private Vector3 mousePoint;
    private Camera mainCam;
    
    private const float speed = 3f;
    public GameObject prefab;
    private bool spellIsActive;
    private Vector2 playerCurrentPos;
    private Vector2 mouseCurrentPos;
    
    //public static int PlayerHealth { get; set; }
    public static Sprite PlayerSprite { get; private set; }
    public static Transform PlayerTransform { get; private set; }
    public static int PlayerHealth { get; set; }


    private void OnEnable()
    {
        inputControls = new InputControls();
        inputControls.Player.Enable();
        inputControls.Player.Move.performed += OnMove;
        inputControls.Player.Move.canceled += OnMove;
        inputControls.Player.Look.performed += OnLook;
        inputControls.Player.Look.canceled += OnLook;
    }

    private void OnMove(InputAction.CallbackContext ctx)
    {
        moveInput = ctx.ReadValue<Vector2>();
    }

    private void OnLook(InputAction.CallbackContext ctx)
    {
        lookInput = ctx.ReadValue<Vector2>();
    }

    private void OnFire(InputAction.CallbackContext ctx)
    {
        fireInput = ctx.ReadValue<float>();
    }
    
    private void MovePlayer()
    {
        playerRB.velocity = new Vector2(moveInput.x * speed, moveInput.y * speed);
    }

    private void PlayerLook()
    {
        Cursor.lockState = CursorLockMode.Confined;
        mousePos= Input.mousePosition;
        mousePoint = mainCam.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, mainCam.nearClipPlane));
    }

    private void Awake()
    {
        PlayerSprite = GetComponent<SpriteRenderer>().sprite;
    }

    // Start is called before the first frame update
    void Start()
    {
        playerRB = GetComponent<Rigidbody2D>();
        PlayerTransform = GetComponent<Transform>();
        mainCam = Camera.main;
        PlayerHealth = 100;
    }

    // Update is called once per frame
    void Update()
    {
        MovePlayer();
        PlayerLook();
        FlipSprite();

        if (inputControls.Player.Fire.WasPressedThisFrame() && spellIsActive == false)
        {
            playerCurrentPos = transform.position;
            mouseCurrentPos = mousePoint;
            CastSpell(playerCurrentPos);
        }
        
        //Debug.Log(PlayerHealth);
        
    }

    private void CastSpell(Vector2 player)
    {
        spellIsActive = true;
        var spell = Instantiate(prefab, player, Quaternion.identity);
        StartCoroutine(routine:MoveSpell(spell));
    }

    IEnumerator MoveSpell(GameObject _spell)
    {
        var spellSpeed = 3f;
        _spell.transform.position = playerCurrentPos;

        while (Vector2.Distance(_spell.transform.position, mouseCurrentPos) > 0.1f)
        {
            _spell.transform.position = Vector2.MoveTowards(_spell.transform.position, mouseCurrentPos, spellSpeed * Time.deltaTime);
            RotateSprite(_spell, mouseCurrentPos);
            yield return null;
        }
        Destroy(_spell);
        spellIsActive = false;
    }

    private void FlipSprite()
    {
        var hasXVelocity = Mathf.Abs(playerRB.velocity.x) > Mathf.Epsilon;
        var hasYVelocity = Mathf.Abs(playerRB.velocity.y) > Mathf.Epsilon;
        if (hasXVelocity)
        {
            transform.localScale = new Vector2(Mathf.Sign(playerRB.velocity.x), 1f);
        }
    }

    private void RotateSprite(GameObject _spell, Vector2 mouse)
    {
        Vector3 moveDir = (Vector3)mouse - _spell.transform.position;
        moveDir = moveDir.normalized;
        float angle = Mathf.Atan2(moveDir.y, moveDir.x) * Mathf.Rad2Deg;
        Quaternion rotateVal = Quaternion.AngleAxis(angle, Vector3.forward);
        _spell.transform.rotation = Quaternion.Slerp(_spell.transform.rotation, rotateVal, Time.deltaTime * 100f);
    }

    public static void ReduceHealth(int damage)
    {
        PlayerHealth -= damage;
    }
    
}
