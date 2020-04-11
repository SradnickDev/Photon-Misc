using System;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Events;

namespace Utilities.General
{
	public class ConnectAndJoinRandom : MonoBehaviourPunCallbacks
	{
		public UnityEvent Connected;

		[SerializeField] private bool m_autoConnect = true;
		[SerializeField] private byte m_version = 1;
		[SerializeField] private byte m_maxPlayer = 6;
		[SerializeField] private GameObject m_player = null;

		public void Start()
		{
			if (m_autoConnect)
			{
				ConnectNow();
			}
		}

		public void ConnectNow()
		{
			PhotonNetwork.ConnectUsingSettings();
			PhotonNetwork.GameVersion = $"{m_version}.{SceneManagerHelper.ActiveSceneBuildIndex}";
		}

		private void OnGUI()
		{
			if (PhotonNetwork.IsConnected) return;
			DrawConnectButton();
		}

		private void DrawConnectButton()
		{
			if (GUI.Button(new Rect((Screen.width / 2f) - 200 / 2f, (Screen.height / 2f) - 100 / 2f,
									200, 100)
						 , "Connect and Join"))
			{
				ConnectNow();
			}
		}

		public override void OnConnectedToMaster()
		{
			PhotonNetwork.JoinRandomRoom();
		}

		public override void OnJoinedRoom()
		{
			Connected?.Invoke();
			CreatePlayer();
		}

		private void CreatePlayer()
		{
			if (m_player)
			{
				PhotonNetwork.Instantiate(m_player.name, transform.position, Quaternion.identity);
			}
		}

		public override void OnJoinRandomFailed(short returnCode, string message)
		{
			var uniqueRoomId = new Guid().ToString();
			PhotonNetwork.CreateRoom(uniqueRoomId,
									 new RoomOptions()
									 {
										 MaxPlayers = m_maxPlayer, BroadcastPropsChangeToAll = true
									 }, null);
		}
	}
}