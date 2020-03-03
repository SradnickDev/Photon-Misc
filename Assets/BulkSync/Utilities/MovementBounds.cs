using UnityEngine;

#pragma warning disable 0649

[RequireComponent(typeof(BoxCollider))]
public class MovementBounds : MonoBehaviour
{
	[SerializeField] private BoxCollider m_collider;
	
	public Vector3 RandomPoint
	{
		get
		{
			if (m_collider == null)
			{
				m_collider = GetComponent<BoxCollider>();
			}

			var colSize = m_collider.size;
			var position = new Vector3(0, 0, 0)
			{
				x = Random.Range(-colSize.x / 2f, colSize.x / 2f),
				y = Random.Range(-colSize.y / 2f, colSize.y / 2f),
				z = Random.Range(-colSize.z / 2f, colSize.z / 2f)
			};

			var idxArr = new int[] {0, 2};
			var axis = idxArr[Random.Range(0, 2)];
			position[axis] = position[axis] < 0f ? -colSize.x / 2f : colSize.x / 2f;

			return transform.TransformPoint(position);
		}
	}

	public Bounds Bounds => m_collider.bounds;
	private void Start()
	{
		m_collider = GetComponent<BoxCollider>();
	}
}
#pragma warning restore 0649