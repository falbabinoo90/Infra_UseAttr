using System;
using System.Diagnostics;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;
using Interfaces;

namespace Utilities
{
    static public class UtilFile
    {
        public const string ExtensionXml = ".xml";
        public const string ExtensionHtml = ".html";
        public const string ExtensionTxt = ".txt";
        public const string ExtensionPng = ".png";
        public const string ExtensionAebd = ".aebd";
        public const string ExtensionBdr = ".bdr";

        public static string GetCurrentDocumentSavePath(ISession i_ISession = null)
        {
            string strDocPath = string.Empty;

            if (i_ISession != null)
            {
                strDocPath = i_ISession.GetSessionWorkingDirectory();
            }

            return strDocPath;
        }

        public static string GetCurrentDocumentDirectory(ISession i_ISession = null)
        {
            string strDocPath = GetCurrentDocumentSavePath(i_ISession);

            // If you've definitely got an absolute path, use Path.GetDirectoryName(path).
            // If you might only get a relative name, use new FileInfo(path).Directory.FullName.
            string strDocDirPath = Path.GetDirectoryName(strDocPath);

            return strDocDirPath;
        }
        public static string GetInputDirectoryPath(ISession i_ISOW = null, bool CreateIfDoesNotExist = false)
        {
            string strInputDirectory = "";

            string strDocPath = GetCurrentDocumentSavePath(i_ISOW);

            if (string.IsNullOrWhiteSpace(strDocPath))
            {
                //SSCError SSCE = GetErrorIfDocumentHasNeverBeenSaved(i_ISOW);
                //SSCE.ThrowAsException();
            }
            else
            {
                strInputDirectory = Path.GetDirectoryName(strDocPath);
                //strInputDirectory += Path.DirectorySeparatorChar + DirSpeosInputFiles + Path.DirectorySeparatorChar;
            }

            return strInputDirectory;
        }


        public static string CleanUserInputPath(string i_InputFilePath_CanBeRelative)
        {
            return i_InputFilePath_CanBeRelative.Trim('"', '\t', '\n', ' ');
        }

