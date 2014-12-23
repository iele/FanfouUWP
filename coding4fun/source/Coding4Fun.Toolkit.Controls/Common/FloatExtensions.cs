using System;

namespace Coding4Fun.Toolkit.Controls.Common
{
	[Obsolete("Moved to Coding4Fun.Toolkit.dll now, Namespace is System")]
	public static class FloatExtensions
    {
		public static double CheckBound(this float value, float maximum)
		{
			return System.FloatExtensions.CheckBound(value, maximum);
		}

		public static double CheckBound(this float value, float minimum, float maximum)
		{
			return System.FloatExtensions.CheckBound(value, minimum, maximum);
		}

	    /// <summary>
	    /// Tests equality with a certain amount of precision.  Default to smallest possible double
	    /// </summary>
	    /// <param name="a">first value</param>
	    /// <param name="b">second value</param> 
	    /// <param name="precision">optional, smallest possible double value</param>
	    /// <returns></returns>
		public static bool AlmostEquals(this float a, float b, double precision = float.Epsilon)
	    {
			return System.FloatExtensions.AlmostEquals(a, b, precision);
	    }
    }
}