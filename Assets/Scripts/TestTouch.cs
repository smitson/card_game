using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TestTouch : MonoBehaviour
{
    private InputManager inputManager;
    private Camera cameraMain;
    private Solitaire solitaire;

    private void Awake() {
       inputManager = InputManager.Instance;
       cameraMain = Camera.main;
    }

    private void OnEnable() {
        inputManager.OnStartTouch += Move;
    }

    private void OnDisable() {
        inputManager.OnStartTouch -= Move;
    }

    public void Move (Vector2 screenPosition, float time) {
        //TODO void deck()    
        // deck click actions

        Debug.Log("pressed");
       //TODO Vector3 screenCoordinates = new Vector3(screenPosition.x, screenPosition.y, cameraMain.nearClipPlane);
    }
}
