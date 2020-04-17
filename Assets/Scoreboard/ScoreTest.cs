using Photon.Pun;
using UnityEngine;
using Utilities;

//just to showcase score changes
public class ScoreTest : MonoBehaviour
{
	public void IncreaseScore()
	{
		PhotonNetwork.LocalPlayer.AddScore(10);
	}

	public void DecreaseScore()
	{
		PhotonNetwork.LocalPlayer.AddScore(-10);
	}
}