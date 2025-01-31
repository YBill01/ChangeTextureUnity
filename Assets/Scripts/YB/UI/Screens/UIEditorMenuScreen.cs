using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using YBSlice.Data;

namespace YBSlice.UI
{
	public class UIEditorMenuScreen : UIPopupScreen
	{
		public event Action<int> OnItemClick;

		[Space]
		[SerializeField]
		private Button m_closeButton;

		[Space]
		[SerializeField]
		private RectTransform m_textureItemsContainer;

		[SerializeField]
		private UITextureItem m_textureItemPrefab;

		private List<UITextureItem> _items;

		private SlicerConfigData _config;
		private UICanvas _ui;
		
		[Inject]
		public void Construct(
			SlicerConfigData config,
			UICanvas ui)
		{
			_config = config;
			_ui = ui;
		}

		protected override void Awake()
		{
			base.Awake();

			_items = new List<UITextureItem>();
		}

		private void OnEnable()
		{
			m_closeButton.onClick.AddListener(CloseButtonOnClick);
		}
		private void OnDisable()
		{
			m_closeButton.onClick.RemoveListener(CloseButtonOnClick);
		}

		protected override void OnPreShow()
		{
			for (int i = 0; i < _config.textureItems.Length; i++)
			{
				UITextureItem item = Instantiate(m_textureItemPrefab, m_textureItemsContainer);
				
				item.Init(_config.textureItems[i], i);

				_items.Add(item);

				item.OnClick += ItemsOnClick;
			}
		}
		protected override void OnHide()
		{
			foreach (UITextureItem item in _items)
			{
				item.OnClick -= ItemsOnClick;
				Destroy(item.gameObject);
			}

			_items.Clear();
		}

		private void ItemsOnClick(int index)
		{
			OnItemClick?.Invoke(index);
		}

		private void CloseButtonOnClick()
		{
			_ui.EditorScreenHide();
		}
	}
}