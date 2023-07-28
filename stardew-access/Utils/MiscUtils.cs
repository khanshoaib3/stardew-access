using System.Collections.Generic;

namespace stardew_access.Utils
{
	public static class MiscUtils
	{
		public delegate void OnMismatchAction();
		public static (T, bool) Cycle<T>(IList<T> list, ref int index, bool back = false, bool? wrapAround = false)
		{
			if (list.Count == 0)
			{
				throw new ArgumentException("List cannot be empty.", nameof(list));
			}

			bool edgeOfList = false;

			if (back)
			{
				index--;
				if (index < 0)
				{
					if (wrapAround == true) index = list.Count - 1;
					else index = 0;
					edgeOfList = true;
				}
			}
			else
			{
				index++;
				if (index >= list.Count)
				{
					if (wrapAround == true) index = 0;
					else index = list.Count - 1;
					edgeOfList = true;
				}
			}

			return (list[index], edgeOfList);
		}

		public static void UpdateAndRunIfChanged(ref int storedValue, int currentValue, OnMismatchAction action)
		{
			if (storedValue != currentValue)
			{
				storedValue = currentValue;
				action();
			}
		}
	}
}
