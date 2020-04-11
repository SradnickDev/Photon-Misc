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

		//disable interactable button so the user cant spam the login button
		m_loginButton.interactable = false;
		Authenticate();
	}

	private void Authenticate()
	{
		//set the correct data as AuthValues
		PhotonNetwork.AuthValues = new AuthenticationValues
		{
			AuthType = CustomAuthenticationType.Custom,
		};

		PhotonNetwork.AuthValues.AddAuthParameter("usernamePost", m_inputFieldName.text);
		PhotonNetwork.AuthValues.AddAuthParameter("passwordPost", m_inputFieldPassword.text);

		Connect();
	}

	//simply connect 
	private void Connect() => PhotonNetwork.ConnectUsingSettings();

	public override void OnConnectedToMaster()
	{
		//wait for callback to join a lobby
		Debug.Log("Successful Connected and Connected to Master!");
		Debug.Log("Joining Lobby");
		
		PhotonNetwork.JoinLobby();
		//usually, here you can change scene to your lobby scene
	}

	public override void OnCustomAuthenticationFailed(string debugMessage)
	{
		//using callbacks to catch errors while auhtentication
		Debug.LogWarning($"CustomAuthenticationFailed : {debugMessage}");
		m_loginButton.interactable = true;
	}

	public override void OnCustomAuthenticationResponse(Dictionary<string, object> data)
	{
		foreach (var entry in data)
		{
			Debug.LogWarning($"CustomAuthenticationResponse : {entry.Key} {entry.Value}");
		}

		m_loginButton.interactable = true;
	}
}