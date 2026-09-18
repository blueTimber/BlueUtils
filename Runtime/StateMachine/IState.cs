namespace BlueUtils.StateMachine
{
	public interface IState<UpdateValue>
	{
		public delegate void StateEndEventHandler();
		/// <summary>
		/// Event triggered when the state ends. This is used to notify the state machine to transition to a different state.
		/// This isn't called when the event ends during the start of the state.
		/// </summary>
		public event StateEndEventHandler OnStateEnd;

		/// <returns>True if the state started successfully. False means the state is already finished.</returns>
		public bool Start(UpdateValue updateValue);

		public UpdateValue Update(UpdateValue updateValue);

		public void End(UpdateValue updateValue);

		public void Cancel();
	}

	public interface IState<UpdateValue, StartValue>
	{
		public delegate void StateEndEventHandler();
		/// <summary>
		/// Event triggered when the state ends. This is used to notify the state machine to transition to a different state.
		/// This isn't called when the event ends during the start of the state.
		/// </summary>
		public event StateEndEventHandler OnStateEnd;

		/// <returns>True if the state started successfully. False means the state is already finished.</returns>
		public bool Start(UpdateValue updateValue, StartValue startValue);

		public UpdateValue Update(UpdateValue updateValue);

		public void End(UpdateValue updateValue);

		public void Cancel();
	}
}
