using SA3D.Common;

namespace SA3D.SA2Event.Model
{
	/// <summary>
	/// Event entry attributes
	/// </summary>
	public enum EventEntryAttribute : uint
	{
		/// <summary>
		/// Has environment mapped materials, use Simple variant of draw function.
		/// </summary>
		HasEnvironment = Flag32.B0,

		/// <summary>
		/// Draw with fog disabled and use EasyDraw.
		/// </summary>
		NoFogAndEasyDraw = Flag32.B1,

		/// <summary>
		/// Use multi-light 1.
		/// </summary>
		Light1 = Flag32.B2,

		/// <summary>
		/// Use multi-light 2.
		/// </summary>
		Light2 = Flag32.B3,

		/// <summary>
		/// Use multi-light 3.
		/// </summary>
		Light3 = Flag32.B4,

		/// <summary>
		/// Use multi-light 4.
		/// </summary>
		Light4 = Flag32.B5,

		/// <summary>
		/// Is a modifier volume and should use ModDraw.
		/// </summary>
		ModifierVolume = Flag32.B6,

		/// <summary>
		/// Renders the entry in reflections.
		/// </summary>
		Reflection = Flag32.B7,

		/// <summary>
		/// Enabled blare for the entry.
		/// </summary>
		Blare = Flag32.B8,

		/// <summary>
		/// Use regular Simple over any Multi or Easy variant
		/// </summary>
		UseSimple = Flag32.B9
	}
}
