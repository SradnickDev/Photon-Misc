using Chat;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

[RequireComponent(typeof(RoomChat))]
public class RoomChatUtility : MonoBehaviourPunCallbacks
{
	[SerializeField] private RoomChat m_roomChat = null;

	public override void OnJoinedRoom()
	{
		var colorCode = ColorUtility.ToHtmlStringRGB(Color.blue);
		m_roomChat.CreateLocalMessage($"<color=#{colorCode}>You joined the Game. </color>");
	}

	public override void OnPlayerEnteredRoom(Player newPlayer)
	{
		var colorCode = ColorUtility.ToHtmlStringRGB(Color.blue);
		m_roomChat.CreateLocalMessage($"<color=#{colorCode}>{newPlayer.NickName} joined the Game. </color>");
	}

	public override void OnPlayerLeftRoom(Player otherPlayer)
	{
		var colorCode = ColorUtility.ToHtmlStringRGB(new Color(1f, 0.5f, 0f));
		m_roomChat.CreateLocalMessage($"<color=#{colorCode}>{otherPlayer.NickName} left the Game.</color>");
	}

}