using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MessageDisplay : MonoBehaviour
{
    [SerializeField]
    Text uiText;

	void Start()
	{
        uiText.text = string.Empty;
	}

    public void DisplayIndefiniteMessage(string text)
    {
        StopCoroutine("_DisplayTimedMessage");
        uiText.text = text;
    }
	
	public void DisplayTimedMessage(string text, float duration)
    {
        StopCoroutine("_DisplayTimedMessage");
        StartCoroutine(_DisplayTimedMessage(text, duration));
    }

    IEnumerator _DisplayTimedMessage(string text, float duration)
    {
        uiText.text = text;

        yield return new WaitForSeconds(duration);

        uiText.text = string.Empty;
    }
}
