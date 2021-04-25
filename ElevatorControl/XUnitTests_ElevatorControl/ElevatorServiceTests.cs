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
        [InlineData(1, true)]
        [InlineData(10, false)]
        [InlineData(-1, false)]
        [InlineData(3, true)]
        public async void GetElevatorTest(int id, bool elevatorFound)
        {
            ///Arrange
            ElevatorsService elevatorsService = new ElevatorsService();
            for (int i = 1; i <= 10; i++)
                elevatorsService.AddElevator(new Elevator());
            ///Act

            Elevator elevator = await elevatorsService.GetElevator(id);
            ///Assert
            Assert.True(elevatorsService.elevators.Count == 10);
            if (elevatorFound)
            {
                Assert.NotNull(elevator);
                Assert.Equal(elevator.Id, id);
            }
            else
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
            for (int i = 1; i <= elevatorCount; i++)
                elevatorsService.AddElevator(new Elevator());
            ///Act

            List<Elevator> elevatorList = await elevatorsService.GetElevator();
            ///Assert
            Assert.True(elevatorsService.elevators.Count == elevatorCount);
            Assert.Equal(elevatorList.Count, elevatorCount);
        }



        /// <summary>
        /// We have 10 elevators id 0 ... 9
        /// every elevator starts with 0 lowestFloor and have 10 totalFloors
        /// </summary>
        /// <param name="id"></param>
        [Theory]
        [InlineData(1, 5, true)]
        [InlineData(2, 15, false)]
        [InlineData(3, 5, true)]
        [InlineData(4, -5, false)]
        [InlineData(-1, 6, false)]
        [InlineData(-4, 6, false)]
        public void CallElevatorTest(int id, int targetFloor, bool requestOk)
        {
            ///Arrange
            ElevatorsService elevatorsService = new ElevatorsService();
            for (int i = 1; i <= 10; i++)
                elevatorsService.AddElevator(new Elevator(10, 0));
            ///Act

            bool reqOk = elevatorsService.CallElevator(id, new ElevatorRequest(targetFloor));
            ///Assert

            if (requestOk)
            {
                Assert.True(reqOk);
            }
            else
            {
                Assert.False(reqOk);
            }
        }


        /// <summary>
        /// We have 10 elevators id 0 ... 9
        /// every elevator starts with 0 lowestFloor and have 10 totalFloors
        /// </summary>
        /// <param name="idTo">how many elevators ( up to 9 ) we call at almost the same time</param>
        /// <param name="milisecondMargin">how many miliseconds we allow  for sending all requests</param>
        /// <param name="requestOk"> do we expect our request to be accepted</param>
        /// <param name="targetFloor">to which floor we call each elevator</param>
        [Theory]
        [InlineData(9, 5, true, 5)]
        [InlineData(9, 15, false, 5)]
        [InlineData(9, 5, true, 3)]
        public void CallMultipleElevatorsTest(int idTo, int targetFloor, bool requestOk, int milisecondMargin)
        {
            ///Arrange

            ElevatorsService elevatorsService = new ElevatorsService();
            for (int i = 1; i <= 10; i++)
                elevatorsService.AddElevator(new Elevator(10, 0));

            Dictionary<int, Tuple<bool, DateTime>> elevatorsRequestsRV = new Dictionary<int, Tuple<bool, DateTime>>();
            DateTime startTime = DateTime.Now;

            ///Act
            for (int i = 0; i <= idTo; i++)
            {
                elevatorsRequestsRV.Add(i, new Tuple<bool, DateTime>(elevatorsService.CallElevator(idTo, new ElevatorRequest(targetFloor)), DateTime.Now));
            }

            ///Assert
            if (requestOk)
            {
                foreach (KeyValuePair<int, Tuple<bool, DateTime>> eleveReqRV in elevatorsRequestsRV)
                {
                    Assert.True(eleveReqRV.Value.Item1);
                    Assert.True(eleveReqRV.Value.Item2.Subtract(startTime).TotalMilliseconds < milisecondMargin);
                }
            }
            else
            {
                foreach (KeyValuePair<int, Tuple<bool, DateTime>> eleveReqRV in elevatorsRequestsRV)
                {
                    Assert.False(eleveReqRV.Value.Item1);
                }
            }
        }
    }
}
