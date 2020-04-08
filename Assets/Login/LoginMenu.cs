using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class LoginMenu : MonoBehaviourPunCallbacks
{
	[SerializeField] private InputField m_inputFieldName = null;
	[SerializeField] private InputField m_inputFieldPassword = null;
	[SerializeField] private Button m_loginButton = null;

	private void Start()
	{
		m_loginButton.interactable = true;
		m_loginButton.onClick.AddListener(TryLogin);
	}

	private void TryLogin()
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

		m_loginButton.interactable = false;
		Authenticate();
	}

	private void Authenticate()
	{
		PhotonNetwork.AuthValues.AuthType = CustomAuthenticationType.Custom;
		PhotonNetwork.AuthValues.AddAuthParameter("name", m_inputFieldName.text);
		PhotonNetwork.AuthValues.AddAuthParameter("password", m_inputFieldPassword.text);

		Connect();
	}

	private void Connect()
	{
		PhotonNetwork.ConnectUsingSettings();
	}

	public override void OnCustomAuthenticationFailed(string debugMessage) { }

	public override void OnCustomAuthenticationResponse(Dictionary<string, object> data) { }
}