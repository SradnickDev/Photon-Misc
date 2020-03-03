using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace Teams
{
	public class TeamAssignment : MonoBehaviourPunCallbacks
	{
		public int MinTeamCount => m_minTeamCount;
		public int MaxTeamSize => m_maxTeamSize;

		[SerializeField] private int m_minTeamCount = 2;
		[SerializeField] private int m_maxTeamSize = 2;
		private bool IsMasterClient => PhotonNetwork.IsMasterClient;

		public override void OnJoinedRoom()
		{
			if (!IsMasterClient) return;
			OnPlayerEnteredRoom(PhotonNetwork.MasterClient);
		}

		public override void OnPlayerEnteredRoom(Player newPlayer)
		{
			if (!IsMasterClient) return;
			TeamExt.AssignToSmallest(newPlayer, m_maxTeamSize, m_minTeamCount);
		}

		public override void OnPlayerLeftRoom(Player otherPlayer)
		{
			if (!IsMasterClient) return;
			TeamExt.RemovePlayerFromTeam(otherPlayer, m_maxTeamSize, m_minTeamCount);
		}
	}
}