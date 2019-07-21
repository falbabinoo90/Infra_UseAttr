using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text.RegularExpressions;
using Utilities;

namespace DataModel
{
   public enum Magnitude
    {
       None,
       Percent,
       Angle,
       Length,
       TimeDuration,
       DateTime,
       Wavelength,
       Temperature,
       Energy,
       Area
   }
   
    public static class MagnitudeExtension
    {
        public static TwoWayDictionary<Magnitude, string> MagnitudeToStringMap { get; } =
            new TwoWayDictionary<Magnitude, string>(
                new Tuple<Magnitude, string>(Magnitude.None, String.Empty),
                new Tuple<Magnitude, string>(Magnitude.Percent,
                    CultureInfo.CurrentCulture.NumberFormat.PercentSymbol),
                new Tuple<Magnitude, string>(Magnitude.Angle, "°"),
                new Tuple<Magnitude, string>(Magnitude.Length, "mm"),
                new Tuple<Magnitude, string>(Magnitude.TimeDuration, "sec"),
                new Tuple<Magnitude, string>(Magnitude.DateTime, "datetime"),
                new Tuple<Magnitude, string>(Magnitude.Wavelength, "nm"),
                new Tuple<Magnitude, string>(Magnitude.Temperature, "K"),
                new Tuple<Magnitude, string>(Magnitude.Energy, "J"),
                new Tuple<Magnitude, string>(Magnitude.Area, "mm2")
            );


        // n'est pas un twoWayDictionary car il y a plusieurs unitées qui ont la même valeur
        public static Dictionary<Magnitude, string> MagnitudeToWBStringMap { get; } =
            new Dictionary<Magnitude, string> {
                         {Magnitude.None, String.Empty},
                         {Magnitude.Percent, CultureInfo.CurrentCulture.NumberFormat.PercentSymbol},
                         {Magnitude.Angle, "degree"},
                         {Magnitude.Length, "mm"},
                         {Magnitude.TimeDuration, "s"},
                         {Magnitude.DateTime, "datetime"},
                         {Magnitude.Wavelength, "nm"},
                         {Magnitude.Temperature, "K"},
    			         {Magnitude.Energy, "J"},
                         {Magnitude.Area, "mm2"}
        };




        public static string GetInternalUnitSuffixSymbol(this Magnitude i_Magnitude)
        {
            string Symbol = null;
            try
            {
                Symbol = MagnitudeToStringMap[i_Magnitude];
            }
            catch (Exception e)
            {
                //e.AlertDevAboutUnexpectedError();
            }
            
            return Symbol;
        }

        public static int GetPrecisionNbDecimales(this Magnitude i_Magnitude)
        {
            int NbDecimales = 5;

            switch (i_Magnitude)
            {
                case Magnitude.Angle:
                    NbDecimales = NumericValue.PrecisionNbDecimalesAngle;
                    break;
                case Magnitude.Length:
                    NbDecimales = NumericValue.PrecisionNbDecimalesLength;
                    break;

                default:
                    NbDecimales = NumericValue.PrecisionNbDecimalesDefault;
                    break;
            }

            return NbDecimales;
        }
    }

    public class NumericValue
    {
        public static int PrecisionNbDecimalesDefault
        {
            get
            {
                return 0;
            }
        }

        public static int PrecisionNbDecimalesAngle
        {
            get
            {
                return 0;//Options.AngleDisplayPrecision;
            }
        }

        public static int PrecisionNbDecimalesLength
        {
            get
            {
                return 3; // Options.LengthDisplayPrecision;
            }
        }