        public static bool IsRelativePath(string i_FilePath)
        {
            string RelativePrefix = @"." + Path.DirectorySeparatorChar;
            return i_FilePath.StartsWith(RelativePrefix);
        }
        public static void MakePathRelative(string i_InputFilePath, ISession i_ISOW, out string o_NewRelativeInputFilePath, bool i_overwrite = true)
        {
            o_NewRelativeInputFilePath = null;

            Debug.Assert(!string.IsNullOrWhiteSpace(i_InputFilePath));
            Debug.Assert(i_ISOW != null);

            string DocInputDirectoryPath = GetInputDirectoryPath(i_ISOW, CreateIfDoesNotExist: true);

            bool IsAlreadyRelative = IsFileBelowDirectory(i_InputFilePath, DocInputDirectoryPath);

            string RelativePrefix = @"." + Path.DirectorySeparatorChar;

            if (i_InputFilePath.StartsWith(RelativePrefix))
            {
                // TODO : 
            }
            else
            {
                string FullPathOfFileUnderDocument = string.Empty;
                if (IsAlreadyRelative)
                {
                    FullPathOfFileUnderDocument = i_InputFilePath;
                }
                else
                {
                    try
                    {
                        string FileName = Path.GetFileName(i_InputFilePath);
                        string NewFilePath = Path.Combine(DocInputDirectoryPath, FileName);

                        if (i_overwrite)
                        {
                            File.Copy(i_InputFilePath, NewFilePath, overwrite: true);
                            FullPathOfFileUnderDocument = NewFilePath;
                        }
                        else
                        {
                            if (!File.Exists(NewFilePath))
                            {
                                File.Copy(i_InputFilePath, NewFilePath, overwrite: false);
                                FullPathOfFileUnderDocument = NewFilePath;
                            }
                        }
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }

                // Rendre le chemin relatif
                if (!string.IsNullOrWhiteSpace(FullPathOfFileUnderDocument))
                {
                    string DocDirectoryPath = GetCurrentDocumentDirectory(i_ISOW);

                    if (FullPathOfFileUnderDocument.StartsWith(DocDirectoryPath, StringComparison.InvariantCultureIgnoreCase))
                    {
                        string PathFromDocFolder = FullPathOfFileUnderDocument.Remove(0, DocDirectoryPath.Length).TrimStart(new char[] { '\\' });
                        o_NewRelativeInputFilePath = RelativePrefix + PathFromDocFolder;
                    }
                    else
                    {
                        o_NewRelativeInputFilePath = FullPathOfFileUnderDocument;
                    }
                }
                else
                {
                    o_NewRelativeInputFilePath = i_InputFilePath;
                }
            }
        }
        public static bool IsFileBelowDirectory(string i_FilePath, string i_DirectoryPath)
        {
            string Separator = Path.DirectorySeparatorChar.ToString();

            string DirectoryPath = string.Format("{0}{1}", i_DirectoryPath, i_DirectoryPath.EndsWith(Separator) ? "" : Separator);

            return i_FilePath.StartsWith(DirectoryPath, StringComparison.OrdinalIgnoreCase);
        }

        public static void BuildAbsoluteInputFilePathIfRelative(ISession i_ISOW, string i_InputFilePath, out string o_AbsoluteFilePath)
        {
            o_AbsoluteFilePath = "";

            string RelativePrefix = @"." + Path.DirectorySeparatorChar;

            if (i_InputFilePath.StartsWith(RelativePrefix))
            {
                string RelativePathWithoutRelativePrefix = i_InputFilePath.Remove(0, RelativePrefix.Length);
                if (!string.IsNullOrEmpty(RelativePathWithoutRelativePrefix))
                {
                    string DocDirectoryPath = GetCurrentDocumentDirectory(i_ISOW);

                    if (!string.IsNullOrEmpty(DocDirectoryPath))
                    {
                        o_AbsoluteFilePath = Path.Combine(DocDirectoryPath, RelativePathWithoutRelativePrefix);
                    }
                }
            }
            else
            {
                o_AbsoluteFilePath = i_InputFilePath;
            }
        }

        public static bool FileExits(string i_AbsoluteInputFilePath)
        {
            return !string.IsNullOrEmpty(i_AbsoluteInputFilePath) && File.Exists(i_AbsoluteInputFilePath);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="FilePath"></param>
        /// <param name="FileFilter">Ex : "Eulumdat|*.elumdat|Intensity files|*.intensity;*.ldt;*.ies|XMP|*.xmp"</param>
        /// <returns></returns>
        public static bool IsFileMatchingFilter(string FilePath, string FileFilter)
        {
            bool Result = false;

            string LowerFilter = FileFilter.ToLower();

            if (LowerFilter.Contains($"*.*"))
            {
                Result = true;
            }
            else
            {
                string DotLowerExt = Path.GetExtension(FilePath)?.ToLower();
                if (LowerFilter.Contains($"*{DotLowerExt}"))
                {
                    Result = true;
                }
            }

            return Result;
        }

        public static string GetRelativePathWithoutRoot(string iRelativePath)
        {
            if (iRelativePath.StartsWith(@".\"))
            {
                return iRelativePath.Remove(0, 2);
            }

            return iRelativePath;
        }

        public static bool HasFileWritePermission(string filename)
        {
            bool result = false;

            FileSecurity accessControlList = File.GetAccessControl(filename);

            if (accessControlList == null)
            {
                return false;
            }

            AuthorizationRuleCollection accessRules = accessControlList.GetAccessRules(true, true, typeof(System.Security.Principal.SecurityIdentifier));

            if (accessRules == null)
            {
                return false;
            }

            WindowsPrincipal currentuser = new WindowsPrincipal(WindowsIdentity.GetCurrent());

            foreach (FileSystemAccessRule rule in accessRules)
            {
                if (0 == (rule.FileSystemRights & (FileSystemRights.WriteData | FileSystemRights.Write)))
                {
                    continue;
                }

                if (rule.IdentityReference.Value.StartsWith("S-1-"))
                {
                    SecurityIdentifier sid = new SecurityIdentifier(rule.IdentityReference.Value);

                    if (!currentuser.IsInRole(sid))
                    {
                        continue;
                    }
                }
                else
                {
                    if (!currentuser.IsInRole(rule.IdentityReference.Value))
                    {
                        continue;
                    }
                }

                if (rule.AccessControlType == AccessControlType.Allow)
                {
                    result = true;
                }
                else if (rule.AccessControlType == AccessControlType.Deny)
                {
                    result = false;
                }
            }

            return result;
        }
    }
}
