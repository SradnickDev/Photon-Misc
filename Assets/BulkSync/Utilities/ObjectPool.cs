using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BulkSync.Unit
{
	public class ObjectPool<T> where T : Component, IRecyclable
	{
		public List<T> Pool => m_pool.ToList();
		private Stack<T> m_pool = new Stack<T>();

		private T m_objectToPool;
		private Transform m_parent;

		public void Initialize(int qty, T obj, Transform parent)
		{
			if (obj == null)
			{
				Debug.LogError("Object is null!");
				return;
			}

			m_objectToPool = obj;
			m_parent = parent;

			for (var i = 0; i < qty; i++)
			{
				IncreasePool();
			}
		}

		private void IncreasePool()
		{
			var obj = CreateObject();
			Push(obj);
		}

		private T CreateObject() => Object.Instantiate(m_objectToPool, m_parent, false);

		public bool TryPop(out T obj)
		{
			if (m_pool.Count == 0)
			{
				obj = null;
				return false;
			}

			obj = m_pool.Pop();
			return true;
		}

		public void Push(T obj) => m_pool.Push(obj);
	}
}