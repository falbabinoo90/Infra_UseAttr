using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPEOS_SC.Infra.DataModel
{
	[Serializable]
	public class SpeosSyncAttribute : System.Attribute
	{
	}

	[Serializable]
	[AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
	public class SpeosSyncSerializedField : SpeosSyncAttribute
	{

		public string AssociatedNonSerializableField { get; set; }
	}

	[Serializable]
	[AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
	public class SpeosSyncDeserializedField : SpeosSyncAttribute
	{

		public string AssociatedSerializableField { get; set; }
	}
}
