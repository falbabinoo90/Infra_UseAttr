using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.ComponentModel;
using Utilities;

namespace DataModel
{
    public class FeatureBase :  IPAttributes
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public List<PError> DetectDefinitionErrors()
        {
            throw new NotImplementedException();
        }

        protected bool CheckSetField(string PropertyName, out PAttribute o_SA)
        {
            bool o_SetIsPossible = true;
            Type t = GetType(); 
            PropertyInfo PI = GetType()?.GetProperty(PropertyName);
            o_SA = PI.GetAttribute();
            #region not yet
            ////PropertyInfo PI = GetType().GetPropertyInfoForAttribute(FieldID, out o_SA); <= Moins performant !

            //if (o_SA == null)
            //{
            //    o_SetIsPossible = false;
            //    Error.AlertDevAboutUnexpectedError($" Attribute could not be found for property '{PropertyName}'");
            //}
            //else
            //{
            //    if (IsComputing)
            //    {
            //        if (ComputeMode == ObjectWrapperExt.EnumComputeMode.Manual)
            //        {
            //            // On permet de modifier les Input lors d'un Compute Manuel... Mais ce n'est pas une bonne idée dans le principe !
            //            //Error.AlertDevAboutUnexpectedError();
            //            //o_SetIsPossible = false;
            //        }
            //        else if (ComputeMode == ObjectWrapperExt.EnumComputeMode.Auto)
            //        {
            //            bool bIsComputeIn = o_SA.AttributeRole == AttributeRole.ComputateIn;
            //            if (bIsComputeIn)
            //            {
            //                o_SetIsPossible = false;
            //                Error.AlertDevAboutUnexpectedError($"Attribute '{o_SA.AttrID}' is an Input, hence its property cannot be set during the Computation process.");
            //            }
            //        }
            //    }
            //}

            //if (o_SetIsPossible && !VersionIsAttributeCompatible(PropertyName))
            //{
            //    o_SetIsPossible = false;
            //}
            #endregion
            return o_SetIsPossible;
        }

        protected bool SetField<X>(ref X field,
                                        X value,
                                        [CallerMemberName] string CallingAttributePropertyName = "")
        {
            bool o_SetDone = false;

            bool SetIsPossible = CheckSetField(CallingAttributePropertyName, out PAttribute SA);
            if (SetIsPossible)
            {
                o_SetDone = PAttributesExt.SetField<X>(this, ref field, value, SA.AttrID );
            }

            return o_SetDone;
        }

        protected T GetField<T>(T InternalFieldValue, bool ConditionForExistance = true, [CallerMemberName] string CallingAttributePropertyName = "")
        {
            return PAttributesExt.GetField<T>(this, InternalFieldValue, ConditionForExistance);
              
        }

        public ICollection<PAttribute> GetAttributes(Type SpecificAttributeType, Func<PAttribute, bool> predicate) 
            => PAttributesExt.IPAttributesAdapter_GetAttributes(this, SpecificAttributeType, predicate);

        public PAttribute GetAttribute(string i_AttrID) 
            => PAttributesExt.IPAttributesAdapter_GetAttribute(this, i_AttrID);

        void IPAttributes.OnAttributeChange(string strPAttrID)
        {
            if (PropertyChanged != null)
            {
                PropertyChangedEventArgs PCEA = new PropertyChangedEventArgs(strPAttrID);
                PropertyChanged.Invoke(this, PCEA);
            }
        }
    }
}
