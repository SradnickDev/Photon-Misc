using System.Collections;
using System.Collections.Generic;
using System.Text;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Chat
{
	[RequireComponent(typeof(PhotonView))]
	public class RoomChat : MonoBehaviourPunCallbacks, IPointerDownHandler
	{
		[SerializeField] private KeyCode m_sendKey = KeyCode.Return;
		[SerializeField] private Text m_message = null;
		[SerializeField] private Transform m_content = null;
		[SerializeField] private InputField m_messageInput = null;

		private Queue<string> m_messageQueue = new Queue<string>();
		private StringBuilder m_messageBuilder = new StringBuilder();

#region Message Limits

		[Header("Send Limitations")]

		//messages will be send every n seconds
		[SerializeField]
		private double SendRate = 1;

		//max capacity of the queue
		[SerializeField] private int QueueCapacity = 10;

		private bool m_canSend = true;
		private double m_nextSendingTime;

#endregion

		private void Start()
		{
			m_messageInput.text = string.Empty;
			m_messageInput.interactable = false;
		}

		private void Update() => PlayerInput();

		private void PlayerInput()
		{
			if (!Input.GetKeyDown(m_sendKey)) return;
			if (m_messageInput.IsInteractable())
			{
				SendAndClose();
			}
			else
			{
				Open();
			}
		}

		private void Open()
		{
			m_messageInput.interactable = true;

			//set focus
			EventSystem.current.SetSelectedGameObject(m_messageInput.gameObject);
		}

		private void SendAndClose()
		{
			//send message if available
			if (!string.IsNullOrEmpty(m_messageInput.text))
			{
				SendMessage();
			}

			//clean input Field
			m_messageInput.text = string.Empty;

			//deselect the InputField
			EventSystem.current.SetSelectedGameObject(null);
			m_messageInput.interactable = false;
		}

		private void SendMessage()
		{
			if (m_canSend)
			{
				//Calculate next Time when the Message/s can be send
				//if less than 0 next out is 0
				var nextOut = m_nextSendingTime - Time.time < 0.0
					? 0.0
					: m_nextSendingTime - Time.time;
				HandleQueueLimit(m_messageInput.text);
				StartCoroutine(HandleMessageLimit(nextOut));
			}
			else
			{
				HandleQueueLimit(m_messageInput.text);
			}
		}

		/// <summary>
		/// Enqueue all Messages.
		/// If Message Queue is full stop adding and ignore Messages.
		/// </summary>
		/// <param name="msg">Message to Send.</param>
		private void HandleQueueLimit(string msg)
		{
			if (m_messageQueue.Count < QueueCapacity)
			{
				m_messageQueue.Enqueue(msg);
			}
			else
			{
				Debug.Log("Message Queue is full.Wait a moment");
			}
		}

		/// <summary>
		/// Calculate and set the Time to send a Message.
		/// "iterate" through all Messages in the Queue
		/// and stores them in as one large string.
		/// Send the large string via RPC.
		/// </summary>
		/// <returns></returns>
		private IEnumerator HandleMessageLimit(double delay)
		{
			m_canSend = false;
			m_nextSendingTime = Time.time + SendRate + delay;
			yield return new WaitForSeconds((float) delay);

			while (m_messageQueue.Count > 0)
			{
				var msg = m_messageQueue.Dequeue();
				m_messageBuilder.Append(msg);
				if (m_messageQueue.Count > 0)
				{
					m_messageBuilder.Append("\n");
				}
			}

			photonView.RPC(nameof(SendMessage), RpcTarget.All, m_messageBuilder.ToString());
			m_messageBuilder.Clear();
			m_canSend = true;
		}

		[PunRPC] 
		private void SendMessage(string text, PhotonMessageInfo info)
		{
			CreateLocalMessage(text, info.Sender);
		}

		private void CreateLocalMessage(string text, Player sender)
		{
			var senderName = FormatName(sender);
			var messageText = Instantiate(m_message, m_content, false);

			messageText.text = senderName + " : " + text;
		}

		public void CreateLocalMessage(string text)
		{
			var messageText = Instantiate(m_message, m_content, false);

			messageText.text = text;
		}

		private string FormatName(Player player)
		{
			var senderName = player.NickName;

			if (string.IsNullOrEmpty(player.NickName))
			{
				senderName = "Someone";
			}

			var localColor = ColorUtility.ToHtmlStringRGB(new Color(0.7f, 0.8f, 0.9f));
			var otherColor = ColorUtility.ToHtmlStringRGB(new Color(0.2f, 0.8f, 1f));
			var name = Equals(player, PhotonNetwork.LocalPlayer)
				? $"<color=#{localColor}> [ {senderName} ]</color>"
				: $"<color=#{otherColor}> [ {senderName} ]</color>";
			return name;
		}

		public void OnPointerDown(PointerEventData eventData)
		{
			if (!m_messageInput.IsInteractable())
			{
				Open();
			}
		}
	}
}