namespace ProcessManager.UI
{
	/// <summary>
	/// The level/severity of a logged message
	/// </summary>
	public enum LogLevel
	{
		/// <summary>
		/// Log all information
		/// </summary>
		Debug = 0,

		/// <summary>
		/// Log most information
		/// </summary>
		Verbose = 1,

		/// <summary>
		/// Log all informative information
		/// </summary>
		Informative = 2,

		/// <summary>
		/// Logs wanring, errors, and standard messages
		/// </summary>
		Normal = 3,

		/// <summary>
		/// Log only critical errors and warnings
		/// </summary>
		Critical = 4,
	};
};