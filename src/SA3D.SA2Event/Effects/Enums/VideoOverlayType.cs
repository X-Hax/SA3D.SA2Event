namespace SA3D.SA2Event.Effects.Enums
{
	/// <summary>
	/// Types of video overlays.
	/// </summary>
	public enum VideoOverlayType : byte
	{
		/// <summary>
		/// None.
		/// </summary>
		None = 0,

		/// <summary>
		/// Overlays the video onto the screen.
		/// </summary>
		Overlay = 1,

		/// <summary>
		/// Renders the video out to a texture that can be used on a model.
		/// </summary>
		Mesh = 2,

		/// <summary>
		/// Identical to <see cref="Overlay"/>.
		/// </summary>
		Overlay1 = 3,

		/// <summary>
		/// Identical to <see cref="Overlay"/>.
		/// </summary>
		Overlay2 = 4,

		/// <summary>
		/// Pauses the video playback.
		/// </summary>
		Pause = 5,

		/// <summary>
		/// Resumes the video playback.
		/// </summary>
		Resume = 6
	}
}
