using UnityEngine;
using System.Collections;

public class camera_lerp : MonoBehaviour {


    private Vector3 headPos = Vector3.zero;    // 頭部
    private Vector3 pre_head = Vector3.zero;
    private Vector3 movevar;                   // 移動量
    private float turnSpeed;                   // 旋轉速度

    public GameObject head;
    public GameObject revise;                  // 放一個object在頭前面，取代修正
	// Use this for initialization
	void Start () {
        movevar = new Vector3(0,0,0.34408f);
        //transform.position = head.transform.position + movevar;
        transform.position = revise.transform.position;
        turnSpeed = 50f;
        pre_head = transform.position;
        headPos = KinectManager.Instance.GetJointPosition(AvatarController.userid, (int)KinectInterop.JointType.Head);
	
        
        //

     
    }
	
	// Update is called once per frame
	void Update () {

        //if (Player.moving)
        //{
        //    transform.position = revise.transform.position;
        //}
        //else
        {
            transform.position = Vector3.Lerp(pre_head, transform.position, 0.2f);
        }
        
        
        //headPos = head.transform.position + movevar;
        headPos = revise.transform.position;   
        //下面這行先註解掉
        //headPos = KinectManager.Instance.GetJointPosition(AvatarController.userid, (int)KinectInterop.JointType.Head);
        pre_head = headPos;


        if (Input.GetKey("q"))
        {
            transform.Rotate(Vector3.up, -turnSpeed * Time.deltaTime * 1 / Time.timeScale);
        }
        if (Input.GetKey("e"))
        {
            transform.Rotate(Vector3.up, turnSpeed * Time.deltaTime * 1 / Time.timeScale);
        }

        //固定攝影機Y軸
        //transform.position = new Vector3(transform.position.x,3.931549f,transform.position.z);
        //transform.position = new Vector3(head.transform.position.x,3.931549f,head.transform.position.z + 0.34408f);
    }
}
