using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace Teams.Demo
{
	//Note : This is only to visualize the teams, nothing else.
	public class TeamView : MonoBehaviourPunCallbacks
	{
		[SerializeField] private TeamAssignment m_teamAssignment = null;
		private List<Text> m_teamInfos = new List<Text>();

		public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
		{
			RefreshOverview();
		}

		public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
		{
			RefreshOverview();
		}

		private void RefreshOverview()
		{
			var teams = PhotonNetwork.CurrentRoom.GetTeams(m_teamAssignment.MinTeamCount);
			CreateLabels(teams.Length);

			//write header
			for (var i = 0; i < teams.Length; i++)
			{
				var teamSize = teams[i];
				m_teamInfos[i].text =
					$"Team {i + 1} [{teamSize}/{m_teamAssignment.MaxTeamSize}] \n " +
					"----------- \n ";
			}

			//write names from player that belongs to a team
			foreach (var player in PhotonNetwork.PlayerList)
			{
				var teamIdx = player.GetTeam();
				m_teamInfos[teamIdx].text += player.ActorNumber + "\n";
			}
		}

		private void CreateLabels(int amount)
		{
			Clear();
			for (var i = 0; i < amount; i++)
			{
				var newLabel = CreateLabel();
				m_teamInfos.Add(newLabel);
			}
		}

		private Text CreateLabel()
		{
			var txt = new GameObject("Text", typeof(Text)).GetComponent<Text>();
			txt.alignment = TextAnchor.UpperCenter;
			txt.fontSize = 50;
			txt.transform.SetParent(transform, false);
			txt.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
			return txt;
		}

		private void Clear()
		{
			for (var i = m_teamInfos.Count - 1; i >= 0; i--)
			{
				Destroy(m_teamInfos[i].gameObject);
			}

			m_teamInfos = new List<Text>();
		}
	}
}