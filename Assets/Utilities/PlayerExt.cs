using Photon.Pun;
using Photon.Realtime;

namespace Utilities
{
	public static class PlayerExt
	{
		public static int GetTeam(this Player player)
		{
			return player.GetPropertyValue(PlayerProperties.Team, -1);
		}

		public static void SetTeam(this Player player, int teamIdx)
		{
			player.SetPropertyValue(PlayerProperties.Team, teamIdx);
		}

		public static bool IsFriendly(this Player player)
		{
			var ownTeam = PhotonNetwork.LocalPlayer.GetTeam();
			var targetTeam = player.GetTeam();
			return ownTeam == targetTeam;
		}
	}
}