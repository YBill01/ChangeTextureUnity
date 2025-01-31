using UnityEngine;
using YBSlice.HFSM.Tools;

namespace YBSlice.Data
{
	[CreateAssetMenu(menuName = "Slicer/SlicerConfigData", fileName = "SlicerConfig", order = 0)]
	public class SlicerConfigData : ScriptableObject
	{
		public ToolMode toolMode;

		[Space]
		public float autoCloseLineDistance = 8.0f;

		[Space]
		public TextureItemData[] textureItems;
	}
}