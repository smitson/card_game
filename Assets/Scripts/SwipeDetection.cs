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

	// Drag + inertia
	private bool isDragging = false;
	private Vector2 lastScreenPos;
	private float inertialWorldVelY = 0f; // world units per second
	[SerializeField]
	private float inertiaDamping = 8f; // higher = stops faster

	private static float[] cardYLocations = new float[] {3.0f, 0.0f, -3.0f, -6.0f, -9.0f, -12.0f, -15.0f, -18.0f, -12.0f};


	private void Awake(){
		inputManager = InputManager.Instance;
		m_MainCamera = Camera.main;
	}
	private void OnEnable() {
		inputManager.OnStartTouch += SwipeStart;
		inputManager.OnEndTouch += SwipeEnd;
		        
        solitaire = FindFirstObjectByType<Solitaire>();
	}

	private void OnDisable() {
		inputManager.OnStartTouch -= SwipeStart;
		inputManager.OnEndTouch -= SwipeEnd;
	}

	private void SwipeStart (Vector2 position, float time) {
		startPosition = position;
		startTime = time;
		isDragging = true;
		lastScreenPos = position;
		inertialWorldVelY = 0f;
	}

	private void SwipeEnd (Vector2 position, float time) {
		endPosition = position;
		endTime = time;
		DetectSwipe();
		// compute release velocity for inertia
		if (m_MainCamera != null)
		{
			Vector3 wStart = m_MainCamera.ScreenToWorldPoint(new Vector3(startPosition.x, startPosition.y, m_MainCamera.nearClipPlane));
			Vector3 wEnd = m_MainCamera.ScreenToWorldPoint(new Vector3(endPosition.x, endPosition.y, m_MainCamera.nearClipPlane));
			float worldDeltaY = wEnd.y - wStart.y;
			float dt = Mathf.Max(0.0001f, endTime - startTime);
			inertialWorldVelY = worldDeltaY / dt; // positive when finger moved up
		}
		isDragging = false;
	}

	private void DetectSwipe(){
			
			totalDistance = Vector3.Distance(startPosition,endPosition);
			totalTime = endTime - startTime;

			Debug.Log("distance " + totalDistance + " time  " + totalTime );
			Debug.Log("Camera position " + m_MainCamera.transform.position );

			if ((totalDistance >= minimumDistance) &&
			(totalTime <= maximumTime) && (totalTime > minimumTime))
			{
				Vector3 direction = endPosition - startPosition;
				Vector2 direction2D = new Vector2(direction.x, direction.y).normalized;
				SwipeDirection(direction2D);
			}
	}

	private void SwipeDirection(Vector2 direction) {
		// Use row-based scrolling on Solitaire's card area rather than moving the camera
		if (Vector2.Dot(Vector2.up, direction) > directionThreshold) {
			if (solitaire != null) solitaire.ScrollBy(-1); // scroll up by one row if possible
			Debug.Log("Swipe up");
		} 
		if (Vector2.Dot(Vector2.down, direction) > directionThreshold) {
			if (solitaire != null) solitaire.ScrollBy(1); // scroll down by one row if possible
			Debug.Log("Swipe down");  
		} 
	}

	private void Update()
	{
		// Track dragging using Input System's current touch position
		if (isDragging && m_MainCamera != null)
		{
			Vector2 current = lastScreenPos;
			if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
			{
				current = Touchscreen.current.primaryTouch.position.ReadValue();
			}
			Vector3 wLast = m_MainCamera.ScreenToWorldPoint(new Vector3(lastScreenPos.x, lastScreenPos.y, m_MainCamera.nearClipPlane));
			Vector3 wCur = m_MainCamera.ScreenToWorldPoint(new Vector3(current.x, current.y, m_MainCamera.nearClipPlane));
			float deltaWorldY = wCur.y - wLast.y; // positive if finger moved up
			if (Mathf.Abs(deltaWorldY) > 0.0001f && solitaire != null)
			{
				// Solitaire converts world delta to row steps and clamps
				solitaire.ScrollByWorldDelta(deltaWorldY);
			}
			lastScreenPos = current;
		}
		else if (Mathf.Abs(inertialWorldVelY) > 0.001f && solitaire != null)
		{
			float dy = inertialWorldVelY * Time.deltaTime;
			solitaire.ScrollByWorldDelta(dy);
			// exponential-like decay
			float decay = 1f - Mathf.Exp(-inertiaDamping * Time.deltaTime);
			inertialWorldVelY *= (1f - decay);
			if (Mathf.Abs(inertialWorldVelY) < 0.001f) inertialWorldVelY = 0f;
		}
	}

}