        public static string GetDisplayTextFromInternalValue(Magnitude i_Magnitude, double i_InternalValue)
        {
            string strDisplayValue = String.Empty;

            string FormatDecimales = new string('#', MagnitudeExtension.GetPrecisionNbDecimales(i_Magnitude));
            
            string NumberFormat;
            if (Math.Abs(i_InternalValue) >= 1e+6 || Math.Abs(i_InternalValue) <= 1e-3 && i_InternalValue != 0)
            {
                NumberFormat = "{0:0." + FormatDecimales + "e+0}";
            }
            else
            {
                NumberFormat = "{0:0." + FormatDecimales + "}";
            }

            switch (i_Magnitude)
            {
                case Magnitude.None:
                    {
                        strDisplayValue = String.Format(NumberFormat, i_InternalValue);
                        break;
                    }
                case Magnitude.Percent:
                    {
                        strDisplayValue = String.Format(NumberFormat + " {1}", i_InternalValue, CultureInfo.CurrentCulture.NumberFormat.PercentSymbol);
                        break;
                    }
                case Magnitude.Angle:
                    {
                        double ValueInDegree = i_InternalValue; 
                        strDisplayValue = String.Format(NumberFormat + " {1}", ValueInDegree, i_Magnitude.GetInternalUnitSuffixSymbol());
                        break;
                    }
                case Magnitude.Length:
                    {
                        strDisplayValue = String.Format(NumberFormat + " {1}", i_InternalValue, i_Magnitude.GetInternalUnitSuffixSymbol());
                        break;
                    }
                case Magnitude.TimeDuration:
                    {
                        //strDisplayValue = TS.ToString(@"hh\:mm\:ss"); <= Ne dépasse pas 24h !

                        TimeSpan TS = TimeSpan.FromSeconds(i_InternalValue);
                        strDisplayValue = string.Format("{0}:{1:mm}:{1:ss}", Math.Floor(TS.TotalHours), TS);

                        break;
                    }
                case Magnitude.DateTime:
                    {
                        DateTime DT = DateTime.FromOADate(i_InternalValue);
                        strDisplayValue = DT.ToString("F");
                        break;
                    }
                default:
                    {
                        strDisplayValue = String.Format(NumberFormat + " {1}", i_InternalValue, i_Magnitude.GetInternalUnitSuffixSymbol());
                        break;
                    }
            }

            return strDisplayValue;
        }


        public static bool GetInternalValueFromDisplayText(Magnitude i_Magnitude, string i_DisplayText, out double o_InternalDoubleValue)
        {
            bool bParseOk = false;
            o_InternalDoubleValue = 0;

            switch (i_Magnitude)
            {
                case Magnitude.None:
                    {
                        bParseOk = NumericValue.DoubleTryParse(i_DisplayText, out o_InternalDoubleValue);
                        break;
                    }
                case Magnitude.Percent:
                    {
                        bParseOk = NumericValue.DoubleTryParse(i_DisplayText, out o_InternalDoubleValue);
                        break;
                    }
                case Magnitude.TimeDuration:
                    {
                        TimeSpan TS;

                        string[] ArrayHMS = i_DisplayText.Split(':');
                        if (ArrayHMS.Length == 3)
                        {
                            string strHH = ArrayHMS[0];
                            string strMM = ArrayHMS[1];
                            string strSS = ArrayHMS[2];

                            if (int.TryParse(strHH, out int HH) && int.TryParse(strMM, out int MM) && int.TryParse(strSS, out int SS))
                            {
                                TS = new TimeSpan(HH, MM, SS);
                                o_InternalDoubleValue = TS.TotalSeconds;
                                bParseOk = true;
                            }
                        }

                        break;
                    }
                case Magnitude.DateTime:
                    {
                        DateTime DT;
                        bParseOk = DateTime.TryParseExact(i_DisplayText, "F", null, DateTimeStyles.AdjustToUniversal, out DT);
                        if (bParseOk)
                        {
                            o_InternalDoubleValue = DT.ToOADate();
                        }
                        break;
                    }
                case Magnitude.Angle:
                    {
                        double dValueInDegree;
                        bParseOk = NumericValue.DoubleTryParse(i_DisplayText, out dValueInDegree);
                        if (bParseOk)
                        {
                            o_InternalDoubleValue = dValueInDegree; // * Math.PI / 180; <= La valeur interne est en dégrés
                        }
                        break;
                    }
                case Magnitude.Length:
                    {
                        //bParseOk = Window.ActiveWindow.Units.Length.TryParse(i_DisplayText, out o_dValueAsNumber); // Texte unités utilisateur => double unités système. Ex : 5 (m) => 0.005_mm
                        bParseOk = NumericValue.DoubleTryParse(i_DisplayText, out o_InternalDoubleValue);
                        break;
                    }
                default:
                    {
                        bParseOk = NumericValue.DoubleTryParse(i_DisplayText, out o_InternalDoubleValue);
                        break;
                    }
            }

            return bParseOk;
        }

