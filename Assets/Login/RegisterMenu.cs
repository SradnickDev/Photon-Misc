using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class RegisterMenu : MonoBehaviour
{
	[SerializeField] private InputField m_inputFieldName = null;
	[SerializeField] private InputField m_inputFieldPassword = null;
	[SerializeField] private Button m_registerButton = null;

	private const string RegisterUrl = "https://sradnickdev.de/photon/account/register.php";

	private void Start()
	{
		m_registerButton.interactable = true;
		m_registerButton.onClick.AddListener(TryRegister);
	}

	private void TryRegister()
	{
		//check if input is valid
		var hasName = !string.IsNullOrEmpty(m_inputFieldName.text) &&
					  !string.IsNullOrWhiteSpace(m_inputFieldName.text);
		var hasPassword = !string.IsNullOrEmpty(m_inputFieldPassword.text) &&
						  !string.IsNullOrWhiteSpace(m_inputFieldPassword.text);

		//stop if input is not valid
		if (!hasName || !hasPassword)
		{
			Debug.LogWarning("Please enter a valid name or password!");
			return;
		}

		m_registerButton.interactable = false;
		StartAccountCoroutine();
	}

	private void StartAccountCoroutine()
	{
		//send data and wait for a response
		StartCoroutine(SendCreateAccountRequest(m_inputFieldName.text, m_inputFieldPassword.text));
	}

	private IEnumerator SendCreateAccountRequest(string username, string password)
	{
		var form = new WWWForm();
		form.AddField("usernamePost", username);
		form.AddField("passwordPost", password);

		Debug.Log("wait for response");

		using (var www = UnityWebRequest.Post(RegisterUrl, form))
		{
			yield return www.SendWebRequest();

			if (www.isNetworkError || www.isHttpError)
			{
				Debug.LogError(www.error);
			}
			else
			{
				EvaluateResponse(www);
			}
		}

		m_registerButton.interactable = false;
		yield return null;
	}

	private void EvaluateResponse(UnityWebRequest www)
	{
		var body = www.downloadHandler.text;

		switch (body)
		{
			case "01":
				Debug.Log("Account created!");
				break;
			case "00":
				Debug.LogWarning("Account already Exists!");
				break;
			default:
				Debug.LogError($"Account creation failed : {body}");
				break;
		}
	}
}