using ElevatorControl.Interfaces;
using ElevatorControl.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElevatorControl.Data
{
    public class TestData
    {

        public static void AddTestData(ElevatorControlContext elevatorControlContext)
        {
            var rng = new Random();
            const int totalFloorsMax = 10;
            const int lowestFloorMin = -10;
            const int lowestFloorMax = 10;
            elevatorControlContext.Elevators.Add(new Elevator((uint)rng.Next(1, totalFloorsMax), rng.Next(lowestFloorMin, lowestFloorMax)));
            elevatorControlContext.Elevators.Add(new Elevator((uint)rng.Next(1, totalFloorsMax), rng.Next(lowestFloorMin, lowestFloorMax)));
            elevatorControlContext.Elevators.Add(new Elevator((uint)rng.Next(1, totalFloorsMax), rng.Next(lowestFloorMin, lowestFloorMax)));
            elevatorControlContext.SaveChanges();
        }

        public static void AddTestData(IElevatorService elevatorsService)
        {
            var rng = new Random();
            const int totalFloorsMax = 10;
            const int lowestFloorMin = -10;
            const int lowestFloorMax = 10;
            elevatorsService.AddElevator(new Elevator((uint)rng.Next(1, totalFloorsMax), rng.Next(lowestFloorMin, lowestFloorMax)));
            elevatorsService.AddElevator(new Elevator((uint)rng.Next(1, totalFloorsMax), rng.Next(lowestFloorMin, lowestFloorMax)));
            elevatorsService.AddElevator(new Elevator((uint)rng.Next(1, totalFloorsMax), rng.Next(lowestFloorMin, lowestFloorMax)));
            
        }
    }
}
