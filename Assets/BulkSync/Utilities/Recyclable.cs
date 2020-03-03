using BulkSync.Unit;
using UnityEngine;

[RequireComponent(typeof(UnitBase))]
public class Recyclable : MonoBehaviour, IRecyclable
{
	private ObjectPool<Recyclable> m_pool;

	public void Setup(ObjectPool<Recyclable> pool)
	{
		m_pool = pool;
	}

	private void OnDisable()
	{
		Recycle();
	}

	public void Recycle()
	{
		m_pool.Push(this.GetComponent<Recyclable>());
	}
}