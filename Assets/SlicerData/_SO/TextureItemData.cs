using UnityEngine;

namespace YBSlice.Data
{
	[CreateAssetMenu(menuName = "Slicer/TextureItemData", fileName = "TextureItem", order = 1)]
	public class TextureItemData : ScriptableObject
	{
		public string displayName;

		[Space]
		public Sprite preview;

		[Space]
		public Texture texture;
	}
}