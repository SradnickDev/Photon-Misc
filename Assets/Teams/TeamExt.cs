using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using Utilities;

namespace Teams
{
	public static class TeamExt
	{
		public static int[] GetTeams(int minTeamCount)
		{
			var retVal = new int[minTeamCount];
			if (PhotonNetwork.InRoom)
			{
				retVal = PhotonNetwork.CurrentRoom.GetTeams(minTeamCount);
			}

			return retVal;
		}

		public static void AssignToSmallest(Player player, int maxTeamSize, int minTeamCount)
		{
			var teams = GetTeams(minTeamCount);
			var smallestTeam = -1;

			//iterate trough all indices to find the smallest Team
			var tempSize = maxTeamSize;
			for (var i = 0; i < teams.Length; i++)
			{
				var teamSize = teams[i];
				if (teamSize < tempSize && teamSize < maxTeamSize)
				{
					smallestTeam = i;
					tempSize = teamSize;
				}
			}

			//if all teams are full, add a new team slot
			if (smallestTeam == -1)
			{
				var tempTeams = teams.ToList();
				tempTeams.Add(0);
				teams = tempTeams.ToArray();
				smallestTeam = teams.Length - 1;
			}

			//increase the team size
			teams[smallestTeam]++;

			//set the player to what team he now belongs
			player.SetTeam(smallestTeam);

			//set team array to room properties
			PhotonNetwork.CurrentRoom.SetTeams(teams);
		}

		public static void RemovePlayerFromTeam(Player player, int maxTeamSize, int minTeamCount)
		{
			var teams = GetTeams(minTeamCount);
			var teamIdx = player.GetTeam();
			teams[teamIdx] = Mathf.Clamp(teams[teamIdx] - 1, 0, maxTeamSize);
			PhotonNetwork.CurrentRoom.SetTeams(teams);
		}
	}
}