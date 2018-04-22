using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleContinue : MonoBehaviour
{

	void Start()
	{
	}
	
	void Update()
	{
        if (Input.GetKeyUp(KeyCode.Space))
        {
            SceneManager.LoadScene("Level1");
        }
	}
}
