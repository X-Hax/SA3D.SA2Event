using SA3D.Common;

namespace SA3D.SA2Event.Model
{
	/// <summary>
	/// Event entry attributes
	/// </summary>
	public enum GCEventEntryAttribute : uint
	{
		/// <summary>
		/// Unknown functionality.
		/// </summary>
		Unk0 = Flag32.B0,

		/// <summary>
		/// Animated scene: Entry has no morph motion.
		/// </summary>
		Scene_NoShapeAnimation = Flag32.B1,

		/// <summary>
		/// Root scene: Enables light rendering on the surface.
		/// </summary>
		Root_EnableLighting = Flag32.B1,

		/// <summary>
		/// Unknown functionality.
		/// </summary>
		Unk2 = Flag32.B2,

		/// <summary>
		/// Animated scene: Entry has no node motion.
		/// </summary>
		Scene_NoNodeAnimation = Flag32.B3,

		/// <summary>
		/// Root scene: Disables shadow rendering on the surface.
		/// </summary>
		Root_DisableShadows = Flag32.B3,

		/// <summary>
		/// Unknown functionality.
		/// </summary>
		Unk4 = Flag32.B4,

		/// <summary>
		/// Unknown functionality.
		/// </summary>
		Unk5 = Flag32.B5,

		/// <summary>
		/// Unknown functionality.
		/// </summary>
		Unk6 = Flag32.B6,

		/// <summary>
		/// Renders the entry in reflections.
		/// </summary>
		Reflection = Flag32.B7,

		/// <summary>
		/// Enabled blare for the entry.
		/// </summary>
		Blare = Flag32.B8,

		/// <summary>
		/// Unknown functionality.
		/// </summary>
		Unk9 = Flag32.B9
	}
}
