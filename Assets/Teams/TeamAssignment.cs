using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace Teams
{
	public class TeamAssignment : MonoBehaviourPunCallbacks
	{
		public int MinTeamCount => m_minTeamCount;
		public int MaxTeamSize => m_maxTeamSize;

		//at least 2 teams make sense
		[SerializeField] private int m_minTeamCount = 2;
		//max size of each team
		[SerializeField] private int m_maxTeamSize = 2;
		private bool IsMasterClient => PhotonNetwork.IsMasterClient;

		//Assign masterClient to a Team
		public override void OnJoinedRoom()
		{
			if (!IsMasterClient) return;
			OnPlayerEnteredRoom(PhotonNetwork.MasterClient);
		}

		//assign each new joined player to a team
		public override void OnPlayerEnteredRoom(Player newPlayer)
		{
			if (!IsMasterClient) return;
			TeamExt.AssignToSmallest(newPlayer, m_maxTeamSize, m_minTeamCount);
		}

		//remove the player who left from his team
		public override void OnPlayerLeftRoom(Player otherPlayer)
		{
			if (!IsMasterClient) return;
			TeamExt.RemovePlayerFromTeam(otherPlayer, m_maxTeamSize, m_minTeamCount);
		}
	}
}