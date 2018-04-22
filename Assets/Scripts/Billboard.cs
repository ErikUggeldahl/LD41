// http://answers.unity.com/answers/52690/view.html

using UnityEngine;

public class Billboard : MonoBehaviour
{	
	void Update()
	{
        transform.LookAt(Camera.main.transform.position, -Vector3.up);
    }
}
