using Photon.Pun;
using Photon.Realtime;

namespace Utilities
{
	public static class PlayerExt
	{
		/// <summary>
		/// Player assigned Team index.
		/// </summary>
		/// <param name="player"></param>
		/// <returns>Team index if assigned,otherwise -1. </returns>
		public static int GetTeam(this Player player)
		{
			return player.GetPropertyValue(PlayerProperties.Team, -1);
		}

		/// <summary>
		/// Save Team index to Player properties.
		/// </summary>
		/// <param name="player">Target playedr</param>
		/// <param name="teamIdx">Team index</param>
		public static void SetTeam(this Player player, int teamIdx)
		{
			player.SetPropertyValue(PlayerProperties.Team, teamIdx);
		}

		/// <summary>
		/// Compares Team index from LocalPlayer and given Player.
		/// </summary>
		/// <param name="player">Target player to check if is friendly or not.</param>
		/// <returns>True if same team index otherwise false.</returns>
		public static bool IsFriendly(this Player player)
		{
			var ownTeam = PhotonNetwork.LocalPlayer.GetTeam();
			var targetTeam = player.GetTeam();
			return ownTeam == targetTeam;
		}

#region PlayerReadyState

		public static void SetReady(this Player player, bool value)
		{
			player.SetPropertyValue(PlayerProperties.Ready, value);
		}

		public static bool IsReady(this Player player)
		{
			return player.GetPropertyValue(PlayerProperties.Ready, false);
		}

#endregion
		
#region Score

		public static void SetScore(this Player player, int amount)
		{
			player.SetPropertyValue(PlayerProperties.Score, amount);
		}

		public static int GetScore(this Player player)
		{
			return player.GetPropertyValue(PlayerProperties.Score, 0);
		}
		
		public static void AddScore(this Player player, int amount)
		{
			player.AddValueToProperty(PlayerProperties.Score, amount);
		}

#endregion
	}
}