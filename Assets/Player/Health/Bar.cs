using UnityEngine;

public class Bar : MonoBehaviour
{
	[SerializeField] private SpriteRenderer FillBar = null;
	[SerializeField] float m_maxX = 300f;

	public void OnValueChanged(float currentHealth, float maxHealth)
	{
		var percentage = currentHealth / maxHealth;
		percentage = Mathf.Clamp01(percentage);

		var scale = FillBar.transform.localScale;
		scale.x = m_maxX * percentage;
		FillBar.transform.localScale = scale;
	}
}