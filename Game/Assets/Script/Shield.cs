using UnityEngine;
using System.Collections;

public class Shield : MonoBehaviour {
    
    protected KinectManager kinectManager;

    private Vector3 leftHandPos = Vector3.zero;    // 左手位置
    private Vector3 leftShoulderPos = Vector3.zero;// 左肩位置
    private float turnSpeed;


    public GameObject AI;
    public GameObject player;
    public GameObject position_for_look;
    public GameObject lefthand;
    public GameObject leftforearm;

    static public bool onshield;

	// Use this for initialization
	void Start () {
        onshield = false;
        turnSpeed = 50f;
        //this.gameObject.transform.rotation = Quaternion.Euler(0, 90, 0);
        //transform.rotation = Quaternion.Euler(90, 0, 0);
        //transform.localRotation = Quaternion.Euler(90, 0, 0);
	}
	
	// Update is called once per frame
	void Update () {
        //Debug.Log(AvatarController.userid + ".........");
        kinectManager = KinectManager.Instance; ;
        leftHandPos = kinectManager.GetJointPosition(AvatarController.userid, (int)KinectInterop.JointType.HandLeft);
        leftShoulderPos = kinectManager.GetJointPosition(AvatarController.userid, (int)KinectInterop.JointType.ShoulderLeft);
        //Debug.Log(leftHandPos + "+++++++++++" + leftShoulderPos);

        //this.gameObject.transform.rotation = Quaternion.Euler(0, 90, 0);
        //transform.rotation = Quaternion.Euler(0, 90, 0);

        //Debug.Log(lefthand.transform.localEulerAngles.z);
        //Debug.Log(leftforearm.transform.localEulerAngles.z);

        if (Input.GetKey("q"))
        {
            transform.Rotate(Vector3.up, -turnSpeed * Time.deltaTime * 1 / Time.timeScale);
        }
        if (Input.GetKey("e"))
        {
            transform.Rotate(Vector3.up, turnSpeed * Time.deltaTime * 1 / Time.timeScale);
        }

        transform.LookAt(position_for_look.transform.position);
        //transform.localRotation = Quaternion.Euler(90, 0, 0);
        //transform.rotation = Quaternion.Euler(90, 0, 0);

        //transform.LookAt(AI.transform.position);
        //transform.localRotation = Quaternion.Euler(90, 0, 0);
        //transform.localRotation = Quaternion.EulerAngles(90, 180, 0);
        //transform.rotation = Quaternion.Euler(0, 180, -lefthand.transform.localEulerAngles.z);
        
        //transform.rotation = Quaternion.Slerp();

        if (leftHandPos.y <= leftShoulderPos.y)
        {
            this.gameObject.SetActive(false);
            onshield = false;
            // Destroy(this.gameObject);
        }
        else if(leftHandPos.y > leftShoulderPos.y)
        {
            this.gameObject.SetActive(true);
            onshield = true;
        }
	}
}
