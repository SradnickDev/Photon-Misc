using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class HealthChanged : UnityEvent<float, float> { }

[RequireComponent(typeof(PhotonView))]
public class Health : MonoBehaviour, IDamageable, IPunObservable
{
	public HealthChanged OnHealthChanged;
	[SerializeField] private float m_maxHealth = 1000;
	private float m_currentHealth;
	private PhotonView m_view;
	private Player m_lastHit;

	private void Awake()
	{
		m_view = GetComponent<PhotonView>();
	}

	private void Start()
	{
		HandleObservable();

		m_currentHealth = m_maxHealth;
		OnHealthChanged.Invoke(m_currentHealth, m_maxHealth);
	}

	private void HandleObservable()
	{
		if (!m_view.ObservedComponents.Contains(this))
		{
			m_view.ObservedComponents.Add(this);
		}
	}

	/// <summary>
	/// Send an damage RPC to the objects owner.
	/// </summary>
	/// <param name="amount">How much damage should be dealt.</param>
	public void ApplyDamage(float amount)
	{
		if (m_view.IsSceneView)
		{
			var info = new PhotonMessageInfo(PhotonNetwork.LocalPlayer,
											 PhotonNetwork.ServerTimestamp, null);
			ApplyDamageInternal(amount, info);
		}

		m_view.RPC(nameof(ApplyDamageInternal), m_view.Owner, amount);
	}

	/// <summary>
	/// Send an heal RPC to the objects owner.
	/// </summary>
	public void ApplyHealth(float amount)
	{
		if (m_view.IsSceneView)
		{
			ApplyHealthInternal(amount);
		}

		m_view.RPC(nameof(ApplyHealthInternal), m_view.Owner, amount);
	}

	[PunRPC]
	private void ApplyDamageInternal(float amount, PhotonMessageInfo info)
	{
		m_lastHit = info.Sender;

		m_currentHealth = Mathf.Clamp(m_currentHealth -= amount, 0, m_maxHealth);
		OnHealthChanged.Invoke(m_currentHealth, m_maxHealth);

		if (m_currentHealth <= 0)
		{
			OnDeath();
		}
	}

	[PunRPC]
	private void ApplyHealthInternal(float amount)
	{
		m_currentHealth = Mathf.Clamp(m_currentHealth += amount, 0, m_maxHealth);

		OnHealthChanged.Invoke(m_currentHealth, m_maxHealth);
	}

	private void OnDeath()
	{
		//do something
		Debug.Log($"{gameObject.name} / {m_view.Owner.NickName} Died");
	}

	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.IsWriting)
		{
			stream.SendNext(m_currentHealth);
		}
		else
		{
			m_currentHealth = (float) stream.ReceiveNext();
			OnHealthChanged.Invoke(m_currentHealth, m_maxHealth);
		}
	}
}