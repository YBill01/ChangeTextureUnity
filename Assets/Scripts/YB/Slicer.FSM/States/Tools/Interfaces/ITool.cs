namespace YBSlice.HFSM.Tools
{
	public interface ITool
	{
		bool Enable { get; set; }

		void Init();
		void Destroy();

		void Update();

		void Click();
	}
}