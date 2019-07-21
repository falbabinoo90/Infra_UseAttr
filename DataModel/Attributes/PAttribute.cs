using System;
using System.Diagnostics;
using System.Reflection;
using Utilities;

namespace DataModel
{
	public enum AttributeEditBehavior
	{
		AlwaysEditable, // Défaut
		AlwaysReadOnly,
		Conditional // 
	}

	public enum AttributeEditContext
	{
		None,
		PropertyDefinitionPanel, // Défaut
		OptionsPanel
	}

	public enum AttributePreSelectionBehavior
	{
		NoPreselection,
		PrimaryPreSelection,
		SecondaryPreSelection
	}

	public enum PAttributeRole
	{
		Neutral,
		RenderingOnly,
		ComputateIn,
		ComputeOut// Défaut
	}

    [Serializable]
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
	public abstract class PAttribute : System.Attribute
	{
		public PropertyInfo PropertyInfo;
		public string ReferencedFieldName { get; set; }

		public string AttrID { get; }

		public PAttribute(string ID)
		{
			this.AttrID = ID;
		}
        
		public virtual bool HasValueFor(IPAttributes iSOW)
		{
			string Value = PropertyInfo.GetValue(iSOW) as string;
			return !string.IsNullOrWhiteSpace(Value);
		}

		protected Assembly AddInAssembly => PropertyInfo?.ReflectedType?.Assembly;

		public override string ToString()
		{
			return AttrID;
		}

		public override bool Equals(object obj)
		{
			PAttribute i_SA = obj as PAttribute;

			if (AttrID != i_SA?.AttrID)
			{
				return false;
			}
			else
			{
				PropertyInfo PI1 = PropertyInfo;
				PropertyInfo PI2 = i_SA?.PropertyInfo;

				//return PI1.DeclaringType == PI2.DeclaringType;  <= possible aussi
				return PI1.Equals(PI2);
			}
		}

		// When implemented in a derived class, gets a unique identifier for this System.Attribute.
		public override object TypeId => base.TypeId;

		public override int GetHashCode() => base.GetHashCode();

		// Valeur en texte indépendant de la langue utilisateur (utilisé pour la sérialisation)
		public virtual string GetValueAsStringInvariant(IPAttributes i_PObjectWithAttributes)
		{
			string InternalValueAsText = string.Empty;
			try
			{
				object PropertyInternalValue = PropertyInfo.GetValue(i_PObjectWithAttributes);
				InternalValueAsText = PropertyInternalValue?.ToString() ?? string.Empty;
			}
			catch
			{
				InternalValueAsText = string.Empty;
			}

			return InternalValueAsText;
		}

		// Valeur en texte indépendant de la langue utilisateur (utilisé pour la sérialisation)
		public bool TrySetValueAsStringInvariant(IPAttributes i_PObjectWithAttributes, string NewValueAsString)
		{
			bool ValueWasModified = false;
			try
			{
				string CurrentValueAsStringInvariant = GetValueAsStringInvariant(i_PObjectWithAttributes);
				if (CurrentValueAsStringInvariant == NewValueAsString)
				{
					// Valeur inchangée
				}
				else
				{
					try
					{
						ValueWasModified = SetValueFromStringInvariant(i_PObjectWithAttributes, NewValueAsString);
					}
					catch
					{
						ValueWasModified = false;
					}
				}
			}
			catch { ValueWasModified = false; }

			return ValueWasModified;
		}

		// Valeur en texte indépendant de la langue utilisateur (utilisé pour la sérialisation)
		protected virtual bool SetValueFromStringInvariant(IPAttributes SOW, string NewValueAsStringInvariant)
		{
			throw new NotImplementedException();
		}

		public object GetValue(IPAttributes SOW)
		{

			object result = null;

			Debug.Assert(SOW != null);

			if (SOW != null)
			{
				try
				{
					result = this.PropertyInfo.GetValue(SOW);

				}
				catch
				{
					result = null;
				}
			}

			return result;
		}

		public void SetValue(IPAttributes SOW, object i_value)
		{
			Debug.Assert(SOW != null);

			if (SOW != null)
			{
				try
				{
					this.PropertyInfo.SetValue(SOW, i_value);
				}
				catch(Exception e)
				{
					//e.AlertDevAboutUnexpectedError();
				}
			}
		}

		// Pour PropertyPage
		public abstract bool GetValueAsStringForDisplay(IPAttributes ISA, ref string o_ValueAsString);

		// Pour PropertyPage
		public abstract bool SetValueFromStringDisplay(IPAttributes ISA, string i_ValueAsString);
			   
		// Implémentation générique à surcharger selon le type
		public virtual bool SetValueToDefault(IPAttributes ISA)
		{
			bool ResultBool = false;

			Debug.Assert(ISA != null);

			if (ISA != null)
			{
				try
				{
					bool CanBeNull = PropertyInfo.PropertyType.IsNullableType();
					if (CanBeNull)
					{
						this.PropertyInfo.SetValue(ISA, null);
						ISA.OnAttributeChange(AttrID);
						ResultBool = true;
					}
				}
				catch
				{
					ResultBool = false;
				}
			}

			return ResultBool;
		}
	}

}
