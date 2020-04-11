using Photon.Realtime;
using Teams;

namespace Utilities
{
	public static class RoomExt
	{
		/// <summary>
		/// Save Team array to room properties.
		/// Each array entry is a team, the content of an index represents the team size.
		/// </summary>
		/// <param name="room">Current Room.</param>
		/// <param name="teams">Array of Team sizes.</param>
		public static void SetTeams(this Room room, int[] teams)
		{
			room.SetPropertyValue(RoomProperties.Teams, teams);
		}

		/// <summary>
		/// Reads team properties from current Room.
		/// </summary>
		/// <param name="room">Current Room</param>
		/// <param name="minTeamCount">If no properties are assigned, this will used to return an array with the minTeamCount as size.</param>
		/// <returns>Each array entry is a team, the content of an index represents the team size.</returns>
		public static int[] GetTeams(this Room room, int minTeamCount)
		{
			var teams = room.GetPropertyValue(RoomProperties.Teams, new int[minTeamCount]);
			return teams;
		}
	}
}