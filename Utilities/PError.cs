using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Utilities
{

    // ---------------------------------------------------------------------------
    [Serializable]
    public class PError
    {
        #region Definition

        public string Title;

        public string Description; // Request + Diagnosis
        public string Advice;

        private string _FullMessage;
        /// <summary>
        /// Description + Advice
        /// </summary>
        public string FullMessage
        {
            set => _FullMessage = value;
            get
            {
                if (!string.IsNullOrWhiteSpace(_FullMessage))
                {
                    return _FullMessage;
                }
                else
                {
                    string Text = string.Join(System.Environment.NewLine, Description ?? string.Empty, Advice ?? string.Empty);
                    return Text.Trim();
                }
            }
        }

        public MessageSeverity Severity;

        // Clefs de resources (NLS)
        public string ID;
        public string CategoryID;

        // InternalInfos
        public string LastSSCMethodCall; // Utile quand l'erreur se produit lors de l'appel d'une API (OptisCore...).
        public string SourceFile;
        public string SourceLine;
        public string InternalMessage;

        public bool IsBug { get; set; } // true => Erreur interne à remonter

        #endregion

        public override string ToString()
        {
            return Title + " | " + FullMessage + " [" + ID + "]";
        }

        #region Exception behavior

        public void ThrowAsException() => throw new PException(this);

        public PException ToException() => new PException(this);

        public static PException MakeSingleExceptionFromErrorList(List<PError> i_PErrors) => new PException(i_PErrors);

        static public List<PError> SearchInException(Exception Ex) => (Ex as PException)?.PErrors;
        static public PError SearchFirstInException(Exception Ex)
        {
            PError SSCE = null;
            List<PError> PErrors = PError.SearchInException(Ex);
            if (PErrors != null && PErrors.Count > 0)
            {
                SSCE = PErrors.First();
            }
            return SSCE;
        }

        #endregion

        #region User Display

        static List<PError> _DisplayedErrorMessages { get; } = new List<PError>();
        static IReadOnlyCollection<PError> ErrorsDisplayedOnce { get => _DisplayedErrorMessages.AsReadOnly(); }

        public void SetDisplayed()
        {
            if (!SameMessageWasAlreadyDisplayed(this))
            {
                _DisplayedErrorMessages.Add(this);
            }
        }

        static public bool SameMessageWasAlreadyDisplayed(PError SSCE)
        {
            return (_DisplayedErrorMessages.Find(SE => SE.ID == SSCE.ID && SE.FullMessage == SSCE.FullMessage) != null);
        }

        #endregion

        public void WriteToDebugTrace()
        {
            if (UtilEnv.IsDebuggerAttached)
            {
                Debug.Write(ToString(), "SPEOS Error");
            }
        }

        public string InternalSpecificInfoText
        {
            get
            {
                string Text = string.Empty;

                if (!string.IsNullOrEmpty(InternalMessage))
                {
                    Text = InternalMessage;
                }

                if (!string.IsNullOrEmpty(Text))
                {
                    Text += Environment.NewLine;
                }

                if (!string.IsNullOrEmpty(LastSSCMethodCall))
                {
                    Text = "Call: " + LastSSCMethodCall;
                }

                if (!string.IsNullOrEmpty(Text))
                {
                    Text += Environment.NewLine;
                }

                if (!string.IsNullOrEmpty(SourceFile))
                {
                    string SourceFileName = SourceFile;
                    try
                    {
                        SourceFileName = Path.GetFileName(SourceFile);
                    }
                    catch { }

                    Text += "Source: '" + SourceFileName + "'";
                    if (!string.IsNullOrEmpty(SourceLine))
                    {
                        Text += ":" + SourceLine;
                    }
                }
                return Text;
            }
        }
    }
}