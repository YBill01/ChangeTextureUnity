using UnityEngine;
using YBSlice.Objects;

namespace YBSlice.Input
{
	public interface IInteraction
	{
		bool InteractionEnable { get; set; }

		GameObject SourceGameObject { get; }

		InteractionCondition InteractionCondition { get; set; }

		void OnTouchInternal(bool enter);
		void OnTouchPointInternal(Vector3 position, Vector3 normal);
		void OnClickInternal(Vector3 position, Vector3 normal);
	}
}