        //private const string RegexForNumericValue = @"-?[\d]*\.?[\d]+([eE]{1}[-+]?[\d]+)?"; // @"-?[\d,\.]+";
        private const string RegexForNumericValue = @"-?[\d,\.]+([eE]{1}[-+]?[\d]+)?";

        static internal bool DoubleTryParse(string i_DoubleValueAsText, out double o_dResult)
        {
            bool bParseOk = false;
            o_dResult = 0;

            // Retirer les unités ou autres décorations pour ne garder que le nombre, mais conserver la notation scientifique !
            Match M = Regex.Match(i_DoubleValueAsText, RegexForNumericValue);
            if (M != null)
            {
                string strDouble = M.Value;

                // Gestion du séparateur...
                //NumberFormatInfo.NumberGroupSeparator
                if (strDouble.Contains(".") && CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator != ".")
                {
                    strDouble.Replace(".", CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator); // <= TODO : voir si c'est vraiment une bonne idée...
                }

                CultureInfo CI;
                if (i_DoubleValueAsText.Contains("."))
                {
                    CI = CultureInfo.InvariantCulture;
                }
                else
                {
                    CI = CultureInfo.CurrentCulture;
                }

                //NumberFormatInfo NFI = new NumberFormatInfo { NumberDecimalSeparator = "." };
                NumberFormatInfo NFI = CI.NumberFormat;
                bParseOk = double.TryParse(strDouble, NumberStyles.Number | NumberStyles.AllowExponent | NumberStyles.AllowDecimalPoint /*| NumberStyles.AllowThousands*/, NFI, out o_dResult);
            }

            return bParseOk;
        }

        public static bool ApplyConstraints(INumericField ISNF, ref double r_ValueAsDouble, out bool o_ValueWasModified)
        {
            bool o_InputValueIsAcceptedButMaybeModified = true;
            o_ValueWasModified = false;

            double InitialValue = r_ValueAsDouble;

            if (ISNF.ConstraintInteger)
            {
                int iValueAsInteger = (int)r_ValueAsDouble;
                r_ValueAsDouble = iValueAsInteger;
            }

            if (ISNF.HasMin)
            {
                if (r_ValueAsDouble < ISNF.MinSI)
                {
                    r_ValueAsDouble = ISNF.MinSI;
                }

                if (ISNF.ExcludeMin)
                {
                    if (r_ValueAsDouble == ISNF.MinSI)
                    {
                        o_InputValueIsAcceptedButMaybeModified = false;
                    }

                }
            }

            if (ISNF.HasMax)
            {
                if (r_ValueAsDouble > ISNF.MaxSI)
                {
                    r_ValueAsDouble = ISNF.MaxSI;
                }

                if (ISNF.ExcludeMax)
                {
                    if (r_ValueAsDouble == ISNF.MaxSI)
                    {
                        o_InputValueIsAcceptedButMaybeModified = false;
                    }
                }
            }

            o_ValueWasModified = (InitialValue != r_ValueAsDouble);

            return o_InputValueIsAcceptedButMaybeModified;
        }

        // ----------------------------------------------------------------------
        // NON STATIC :

        public NumericValue(Magnitude i_Magnitude, double i_InternalValue)
        {
            Magnitude = i_Magnitude;
            InternalValue = i_InternalValue;
        }

        public readonly Magnitude Magnitude = Magnitude.None;

        public double InternalValue { get; set; }

        public string DisplayValue
        {
            get => GetDisplayTextFromInternalValue(Magnitude, InternalValue);
            set
            {
                bool bParseOk = GetInternalValueFromDisplayText(Magnitude, value, out double DoubleInternalValue);
                if (bParseOk)
                {
                    InternalValue = DoubleInternalValue;
                }
                else
                {
                    throw new ArgumentException("Input string is not compatible with the expected format", "DisplayValue");
                }
            }
        }
    }
}
