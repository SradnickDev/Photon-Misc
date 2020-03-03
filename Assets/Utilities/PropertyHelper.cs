using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace Utilities
{
	public static class PropertyHelper
	{
#region Player

		/// <summary>
		/// Check Player Properties for a Property Value.
		/// </summary>
		/// <param name="player">Photon Player</param>
		/// <param name="property">Property as string</param>
		/// <param name="defaultValue">Fallback Value.</param>
		/// <typeparam name="T">Type</typeparam>
		/// <returns>Property Value</returns>
		public static T GetPropertyValue<T>(this Player player, string property, T defaultValue)
		{
			if (player.CustomProperties.TryGetValue(property, out var value))
			{
				return (T) value;
			}

			return defaultValue;
		}

		/// <summary>
		/// Check Player Properties for a Property Value and set it.
		/// </summary>
		/// <param name="player">Photon Player</param>
		/// <param name="property">Property as string</param>
		/// <param name="value">Value to set.</param>
		/// <typeparam name="T">Type</typeparam>
		public static void SetPropertyValue<T>(this Player player, string property, T value)
		{
			var customProp = new Hashtable()
			{
				{property, value}
			};
			player.SetCustomProperties(customProp);
		}

		/// <summary>
		/// Check Player Properties for a Property Value, add given value to it.
		/// </summary>
		/// <param name="player">Photon Player</param>
		/// <param name="property">Property as string</param>
		/// <param name="value">Value to set.</param>
		public static void AddValueToProperty(this Player player, string property, int value)
		{
			var defaultValue = GetPropertyValue(player, property, 0);
			defaultValue += value;

			var scoreProp = new Hashtable()
			{
				{property, defaultValue}
			};
			player.SetCustomProperties(scoreProp);
		}

		/// <summary>
		/// Deletes Property if DeleteNullProperties is set to true in Room Options.
		/// </summary>
		/// <param name="player">Photon Player</param>
		/// <param name="property">Property as string</param>
		public static void DeleteProperty(this Player player, string property)
		{
			var customProp = new Hashtable()
			{
				{property, null}
			};
			player.SetCustomProperties(customProp);
		}

#endregion

#region Room

		/// <summary>
		/// Check Room Properties for a Property Value.
		/// </summary>
		/// <param name="room">Photon Room</param>
		/// <param name="property">Property as string</param>
		/// <param name="defaultValue">Fallback Value.</param>
		/// <typeparam name="T">Type</typeparam>
		/// <returns>Property Value</returns>
		public static T GetPropertyValue<T>(this Room room, string property, T defaultValue)
		{
			if (room.CustomProperties.TryGetValue(property, out var value))
			{
				return (T) value;
			}

			return defaultValue;
		}

		/// <summary>
		/// Check Room Properties for a Property Value and set it.
		/// </summary>
		/// <param name="room">Photon Room</param>
		/// <param name="property">Property as string</param>
		/// <param name="value">Value to set.</param>
		/// <typeparam name="T">Type</typeparam>
		public static void SetPropertyValue<T>(this Room room, string property, T value)
		{
			var customProp = new Hashtable()
			{
				{property, value}
			};
			room.SetCustomProperties(customProp);
		}

		/// <summary>
		/// Check Room Properties for a Property Value, add given value to it.
		/// </summary>
		/// <param name="room">Photon Room</param>
		/// <param name="property">Property as string</param>
		/// <param name="value">Value to set.</param>
		public static void AddValueToProperty(this Room room, string property, int value)
		{
			var defaultValue = GetPropertyValue(room, property, 0);
			defaultValue += value;

			var scoreProp = new Hashtable()
			{
				{property, defaultValue}
			};
			room.SetCustomProperties(scoreProp);
		}

#endregion
	}
}