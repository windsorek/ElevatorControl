using ElevatorControl.Models;
using System;
using System.Threading;
using Xunit;

namespace XUnitTests_ElevatorControl
{
    public class ElevatorClassTests
    {
        [Fact]
        public void ElevatorClassInitTest()
        {
            ///Arrange
            Elevator elevator = new Elevator();
            ///Act

            ///Assert
            Assert.Equal(elevator.LowestFloor, elevator.CurrentFloor);
        }

        [Theory]
        [InlineData(-5,false)]
        [InlineData(-1,false)]
        [InlineData(0,true)]
        [InlineData(5,true)]
        [InlineData(10,true)]
        [InlineData(15,false)]
        [InlineData(20,false)]
        public void ElevatorRequestFloor(int targetFloor, bool expectedResult)
        {
            ///Arrange
            Elevator elevator = new Elevator(10, 0);
            ElevatorRequest elevatorRequest = new ElevatorRequest(targetFloor);
            ///Act
            bool retVal = elevator.CallMe(elevatorRequest);
            ///Assert
            Assert.Equal(expectedResult,retVal);
        }

        [Theory]
        [InlineData(-5, false)]
        [InlineData(-1, false)]
        [InlineData(0, true)]
        [InlineData(5, true)]
        [InlineData(10, true)]
        [InlineData(15, false)]
        [InlineData(20, false)]
        public void ElevatorRequestFloor2(int targetFloor, bool expectedResult)
        {
            ///Arrange
            Elevator elevator = new Elevator(10, 0);
            ///Act
            bool retVal = elevator.CallMe(targetFloor);
            ///Assert
            Assert.Equal(expectedResult, retVal);
        }


        [Theory]
        [InlineData(-5, false)]
        [InlineData(-1, false)]
        [InlineData(0, true)]
        [InlineData(5, true)]
        [InlineData(10, true)]
        [InlineData(15, false)]
        [InlineData(20, false)]
        public void ElevatorCheckIfArrivesCorrectly(int targetFloor, bool expectedResult)
        {
            ///Arrange
            int startingFloor = 0;
            int floorMovingSpeed = 1000;//ms
            int doorOperatingSpeed = 1000;//ms
            Elevator elevator = new Elevator(10, startingFloor);
            ElevatorRequest elevatorRequest = new ElevatorRequest(targetFloor);
            ///Act
            bool retVal = elevator.CallMe(elevatorRequest);
            ///Assert
            Assert.Equal(expectedResult, retVal);
            if (expectedResult)
            {
                Thread.Sleep(floorMovingSpeed * (targetFloor - startingFloor) + 2 * doorOperatingSpeed + 1500);
                Assert.Equal(elevator.CurrentFloor, targetFloor);
                Assert.True(elevator.DoorState == DoorState.Opened,$"DoorState {elevator.DoorState} but should be opened ");
                Assert.True(elevator.ElevatorState == ElevatorState.Ready, $"Elevator State {elevator.ElevatorState} but should be ready");
            }else
            {
                Assert.Equal(elevator.CurrentFloor, startingFloor);
            }
        }
    }
}
