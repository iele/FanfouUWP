using System;

namespace Coding4Fun.Toolkit.Controls
{
	[Obsolete("Moved to Coding4Fun.Toolkit.dll now, Namespace is Coding4Fun.Toolkit")]
	public static class Numbers
	{
		public static float Max(params int[] numbers)
		{
			return Toolkit.Numbers.Max(numbers);
		}

		public static float Min(params int[] numbers)
		{
			return Toolkit.Numbers.Min(numbers);
		}

		public static float Max(params float[] numbers)
		{
			return Toolkit.Numbers.Max(numbers);
		}

		public static float Min(params float[] numbers)
		{
			return Toolkit.Numbers.Min(numbers);
		}

		public static double Max(params double[] numbers)
		{
			return Toolkit.Numbers.Max(numbers);
		}

		public static double Min(params double[] numbers)
		{
			return Toolkit.Numbers.Min(numbers);
		}
	}
}