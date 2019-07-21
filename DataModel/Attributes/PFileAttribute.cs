using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Utilities;
using Interfaces;

namespace DataModel
{
    [Serializable]
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public class PFileAttribute : PAttribute
    {
        public PFileAttribute(string ID) : base(ID)
        {
        }

        /// <summary>
        /// Par convention :
        /// - Si la définition contient des '|' le filtre doit obéir directement au format attendu par le FileDialog.
        ///		Ex : "Eulumdat|*.elumdat|Intensity files|*.intensity;*.ldt;*.ies|XMP|*.xmp"
        /// - Sinon la définition utilise les types définis par OptisCore (cf constOptisFileType) séparés par des virgules.
        ///		Ces derniers peuvent être groupés avec des '+', auquel cas il faut nommer le groupe d'après une resource en utilisant '='.
        ///		Le symbole '*' ajoute le filtre pour tous les fichiers (*.*)
        ///		Ex : "fileTypeEulumdat, fileTypeXMP, fileTypeIES + fileTypeOptisIntensity = Resources.PropertyDisplayFileFilterAllFiles, *"
        /// </summary>
        public string Filter
        {
            set
            {
                _FilterExtensions = value; // TransformKernelFilterEnumToExtensions(value, Resources.FileFilterAllFiles, Resources.FileFilterSupportedFiles);
            }
            get
            {
                return _FilterExtensions;
            }
        }
        string _FilterExtensions;

        //static string TransformKernelFilterEnumToExtensions(string i_FilterThatCanBeKernelFileTypes, string i_DescriptionForAllFiles = null, string i_DescriptionForAllSupportedFiles = null)
        //{
        //    string o_FileTypeFilterForFileDialog = string.Empty;

        //    if (i_FilterThatCanBeKernelFileTypes.Contains("|"))
        //    {
        //        // La définition d'attribut donne directement un filtre 'WinFormsDialog'
        //        o_FileTypeFilterForFileDialog = i_FilterThatCanBeKernelFileTypes;
        //    }
        //    else
        //    {
        //        // La définition d'attribut donne un filtre au format 'OptisCore'
        //        List<string> AllSupportedExtensions = new List<string>();

        //        List<string> Types = new List<string>();
        //        string[] OptisCoreTypes = i_FilterThatCanBeKernelFileTypes.Split(new char[] { ',' });
        //        foreach (string OptisCoreType in OptisCoreTypes)
        //        {
        //            string OptisCoreTypeTrimmed = OptisCoreType.Trim();

        //            string Description = string.Empty;
        //            List<string> ListOfExtensionsFortype = new List<string>();

        //            if (OptisCoreTypeTrimmed == "*")
        //            {
        //                //Description = Resources.PropertyDisplayFileFilterAllFiles;
        //                Description = i_DescriptionForAllFiles ?? string.Empty;
        //                ListOfExtensionsFortype.Add("*.*");
        //            }
        //            else
        //            {
        //                // Extension unique
        //                API_File.GetSingleFileTypeFilter(OptisCoreTypeTrimmed, out string Extension, out Description);
        //                ListOfExtensionsFortype.Add(Extension);
        //            }

        //            string Extensions = string.Join(";", ListOfExtensionsFortype.ToArray());
        //            if (!string.IsNullOrEmpty(Extensions))
        //            {
        //                Types.Add(Description + "|" + Extensions);
        //            }

        //            AllSupportedExtensions.Add(Extensions);
        //        }

        //        // Ajout en tête du filtre de sélection de tous les fichiers compatibles
        //        if (!string.IsNullOrEmpty(i_DescriptionForAllSupportedFiles) && AllSupportedExtensions.Count > 1)
        //        {
        //            string AllSupportedExtensionsFilter = string.Join(";", AllSupportedExtensions);
        //            Types.Insert(0, i_DescriptionForAllSupportedFiles + "|" + AllSupportedExtensionsFilter);
        //        }

        //        o_FileTypeFilterForFileDialog = string.Join("|", Types.ToArray());
        //    }

        //    return o_FileTypeFilterForFileDialog;
        //}

        public override bool HasValueFor(IPAttributes iSOW)
        {
            object FieldValue = this.PropertyInfo.GetValue(iSOW);
            return FieldValue != null && !string.IsNullOrWhiteSpace(FieldValue.ToString());
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
                    o_ValueAsString = this.PropertyInfo.GetValue(SOW) as string;
                    ResultBool = true;
                }
                catch
                {
                    ResultBool = false;
                }
            }

