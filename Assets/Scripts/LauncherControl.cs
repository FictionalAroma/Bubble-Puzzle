using BubblePuzzle.Input;
using UnityEngine;
using UnityEngine.InputSystem;

public class LauncherControl : MonoBehaviour
{
    private PlayerInputControls _controls;
    [SerializeField] private Camera cameraToPointOn;

    [SerializeField] private BounceBubble objectToShoot;

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
        var lookAngle = GetAngleFromLaunchToMouse();
        //Debug.Log(lookAngle);
        transform.rotation = Quaternion.AngleAxis(lookAngle, Vector3.forward);
    }

    private float GetAngleFromLaunchToMouse()
    {
        var lookDir = cameraToPointOn.ScreenToWorldPoint(Mouse.current.position.ReadValue()) - transform.position;
        //Debug.Log(lookDir);
        float lookAngle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 90f;
        return lookAngle;
    }


    public void OnFire(InputAction.CallbackContext context)
    {
        var newBubble = Instantiate(objectToShoot, transform.position, Quaternion.identity);
        newBubble.Launch(cameraToPointOn.ScreenToWorldPoint(Mouse.current.position.ReadValue()) - transform.position);
    }
}
