using System;


namespace SPEOS_SC.Infra.DataModel
{
	[Serializable]
	public class SpeosParameterOnAttribute : SpeosParameter
	{
		private SpeosAttribute _SpeosAttribute = null;
		private ISpeosAttributes _ISA = null;

		public SpeosParameterOnAttribute(SpeosAttribute i_SpeosAttribute, ISpeosAttributes i_ISA)
		{
			_SpeosAttribute = i_SpeosAttribute;
			_ISA = i_ISA;

			_SpeosAttribute.GetUserTextFor(i_ISA, out string o_CatUserText, out string o_UserText, true);
			CategoryUserName = o_CatUserText;
			FieldUserName = o_UserText;
		}

		//Id in the format CategoryId/Id
		public override string FieldId
		{
			get
			{
				return _SpeosAttribute?.AttrID;
			}
		}

		public override string CategoryId
		{
			get
			{
				return _SpeosAttribute?.CategoryID;
			}
		}

		//UserName in the format : CategoryUserName/UserName
		public override string FieldUserName
		{
			get
			{
				return _SpeosAttribute?.UserName;
			}
		}

		public override string CategoryUserName 
		{
			get
			{
				return _SpeosAttribute?.CategoryUserName;
			}
		}

		//Input or Output
		public override ParameterDirection Direction
		{
			get
			{
				ParameterDirection result = ParameterDirection.Undefined;
				if (_SpeosAttribute?.AttributeRole == SpeosAttributeRole.ComputateIn)
				{
					result = ParameterDirection.Input;
				}
				else
				{
					result = ParameterDirection.Output;
				}
				return result;
			}
		}

		public override SpeosMagnitude Magnitude
		{
			get
			{
				SpeosMagnitude result = SpeosMagnitude.None;
				if (_SpeosAttribute is SpeosParameterNumberAttribute numAttr)
				{
					result = numAttr.Magnitude;
				}
				return result;
			}
		}

		public override object Value
		{
			get
			{
				return _SpeosAttribute?.GetValue(_ISA);
			}
			set
			{
				_SpeosAttribute?.SetValue(_ISA, value);
			}
		}

		public override bool IsEnabled
		{
			get
			{
				return _SpeosAttribute.IsEnabledFor(_ISA);
			}
		}

		public override bool IsValueMandatory
		{
			get
			{
				return _SpeosAttribute.IsMandatoryFor(_ISA);
			}
		}

		public override bool IsVisible
		{
			get
			{
				return _SpeosAttribute.IsVisibleFor(_ISA);
			}
		}
	}
}
