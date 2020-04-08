using Photon.Pun;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshPro))]
public class NamePlate : MonoBehaviour
{
	[SerializeField] private PhotonView m_view = null;
	[SerializeField] private TextMeshPro m_textMesh = null;

	private void Start()
	{
		SetName();
	}

	private void SetName()
	{
		m_textMesh.text = m_view.Owner.NickName;
	}
}