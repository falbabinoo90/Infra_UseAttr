using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace DataModel
{
	public static class DataManagement
    {
		public static List<PAttribute> GetAttributes(Type PObjectType, Type SpecificAttributeType = null, Func<PAttribute, bool> predicate = null)
		{
			List<PAttribute> Results = new List<PAttribute>();

			Debug.Assert(PObjectType != null);

			foreach (PropertyInfo PI in PObjectType.GetProperties().Where(prop => Attribute.IsDefined(prop, typeof(PAttribute))))
			{
				object[] ListOfAttributes = PI.GetCustomAttributes(SpecificAttributeType ?? typeof(PAttribute), false);
				foreach (PAttribute SA in ListOfAttributes)
				{
					Debug.Assert(SA != null);
					if (predicate == null || predicate(SA))
					{
						SA.PropertyInfo = PI;

						Results.Add(SA as PAttribute);
						break;
					}
				}
			}

			return Results;
		}

		internal static PropertyInfo GetPropertyInfoForAttribute(this Type PObjectType, string AttrID, out PAttribute o_SA)
		{
			o_SA = null;
			PropertyInfo ResultPI = null;

			List<PAttribute> Results = new List<PAttribute>();

			foreach (PropertyInfo PI in PObjectType.GetProperties().Where(prop => Attribute.IsDefined(prop, typeof(PAttribute))))
			{
				object[] ListOfAttributes = PI.GetCustomAttributes(typeof(PAttribute), false);
				foreach (PAttribute SA in ListOfAttributes)
				{
					Debug.Assert(SA != null);
					if (SA.AttrID == AttrID)
					{
						o_SA = SA;
						ResultPI = PI;
						break;
					}
				}
				if (ResultPI != null)
				{
					break;
				}
			}

			return ResultPI;
		}

		internal static PAttribute GetAttribute(this PropertyInfo PI)
		{
			PAttribute SA = PI?.GetCustomAttributes(typeof(PAttribute), true)?.FirstOrDefault() as PAttribute;
			return SA;
		}
    }
}
