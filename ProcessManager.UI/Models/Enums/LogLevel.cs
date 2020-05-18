namespace ProcessManager.UI
{
	/// <summary>
	/// The level/severity of a logged message
	/// </summary>
	public enum LogLevel
	{
		/// <summary>
		/// A generic info log
		/// </summary>
		Info = 0,

		/// <summary>
		/// Warnings, a process creation failed, unable to read project data from file, and such
		/// </summary>
		Warning = 1,

		/// <summary>
		/// An error has occured, unexpected return value, expcetions, and such
		/// </summary>
		Error = 2,

		/// <summary>
		/// Log only critical errors and warnings
		/// </summary>
		Critical = 4,
	};

};