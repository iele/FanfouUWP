using System;

namespace Coding4Fun.Toolkit.Controls.Common
{
	[Obsolete("Moved to Coding4Fun.Toolkit.dll now, Namespace is System, ")]
	public static class DoubleExtensions
    {
		public static double CheckBound(this double value, double maximum)
		{
			return System.DoubleExtensions.CheckBound(value, maximum);
		}

		public static double CheckBound(this double value, double minimum, double maximum)
		{
			return System.DoubleExtensions.CheckBound(value, minimum, maximum);
		}

	    /// <summary>
	    /// Tests equality with a certain amount of precision.  Default to smallest possible double
	    /// </summary>
	    /// <param name="a">first value</param>
	    /// <param name="b">second value</param> 
	    /// <param name="precision">optional, smallest possible double value</param>
	    /// <returns></returns>
	    public static bool AlmostEquals(this double a, double b, double precision = Double.Epsilon)
	    {
			return System.DoubleExtensions.AlmostEquals(a, b, precision);
	    }
    }
}