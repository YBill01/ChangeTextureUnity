using UnityEngine;
using YBSlice.Input;

namespace YBSlice.Objects
{
	public abstract class InteractionBehaviour : MonoBehaviour, IInteraction
	{
		[field: SerializeField]
		public bool InteractionEnable { get; set; }

		protected GameObject _sourceGameObject;
		public GameObject SourceGameObject => _sourceGameObject;

		[SerializeField]
		protected InteractionCondition m_interactionCondition;
		public InteractionCondition InteractionCondition
		{
			get => m_interactionCondition;
			set
			{
				m_interactionCondition = value;
			}
		}

		protected virtual void Awake()
		{
			_sourceGameObject = gameObject;
		}

		public virtual void OnTouchInternal(bool enter)
		{
			
		}
		public virtual void OnTouchPointInternal(Vector3 position, Vector3 normal)
		{
			
		}
		public virtual void OnClickInternal(Vector3 position, Vector3 normal)
		{
			
		}
	}
}