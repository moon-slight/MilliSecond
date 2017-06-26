using UnityEngine;
using System.Collections;

public class CameraPitchCorrector : MonoBehaviour 
{
	[Tooltip("Smooth factor used for the camera re-orientation.")]
	public float smoothFactor = 10f;
	
	void Update () 
	{
		Vector3 jointDir = transform.rotation * Vector3.up;  // up : 0,1,0
        Vector3 projectedDir = Vector3.ProjectOnPlane(jointDir, Vector3.forward);   // 投影向量到一个平面上,该平面由垂直到该法线的平面定义。 forward : 0,0,1

        Quaternion invPitchRot = Quaternion.FromToRotation(projectedDir, Vector3.up); // 從fromDirection到toDirection創建一個旋轉
		transform.rotation = Quaternion.Slerp(transform.rotation, transform.rotation * invPitchRot, smoothFactor * Time.deltaTime);
	}
}
