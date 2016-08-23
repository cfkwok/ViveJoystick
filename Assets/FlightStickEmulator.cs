using UnityEngine;
using System.Collections;

public class FlightStickEmulator : MonoBehaviour {

    public GameObject character;
    public float speed = 1F;
    public float MAXIMUM_MOVE_DISTANCE = 0.1F;  // The maxmimum units the object will travel per Update() call
    public float POSITION_DIFFERENCE_THRESHOLD = 260.0F;  // Used to account for being near the 0 or 360 rotation mark
    public float TILT_ANGLE_THRESHOLD = 8.0F;  // Amount of units user can tilt before movement is active

    private SteamVR_Controller.Device controller { get { return SteamVR_Controller.Input((int)trackedObj.index); } } 
	private SteamVR_TrackedObject trackedObj;

    private Valve.VR.EVRButtonId gripButton = Valve.VR.EVRButtonId.k_EButton_Grip;
    private bool gripButtonDown = false;

    private Valve.VR.EVRButtonId leftDPad = Valve.VR.EVRButtonId.k_EButton_DPad_Left;
    private bool leftDPadDown = false;

    private Valve.VR.EVRButtonId rightDPad = Valve.VR.EVRButtonId.k_EButton_DPad_Right;
    private bool rightDPadDown = false;

    private Valve.VR.EVRButtonId upDPad = Valve.VR.EVRButtonId.k_EButton_DPad_Up;
    private bool upDPadDown = false;

    private Valve.VR.EVRButtonId downDPad = Valve.VR.EVRButtonId.k_EButton_DPad_Down;
    private bool downDPadDown = false;

    private Vector3 initialRotation;
    private bool updateMovement = false;    

    // Use this for initialization
    void Start () {
		trackedObj = GetComponent<SteamVR_TrackedObject>();      
    }
	
	// Update is called once per frame
	void Update () {
        if (controller == null)
        {
            Debug.Log("Controller not initialized");
            return;
        }

        // Get the controller's rotation differences
        Vector3 currentRotation = trackedObj.transform.eulerAngles;
        float xDifference = currentRotation.x - initialRotation.x;
        float zDifference = currentRotation.z - initialRotation.z;

        // Get absolute value to do logic for tilt threshold
        float xAbsDifference = Mathf.Abs(xDifference);
        float zAbsDifference = Mathf.Abs(zDifference);

        Vector3 objectPosition = character.transform.position;
        float newXPosition = objectPosition.x;
        float newZPosition = objectPosition.z;

        float movementDistance = 0.005F;  // Number set to be multiplied with the difference for new position of character

        gripButtonDown = controller.GetPressDown(gripButton);  // Grip button is to recalibrate the position of the remote
        leftDPadDown = SteamVR_Controller_Extension.GetDPadPress(controller, leftDPad);  
        rightDPadDown = SteamVR_Controller_Extension.GetDPadPress(controller, rightDPad);
        downDPadDown = SteamVR_Controller_Extension.GetDPadPress(controller, downDPad);
        upDPadDown = SteamVR_Controller_Extension.GetDPadPress(controller, upDPad);

        if (gripButtonDown)
        {
            UpdatePosition();
        }     

        if (leftDPadDown)
        {
            character.transform.Rotate(Vector3.down * Time.deltaTime);
        }

        if (rightDPadDown)
        {
            character.transform.Rotate(Vector3.up * Time.deltaTime);
        }

        if (upDPadDown)
        {
            character.transform.Translate(Vector3.up * Time.deltaTime);
        }

        if (downDPadDown)
        {
            character.transform.Translate(Vector3.down * Time.deltaTime);
        }

        // Overflow math needed, ie. x initial position is 350. Moving the controller could make it reset to 0
        if (xAbsDifference > POSITION_DIFFERENCE_THRESHOLD)
        {
            if (currentRotation.x > initialRotation.x)
            {
                xDifference = currentRotation.x - (initialRotation.x + 360);
            } else
            {
                xDifference = initialRotation.x - (currentRotation.x + 360);
            }
            xAbsDifference = Mathf.Abs(xDifference);
        }

        if (zAbsDifference > POSITION_DIFFERENCE_THRESHOLD)
        {            
            if (currentRotation.z > initialRotation.z)
            {
                zDifference = currentRotation.z - (initialRotation.z + 360);
            }
            else
            {
                zDifference = (currentRotation.z + 360) - initialRotation.z;
            }
            zAbsDifference = Mathf.Abs(zDifference);
        }

        // Perform the object movement calculations based on controller movement
        if (xAbsDifference > TILT_ANGLE_THRESHOLD && xAbsDifference < POSITION_DIFFERENCE_THRESHOLD)
        {
            float temp = objectPosition.x;
            newXPosition = objectPosition.x + (xDifference * movementDistance);
            float xDistanceDifference = newXPosition - temp;

            // Limit the "speed" for the movement
            if (xDistanceDifference > MAXIMUM_MOVE_DISTANCE)
            {
                newXPosition = temp + MAXIMUM_MOVE_DISTANCE;
            } else if (xDistanceDifference < -MAXIMUM_MOVE_DISTANCE)
            {
                newXPosition = temp - MAXIMUM_MOVE_DISTANCE;
            }

            updateMovement = true;
        }

        // Perform the object movement calculations based on controller movement
        if (zAbsDifference > TILT_ANGLE_THRESHOLD && zAbsDifference < POSITION_DIFFERENCE_THRESHOLD)
        {
            float temp = objectPosition.z;
            newZPosition = objectPosition.z + (zDifference * movementDistance);
            float zDistanceDifference = newZPosition - temp;

            // Limit the "speed" for the movement
            if (zDistanceDifference > MAXIMUM_MOVE_DISTANCE)
            {
                newZPosition = temp + MAXIMUM_MOVE_DISTANCE;
            }
            else if (zDistanceDifference < -MAXIMUM_MOVE_DISTANCE)
            {
                newZPosition = temp - MAXIMUM_MOVE_DISTANCE;
            }

            updateMovement = true;
        }

        // Move the object with new values
        if (updateMovement)
        {
            Vector3 newObjectPosition = new Vector3(newXPosition, objectPosition.y, newZPosition);
            character.transform.position = Vector3.Lerp(objectPosition, newObjectPosition, speed);
            updateMovement = false;
        }
    }

    // Recalibrate the initial controller position to what it currently is
    void UpdatePosition ()
    {
        initialRotation = trackedObj.transform.eulerAngles;
    }
}
