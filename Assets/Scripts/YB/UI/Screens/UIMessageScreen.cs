using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using TMPro;
using UnityEngine;

namespace YBSlice.UI
{
	public class UIMessageScreen : UIPopupScreen
	{
		[Space]
		[SerializeField]
		private TMP_Text m_messageText;

		private CancellationTokenSource _cts;

		public void SetMessage(string message)
		{
			m_messageText.text = message;

			_cts?.Cancel();
			_cts?.Dispose();

			_cts = new CancellationTokenSource();

			RunTaskHide(_cts.Token)
				.Forget();
		}

		private async UniTask RunTaskHide(CancellationToken token)
		{
			await UniTask.Delay(TimeSpan.FromSeconds(2), cancellationToken: token);

			SelfHideInternal();
		}

		private void OnDestroy()
		{
			_cts?.Cancel();
			_cts?.Dispose();
		}
	}
}