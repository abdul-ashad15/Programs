using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Resources; 

namespace WebDX.Api
{
	static class DescriptionResources
	{
		public static string GetString(string key, string prefix, Assembly targetAssembly)
		{
			if (string.IsNullOrEmpty(key))
				throw new ArgumentNullException("key");

			ResourceManager rm = new ResourceManager(prefix + ".Resources.CommonResources", targetAssembly);
			return rm.GetString(key);
		}
	}
}
