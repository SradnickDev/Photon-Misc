using UnityEngine;

public static class Helper
{
	/// <summary>
	/// True, if Mouse is in Window.
	/// </summary>
	public static bool IsMouseOnScreen()
	{
		var rect = new Rect(0, 0, Screen.width, Screen.height);
		return rect.Contains(Input.mousePosition);
	}

	/// <summary>
	/// Mouse Position in fixed World Position.
	/// </summary>
	public static Vector3 GetMouseInWorld(Camera cam)
	{
		var mousePos = Input.mousePosition;
		var ray = cam.ScreenPointToRay(mousePos);
		var diff = -ray.origin.z / ray.direction.z;
		return ray.origin + ray.direction * diff;
	}
}