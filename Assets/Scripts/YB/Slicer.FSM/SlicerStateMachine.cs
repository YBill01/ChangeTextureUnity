using YB.HFSM;

namespace YBSlice.HFSM
{
	public class SlicerStateMachine : StateMachine
	{
		public SlicerStateMachine(params State[] states) : base(states) { }
	}
}