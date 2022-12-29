using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class PlayerController : MonoBehaviour
{
    private InputControls inputControls;
    private Rigidbody2D playerRB;
    private Vector2 moveInput;
    private Vector2 lookInput;
    private float fireInput;
    private Vector3 mousePos;
    private Vector3 mousePoint;
    private Camera mainCam;
    
    private const float speed = 3f;
    public bool spellIsActive;
    private Vector2 playerCurrentPos;
    private Vector2 mouseCurrentPos;

    public static Sprite PlayerSprite { get; private set; }
    public static Transform PlayerTransform { get; private set; }
    public int CurrentSpellKey { get; private set; }
    public static int PlayerHealth { get; set; }

    public delegate void CastSpellEvent(int spellKey ,Vector2 playerPos, Vector2 mousePos);
    public static event CastSpellEvent castSpellEvent;
    
    
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
        CurrentSpellKey = 1;
    }

    // Update is called once per frame
    void Update()
    {
        MovePlayer();
        PlayerLook();
        FlipSprite();
        GetCurrentSpellKey();
        if (inputControls.Player.Fire.WasPressedThisFrame() && spellIsActive == false)
        {
            playerCurrentPos = transform.position;
            mouseCurrentPos = mousePoint;
            castSpellEvent?.Invoke(CurrentSpellKey, playerCurrentPos, mouseCurrentPos);
        }
    }

    private void GetCurrentSpellKey()
    {
        KeyCode[] keyCodes =
        {
            KeyCode.Alpha1,
            KeyCode.Alpha2,
            KeyCode.Alpha3,
            KeyCode.Alpha4
        };
        for (int i = 0; i < keyCodes.Length; i++)
        {
            if (Input.GetKeyDown(keyCodes[i]))
            {
                CurrentSpellKey = i + 1;
                Debug.Log($"Current Spell Key is {CurrentSpellKey}");
            }
        }
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
    
    public static void ReduceHealth(int damage)
    {
        PlayerHealth -= damage;
    }
    
}
