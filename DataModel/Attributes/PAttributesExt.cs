using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace DataModel
{
	public static class PAttributesExt
	{

		// *******************************************************
		// IPAttributes
		internal static ICollection<PAttribute> IPAttributesAdapter_GetAttributes(this IPAttributes ISA_Obj, Type SpecificAttributeType, Func<PAttribute, bool> predicate = null)
		{
			return DataManagement.GetAttributes(ISA_Obj.GetType(), SpecificAttributeType, predicate);
		}

		internal static PAttribute IPAttributesAdapter_GetAttribute(this IPAttributes ISA_Obj, string i_AttrID)
		{
			ICollection<PAttribute> ListOfAttributes = ISA_Obj.GetAttributes(typeof(PAttribute), null);
			PAttribute SA = ListOfAttributes.Where(sa => sa.AttrID == i_AttrID).DefaultIfEmpty(null).First();

			return SA;
		}

        #region Not needed yet
        //// --------------------------------------------------------------------------------------
        //// A surcharger pour définir des règles d'état ReadOnly conditionnel.
        //// Attention : les évaluations faites dans cette méthode doivent être rapides. Tout calcul lourd (accès fichier...) doit être déporté dans le 'Compute', et le résultat de son évaluation stoqué sur l'objet.
        //// Remarque : Par défaut la visibilité est contrôlée par la non-nullité du Get de la propriété (dont le type doit être Nullable).
        //internal static void IPAttributesAdapter_GetPFieldCurrentState(this IPAttributes ISA_Obj, string strPAttrID, out bool o_Visible, out bool o_Editable, out bool o_Mandatory) // IPObjectWrapper.
        //{
        //	Type ThisObjectWrapperType = ISA_Obj.GetType();
        //	PropertyInfo PI = DataModel.GetPropertyInfoForPAttribute(ThisObjectWrapperType, strPAttrID, out PAttribute SA);

        //	if (SA == null)
        //	{
        //		PError.AlertDevAboutUnexpectedError();

        //		o_Visible = false;
        //		o_Editable = false;
        //		o_Mandatory = false;
        //	}
        //	else
        //	{
        //		bool AttributeExistsForObject = SA.AttributeIsCompatibleWithObjectVersion(ISA_Obj);

        //		o_Visible = false;
        //		if (AttributeExistsForObject)
        //		{
        //			if (SA is PLinkAttribute)
        //			{
        //				o_Visible = true;
        //			}
        //			else
        //			{
        //				if (PI != null)
        //				{
        //					//bool CanBeNull = PI.PropertyType.IsNullableType(); //Nullable.GetUnderlyingType(Field.FieldType)
        //					try
        //					{
        //						if (PI.GetValue(ISA_Obj) != null)
        //						{
        //							o_Visible = true;
        //						}
        //						else
        //						{
        //							// Par défaut, une valeur 'nulle' signifie que l'attribut est caché (fonctionnement du Panel 'Properties' de SpaceClaim)
        //							o_Visible = false;
        //						}
        //					}
        //					catch (TargetInvocationException TIE)
        //					{
        //						if (TIE.InnerException is OperationCanceledException)
        //						{
        //							// se produit au cours d'un Pull sur un Body utilisé
        //						}
        //						else
        //						{
        //							// "MissingMethodException: Method not found: 'System.Nullable`1<!!0>"
        //							// Il est arrivé qu'on tombe ici pour des propriétés 'Nullable' => Si cela se reproduit, voir pourquoi et indiquer l'anomalie au développeur par un message en argument de l'alerte ci-dessous.
        //							TIE.AlertDevAboutUnexpectedError();
        //						}
        //					}
        //				}
        //			}
        //		}

        //		o_Editable = SA.EditBehavior != PAttributeEditBehavior.AlwaysReadOnly;
        //		o_Mandatory = false;
        //	}
        //}

        //// A surcharger pour les objets qui ont des attributs dont les textes du label et/ou de la catégorie qui peuvent changer en fonction de règles d'état
        //// Attention : les évaluations faites dans cette méthode doivent être rapides. Tout calcul lourd (accès fichier...) doit être déporté dans le 'Compute', et le résultat de son évaluation stoqué sur l'objet.
        //// Avertissement : avant de recourrir à cette technique, privilégier le fait d'avoir des attributs différents (car la plupart du temps si le texte change c'est qu'il ne s'agit pas de la même donnée).
        //internal static bool IPAttributesAdapter_GetPFieldConditionnalUserText(this IPAttributes ISA_Obj, string strPAttrID, out string o_ConditionnalUserTextForCategory, out string o_ConditionnalUserTextForLabel)
        //{
        //	bool ApplyConditionnalText = false;
        //	o_ConditionnalUserTextForCategory = null;
        //	o_ConditionnalUserTextForLabel = null;
        //	return ApplyConditionnalText;
        //}

        //// A surcharger pour les objets qui ont des attributs dont les textes du label et/ou de la catégorie qui peuvent changer en fonction de règles d'état
        //// Attention : les évaluations faites dans cette méthode doivent être rapides. Tout calcul lourd (accès fichier...) doit être déporté dans le 'Compute', et le résultat de son évaluation stoqué sur l'objet.
        //// Avertissement : avant de recourrir à cette technique, privilégier le fait d'avoir des attributs différents (car la plupart du temps si le texte change c'est qu'il ne s'agit pas de la même donnée).
        //internal static bool IPAttributesAdapter_GetPFieldDefaultTextValue(this IPAttributes ISA_Obj, PAttribute i_Attribute, out string o_DefaultTextValue)
        //{
        //	bool HasDefaultText = false;

        //	o_DefaultTextValue = i_Attribute?.NoValueText;

        //	if (!string.IsNullOrWhiteSpace(o_DefaultTextValue))
        //	{
        //		HasDefaultText = true;
        //	}

        //	return HasDefaultText;
        //}

        //// A surcharger pour les objets qui ont des attributs de type Enum dont les valeurs possibles dépendent de l'état de l'objet, ou doivent être fournies par le Kernel
        //internal static bool IPAttributesAdapter_GetPFieldEnumPossibleValues(this IPAttributes ISA_Obj, string strPAttrID, out List<string> o_EnumPossibleNames, out List<string> o_EnumPossibleIDs)
        //{
        //	o_EnumPossibleNames = null;
        //	o_EnumPossibleIDs = null;

        //	PAttribute SA = (ISA_Obj as IPAttributes).GetAttribute(strPAttrID);
        //	PEnumAttribute SEA = SA as PEnumAttribute;
        //	if (SEA == null)
        //	{
        //		PError.AlertDevAboutUnexpectedError("Attribute is not an Enum: " + strPAttrID);
        //		return false;
        //	}
        //	else
        //	{
        //		// Les valeurs possibles de l'enum sont définies globalement au niveau de l'attribut
        //		if (SEA.EnumType != null)
        //		{
        //			SEA.EnumTypeGetPossibleValues(out o_EnumPossibleIDs, out o_EnumPossibleNames);
        //			return true;
        //		}
        //	}

        //	return false;
        //}

        //// A surcharger si besoin pour définir si une selection est disponible ou pas 
        //internal static void IPAttributesAdapter_GetPCmdSelectionCurrentState(this IPAttributes ISA_Obj, string strPAttrID, out bool o_Enable) // IPObjectWrapper.
        //{
        //	ISA_Obj.GetPFieldCurrentState(strPAttrID, out bool bVisible, out o_Enable, out bool bMandatory);
        //}

        //// --------------------------------------------------------------------------------------
        //internal static void FindFieldIDIfNull(this IPAttributes ISA_Obj, ref string FieldID, int CallStackDepthFromProperty)
        //{
        //	// Par convention, si FieldID est null cela veut dire qu'il faut le récupérer par réflexion sur la propriété appelante
        //	if (FieldID == null)
        //	{
        //		PError.AlertDevAboutUnexpectedError("Cette convention n'est pas une bonne idée, et le field ID ne doit pas être 'null'");

        //		try
        //		{
        //			Type ThisObjectWrapperType = ISA_Obj.GetType();

        //			StackFrame SF = new StackFrame(CallStackDepthFromProperty + 1, false); // <= Attention : cette méthode ne doit être appelée directement ou indirectement que par des Propriétés de définition d'attributs à travers un nombre de couches d'appels passé en paramètre (Ex : CallStackDepthFromProperty==0 => appel direct) !
        //			MethodBase MB = SF.GetMethod();
        //			if (MB.Name.StartsWith("set_"))
        //			{
        //				string PropertyName = MB.Name.Substring("set_".Length);
        //				PropertyInfo PI = ThisObjectWrapperType.GetProperty(PropertyName);
        //				PAttribute SA = PI.GetCustomAttribute<PAttribute>();
        //				FieldID = SA.AttrID;
        //			}
        //		}
        //		catch
        //		{
        //		}
        //	}

        //	if (FieldID == null)
        //	{
        //		throw new Exception("Unknown FieldID");
        //	}
        //}

        //public static Exception DetectDefinitionErrorsAsException(this IPObjectWrapper i_ISOW)
        //{
        //	Exception o_Ex = null;

        //	List<PError> ListOfErrorWithInputField = null;
        //	try
        //	{
        //		ListOfErrorWithInputField = i_ISOW.DetectDefinitionErrors();
        //	}
        //	catch (Exception Ex)
        //	{
        //		ListOfErrorWithInputField = null;
        //		o_Ex = Ex;
        //	}

        //	if (ListOfErrorWithInputField != null && ListOfErrorWithInputField.Count > 0)
        //	{
        //		o_Ex = PError.MakeSingleExceptionFromErrorList(ListOfErrorWithInputField);
        //	}

        //	return o_Ex;
        //}

        //public static List<PError> CheckUserInputMandatoryAttributes(this IPAttributes ISA_Obj)
        //{
        //	List<PError> Result = new List<PError>();

        //	// Vérifier les champs mandatory
        //	ICollection<PAttribute> ListOfAttributes = ISA_Obj.GetAttributes(null);
        //	foreach (PAttribute SA in ListOfAttributes)
        //	{
        //		CheckUserInputMandatoryAttribute(ISA_Obj, SA, ref Result);
        //	}

        //	return Result;
        //}

        //public static void CheckUserInputMandatoryAttribute(IPAttributes ISA_Obj, PAttribute SA, ref List<PError> Result)
        //{
        //	// TODO : Possibilité de valuer une erreur sur un champ dans le Wrapper métier (Compute, Set...), en stoquant l'info dans un Dict<string AttrId, PError> du WrapperBase. Mais voir qd il faudrait vider cette valeur...
        //	// PError SSCE = SA_Obj.GetFieldError(SA.AttrID);
        //	// if (SSCE!=null) Result.Add(SSCE);

        //	ISA_Obj.GetPFieldCurrentState(SA.AttrID, out bool Visible, out bool Editable, out bool Mandatory); //if (SA.IsMandatoryFor(ISA_Obj)) ?
        //	if (Visible && Editable && Mandatory)
        //	{
        //		bool HasValue = SA.HasValueFor(ISA_Obj);

        //		switch (SA)
        //		{
        //			case PFileAttribute SFA:
        //				{
        //					string FilePathThatCanBeRelative = SA.PropertyInfo.GetValue(ISA_Obj) as string;
        //					if (string.IsNullOrWhiteSpace(FilePathThatCanBeRelative))
        //					{
        //						string FieldUserName = SFA.GetUserTextFor(ISA_Obj, true, true);
        //						// TODO : revoir ces erreurs pour qu'elles soient liées à des ressources (NLS)
        //						//PError.CreateFromResource(MessageSeverity.Warning, i_RM, Key, FieldUserName);
        //						PError SSCE = new PError() { FullMessage = "Missing file: " + FieldUserName };
        //						Result.Add(SSCE);
        //					}
        //					else
        //					{
        //						// On vérifie l'existance du fichier
        //						if (ISA_Obj is IPObjectWrapper ISOW)
        //						{
        //							string FilePathAbsolute = ISOW.PrepareFilePath(FilePathThatCanBeRelative, RaiseErrorIfFileNotFound: false);
        //							PError SSCE = UtilFile.TestFileExistance(FilePathAbsolute);
        //							if (SSCE != null)
        //							{
        //								Result.Add(SSCE);

        //							}
        //							/*if (ISOW is IPOpticalProperties) //Code specifique à eviter dans les classes de bases et d extensions !
        //							{
        //								if (SSCE != null)
        //								{
        //									ISOW.StatusException = new Exception(SSCE.FullMessage); //PError.MakeSingleExceptionFromErrorList(Result);
        //								}
        //								else
        //								{
        //									ISOW.StatusException = null;
        //								}
        //							}*/
        //						}
        //					}

        //					break;
        //				}
        //			case PLinkAttribute SSA:
        //				{
        //					if (!HasValue)
        //					{
        //						// TODO NLS : revoir ces erreurs pour qu'elles soient liées à des ressources (NLS)
        //						//PError.CreateFromResource(MessageSeverity.Warning, i_RM, Key, FieldUserName);
        //						Result.Add(new PError() { FullMessage = "Missing selection: " + SSA.GetUserTextFor(ISA_Obj, true, true) });
        //					}
        //					break;
        //				}
        //			case PChildAttribute SLA:
        //				{
        //					if (!HasValue)
        //					{
        //						// TODO NLS : revoir ces erreurs pour qu'elles soient liées à des ressources (NLS)
        //						//PError.CreateFromResource(MessageSeverity.Warning, i_RM, Key, FieldUserName);
        //						Result.Add(new PError() { FullMessage = "Missing item in list: " + SLA.GetUserTextFor(ISA_Obj, true, true) });
        //					}
        //					break;
        //				}

        //				// ********************************************************
        //				// TODO : à compléter au fur et à mesure des besoins !!!
        //				// ********************************************************
        //		}

        //	}
        //}

        //public static List<PError> CheckStatusOfInputObjects(this IPAttributes ISA_Obj)
        //{
        //	List<PError> o_InputErrors = new List<PError>();

        //	GetInputDocObjects(ISA_Obj, out List<Tuple<IDocObject, PAttribute>> InputDocObjects);

        //	foreach (Tuple<IDocObject, PAttribute> IDO_SA in InputDocObjects)
        //	{
        //		string AttrUserName = IDO_SA.Item2?.UserName ?? "null attribute";

        //		if (IDO_SA.Item1 == null)
        //		{
        //			PError SSCE = PError.CreateNoResource(MessageSeverity.Warning, "Null reference", $"Attribute '{AttrUserName}' is a null reference");
        //			o_InputErrors.Add(SSCE);
        //		}
        //		else
        //		{
        //			if (IDO_SA.Item1.IsDeleted)
        //			{
        //				// => "Attribute '{0}' references a deleted object"
        //				PError SSCE = PError.CreateFromResource(MessageSeverity.Warning, Resource_ErrorMsg.ResourceManager, "InputObjectIsDeleted", AttrUserName);
        //				o_InputErrors.Add(SSCE);
        //			}
        //			else
        //			{
        //				string ObjectUserName = IDO_SA.Item1.GetUserName();

        //				IPObjectWrapper ISOW = IDO_SA.Item1.GetPObjectWrapper();
        //				if (ISOW != null)
        //				{
        //					if (ISOW.StatusException != null)
        //					{
        //						// => "Attribute '{0}' references an object in error: '{1}'"
        //						PError SSCE = PError.CreateFromResource(MessageSeverity.Warning, Resource_ErrorMsg.ResourceManager, "InputObjectHasErrors", AttrUserName, ObjectUserName);
        //						o_InputErrors.Add(SSCE);
        //					}
        //				}
        //			}
        //		}
        //	}

        //	return o_InputErrors;
        //}

        //public static IEnumerable<IPObjectWrapper> GetInputPObjectWrappers(this IPAttributes ISA_Obj)
        //{
        //	PAttributesExt.GetInputPObjects(ISA_Obj, out List<Tuple<ICustomObject, PAttribute>> InputPObjects);

        //	return InputPObjects.Select(ICO_SA => ICO_SA?.Item1?.GetPObjectWrapper()).Where(isow => isow != null);
        //}

        //public static void GetInputPObjects(this IPAttributes ISA_Obj, out List<Tuple<ICustomObject, PAttribute>> o_InputPObjects)
        //{
        //	o_InputPObjects = new List<Tuple<ICustomObject, PAttribute>>();

        //	ICollection<PAttribute> SAs = ISA_Obj.GetAttributes(null, SA => (SA is PLinkAttribute || SA is PChildAttribute) && SA.AttributeRole == PAttributeRole.ComputateIn);
        //	foreach (PAttribute SA in SAs)
        //	{
        //		if (SA is PLinkAttribute SSA)
        //		{
        //			object AttributeList = SA.PropertyInfo?.GetValue(ISA_Obj);
        //			if (AttributeList != null)
        //			{
        //				if (SSA.Multiple)
        //				{
        //					if (AttributeList is IList<IDocObject> ListOfSelectedObjects)
        //					{
        //						foreach (IDocObject IDO in ListOfSelectedObjects)
        //						{
        //							if (IDO.IsPObject(out ICustomObject ICO))
        //							{
        //								o_InputPObjects.Add(new Tuple<ICustomObject, PAttribute>(ICO, SA));
        //							}
        //						}
        //					}
        //				}
        //				else
        //				{
        //					// PError.AlertDevAboutUnexpectedError($"Is up to date error : PLinkAttribute '{SA.AttrID}' > IDocObject list expected");
        //				}
        //			}
        //		}
        //		else
        //		{
        //			if (SA is PChildAttribute SCA && SCA.IsPWrapper)
        //			{
        //				// Get its content from its property info
        //				object SourcePropertyValue = SCA.PropertyInfo?.GetValue(ISA_Obj);
        //				if (SourcePropertyValue != null)
        //				{
        //					if (SCA.Multiple)
        //					{
        //						if (SourcePropertyValue is IBindingList ChildIBL)
        //						{
        //							foreach (var CurrentIteratedChildAttribute in ChildIBL)
        //							{
        //								if (CurrentIteratedChildAttribute is IDocObject IDO)
        //								{
        //									if (IDO.IsPObject(out ICustomObject ICO))
        //									{
        //										o_InputPObjects.Add(new Tuple<ICustomObject, PAttribute>(ICO, SA));
        //									}
        //								}
        //							}
        //						}
        //					}
        //					else
        //					{
        //						//PError.AlertDevAboutUnexpectedError($"Is up to date error : child attribute '{SA.AttrID}' > binding list expected");
        //						if (SourcePropertyValue is IPObjectWrapper IChildOW)
        //						{
        //							o_InputPObjects.Add(new Tuple<ICustomObject, PAttribute>(IChildOW.GetCustomObject(), SA));
        //						}
        //					}
        //				}
        //			}
        //		}
        //	}
        //}

        //public static void GetInputDocObjects(this IPAttributes ISA_Obj, out List<Tuple<IDocObject, PAttribute>> o_InputDocObjects)
        //{
        //	o_InputDocObjects = new List<Tuple<IDocObject, PAttribute>>();

        //	ICollection<PAttribute> SAs = ISA_Obj.GetAttributes(null, SA => (SA is PLinkAttribute || SA is PChildAttribute) && SA.AttributeRole == PAttributeRole.ComputateIn);
        //	foreach (PAttribute SA in SAs)
        //	{
        //		if (SA is PLinkAttribute SSA)
        //		{
        //			object AttributeValue = SA.PropertyInfo?.GetValue(ISA_Obj);
        //			if (AttributeValue != null)
        //			{
        //				IList<IDocObject> ListOfSelectedObjects = AttributeValue as IList<IDocObject>;
        //				if (ListOfSelectedObjects != null)
        //				{
        //					foreach (IDocObject IDO in ListOfSelectedObjects)
        //					{
        //						o_InputDocObjects.Add(new Tuple<IDocObject, PAttribute>(IDO, SA));
        //					}
        //				}
        //				else if (AttributeValue is IDocObject IDO)
        //				{
        //					o_InputDocObjects.Add(new Tuple<IDocObject, PAttribute>(IDO, SA));
        //				}
        //			}
        //		}
        //		else if (SA is PChildAttribute SCA)
        //		{
        //			object SourcePropertyValue = SCA.PropertyInfo?.GetValue(ISA_Obj);
        //			if (SourcePropertyValue != null)
        //			{
        //				if (SCA.Multiple)
        //				{
        //					if (SCA.IsPWrapper) // Liste d'enfants Wrappers (DocObjects)
        //					{
        //						IEnumerable ChildrenCustomObjects = SourcePropertyValue as IEnumerable;
        //						Debug.Assert(ChildrenCustomObjects != null);
        //						if (ChildrenCustomObjects != null)
        //						{
        //							foreach (object PCustomObject in ChildrenCustomObjects)
        //							{
        //								ICustomObject ICO = PCustomObject as ICustomObject;
        //								if (ICO != null)
        //								{
        //									o_InputDocObjects.Add(new Tuple<IDocObject, PAttribute>(ICO, SA));
        //								}
        //							}
        //						}
        //					}
        //					else // Liste de sous-objets non-Wrappers
        //					{
        //						IBindingList ChildrenIBL = SourcePropertyValue as IBindingList;
        //						Debug.Assert(ChildrenIBL != null);
        //						if (ChildrenIBL != null)
        //						{
        //							foreach (object CurrentIteratedChild in ChildrenIBL)
        //							{
        //								IPAttributes ISA = CurrentIteratedChild as IPAttributes;
        //								if (ISA != null)
        //								{
        //									GetInputDocObjects(ISA, out List<Tuple<IDocObject, PAttribute>> ChildInputWrappers); // => Récursif : récupérer les Wrappers enfants
        //									o_InputDocObjects.AddRange(ChildInputWrappers);
        //								}
        //							}
        //						}
        //					}
        //				}
        //				else // Objet unique aggrégé (ex : HOA)
        //				{
        //					if (SCA.IsPWrapper) // Liste d'enfants Wrappers (DocObjects)
        //					{
        //						ICustomObject ICO = SourcePropertyValue as ICustomObject;
        //						if (ICO == null)
        //						{
        //							// L'enfant n'existe pas (attribut inactif)
        //						}
        //						else
        //						{
        //							o_InputDocObjects.Add(new Tuple<IDocObject, PAttribute>(ICO, SA));
        //						}
        //					}
        //					else // Liste de sous-objets non-Wrappers
        //					{
        //						IPAttributes ISA = SourcePropertyValue as IPAttributes;
        //						if (ISA == null)
        //						{
        //							// L'enfant n'existe pas (attribut inactif)
        //						}
        //						else
        //						{
        //							GetInputDocObjects(ISA, out List<Tuple<IDocObject, PAttribute>> ChildInputWrappers); // => Récursif : récupérer les Wrappers enfants
        //							o_InputDocObjects.AddRange(ChildInputWrappers);
        //						}
        //					}
        //				}
        //			}
        //		}
        //	}
        //}

        internal static bool SetField<X>(this IPAttributes ISA_Obj, ref X r_Field, X i_Value, string i_AttrID) // " [CallerMemberName] string FieldID = null " <= attention : le nom de propriété peut être différent du nom P
        {
            Debug.Assert(ISA_Obj != null);

            //// Par convention, si FieldID est null cela veut dire qu'il faut le récupérer par réflexion sur la propriété appelante
            //PAttributesExt.FindFieldIDIfNull(ISA_Obj, ref i_AttrID, 2);

            //// ------------------------------------------------------------------------------------
            //// Garde-fou pour empêcher un bug dans la définition des attributs P.
            //if (UtilEnv.IsBuiltInDebugMode)
            //{
            //    if (r_Field == null)
            //    {
            //        // Champ Nullable vide
            //        // => Il faudrait tester d'après le type d'attribut de la propriété "i_AttrID" de la classe de ISA_Obj
            //    }
            //    else
            //    {
            //        Type T = r_Field.GetType();
            //        bool bIsSerializableBySpaceClaim = UtilSerializer.IsTypeSerializableBySpaceClaim(T);
            //        if (!bIsSerializableBySpaceClaim)
            //        {
            //            // /!\ Si quelqu'un tombe ici c'est qu'il a utilisé un type non-sérialisable pour un attribut P.
            //            // C'est un bug car la valeur ne sera pas sauvegardée (et entre-autre le Undo ne fonctionnera pas)
            //            //PError.AlertDevAboutUnexpectedError("P attribute must only set its value on a field whose type is upported by SpaceClaim serialization.");
            //        }
            //    }
            //}
            // Remarque : on pourrait faire de l'analyse syntaxique de code plutôt que de tester au RunTime.
            // https://github.com/dotnet/roslyn/wiki/How-To-Write-a-C%23-Analyzer-and-Code-Fix 
            // https://stackoverflow.com/questions/19594847/how-to-get-all-properties-that-are-anotated-with-some-attribute
            // ------------------------------------------------------------------------------------



            if (EqualityComparer<X>.Default.Equals(r_Field, i_Value))
            {
                return false;
            }
            else
            {
                //if (ISA_Obj is IPObjectWrapper)
                //{
                //    if (!WriteBlock.IsActive)
                //    {
                //        // A priori cette situation n'est pas normale : si cela se produit voir s'il y a des cas "légitimes" avant de modifier ce test.
                //        PError.AlertDevAboutUnexpectedError("SetField called outside WriteBlock for a CustomWrapper"); // => ne provoque pas d'événement "SessionChanged", ne permet pas d'Undo, etc...
                //    }
                //}

                r_Field = i_Value;
                ISA_Obj.OnAttributeChange(i_AttrID);

                return true;
            }
        }

        internal static X GetField<X>(this IPAttributes ISA_Obj, X InternalFieldValue, bool ConditionForExistance = true)
        {
            if (ConditionForExistance)
            {
                return InternalFieldValue;
            }
            else
            {
                return default(X); // <= "string is a reference type and the default value for all reference types is null."
            }
        }


        //internal static void SetPFieldNullable<NonNullableGenericType>(this IPAttributes ISA_Obj, ref NonNullableGenericType InternalNonNullableField, NonNullableGenericType? NullableValue, string FieldID = null)
        //	where NonNullableGenericType : struct, IComparable // "need the constraint to where T : struct, IComparable to ensure that T can only be a value type"
        //{
        //	if (NullableValue.HasValue)
        //	{
        //		PAttributesExt.FindFieldIDIfNull(ISA_Obj, ref FieldID, 2);

        //		SetPField(ISA_Obj, ref InternalNonNullableField, NullableValue.Value, FieldID);
        //	}
        //	else
        //	{
        //		// Set d'une valeur 'null' sur une propriété 'Nullable'.
        //		// Le champ sérialisé ne peut pas être lui-même nullable donc on se contente de ne rien faire.
        //		// Ne pas lever d'exception car cela bloquerait certains algos génériques (exemple: Ctrl+Move pour copier un capteur d'intensité ne marcherait pas)
        //		// throw new Exception("Value cannot be set to null"); <= NON
        //	}
        //}


        //internal static void SetPSelectionField(this IPAttributes ISA_Obj, ref PLink r_Field, IDocObject i_DocObject, string i_AttrID)  // " [CallerMemberName] string FieldID = null " <= attention : le nom de propriété peut être différent du nom P
        //{
        //	Debug.Assert(ISA_Obj != null);

        //	// Par convention, si FieldID est null cela veut dire qu'il faut le récupérer par réflexion sur la propriété appelante
        //	PAttributesExt.FindFieldIDIfNull(ISA_Obj, ref i_AttrID, 2);


        //	if (EqualityComparer<IDocObject>.Default.Equals(r_Field?.PointedObject, i_DocObject))
        //	{

        //	}
        //	else
        //	{
        //		PLinkAttribute selectionAttribute = ISA_Obj.GetAttribute(i_AttrID) as PLinkAttribute;

        //		if (selectionAttribute != null)
        //		{
        //			bool bIsCompatible = false;
        //			switch (selectionAttribute.SelectionType)
        //			{
        //				case PSelectionType.AXIS:
        //					if (i_DocObject.IsA(EnumGeometryDimensionType.LINEAR))
        //					{
        //						bIsCompatible = true;
        //					}
        //					break;
        //				case PSelectionType.POINT:
        //					if (i_DocObject.IsA(EnumGeometryDimensionType.PONCTUAL))
        //					{
        //						bIsCompatible = true;
        //					}
        //					break;
        //				case PSelectionType.FACE:
        //					if (i_DocObject.IsA(EnumGeometryDimensionType.SURFACIC))
        //					{
        //						bIsCompatible = true;
        //					}
        //					break;
        //				case PSelectionType.CURVE:
        //					if (i_DocObject.IsA(EnumGeometryDimensionType.CURVE))
        //					{
        //						bIsCompatible = true;
        //					}
        //					break;
        //				case PSelectionType.BODY:
        //					if (i_DocObject.IsA(EnumGeometryDimensionType.SOLID))
        //					{
        //						bIsCompatible = true;
        //					}
        //					break;
        //				case PSelectionType.NO_DEFINED:
        //					bIsCompatible = true; // Dans ce cas il faut s'assurer que la selection est compatible avec les types définis dans AllowableTypes []
        //					break;
        //			}

        //			if (bIsCompatible || i_DocObject == null) // Si i_DocObject est null on n'a pas besoin de vérifier la compatibilité
        //			{
        //				r_Field = new PLink(i_DocObject);

        //				// c'est un PLink donc on doit sérialiser
        //				ISA_Obj.OnAttributeChange(i_AttrID);
        //			}
        //			else
        //			{
        //				//throw new NotImplementedException();
        //				const string Message = "Selection non compatible";
        //				Exception e = new Exception(Message);
        //				e.PopUpAsReportStatus();
        //				throw e;

        //			}
        //		}
        //	}
        //}

        //internal static void SetPSelectionField(this IPAttributes ISA_Obj, ref IDocObject r_Field, IDocObject i_DocObject, string i_AttrID)  // " [CallerMemberName] string FieldID = null " <= attention : le nom de propriété peut être différent du nom P
        //{
        //	Debug.Assert(ISA_Obj != null);

        //	// Par convention, si FieldID est null cela veut dire qu'il faut le récupérer par réflexion sur la propriété appelante
        //	PAttributesExt.FindFieldIDIfNull(ISA_Obj, ref i_AttrID, 2);


        //	if (EqualityComparer<IDocObject>.Default.Equals(r_Field, i_DocObject))
        //	{

        //	}
        //	else
        //	{
        //		PLinkAttribute selectionAttribute = ISA_Obj.GetAttribute(i_AttrID) as PLinkAttribute;

        //		if (selectionAttribute != null)
        //		{
        //			bool bIsCompatible = false;
        //			switch (selectionAttribute.SelectionType)
        //			{
        //				case PSelectionType.AXIS:
        //					if (i_DocObject.IsA(EnumGeometryDimensionType.LINEAR))
        //					{
        //						bIsCompatible = true;
        //					}
        //					break;
        //				case PSelectionType.POINT:
        //					if (i_DocObject.IsA(EnumGeometryDimensionType.PONCTUAL))
        //					{
        //						bIsCompatible = true;
        //					}
        //					break;
        //				case PSelectionType.FACE:
        //					if (i_DocObject.IsA(EnumGeometryDimensionType.SURFACIC))
        //					{
        //						bIsCompatible = true;
        //					}
        //					break;
        //				case PSelectionType.CURVE:
        //					if (i_DocObject.IsA(EnumGeometryDimensionType.CURVE))
        //					{
        //						bIsCompatible = true;
        //					}
        //					break;
        //				case PSelectionType.BODY:
        //					if (i_DocObject.IsA(EnumGeometryDimensionType.SOLID))
        //					{
        //						bIsCompatible = true;
        //					}
        //					break;
        //				case PSelectionType.NO_DEFINED:
        //					bIsCompatible = true; // Dans ce cas il faut s'assurer que la selection est compatible avec les types définis dans AllowableTypes []
        //					break;
        //			}

        //			if (bIsCompatible || i_DocObject == null) // Si i_DocObject est null on n'a pas besoin de vérifier la compatibilité
        //			{
        //				r_Field = i_DocObject;
        //				ISA_Obj.OnAttributeChange(i_AttrID);
        //			}
        //			else
        //			{
        //				//throw new NotImplementedException();
        //				const string Message = "Selection non compatible";
        //				Exception e = new Exception(Message);
        //				e.PopUpAsReportStatus();
        //				throw e;

        //			}
        //		}
        //	}
        //}

        //public static NonNullableGenericType? GetPFieldNullable<NonNullableGenericType>(this IPAttributes ISA_Obj, NonNullableGenericType InternalNonNullableField, bool ConditionForExistance)
        //	where NonNullableGenericType : struct, IComparable // "need the constraint to where T : struct, IComparable to ensure that T can only be a value type"
        //{
        //	if (ConditionForExistance)
        //	{
        //		return InternalNonNullableField;
        //	}
        //	else
        //	{
        //		return null;
        //	}
        //}

        //// --------------------------------------------------------------------------------------------
        //private static List<IDocObject> GetDeterminantsRelatedToSelectionAttributes(this IPAttributes i_ItfPObject)
        //{
        //	List<IDocObject> Result = new List<IDocObject>(); // => HashSet (ISet) garantirait l'unicité, mais n'a pas  de Addrange

        //	ICollection<PAttribute> ListOfImpactingAttributes = i_ItfPObject.GetAttributes(typeof(PLinkAttribute), SA => SA.AttributeRole == PAttributeRole.ComputateIn);
        //	foreach (PLinkAttribute SA in ListOfImpactingAttributes)
        //	{
        //		object Value = null;
        //		try
        //		{
        //			Value = SA.PropertyInfo.GetValue(i_ItfPObject);
        //		}
        //		catch (Exception Ex)
        //		{
        //			Ex.AlertDevAboutUnexpectedError();
        //			Value = null;
        //		}

        //		if (Value != null)
        //		{
        //			IList<IDocObject> ValueAsListOfDocObject = Value as List<IDocObject>;
        //			IList<ICustomObject> ValueAsListOfCO = Value as List<ICustomObject>;

        //			if (ValueAsListOfDocObject != null)
        //			{
        //				Result.AddRange(ValueAsListOfDocObject);
        //			}
        //			else
        //			{
        //				if (null != ValueAsListOfCO)
        //				{
        //					foreach (IDocObject docObj in ValueAsListOfCO) // les listes ne font pas l'ajout elles-même
        //					{
        //						Result.Add(docObj);
        //					}
        //				}
        //				else
        //				{
        //					if (Value is IDocObject SingleValue)
        //					{
        //						if (SingleValue is PLink sl)
        //						{
        //							Result.Add(sl.PointedObject);
        //						}
        //						else
        //						{
        //							Result.Add(SingleValue);
        //						}
        //					}
        //				}
        //			}
        //		}
        //	}

        //	return Result;
        //}

        //public static ICollection<IDocObject> GetDeterminantsRelatedToAttributes(this IPAttributes i_ItfPObject)
        //{
        //	Debug.Assert(i_ItfPObject != null);

        //	List<IDocObject> Result = GetDeterminantsRelatedToSelectionAttributes(i_ItfPObject);

        //	// Traitement des attributs sur les sous-objets
        //	ICollection<PAttribute> ListOfChildrenImpactingAttributes = i_ItfPObject.GetAttributes(typeof(PChildAttribute), SA => SA.AttributeRole == PAttributeRole.ComputateIn);
        //	foreach (PChildAttribute SCA in ListOfChildrenImpactingAttributes)
        //	{
        //		bool IsAttributeExisting = SCA.IsVisibleFor(i_ItfPObject);
        //		if (!IsAttributeExisting)
        //		{
        //			// l'attribut n'existe pas dans l'état actuel de l'objet donc on ne le prend pas en compte.
        //			// Remarque : s'il reste des sélections dans un champ inactif il faudrait peut-être les vider lorsque l'attribut est désactivé (sur le OnAttributeChange de celui qui en est la cause ?)
        //		}
        //		else
        //		{
        //			List<IPAttributes> Children = SCA.GetChildren(i_ItfPObject);
        //			foreach (IPAttributes ISA in Children)
        //			{
        //				ICollection<IDocObject> ChildResult = GetDeterminantsRelatedToSelectionAttributes(ISA);
        //				if (ChildResult != null)
        //				{
        //					Result.AddRange(ChildResult);
        //				}
        //			}
        //		}
        //	}

        //	return Result;
        //}

        //public static void AddOrRemoveChildAttributeList(this IPAttributes i_ItfPObject, PAttribute iSA, PChildAttribute iChildObject)
        //{
        //	PropertyInfo PI = iSA.PropertyInfo;

        //	object attributeValue = PI.GetValue(i_ItfPObject);

        //	if (iSA is PChildAttribute SCA)
        //	{
        //		if (attributeValue != null)
        //		{
        //			IBindingList AttrChildrenList = attributeValue as IBindingList;
        //			if (AttrChildrenList == null)
        //			{
        //				PError.AlertDevAboutUnexpectedError("BindingList expected");
        //			}
        //			else
        //			{
        //				PChildAttribute AlreadyinListSOB = null;
        //				{
        //					foreach (var v in AttrChildrenList)
        //					{
        //						PChildAttribute SOB = v as PChildAttribute;
        //						if (SOB != null)
        //						{
        //							if (SOB == iChildObject)
        //							{
        //								AlreadyinListSOB = SOB;
        //								break;
        //							}
        //						}
        //					}

        //				}

        //				if (AlreadyinListSOB != null)
        //				{
        //					AttrChildrenList.Remove(AlreadyinListSOB);
        //				}
        //				else
        //				{
        //					PChildAttribute NewPObject = null;
        //					try
        //					{
        //						NewPObject = AttrChildrenList.AddNew() as PChildAttribute; // <= Pratique !
        //					}
        //					catch (Exception Ex)
        //					{
        //						Ex.AlertDevAboutUnexpectedError();
        //					}

        //					if (NewPObject != null)
        //					{
        //						//i_ItfPObject.LinkToParent(i_ItfPObject, NewPObject);
        //						NewPObject = iChildObject; // <= TEMP ! : il faudrait sans doute se baser sur IToolBehavior à laquelle le PObject doit adhérer !!!
        //					}
        //				}
        //			}
        //		}
        //	}
        //	//else
        //	//{
        //	//	int toto = -1;
        //	//	//if (iSA is PLinkAttribute SSA)
        //	//	//{
        //	//	//	List<IDocObject> AttrList = attributeValue as List<IDocObject>;

        //	//	//	if (AttrList != null)
        //	//	//	{
        //	//	//		Debug.Assert(SSA.Multiple);

        //	//	//		if (AttrList.Contains(iDocObject))
        //	//	//		{
        //	//	//			AttrList.Remove(iDocObject);
        //	//	//			OnListChanged();
        //	//	//		}
        //	//	//		else
        //	//	//		{
        //	//	//			// If the selected item is a face, we check if this face is not already part of the selection
        //	//	//			if (iDocObject is IDesignFace && AttrList.Count >= 1)
        //	//	//			{
        //	//	//				// check if the selected face's ancestor is part of the list and add the face in the list if not
        //	//	//				IDesignBody desBody = iDocObject.GetAncestor<IDesignBody>();
        //	//	//				if (!AttrList.Contains(desBody))
        //	//	//				{
        //	//	//					AttrList.Add(iDocObject);
        //	//	//					OnListChanged();
        //	//	//				}
        //	//	//				else
        //	//	//				{
        //	//	//					Application.ReportStatus("Face already part of a selected Solid", StatusMessageType.Information, null); // TODO NLS
        //	//	//				}
        //	//	//			}
        //	//	//			// If the selected item is a body, we check if one face of this body is not already part of the selection
        //	//	//			else if (iDocObject is IDesignBody && AttrList.Count >= 1)
        //	//	//			{
        //	//	//				IDesignBody desBody = iDocObject as IDesignBody;
        //	//	//				bool tmpFoundFace = false;
        //	//	//				foreach (var desface in desBody.Faces)
        //	//	//				{
        //	//	//					// check if on face of the selected body is part of the list
        //	//	//					if (AttrList.Contains(desface))
        //	//	//					{
        //	//	//						tmpFoundFace = true;
        //	//	//						Application.ReportStatus("Solid contains a Face already part of the selection", StatusMessageType.Information, null); // TODO NLS
        //	//	//						break;
        //	//	//					}
        //	//	//				}
        //	//	//				// if no face has been found, add the body in the list
        //	//	//				if (!tmpFoundFace)
        //	//	//				{
        //	//	//					AttrList.Add(iDocObject);
        //	//	//					OnListChanged();
        //	//	//				}
        //	//	//			}
        //	//	//			else
        //	//	//			{
        //	//	//				AttrList.Add(iDocObject);
        //	//	//				OnListChanged();
        //	//	//			}
        //	//	//		}
        //	//	//	}
        //	//	//	else
        //	//	//	{
        //	//	//		Debug.Assert(!SSA.Multiple);

        //	//	//		IDocObject tmpObj = PI.GetValue(PObjectItf) as IDocObject;

        //	//	//		if (tmpObj == iDocObject)
        //	//	//		{
        //	//	//			PI.SetValue(PObjectItf, null);
        //	//	//		}
        //	//	//		else
        //	//	//		{
        //	//	//			PI.SetValue(PObjectItf, iDocObject);
        //	//	//		}

        //	//	//		OnListChanged();
        //	//	//	}

        //	//	//	// -----------------------------------------
        //	//	//	PObjectItf.OnAttributeChange(iSA.ID);
        //	//	//}
        //	//}
        //}

        //public static bool CleanLostObjectReferences(this IPAttributes ISA_Obj)
        //{
        //	bool o_ObjectWasModified = false;

        //	ICollection<PAttribute> ListSA = ISA_Obj.GetAttributes(null, SA => (SA is PLinkAttribute || SA is PChildAttribute) && SA.AttributeRole == PAttributeRole.ComputateIn);
        //	foreach (PAttribute SA in ListSA)
        //	{
        //		if (SA is PLinkAttribute SSA)
        //		{
        //			object AttributeValue = SA.PropertyInfo?.GetValue(ISA_Obj);
        //			if (AttributeValue != null)
        //			{
        //				IList<IDocObject> ListOfSelectedObjects = AttributeValue as IList<IDocObject>;
        //				if (ListOfSelectedObjects != null)
        //				{
        //					o_ObjectWasModified = ListOfSelectedObjects.RemoveWhere(ido => ido == null || ido.IsDeleted);
        //				}
        //				else if (AttributeValue is IDocObject IDO)
        //				{
        //					if (IDO.IsDeleted)
        //					{
        //						SA.PropertyInfo?.SetValue(ISA_Obj, null); // => Set => PCommit (souvent inclus dans l'implémentation du Set de la propriété) => ArgumentException si contient un objet Deleted.
        //						o_ObjectWasModified = true;
        //					}
        //				}
        //			}
        //		}
        //		else if (SA is PChildAttribute SCA)
        //		{
        //			object SourcePropertyValue = SCA.PropertyInfo?.GetValue(ISA_Obj);
        //			if (SourcePropertyValue != null)
        //			{
        //				if (SCA.Multiple)
        //				{
        //					if (SCA.IsPWrapper) // Liste d'enfants Wrappers (DocObjects)
        //					{
        //						IList<IDocObject> ChildrenCustomObjects = SourcePropertyValue as IList<IDocObject>;
        //						Debug.Assert(ChildrenCustomObjects != null);
        //						if (ChildrenCustomObjects != null)
        //						{
        //							o_ObjectWasModified = ChildrenCustomObjects.RemoveWhere(ido => ido == null || ido.IsDeleted);
        //						}
        //					}
        //					else // Liste de sous-objets non-Wrappers
        //					{
        //						IBindingList ChildrenIBL = SourcePropertyValue as IBindingList;
        //						Debug.Assert(ChildrenIBL != null);
        //						if (ChildrenIBL != null)
        //						{
        //							List<IPAttributes> ChildrenToRemove = new List<IPAttributes>();
        //							foreach (object CurrentIteratedChild in ChildrenIBL)
        //							{
        //								IPAttributes ISA = CurrentIteratedChild as IPAttributes;
        //								if (ISA != null)
        //								{
        //									bool ChildCleaned = CleanLostObjectReferences(ISA); // => Récursif
        //									if (ChildCleaned)
        //									{
        //										ChildrenToRemove.Add(ISA);
        //										o_ObjectWasModified = true;
        //									}
        //								}
        //							}

        //							foreach (IPAttributes Child in ChildrenToRemove)
        //							{
        //								ChildrenIBL.Remove(Child);
        //							}
        //						}
        //					}
        //				}
        //				else // Objet unique aggrégé
        //				{
        //					if (SCA.IsPWrapper) // enfant Wrapper (DocObjects)
        //					{
        //						ICustomObject ICO = SourcePropertyValue as ICustomObject;
        //						if (ICO != null && ICO.IsDeleted)
        //						{
        //							SA.PropertyInfo?.SetValue(ISA_Obj, null);
        //							o_ObjectWasModified = true;
        //						}
        //					}
        //					else // sous-objets non-Wrapper
        //					{
        //						IPAttributes ISA = SourcePropertyValue as IPAttributes;
        //						Debug.Assert(ISA != null);
        //						if (ISA != null)
        //						{
        //							o_ObjectWasModified = CleanLostObjectReferences(ISA); // => Récursif
        //						}
        //					}
        //				}
        //			}
        //		}
        //	}

        //	return o_ObjectWasModified;
        //}


        ///*
        // * Anciennement, on essayait de faire les actions unitairement sur la liste mais étant donné qu'il est prévu de permettre à l'utilisateur
        // * de faire des sélection multiple, par option, et que chaque item ici fera de la sérialization des monikers.
        // * il vaut mieux limiter ces actions, donc on gère l'ajout de tout les items de la liste ici.
        // *
        // * Comme la sélection SpaceClaim, si on trouve un des items sélectionnés dans la liste de sélection, on switch en mode "remove",
        // * et on ne traite que ces éléments
        // *
        // * 
        // *
        // */
        //public static void ApplySelection(this IPAttributes i_ItfPObject, PAttribute iSA,
        //	ICollection<IDocObject> listOfSelectedElements)
        //{
        //	if (i_ItfPObject != null && iSA != null && listOfSelectedElements != null &&
        //		iSA.PropertyInfo != null && listOfSelectedElements.Count != 0)
        //	{
        //		PropertyInfo PAttributeProperty = iSA.PropertyInfo;

        //		if (iSA is PLinkAttribute selectionAttribute)
        //		{
        //			object AttributeList = PAttributeProperty.GetValue(i_ItfPObject);
        //			if (AttributeList is IList<IDocObject> listOfDocObjects &&
        //				selectionAttribute.Multiple)
        //			{
        //				IEnumerable<IDocObject> listOfObjectsToRemove = listOfDocObjects.Union(listOfSelectedElements);
        //				if (listOfObjectsToRemove.Count() == 0)
        //				{
        //					foreach (IDocObject o in listOfSelectedElements)
        //					{
        //						//TODO : ajouter le filtrage par type supporté par l'attribut
        //						listOfDocObjects.Add(o);

        //					}
        //				}
        //				else
        //				{
        //					foreach (IDocObject o in listOfObjectsToRemove)
        //					{
        //						listOfDocObjects.Remove(o);
        //					}
        //				}
        //			}
        //			else if (AttributeList is IDocObject singleDocObject &&
        //					 !selectionAttribute.Multiple)
        //			{

        //			}
        //			else
        //			{
        //				PError.AlertDevAboutUnexpectedError("should be an IList of IDocObject or single IDocObject, or are badly paired");
        //			}
        //		}
        //		else if (iSA is PChildAttribute childAttribute)
        //		{
        //			//childAttribute.
        //		}
        //	}
        //	else
        //	{
        //		PError.AlertDevAboutUnexpectedError("ApplySelection input attributes are invalid");
        //	}
        //}
        #endregion
    }
}
