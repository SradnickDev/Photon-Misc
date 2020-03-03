using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Events;

namespace Utilities.General
{
	public class ConnectAndJoinRandom : MonoBehaviourPunCallbacks
	{

		public UnityEvent Connected;
		public bool AutoConnect = true;

		public byte Version = 1;

		public void Start()
		{
			if (this.AutoConnect)
			{
				this.ConnectNow();
			}
		}

		public void ConnectNow()
		{
			PhotonNetwork.ConnectUsingSettings();
			PhotonNetwork.GameVersion =
				this.Version + "." + SceneManagerHelper.ActiveSceneBuildIndex;
		}

		private void OnGUI()
		{
			if (PhotonNetwork.IsConnected) return;
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

		public override void OnJoinedLobby()
		{
			PhotonNetwork.JoinRandomRoom();
		}

		public override void OnJoinedRoom()
		{
			Connected?.Invoke();
		}

		public override void OnJoinRandomFailed(short returnCode, string message)
		{
			PhotonNetwork.CreateRoom(null,
									 new RoomOptions()
										 {MaxPlayers = 4, BroadcastPropsChangeToAll = true}, null);
		}
	}
}