            return ResultBool;
        }

        public override bool SetValueFromStringDisplay(IPAttributes SOW, string i_ValueAsString)
        {
            bool ResultBool = SetValueFromStringInvariant(SOW, i_ValueAsString);

            return ResultBool;
        }

        protected override bool SetValueFromStringInvariant(IPAttributes ISOW, string NewValueAsStringInvariant)
        {
            bool ValueWasModified = false;

            if (ISOW != null)
            {
                try
                {
                    this.PropertyInfo.SetValue(ISOW, NewValueAsStringInvariant);
                    ISOW.OnAttributeChange(AttrID);
                    ValueWasModified = true;
                }
                catch
                {
                    ValueWasModified = false;
                }
            }

            return ValueWasModified;
        }

        public bool ProcessNewFilePath(ISession ISOW, string i_InputFilePath_CanBeRelative, out string FilePathToSave)
        {
            return ProcessNewFilePath(ISOW, Filter, i_InputFilePath_CanBeRelative, out FilePathToSave);
        }

        public static bool ProcessNewFilePath(ISession ISOW, string Filter, string i_InputFilePath_CanBeRelative, out string o_FilePathToSave)
        {
            bool o_AcceptValue = false;

            o_FilePathToSave = i_InputFilePath_CanBeRelative;

            if (ISOW != null)
            {
                if (string.IsNullOrWhiteSpace(i_InputFilePath_CanBeRelative))
                {
                    // On veut vider le champ
                    o_FilePathToSave = string.Empty;
                    o_AcceptValue = true;
                }
                else
                {
                    string InputFilePath = UtilFile.CleanUserInputPath(i_InputFilePath_CanBeRelative);

                    // On vérifie que le fichier existe (?)
                    UtilFile.BuildAbsoluteInputFilePathIfRelative(ISOW, InputFilePath, out string AbsoluteFilePath);
                    if (!File.Exists(AbsoluteFilePath))
                    {
                        // => On refuse la modification car le fichier n'existe pas
                        // TODO ?
                        // char[] invalidFileChars = Path.GetInvalidFileNameChars();
                    }
                    else
                    {
                        bool IsFileCompatible = UtilFile.IsFileMatchingFilter(InputFilePath, Filter); // Check extension
                        if (IsFileCompatible)
                        {
                            MakeRelativeIfPossible(ISOW, InputFilePath, out o_FilePathToSave);
                            o_AcceptValue = true;
                        }
                    }
                }
            }

            return o_AcceptValue;
        }

        public static void MakeRelativeIfPossible(ISession ISOW, string i_InputFilePath_CanBeRelative, out string o_FilePathRelativeIfPossible)
        {
            o_FilePathRelativeIfPossible = i_InputFilePath_CanBeRelative;

            bool IsRelativePath = UtilFile.IsRelativePath(i_InputFilePath_CanBeRelative);
            if (!IsRelativePath)
            {
                string DocSavePath = UtilFile.GetCurrentDocumentSavePath(ISOW);
                if (string.IsNullOrWhiteSpace(DocSavePath))
                {
                    // Le document n'a pas été sauvé : on ne peut pas copier un fichier en relatif... Que faire ?
                    // - interdire de créer une source... non ce serait trop gênant !
                    // - refuser de valuer ce champ ?
                    // - permettre de passer en relatif plus tard... (au save ?)
                }
                else
                {
                    //bool MakePathRelative = false;

                    //bool IsInputFileBelowDocumentFolder = UtilFile.IsInputFileInDocumentInputFolder(i_InputFilePath_CanBeRelative, ISOW);
                    //if (IsInputFileBelowDocumentFolder)
                    //{
                    //    // l'utilisateur a sélectionné un chemin sous le dossier "Input Files" => on rend le chemin relatif automatiquement
                    //    MakePathRelative = true;
                    //}
                    //else
                    //{
                    //    if (POptions.AutoCopyInputFolder)
                    //    {
                    //        if (i_InputFilePath_CanBeRelative.EndsWith(UtilFile.ExtensionLightBox, StringComparison.InvariantCultureIgnoreCase))
                    //        {
                    //            // Exception temporaire pour les ficheirs LightBox (Bug 35380)
                    //        }
                    //        else
                    //        {
                    //            MakePathRelative = true;
                    //        }
                    //    }
                    //    // TODO ?
                    //    // DialogResult DR2 = MessageBox.Show("Do you want to copy the file so that it will be included in document's directory?", "File selected out of document scope", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    //    // MakePathRelative = (DR2 == DialogResult.Yes);
                    //}

                    //if (MakePathRelative)
                    //{
                        UtilFile.MakePathRelative(i_InputFilePath_CanBeRelative, ISOW, out string NewRelativeFile);
                        if (!string.IsNullOrWhiteSpace(NewRelativeFile))
                        {
                            o_FilePathRelativeIfPossible = NewRelativeFile;
                        }
                    //}
                }
            }
        }
    }

}
