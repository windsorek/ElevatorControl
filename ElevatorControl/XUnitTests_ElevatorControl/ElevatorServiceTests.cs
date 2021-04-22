using ElevatorControl.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace XUnitTests_ElevatorControl
{
    public class ElevatorServiceTests
    {
        [Fact]
        public void ElevatorServiceClassInit()
        {
            ///Arrange
            ElevatorsService elevatorsService = new ElevatorsService();
            ///Act

            ///Assert
            Assert.True(elevatorsService.elevators.Count == 0);
        }
        [Theory]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(100)]
        [InlineData(1000)]
        [InlineData(10000)]
        public void ElevatorServiceAddElevatorTest(uint elevatorsToAdd)
        {
            ///Arrange
            ElevatorsService elevatorsService = new ElevatorsService();
            ///Act
            for (int i = 1; i <= elevatorsToAdd; i++)
                elevatorsService.AddElevator(new Elevator());
            ///Assert
            Assert.True(elevatorsService.elevators.Count == elevatorsToAdd);
        }

        /// <summary>
        /// We have 10 elevators id 0 ... 9
        /// </summary>
        /// <param name="id"></param>
        [Theory]
        [InlineData(1,true)]
        [InlineData(10,false)]
        [InlineData(-1, false)]
        [InlineData(3, true)]
        public async void GetElevatorTest(int id, bool elevatorFound)
        {
            ///Arrange
            ElevatorsService elevatorsService = new ElevatorsService();
            ///Act
            for (int i = 1; i <= 10; i++)
                elevatorsService.AddElevator(new Elevator());
            Elevator elevator = await elevatorsService.GetElevator(id);
            ///Assert
            Assert.True(elevatorsService.elevators.Count == 10);
            if(elevatorFound)
            {
                Assert.NotNull(elevator);
                Assert.Equal(elevator.Id , id);
            }else
            {
                Assert.Null(elevator);
            }
        }

        [Fact]
        public async void GetAllElevatorTest()
        {
            ///Arrange
            ElevatorsService elevatorsService = new ElevatorsService();
            int elevatorCount = 10;
            ///Act
            for (int i = 1; i <= elevatorCount; i++)
                elevatorsService.AddElevator(new Elevator());
            List<Elevator> elevatorList = await elevatorsService.GetElevator();
            ///Assert
            Assert.True(elevatorsService.elevators.Count == elevatorCount);
            Assert.Equal(elevatorList.Count, elevatorCount);
        }
    }
}
