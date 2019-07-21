using System;
using System.Collections.Generic;
using System.ComponentModel;

using Utilities;


namespace DataModel
{
	public interface IPAttributes : INotifyPropertyChanged // "to support OneWay/TwoWay bindings, the underlying data must implement INotifyPropertyChanged"
	{
		ICollection<PAttribute> GetAttributes(Type i_SpecificAttributeType, Func<PAttribute, bool> i_Predicate = null);
		PAttribute GetAttribute(string i_AttrID);

		void OnAttributeChange(string i_AttrID);

		////-------------------------------------------------------------------------------------
		//// Permet de définir des règles d'état ReadOnly conditionnel.
		//// Attention : les évaluations faites dans cette méthode doivent être rapides. Tout calcul lourd (accès fichier...) doit être déporté dans le 'Compute', et le résultat de son évaluation stoqué sur l'objet.
		//// Pour l'instant la visibilité est contrôlée par la non-nullité du Get de la propriété (dont le type doit être Nullable). A voir si l'on souhaite continuer ainsi ou faire comme pour le ReadOnly.
		//void GetPFieldCurrentState(string i_AttrID, out bool o_Visible, out bool o_Editable, out bool o_Mandatory);
		//bool GetPFieldConditionnalUserText(string i_AttrID, out string o_ConditionnalUserTextForCategory, out string o_ConditionnalUserTextForLabel);
		////bool GetPFieldEnumPossibleValues(string i_AttrID, out List<string> o_EnumPossibleNames, out List<string> o_EnumPossibleIDs);
		//bool GetFieldDefaultTextValue(PAttribute i_Attribute, out string o_DefaultTextValue);

		////void GetPCmdSelectionCurrentState(string i_AttrID, out bool o_Enable);

		//-------------------------------------------------------------------------------------
		/// <summary>
		/// Possibilité de surcharger le code d'ajout d'un enfant. Cela a été demandé par OSD, mais en théorie le code devrait être générique et commun, basé essentiellement sur le type de l'objet enfant.
		/// </summary>
		/// <param name="i_AttrID"></param>
		/// <param name="nIndex">Attention : la convention générale est de considérer une valeur négative (ex : -1) comme une demane d'ajout en fin de liste. Ne pas confondre avec la valeur '0' qui doit insérer en tête de liste.</param>
		/// <returns></returns>
		//bool AttributeAddNewChild(string i_AttrID, int nIndex);

		// Interne : ne pas appeler par une autre méthode que IPAttributesAdapter_DeleteAttributeChild
		//void AttributeChildPreDelete(string i_AttrID, int nIndex, out bool o_bDeletedIsAllowed);

		//-------------------------------------------------------------------------------------
		// Exécute la vérification des attributs en Input (un Wrapper enregistrera le résultat dans StatusException)
		List<PError> DetectDefinitionErrors();

		//Exception DefinitionException { get; }
	}
}
