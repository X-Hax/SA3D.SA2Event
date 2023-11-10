namespace SA3D.SA2Event.Effects.Enums
{
	/// <summary>
	/// The way in which <see cref="ObjectLighting"/> fades in.
	/// </summary>
	public enum LightFadeMode : uint
	{
		/// <summary>
		/// Dont render in at all (?).
		/// </summary>
		None = 0,

		/// <summary>
		/// Render light immidiately.
		/// </summary>
		Cut = 1,

		/// <summary>
		/// Fade the light.
		/// </summary>
		FadeIn = 2
	}
}
