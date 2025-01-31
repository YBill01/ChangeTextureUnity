using System;
using UnityEngine;
using UnityEngine.UI;
using YBSlice.Data;

namespace YBSlice.UI
{
	[RequireComponent(typeof(Button), typeof(Image))]
	public class UITextureItem : MonoBehaviour
	{
		public event Action<int> OnClick;

		public int Index {  get; private set; }

		private TextureItemData _textureItemData;

		private Button _button;

		private void Awake()
		{
			_button = GetComponent<Button>();
		}

		private void OnEnable()
		{
			_button.onClick.AddListener(ButtonOnClick);
		}
		private void OnDisable()
		{
			_button.onClick.RemoveListener(ButtonOnClick);
		}

		public void Init(TextureItemData textureItemData, int index)
		{
			Index = index;

			_textureItemData = textureItemData;
			
			GetComponent<Image>().sprite = _textureItemData.preview;
		}

		private void ButtonOnClick()
		{
			OnClick?.Invoke(Index);
		}
	}
}