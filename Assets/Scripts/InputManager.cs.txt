using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(-1)]

public class InputManager : MonoBehaviour
{
    public delegate void StartTouchEvent(Vector2 position, float time);
    public event StartTouchEvent OnStartTouch;
    public delegate void EndTouchEvent(Vector2 position, float time);
    public event EndTouchEvent OnEndTouch;

    private TouchControls TouchControls;
    private static InputManager _instance;

    public static InputManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<InputManager>();

                if (_instance == null)
                {
                    GameObject gameObject = new GameObject("InputManager");
                    _instance = gameObject.AddComponent<InputManager>();
                }
            }

            return _instance;
        }
    }


    private void Awake() {

        TouchControls = new TouchControls();
        
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable() {
        if (TouchControls != null) TouchControls.Enable();
    }

    private void OnDisable() {
        if (TouchControls != null) TouchControls.Disable();
    }

    private void Start () {
        TouchControls.Touch.PrimaryContact.started += ctx => StartTouch(ctx);
        TouchControls.Touch.PrimaryContact.canceled += ctx => EndTouch(ctx);
    }

    private void StartTouch(InputAction.CallbackContext context) {
            Debug.Log ("Touch started ");
            //if no one listening to event then call  
            if (OnStartTouch != null ) OnStartTouch(TouchControls.Touch.PrimaryPostion.ReadValue<Vector2>(), (float)context.startTime);

    }

    private void EndTouch(InputAction.CallbackContext context) {
        Debug.Log ("Touch ended" );
                    //if no one listening to event then call  
        if (OnEndTouch != null ) OnEndTouch(TouchControls.Touch.PrimaryPostion.ReadValue<Vector2>(), (float)context.time);

    }
}
