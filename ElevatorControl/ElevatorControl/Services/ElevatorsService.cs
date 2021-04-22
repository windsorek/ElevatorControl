﻿using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ElevatorControl.Interfaces;
using Microsoft.Extensions.Logging;

namespace ElevatorControl.Models
{
    public class ElevatorsService : IElevatorService
    {
        public ConcurrentDictionary<int,Elevator> elevators { get; set; }

        private readonly ILogger<Elevator> _logger;

        public ElevatorsService(ILogger<Elevator> logger = null)
        {
            elevators = new ConcurrentDictionary<int,Elevator>();
            _logger = logger;
        }
        public async Task<List<Elevator>> GetElevator()
        {
            return await Task.Run(() => elevators.Select(e => e.Value).ToList());
        }

        public async Task<Elevator> GetElevator(int id)
        {
            return await Task.Run(() => elevators.FirstOrDefault(e => e.Key ==id).Value);
        }

        public bool AddElevator(Elevator elevator)
        {
            if(elevator.Id == 0 && elevators.Count > 0)
            {
                ///find first available

                elevator.Id = elevators.Max(e => e.Value.Id) + 1;
            }
            elevator.Logger = _logger;
            return elevators.TryAdd(elevator.Id, elevator);
        }
    }
}