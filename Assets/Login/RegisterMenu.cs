using UnityEngine;
using UnityEngine.UI;

public class RegisterMenu : MonoBehaviour
{
	[SerializeField] private InputField m_inputFieldName = null;
	[SerializeField] private InputField m_inputFieldPassword = null;
	[SerializeField] private Button m_registerButton = null;

	private void Start()
	{
		m_registerButton.interactable = true;
		m_registerButton.onClick.AddListener(TryRegister);
	}

	private void TryRegister()
	{
		var hasName = !string.IsNullOrEmpty(m_inputFieldName.text) &&
					  !string.IsNullOrWhiteSpace(m_inputFieldName.text);
		var hasPassword = !string.IsNullOrEmpty(m_inputFieldPassword.text) &&
						  !string.IsNullOrWhiteSpace(m_inputFieldPassword.text);

		if (!hasName || !hasPassword)
		{
			Debug.LogWarning("Please enter a valid name or password!");
			return;
		}

		m_registerButton.interactable = false;

		SendData();
	}

	private void SendData() { }
}