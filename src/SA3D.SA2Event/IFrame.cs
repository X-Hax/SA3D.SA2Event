namespace SA3D.SA2Event
{
	/// <summary>
	/// Common interface for all structures with a start frame.
	/// </summary>
	public interface IFrame
	{
		/// <summary>
		/// Frame at which the data takes effect.
		/// </summary>
		public uint Frame { get; set; }
	}
}
