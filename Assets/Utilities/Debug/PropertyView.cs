using System.Collections;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

namespace Utilities.Debug
{
	/// <summary>
	/// Contains no relevant Game code.
	/// Only purposes is to gain some information to debug the Client.
	/// </summary>
	public class PropertyView : MonoBehaviour
	{
		[SerializeField] private Text m_content = null;
		[SerializeField] private GameObject m_view = null;

		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.KeypadMinus) || Input.GetKeyDown(KeyCode.Minus))
			{
				m_view.SetActive(!m_view.activeInHierarchy);
			}
		}

		private void OnGUI()
		{
			if (!m_view.activeInHierarchy) return;
			var text = "";
			text += $"Region : {PhotonNetwork.CloudRegion}\n";
			text += $"State : {PhotonNetwork.NetworkClientState}\n";
			text += $"In Room : {PhotonNetwork.InRoom}\n";
			text += $"Room Name : {(PhotonNetwork.CurrentRoom != null ? PhotonNetwork.CurrentRoom.Name : null )}\n";
			text += $"Rooms : {PhotonNetwork.CountOfRooms}\n";
			text += $"In Lobby : {PhotonNetwork.InLobby}\n";
			text += $"Is MasterClient : {PhotonNetwork.LocalPlayer.IsMasterClient}\n";

			if (PhotonNetwork.InLobby)
			{
				text +=
					$"Lobby Infos: {PhotonNetwork.CurrentLobby.Name ?? ""} , {PhotonNetwork.CurrentLobby.Type}\n";
			}

			text += $"Connected and Ready : {PhotonNetwork.IsConnectedAndReady}\n";
			text += $"Game Version: {PhotonNetwork.GameVersion}\n";
			text += $"App Version: {PhotonNetwork.PhotonServerSettings.AppSettings.AppVersion}\n";
			text += $"Count of all Players : {PhotonNetwork.CountOfPlayersInRooms}\n";

			if (PhotonNetwork.LocalPlayer != null && PhotonNetwork.InRoom)
			{
				text += $"###Player Properties###\n";
				foreach (var property in PhotonNetwork.LocalPlayer.CustomProperties)
				{
					var value = property.Value;
					if (value is IEnumerable enumerable)
					{
						value = "";
						foreach (var t in enumerable)
						{
							value += t + " , ";
						}
					}

					var prop = $"Player Props : {property.Key} = {value}\n";
					text += prop;
				}

				text += $"###Room Properties###\n";
				foreach (var property in PhotonNetwork.CurrentRoom.CustomProperties)
				{
					var value = property.Value;
					if (value is IEnumerable enumerable)
					{
						value = "";
						foreach (var t in enumerable)
						{
							value += t + " , ";
						}
					}

					var prop = $"Room Props : {property.Key} = {value}\n";
					text += prop;
				}
			}

			m_content.text = text;
		}
	}
}