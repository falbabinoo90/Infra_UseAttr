using System;
using System.Diagnostics;
using System.Reflection;
using Utilities;
using Interfaces;

namespace DataModel
{

	[Serializable]
	[AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
	public class PParameterAttribute : PAttribute, INumericField
    {
		public PParameterAttribute(string ID)
		: base(ID)
		{
			Magnitude = Magnitude.None;
			ConstraintInteger = false;
			HasMin = false;
			HasMax = false;
			MinSI = 0;
			MaxSI = 0;
			ExcludeMin = false;
			ExcludeMax = false;
		}

		Magnitude _Magnitude;
		public Magnitude Magnitude
		{
			get => _Magnitude;
			set
			{
				_Magnitude = value;
			}
		}

		public bool ConstraintInteger { get; set; }
		public bool HasMin { get; set; }
		public bool HasMax { get; set; }
		public double MinSI { get; set; }
		public double MaxSI { get; set; }
		public bool ExcludeMin { get; set; }
		public bool ExcludeMax { get; set; }

		public double TryGetAsDouble(ISession SOW)
		{
			double result = 0.0;
			Debug.Assert(SOW != null);

			if (SOW != null)
			{

				object ValueAsObj = this.PropertyInfo.GetValue(SOW);
				if (ValueAsObj != null)
				{
					bool IsNullable = UtilReflection.IsNullableType(ValueAsObj.GetType());
					if (IsNullable)
					{
                        // the attribute does not exist
					}
					else
					{
						try
						{
							result = Convert.ToDouble(ValueAsObj); // A double-precision floating-point number that is equivalent to value, or ZERO if value is null.
						}
						catch
						{

						}
					}
				}


			}
			return result;
		}

		public int TryGetAsInteger(ISession SOW)
		{
			int result = 0;
			Debug.Assert(SOW != null);

			if (SOW != null)
			{

				object ValueAsObj = this.PropertyInfo.GetValue(SOW);
				if (ValueAsObj != null)
				{
					bool IsNullable = UtilReflection.IsNullableType(ValueAsObj.GetType());
					if (IsNullable)
					{
                        // => the attribute does not exist
                    }
                    else
					{
						try
						{
							result = Convert.ToInt32(ValueAsObj);
						}
						catch
						{

						}
					}
				}
			}
			return result;
		}

		public override bool GetValueAsStringForDisplay(IPAttributes SOW, ref string o_ValueAsString)
		{
			bool ResultBool = false;
			o_ValueAsString = null;

			Debug.Assert(SOW != null);

			if (SOW != null)
			{
				try
				{
					object ValueAsObj = this.PropertyInfo.GetValue(SOW);
					if (ValueAsObj != null)
					{
						bool IsNullable = UtilReflection.IsNullableType(ValueAsObj.GetType());
						if (IsNullable)
						{
                            // => the attribute does not exist
                        }
                        else
						{
							try
							{
								double ValueAsDouble = 0;
								if (ValueAsObj is TimeSpan ts )
								{
									ValueAsDouble = ts.TotalSeconds;
								}
								else
								{
									ValueAsDouble = Convert.ToDouble(ValueAsObj); // A double-precision floating-point number that is equivalent to value, or ZERO if value is null.
								}
								o_ValueAsString = NumericValue.GetDisplayTextFromInternalValue(Magnitude, ValueAsDouble);
								ResultBool = true;
							}
							catch
							{

							}
						}
					}
				}
				catch
				{
					ResultBool = false;
				}
			}

			return ResultBool;
		}

		public bool CheckValue(double ValueAsDouble)
		{
			return NumericValue.ApplyConstraints(this, ref ValueAsDouble, out bool o_ValueWasModified) && !o_ValueWasModified;
		}

		public override bool SetValueFromStringDisplay(IPAttributes ISA, string i_ValueAsString)
		{
			bool ValueWasModified = false;

			if (ISA != null)
			{
				if (NumericValue.GetInternalValueFromDisplayText(Magnitude, i_ValueAsString, out double DoubleValue))
				{
					ValueWasModified = SetValueFromDouble(ISA, DoubleValue);
				}
			}

			return ValueWasModified;
		}

		protected override bool SetValueFromStringInvariant(IPAttributes ISA, string NewValueAsStringInvariant)
		{
			bool ValueWasModified = false;

			if (double.TryParse(NewValueAsStringInvariant, out double DoubleValue))
			{
				ValueWasModified = SetValueFromDouble(ISA, DoubleValue);
			}

			return ValueWasModified;
		}

		private bool SetValueFromDouble(IPAttributes ISA, double dValue)
		{
			bool ResultBool = false;

			try
			{
				Type T = this.PropertyInfo.PropertyType;
                
				Type UnderlyingTypeIfNullable = Nullable.GetUnderlyingType(T);
				if (UnderlyingTypeIfNullable != null)
				{
					T = UnderlyingTypeIfNullable;
				}

				bool ValueWasAcceptedButMaybeModified = NumericValue.ApplyConstraints(this, ref dValue, out bool o_ValueWasModified);

				if (ValueWasAcceptedButMaybeModified && !o_ValueWasModified )
				{
					object StronglyTypedValue = UtilNumeric.DoubleToNumericType(T, dValue);
					if (StronglyTypedValue == null)
					{
						throw new ArgumentException("Invalid value for property type", AttrID);
					}
					else
					{
						this.PropertyInfo.SetValue(ISA, StronglyTypedValue);
						ISA.OnAttributeChange(AttrID);
						ResultBool = true;
					}
				}
			}
			catch (TargetException)
			{
				throw;
			}
			catch (TargetInvocationException)
			{
				throw;
			}
			catch (Exception)
			{
				throw; 
			}

			return ResultBool;
		}
	}

}
