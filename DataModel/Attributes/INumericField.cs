namespace DataModel
{
	public interface INumericField
	{
		Magnitude Magnitude { get; }

		bool ConstraintInteger { get; set; }
		bool HasMin { get; set; }
		bool HasMax { get; set; }
		double MinSI { get; set; }
		double MaxSI { get; set; }
		bool ExcludeMin { get; set; }
		bool ExcludeMax { get; set; }
	}
}
