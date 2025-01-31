using DG.Tweening;
using System;
using UnityEngine;

public abstract class UIPopupScreen : UIFadeScreen
{
	[Space]
	[SerializeField]
	protected RectTransform m_contentRectTransform;

	protected float _contentFadeInDuration;
	protected float _contentFadeOutDuration;

	protected float _contentFadeYStartPosition;

	protected CanvasGroup _contentCanvasGroup;

	protected override void Awake()
	{
		_contentCanvasGroup = m_contentRectTransform.GetComponent<CanvasGroup>();

		base.Awake();
	}

	protected override void OnInit()
	{
		_fadeInDuration = 0.075f;
		_fadeOutDuration = 0.125f;

		_contentFadeInDuration = 0.05f;
		_contentFadeOutDuration = 0.075f;

		_contentFadeYStartPosition = -32.0f;
	}

	protected override void SetDefault()
	{
		base.SetDefault();

		m_contentRectTransform.anchoredPosition = Vector3.zero;
		_contentCanvasGroup.alpha = 0.0f;
		_contentCanvasGroup.blocksRaycasts = false;
	}

	protected override void OnImmediate()
	{
		base.OnImmediate();

		m_contentRectTransform.DOComplete();
		_contentCanvasGroup.DOComplete();
	}

	protected override void FadeIn(Action action = null)
	{
		base.FadeIn(action);
	}
	protected override void FadeOut(Action action = null)
	{
		_canvasGroup.blocksRaycasts = false;
		_contentCanvasGroup.blocksRaycasts = false;

		_contentCanvasGroup.DOFade(0.0f, _contentFadeInDuration)
			.From(1.0f)
			.SetEase(Ease.OutQuad);

		m_contentRectTransform.DOAnchorPosY(_contentFadeYStartPosition, _contentFadeInDuration)
			.From(new Vector2(0.0f, 0.0f))
			.OnComplete(() =>
			{
				base.FadeOut(action);
			});
	}

	protected override void FadeInComplete(Action action = null)
	{
		_contentCanvasGroup.blocksRaycasts = false;

		_contentCanvasGroup.DOFade(1.0f, _contentFadeInDuration)
			.From(0.0f)
			.SetEase(Ease.OutQuad);

		m_contentRectTransform.DOAnchorPosY(0.0f, _contentFadeInDuration)
			.From(new Vector2(0.0f, _contentFadeYStartPosition))
			.OnComplete(() =>
			{
				_canvasGroup.blocksRaycasts = true;
				_contentCanvasGroup.blocksRaycasts = true;

				base.FadeInComplete(action);
			});
	}
}