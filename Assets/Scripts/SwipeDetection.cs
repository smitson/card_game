using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class SwipeDetection : MonoBehaviour 
{
	private Solitaire solitaire;
	[SerializeField]
	private float minimumDistance = .2f;
	[SerializeField]
	private float maximumTime = 1f;

	[SerializeField]
	private float minimumTime = 1f;

	[SerializeField]
	private float directionThreshold = .9f;

	[SerializeField]
	private float swipeUpAmount = .8f;

	[SerializeField]
	private float swipeDownAmount = -1.0f;
	

	private float yLocation;

	private float swipeDown;
	
	
	private Camera m_MainCamera;
	
	private InputManager inputManager;
	private Vector2 startPosition;
	private float startTime;
	private float yLocSwipe;
	private Vector2 endPosition;
	private float endTime;

	private float totalDistance;
	private float totalTime;

	private static float[] cardYLocations = new float[] {3.0f, 0.0f, -3.0f, -6.0f, -9.0f, -12.0f, -15.0f, -18.0f, -12.0f};


	private void Awake(){
		inputManager = InputManager.Instance;
	}
	private void OnEnable() {
		inputManager.OnStartTouch += SwipeStart;
		inputManager.OnEndTouch += SwipeEnd;
		        
        solitaire = FindObjectOfType<Solitaire>();
	}

	private void OnDisable() {
		inputManager.OnStartTouch -= SwipeStart;
		inputManager.OnEndTouch -= SwipeEnd;
	}

	private void SwipeStart (Vector2 position, float time) {
		startPosition = position;
		startTime = time;
	}

	private void SwipeEnd (Vector2 position, float time) {
		endPosition = position;
		endTime = time;
		DetectSwipe();
	}

	private void DetectSwipe(){
			
			totalDistance = Vector3.Distance(startPosition,endPosition);
			totalTime = endTime - startTime;

			Debug.Log("distance " + totalDistance + " time  " + totalTime );
			Debug.Log("Camera position " + Camera.main.transform.position );

			if ((totalDistance >= minimumDistance) &&
			(totalTime <= maximumTime) && (totalTime > minimumTime))
			{
				Vector3 direction = endPosition - startPosition;
				Vector2 direction2D = new Vector2(direction.x, direction.y).normalized;
				SwipeDirection(direction2D);
			}
	}

	private void SwipeDirection(Vector2 direction) {
		m_MainCamera = Camera.main;
		yLocation = Camera.main.transform.position[1]; 

		//TODO work out how much to move camera
		if (Vector2.Dot(Vector2.up, direction) > directionThreshold) {
			Debug.Log("Camera position " + m_MainCamera.transform.position );

			if (yLocation > swipeUpAmount){
				Debug.Log("yloc = " + yLocation + " swipeUpAmount = " + swipeUpAmount);
				m_MainCamera.transform.position = m_MainCamera.transform.position + new Vector3(0, swipeUpAmount, 0);
			} 
			else
			{
				m_MainCamera.transform.position = new Vector3(0f, 0f, -1.0f);
			};

			Debug.Log("Swipe up");
			Debug.Log(solitaire.numRow);	
	        Debug.Log("yAxis = " + yLocation);	
		} 
		if (Vector2.Dot(Vector2.down, direction) > directionThreshold) {
			yLocSwipe = cardYLocations[solitaire.numRow];	
			Debug.Log("Camera position " + m_MainCamera.transform.position );

			Debug.Log("yLoc = " + yLocation + " swipeDownAmount = " + swipeDownAmount);
			
			swipeDown = yLocation + swipeDownAmount;

			Debug.Log(" swideDown = " + swipeDown);

			if (swipeDown > -10 ) { 
			Debug.Log("yloc = " + yLocation + " swipeDownAmount = " + swipeUpAmount);
			m_MainCamera.transform.position = m_MainCamera.transform.position + new Vector3(0, swipeDownAmount, 0);
			} 
			else
			{
				m_MainCamera.transform.position = new Vector3(0f, -10.0f, -1.0f);
			};
			Debug.Log("Swipe down");		
           	Debug.Log("yAxis = " + yLocation);	
		} 
	}

}
