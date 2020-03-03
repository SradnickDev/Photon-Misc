using Photon.Realtime;
using Teams;

namespace Utilities
{
	public static class RoomExt
	{
		public static void SetTeams(this Room room, int[] teams)
		{
			room.SetPropertyValue(RoomProperties.Teams, teams);
		}

		public static int[] GetTeams(this Room room, int minTeamCount)
		{
			var teams = room.GetPropertyValue(RoomProperties.Teams, new int[minTeamCount]);
			return teams;
		}
	}
}