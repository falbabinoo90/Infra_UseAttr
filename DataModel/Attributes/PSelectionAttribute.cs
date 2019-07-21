using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Resources;

using SpaceClaim.Api.V18;
using SpaceClaim.Api.V18.Geometry;

using SPEOS.Services.CSharp;

namespace SPEOS_SC.Infra.DataModel
{
	[Serializable]
	[AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
	public class SpeosLinkAttribute : SpeosAttribute, ISpeosListAttribute
	{
		public SpeosLinkAttribute(string ID) : base(ID) { }

		public bool Multiple { get; set; }

		public bool WithUpAndDownFunction { get; set; }

		public override bool HasValueFor(ISpeosAttributes iSOW)
		{
			object SelectionPropertyValue = this.PropertyInfo.GetValue(iSOW);

			if (this.Multiple)
			{
				if (SelectionPropertyValue == null)
				{
					return false;
				}
				else
				{
					IList<IDocObject> ListOfSelections = SelectionPropertyValue as IList<IDocObject>;
					if (ListOfSelections == null)
					{
						return false;
					}
					else
					{
						return ListOfSelections.Count > 0;
					}
				}
			}
			else
			{
				IDocObject Selection = SelectionPropertyValue as IDocObject;
				return Selection != null;
			}
		}

		private Type[] _T;
		public Type[] AllowableTypes
		{
			get
			{
				switch (SelectionType)
				{
					case SpeosSelectionType.AXIS:
						return new[] { typeof(ITrimmedCurve), typeof(IDesignEdge), typeof(IDatumLine), typeof(IDesignCurve), typeof(ICoordinateAxis) };
					case SpeosSelectionType.POINT:
						return new[] { typeof(IDesignCurve), typeof(IDatumPoint), typeof(ICoordinateSystem) };
					case SpeosSelectionType.FACE:
						return new[] { typeof(IDesignFace) };
					case SpeosSelectionType.CURVE:
						return new[] { typeof(IDesignCurve), typeof(IDesignEdge) };
					case SpeosSelectionType.BODY:
						return new[] { typeof(IDesignBody) };
					case SpeosSelectionType.AXIS_SYSTEM:
						return new[] { typeof(ICoordinateSystem) };
					case SpeosSelectionType.PLANE:
						return new[] { typeof(IDatumPlane), typeof(IDesignFace) };
					case SpeosSelectionType.NO_DEFINED:
						return _T;
					case SpeosSelectionType.CO:
						return new[] { typeof(ICustomObject) };
					default:
						return null;
				}
			}
			set => _T = value;
		}

		// Types a filtrer dans la méthode AdjustSelection d'un Tool si on sélectionne un CustomObject. C'est une liste de type d'interfaces
		public Type[] AllowedWrapperInterfaceTypes { get; set; }

		public SpeosSelectionType SelectionType { get; set; } = SpeosSelectionType.NO_DEFINED;

		public string SelectionTypeUserName
		{
			get
			{
				ResourceManager RM = Properties.Resources.ResourceManager;
				string Key = "SpeosSelectionType_" + SelectionType.ToString();
				string Text = RM?.GetTextNoException(Key);
				if (RM == null)
				{
					Text = Key;
					SpeosError.AlertDevAboutUnexpectedError(string.Format("Add type '{0}' to 'Resources_Types.resx' file", Key));
				}
				return Text;
			}
		}

		public IList<IDocObject> TryGetAsListOfDocObjects(ISpeosAttributes SOW)
		{
			Debug.Assert(SOW != null);
			IList<IDocObject> result = null;
			if (SOW != null)
			{
				try
				{
					object ObjValue = this.PropertyInfo.GetValue(SOW);
					result = ObjValue as IList<IDocObject>;
					if (result == null)
					{
						if (ObjValue is IDocObject IDO)
						{
							result = new List<IDocObject>() { IDO };
						}
					}
				}
				catch
				{
				}
			}

			return result;
		}

		public object TryGetAsObject(ISpeosAttributes SOW)
		{
			object result = null;
			if (SOW != null)
			{
				try
				{
					result = this.PropertyInfo.GetValue(SOW);
				}
				catch
				{
				}
			}

			return result;
		}

		public SpeosAttributePreSelectionBehavior PreSelectionBehaviour { get; set; } = SpeosAttributePreSelectionBehavior.NoPreselection;

		// Retourne une chaine vide si pas de sélection
		// Single => le chemin de la sélection
		// Multiple => Nb éléments
		public override bool GetValueAsStringForDisplay(ISpeosAttributes iSOW, ref string o_ValueAsString)
		{
			object SelectionPropertyValue = this.PropertyInfo.GetValue(iSOW);

			if (this.Multiple)
			{
				if (SelectionPropertyValue == null)
				{
					return false;
				}
				else
				{
					IList<IDocObject> ListOfSelections = SelectionPropertyValue as IList<IDocObject>;
					if (ListOfSelections == null)
					{
						return false;
					}
					else
					{
						return ListOfSelections.Count > 0;
					}
				}
			}
			else
			{
				IDocObject Selection = SelectionPropertyValue as IDocObject;
				return Selection != null;
			}
		}

		public override bool SetValueFromStringDisplay(ISpeosAttributes ISA, string i_ValueAsString)
		{
			return false; //throw new NotImplementedException();
		}

		public bool DeleteItem(ISpeosAttributes i_ItfSpeosObject, int nIndex)
		{
			bool o_DeleteSuccessful = false;

			if (Multiple)
			{
				i_ItfSpeosObject.AttributeChildPreDelete(AttrID, nIndex, out bool bDeletedIsAllowed); // Pour surcharge avec comportement spécifique au Wrapper
				if (bDeletedIsAllowed)
				{
					if (PropertyInfo.GetValue(i_ItfSpeosObject) is IList<IDocObject> ListOfIDO)
					{
						ListOfIDO.RemoveAt(nIndex);
						o_DeleteSuccessful = true;
					}
				}
			}

			return o_DeleteSuccessful;
		}

		/// <summary>
		/// Inverse les positions de 2 éléments de la liste
		/// </summary>
		/// <param name="i_ItfSpeosObject"></param>
		/// <param name="i_Index1"></param>
		/// <param name="i_Index2"></param>
		/// <returns></returns>
		public void MoveChild(ISpeosAttributes i_ItfSpeosObject, int i_Index1, int i_Index2)
		{
			if (Multiple)
			{
				object AttributeValue = PropertyInfo.GetValue(i_ItfSpeosObject);
				if (AttributeValue is IList IBL)
				{
					object NewObj = IBL[i_Index1];
					IBL.Remove(NewObj);
					IBL.Insert(i_Index2, NewObj);
				}
			}
		}
	}
}
