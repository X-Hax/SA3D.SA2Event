namespace SA3D.SA2Event.Effects.Enums
{
	/// <summary>
	/// Types of screen effects.
	/// </summary>
	public enum ScreenEffectType : byte
	{
		/// <summary>
		/// No effect.
		/// </summary>
		None = 0,

		/// <summary>
		/// Fades into foreground.
		/// </summary>
		ForegroundFadeIn = 1,

		/// <summary>
		/// Cuts into the foreground.
		/// </summary>
		ForegroundCutIn = 2,

		/// <summary>
		/// Fades into a texture.
		/// </summary>
		TextureFadeIn = 3,

		/// <summary>
		/// Cuts into a texture.
		/// </summary>
		TextureCutIn = 4,

		/// <summary>
		/// Fades into the background.
		/// </summary>
		BackgroundFadeIn = 5,

		/// <summary>
		/// Cuts into the background.
		/// </summary>
		BackgroundCutIn = 6
	}
}
