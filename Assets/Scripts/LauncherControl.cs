using BubblePuzzle.Input;
using UnityEngine;
using UnityEngine.InputSystem;

public class LauncherControl : MonoBehaviour
{
    private PlayerInputControls _controls;
    [SerializeField]
    private Camera _camera;

    private InputAction _look;
    private InputAction _fire;

    private void Awake()
    {
        _controls = new PlayerInputControls();

    }

    private void OnEnable()
    {
        _look = _controls.Player.Look;
        
        _look.Enable();

        _fire = _controls.Player.Fire;
        _fire.performed += OnFire;
        _controls.Player.Enable();

        _controls.Enable();
    }

    private void OnDisable()
    {
        DeactivateControls();
    }

    private void DeactivateControls()
    {
        _look.Disable();
        _fire.Disable();
    }


    // Update is called once per frame
    void Update()
    {
        var lookDir = _camera.ScreenToWorldPoint(Mouse.current.position.ReadValue()) - transform.position;
        //Debug.Log(lookDir);
        float lookAngle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg;
        //Debug.Log(lookAngle);
        transform.rotation = Quaternion.AngleAxis(lookAngle, Vector3.forward);

    }

    private void FixedUpdate()
    {
    }


    public void OnFire(InputAction.CallbackContext context)
    {
        Debug.Log("Fire Pressed");
    }
}
