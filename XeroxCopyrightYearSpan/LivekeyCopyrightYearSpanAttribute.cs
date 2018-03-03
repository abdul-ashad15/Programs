using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

[assembly:CLSCompliant(true)]
namespace LivekeyCopyrightYearSpan
{
	/// <summary>
	/// Refer to this from every project in every solution. AssemblyInfo.cs for every project is edited
	/// by the production build code.
	/// </summary>
	[AttributeUsage(AttributeTargets.All)]
	public class LivekeyCopyrightYearSpanAttribute : System.Attribute
	{
		//Private fields.
		private string yearsList;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="years">A string such as 2006-2010</param>
		public LivekeyCopyrightYearSpanAttribute(string years)
		{
			yearsList = years;
		}

		/// <summary>
		/// A string such as 2006-2010
		/// </summary>
		public virtual string YearsList
		{
			get { return yearsList; }
		}

		/// <summary>
		/// A string such as 2006-2010, to satisfy FxCop
		/// </summary>
		public string Years
		{
			get { return yearsList; }
		}
	}
}