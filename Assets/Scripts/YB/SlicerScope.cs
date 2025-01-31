using UnityEngine;
using VContainer;
using VContainer.Unity;
using YBSlice.Data;
using YBSlice.Debugger;
using YBSlice.Input;
using YBSlice.UI;

namespace YBSlice
{
	public class SlicerScope : LifetimeScope
	{
		[Space]
		[SerializeField]
		private SlicerConfigData m_config;

		[Space]
		[SerializeField]
		private Transform m_testCubeContainer;

		protected override void Configure(IContainerBuilder builder)
		{
			builder.RegisterInstance(m_config);

			builder.RegisterComponentInHierarchy<InputUIControl>();
			builder.RegisterComponentInHierarchy<InputCameraControl>();
			builder.RegisterComponentInHierarchy<InputInteractionControl>();

			builder.RegisterComponentInHierarchy<UICanvas>();

			builder.RegisterComponentInHierarchy<SlicerDebug>();

			builder.RegisterEntryPoint<SlicerFlow>()
				.WithParameter(m_testCubeContainer)
				.AsSelf();
		}
	}
}