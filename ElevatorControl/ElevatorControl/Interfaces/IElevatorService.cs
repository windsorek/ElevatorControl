using ElevatorControl.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElevatorControl.Interfaces
{
    public interface IElevatorService
    {
        Task<Elevator> GetElevator(int id);
        Task<List<Elevator>> GetElevator();
        bool AddElevator(Elevator elevator);
        bool CallElevator(int id, ElevatorRequest elevatorRequest);
    }
}
