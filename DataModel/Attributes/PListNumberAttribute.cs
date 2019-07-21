using System;
using System.Collections;


namespace SPEOS_SC.Infra.DataModel
{
	// NE DEVRAIT PLUS ETRE UTILISE : TODO => à nettoyer si BFL n'en a plus besoin
	// Besoin BFL/LPN : temporaire (à généraliser en gérant des sous-wrappers avec des sélections possibles)
	[AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
	public class SpeosListNumberAttributeAttribute : SpeosAttribute
	{
		public SpeosListNumberAttributeAttribute(string ID) : base(ID)
		{
		}

		public override bool HasValueFor(ISpeosAttributes iSOW)
		{
			object SelectionPropertyValue = this.PropertyInfo.GetValue(iSOW);

			if (SelectionPropertyValue == null)
			{
				return false;
			}
			else
			{
				IList ListOfSimpleType = SelectionPropertyValue as IList;
				if (ListOfSimpleType == null)
				{
					return false;
				}
				else
				{
					return ListOfSimpleType.Count > 0;
				}
			}
		}

		public override bool GetValueAsStringForDisplay(ISpeosAttributes ISA, ref string o_ValueAsString)
		{
			return false;
			//o_ValueAsString = GetValue(ISA).ToString();
			//return true;
		}

		public override bool SetValueFromStringDisplay(ISpeosAttributes ISA, string i_ValueAsString)
		{
			return false;
			//throw new NotImplementedException();
		}
	}
}
