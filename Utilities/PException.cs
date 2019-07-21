using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Utilities
{
    [Serializable] 
    public class PException : Exception
    {
        public PException(PError i_SSCE, Exception i_InnerException = null)
            : base(i_SSCE?.FullMessage, i_InnerException)
        {
            _PErrors = new List<PError>();
            if (i_SSCE != null)
            {
                _PErrors.Add(i_SSCE);
            }
        }

        public PException(List<PError> i_ListOfSSCE)
        : base(i_ListOfSSCE != null && i_ListOfSSCE.Count > 0 ?
                    (i_ListOfSSCE.Count > 1 ?
                        string.Format("Several errors: {0}", i_ListOfSSCE.Count) :
                        i_ListOfSSCE[0].FullMessage) :
                    "")
        {
            _PErrors = new List<PError>();
            if (i_ListOfSSCE != null)
            {
                _PErrors = i_ListOfSSCE;
            }
        }

        List<PError> _PErrors = null;
        public List<PError> PErrors
        {
            get
            {
                return _PErrors;
            }
        }

        public PError SSCE
        {
            get
            {
                return _PErrors != null && _PErrors.Count > 0 ? _PErrors[0] : null;
            }
        }
    }

    // ---------------------------------------------------------------------------
    public enum MessageSeverity
    {
        Info,
        Warning,
        Critical,
        QuestionYesNo,
        QuestionOkCancel,
        QuestionAbortRetryIgnore,
    }

}
