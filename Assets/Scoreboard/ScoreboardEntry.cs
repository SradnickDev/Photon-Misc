using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

public class ScoreboardEntry : MonoBehaviour
{
	[SerializeField] private Text m_label = null;
	public Player Player => m_player;
	public int Score => m_player.GetScore();

	private Player m_player;

	//store player for this entry
	//set init value and color
	public void Set(Player player)
	{
		m_player = player;
		UpdateScore();
		m_label.color = PhotonNetwork.LocalPlayer == m_player ? Color.green : Color.red;
	}

	//update label bases on score and name
	public void UpdateScore()
	{
		m_label.text = $"{m_player.NickName} : Score {m_player.GetScore()}";
	}
}