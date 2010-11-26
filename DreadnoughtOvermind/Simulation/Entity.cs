using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace DreadnoughtOvermind.Simulation {
	public class SimulationEntity {
		public Stopwatch Timer = new Stopwatch();
		public virtual bool Simulate() {
			Timer.Reset();
			Timer.Start();
			return true; // requeue
		}
	}
}
