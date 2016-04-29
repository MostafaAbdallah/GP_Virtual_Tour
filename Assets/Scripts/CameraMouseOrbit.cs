using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CameraMouseOrbit : MonoBehaviour
{

	#region Singleton

	static CameraMouseOrbit instance;

	public static CameraMouseOrbit Instance {
		get {
			if (!instance)
				instance = new GameObject ("CameraMouseOrbit").AddComponent<CameraMouseOrbit> ();

			return instance;
		}
	}

	#endregion

	public Transform target;
	public GameObject YlimitGO;
	public float distance = 1.0f;
	public float xSpeed = 120.0f;
	public float ySpeed = 120.0f;

	public float yMinLimit = 20f;
	public float yMaxLimit = 40f;

	public float xDisLimit = 1f;
	public float yDisLimit = 0.2f;



	public float distanceMin = 2f;
	public float distanceMax = 25f;
	public float camDistanceInX = 2.0f;
	public float camDistanceInY = 1.0f;
	public float smoothTime = 2f;

	public float rotationYAxis = 0.0f;
	public float rotationXAxis = 0.0f;

	public float velocityX = 0.0f;
	public float velocityY = 0.0f;


	private bool rotAroundUp = false;
	private bool rotAroundRight = false;
	private bool moveOnZ = false;
	private bool moveOnY = false;
	private bool moveOnX = false;
	private bool freezeMouseEffect = false;


	private int sign = 1;
	private bool readyToMove = true;
	private bool oneClick = false;
	private bool onLocalView = false;
	private bool manualControl = true;
	private float doubleClickStart;
	private float doubleClickTime = 0.4f;
	private Transform objectToSee;
	private Vector3 newPos;
	private Vector3 lastPos;
	private Quaternion lastRot;
	private float relCameraPosMag = 2;
	private Vector3 InitPostionCam;
	private Vector3 InitPostionTarget;
	private Vector3 targetPos;
	private Quaternion Initrotation;
	bool first = true;
	Vector3 position;
	float smooth = 1.0f;
	float scroll;


    //=================Kinect===============================
    public Text TestText;
   public bool CloseHand = false;
   public float handX = 0.0f;
   public float handY = 0.0f;
    //======================================================
	//GameObject clientUI, mainCamera;

	void Awake ()
	{
		//prevent multiple instances of singleton
		if (instance == null)
			instance = this;
		if (this != instance)
			Destroy (this);

		Application.targetFrameRate = 40;

	}

	void Start ()
	{
		Vector3 angles = transform.eulerAngles;
		//rotationYAxis = angles.y;
		//rotationXAxis = angles.x;

		//distanceMax = distance;


		Quaternion toRotation = Quaternion.Euler (rotationXAxis, rotationYAxis, 0);
		Initrotation = toRotation;

		Vector3 negDistance = new Vector3 (0.0f, 0.0f, -distance);
		Vector3 position = Initrotation * negDistance + target.position;

		transform.rotation = Initrotation;
		transform.position = position;
		InitPostionCam = transform.position;
		InitPostionTarget = target.position;
		targetPos = target.position;
	}


	void Update ()
	{
        if (Input.GetKey(KeyCode.A)) {
            SetHandState();
        }


		if (onLocalView) {
			if (Input.GetMouseButtonDown (1)) {
				if (oneClick == false) {
					oneClick = true;
					doubleClickStart = Time.time;
				} else if ((Time.time - doubleClickStart) < doubleClickTime) {
					readyToMove = false;
					StartCoroutine (backCameraTo ());
					oneClick = false;
				} else {
					oneClick = false;
				}

			}
		}

		if (target && manualControl) {

			if ((Input.GetMouseButton (1) && !freezeMouseEffect) ) {

				velocityX += xSpeed * Input.GetAxis ("Mouse X") * 0.02f;
				velocityY += ySpeed * Input.GetAxis ("Mouse Y") * 0.02f;
			} else if (rotAroundUp) {
				velocityX += xSpeed * 0.2f * sign * 0.02f;
			} else if (RotAroundRight) {
				velocityY += ySpeed * 0.2f * sign * 0.02f;

			}

			if (moveOnZ) {
				distance += (0.02f * sign * 5);
			}

			if (!freezeMouseEffect) {
				scroll = -Input.GetAxis ("Mouse ScrollWheel");
				distance += (scroll * 2);

			}


			if (distance < distanceMin)
				distance = distanceMin;
			else if (distance > distanceMax)
				distance = distanceMax;
			else
				transform.Translate (0, 0, (scroll * -5), Space.Self);

			if (scroll != 0)
				scroll /= 1.2f;

			Quaternion fromRotation = Quaternion.Euler (transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 0);
			Quaternion toRotation = Quaternion.Euler (rotationXAxis, rotationYAxis, 0);
			Quaternion rotation = toRotation;

			Vector3 negDistance = new Vector3 (0.0f, 0.0f, -distance);
			position = rotation * negDistance + target.position;

			transform.rotation = rotation;
			transform.position = new Vector3 (ClampPosX (position.x, -3.40f, 3.4f), ClampPos (position.y, -1.0f, 1.6f), position.z);
			target.rotation = rotation;

			if (first) {
				first = false;

			}
			rotationYAxis += velocityX;
			rotationXAxis -= velocityY;
			rotationXAxis = ClampAngle (rotationXAxis, yMinLimit, yMaxLimit);

			velocityX = Mathf.Lerp (velocityX, 0, Time.deltaTime * smoothTime);
			velocityY = Mathf.Lerp (velocityY, 0, Time.deltaTime * smoothTime);


		}

        //======================== Kinect =========================

        if (CloseHand) {

            velocityX += xSpeed * handX * 0.02f;
            velocityY += ySpeed * handY * 0.02f;

            Quaternion fromRotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 0);
            Quaternion toRotation = Quaternion.Euler(rotationXAxis, rotationYAxis, 0);
            Quaternion rotation = toRotation;

            Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
            position = rotation * negDistance + target.position;

            transform.rotation = rotation;
            transform.position = new Vector3(ClampPosX(position.x, -3.40f, 3.4f), ClampPos(position.y, -1.0f, 1.6f), position.z);
            target.rotation = rotation;

            rotationYAxis += velocityX;
            rotationXAxis -= velocityY;
            rotationXAxis = ClampAngle(rotationXAxis, yMinLimit, yMaxLimit);

            velocityX = Mathf.Lerp(velocityX, 0, Time.deltaTime * smoothTime);
            velocityY = Mathf.Lerp(velocityY, 0, Time.deltaTime * smoothTime);
        }
        //=========================================================


		if (moveOnX) { /*&& (Mathf.Abs (target.position.x - InitPostionTarget.x) < xDisLimit /*|| Mathf.Sign (target.position.x - InitPostionTarget.x) != Mathf.Sign (sign))) {*/
			Debug.Log (target.right.x);
			targetPos = new Vector3 (target.localPosition.x + (sign * camDistanceInX) * target.right.x, target.localPosition.y + (sign * camDistanceInX) * target.right.y, target.localPosition.z + (sign * camDistanceInX) * target.right.z);
		
			// targetPos = target.right * (sign * camDistanceInX);
		} else if (moveOnY && (Mathf.Abs (target.position.y - InitPostionTarget.y) < yDisLimit || Mathf.Sign (target.position.y - InitPostionTarget.y) != Mathf.Sign (sign))) {       // else if (moveOnY && Mathf.Abs(transform.position.y - YlimitGO.transform.position.y) < yDisLimit)
			print ("Mathf.Abs(target.position.y - InitPostionTarget.y) =  " + Mathf.Abs (target.position.y - InitPostionTarget.y));
			targetPos = new Vector3 (target.position.x, target.position.y + (sign * camDistanceInY), target.position.z);
		}
		targetPos = new Vector3 (ClampPosX (targetPos.x, -3.40f, 3.4f), ClampPos (targetPos.y, -1.0f, 1.0f), target.position.z);
		target.localPosition = Vector3.Lerp (target.localPosition, targetPos, Time.deltaTime * smooth);

	}

	public static float ClampAngle (float angle, float min, float max)
	{
		if (angle < -360F)
			angle += 360F;
		if (angle > 360F)
			angle -= 360F;
		return Mathf.Clamp (angle, min, max);
	}

	public static float ClampPos (float Y, float min, float max)
	{
        
		return Mathf.Clamp (Y, min, max);
	}

	public static float ClampPosX (float x, float min, float max)
	{

		return Mathf.Clamp (x, min, max);
	}

	public bool RotAroundRight {
		get { return rotAroundRight; }
		set { rotAroundRight = value; }
	}

	public bool RotAroundUp {
		get { return rotAroundUp; }
		set { rotAroundUp = value; }
	}

	public int Sign {
		get { return sign; }
		set { sign = value; }
	}

	public bool ReadyToMove {
		get { return readyToMove; }
		set { readyToMove = value; }
	}

	public bool OnLocalView {
		get { return onLocalView; }
		set { onLocalView = value; }
	}

	public void MoveCam (GameObject target)
	{
		manualControl = false;
		readyToMove = false;
		objectToSee = target.transform;
		lastPos = transform.position;
		lastRot = transform.rotation;
		print ("lastRot = " + lastRot);
		StartCoroutine (moveCameraTo ());
	}

	public void BackCam ()
	{
		readyToMove = false;
		StartCoroutine (backCameraTo ());
	}

	public void resetCameraPosition ()
	{

		readyToMove = false;
		manualControl = false;
		StartCoroutine (resetCam ());
	}

	public bool MoveOnZ {
		get { return moveOnZ; }
		set { moveOnZ = value; }
	}

	public bool MoveOnY {
		get { return moveOnY; }
		set { moveOnY = value; }
	}

	public bool MoveOnX {
		get { return moveOnX; }
		set { moveOnX = value; }
	}

	public bool FreezeMouseEffect {
		get { return freezeMouseEffect; }
		set { freezeMouseEffect = value; }
	}

	IEnumerator moveCameraTo ()
	{

		Vector3 abovePos = objectToSee.position + Vector3.up * relCameraPosMag;

		// print("Above = " + abovePos);
		//print("(transform.position = " + transform.position);
		while (Vector3.Distance (transform.position, abovePos) > 0.1f) {

			yield return new WaitForSeconds (1 / 60);
			// abovePos = ObjectToSee.position + Vector3.up * relCameraPosMag;

			transform.position = Vector3.Lerp (transform.position, abovePos, smooth * Time.deltaTime);

			transform.LookAt (objectToSee.position, Vector3.up);

		}
		Quaternion targetRotation = Quaternion.LookRotation (-1 * objectToSee.up);
		Debug.Log (Vector3.Angle (transform.rotation.eulerAngles, targetRotation.eulerAngles));
		while (Vector3.Angle (transform.rotation.eulerAngles, targetRotation.eulerAngles) > 0.2f) {
			yield return new WaitForSeconds (1 / 60);
			transform.rotation = Quaternion.Slerp (transform.rotation, targetRotation, Time.deltaTime * 3.0F);
		}

		readyToMove = true;
		onLocalView = true;
		print ("transform.rotation; = " + transform.rotation);
	}

	IEnumerator backCameraTo ()
	{
		print ("Before transform.rotation; = " + transform.rotation);
		//Vector3 Pos = ObjectToSee.position + Vector3.up * relCameraPosMag;
		print ("Vector3.Distance(lastPos, transform.position) = " + Vector3.Distance (lastPos, transform.position));

		while (Vector3.Distance (lastPos, transform.position) > 0.01f) {

			yield return new WaitForSeconds (1 / 60);
			//abovePos = ObjectToSee.position + Vector3.up * relCameraPosMag;

			transform.position = Vector3.Lerp (transform.position, lastPos, smooth * Time.deltaTime);

			// transform.LookAt(ObjectToSee.position, Vector3.up);

			transform.rotation = Quaternion.Slerp (transform.rotation, lastRot, smooth * Time.deltaTime);

		}
		readyToMove = true;
		onLocalView = false;
		manualControl = true;
		print ("After transform.rotation; = " + transform.rotation);

	}

	IEnumerator resetCam ()
	{

		while (Vector3.Distance (InitPostionCam, transform.position) > 0.01f) {

			yield return new WaitForSeconds (1 / 60);
			transform.position = Vector3.Lerp (transform.position, InitPostionCam, smooth * Time.deltaTime);
			targetPos = InitPostionTarget;
			target.position = Vector3.Lerp (target.position, InitPostionTarget, 10 * Time.deltaTime);
			transform.rotation = Quaternion.Lerp (transform.rotation, Initrotation, Time.deltaTime * 3.0F);


		}
		/*while (Vector3.Angle(transform.forward, target.forward) > 0.2f)
        {
            print("Angle = " + Vector3.Angle(transform.forward, target.forward));
            yield return new WaitForSeconds(1 / 60);
            transform.rotation = Quaternion.Lerp(transform.rotation, target.rotation, Time.deltaTime * 3.0F);
        }*/
		// this.transform.LookAt(target);

		readyToMove = true;
		onLocalView = false;
		manualControl = true;
		distance = 1.87f;
		//xSpeed = 60.0f;
		//ySpeed = 60.0f;

		//yMinLimit = 10f;
		//yMaxLimit = 60f;

		//distanceMin = 2f;
		//distanceMax = 25f;
		//camDistanceInX = 2.0f;
		//smoothTime = 2f;

		rotationYAxis = 0.0f;
		rotationXAxis = 12.0f;

		velocityX = 0.0f;
		velocityY = 0.0f;
	}

    //==================================kinect==============================

    public void SetHandCooridnate( float handx, float handy) {
        this.handX = handx;
        this.handY = handy;
        TestText.text = "SetHandCooridnate Called with handx = " + handx + " and handy = " + handy;
    }

    public void SetHandState()
    {
        TestText.text = "SetHandState Called";
        //this.CloseHand = closeHand;

    }
    //=====================================================================
}
