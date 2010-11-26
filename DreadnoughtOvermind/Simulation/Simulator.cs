using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using System.Threading;

namespace DreadnoughtOvermind.Simulation {
	class Simulator {
		ConcurrentQueue<SimulationEntity> simulationQueue = new ConcurrentQueue<SimulationEntity>();
		public int Stepping;
		public bool Running;
		public Simulator(int stepping) {
			Stepping = stepping;
		}
		public void Add(SimulationEntity ent) {
			simulationQueue.Enqueue(ent);
		}
		public void Start() {
			SimulationEntity entity = null;
			Running = true;
			while(Running) {
				while(simulationQueue.TryDequeue(out entity)) {
					while(entity.Timer.ElapsedMilliseconds < Stepping) {
						Thread.Sleep(1);
					}
					if(entity.Simulate()) {
						simulationQueue.Enqueue(entity);
					}
				}
				Thread.Sleep(100); // Nichts zu simulieren also pennen
			}
		}
	}
}
