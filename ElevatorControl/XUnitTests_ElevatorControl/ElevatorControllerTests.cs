using ElevatorControl.Controllers;
using ElevatorControl.Interfaces;
using ElevatorControl.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace XUnitTests_ElevatorControl
{
    public class ElevatorControllerTests
    {
        [Fact]
        public async Task GetElevatorsTest()
        {
            // Arrange
            Elevator elevator = new Elevator(10, 0);
            List<Elevator> elevators = new List<Elevator>();
            elevators.Add(elevator);
            Mock<IElevatorService> mockElevatorService = new Mock<IElevatorService>();

            mockElevatorService.Setup(mock => mock.GetElevator()).ReturnsAsync(elevators);

            ElevatorsController controller = new ElevatorsController(mockElevatorService.Object);

            // Act
            var result = await controller.GetElevator();

            // Assert
            Assert.IsType<ActionResult<IEnumerable<Elevator>>>(result);
            Assert.Equal(result.Value.ToList().Count(), elevators.Count());

        }

        [Theory]
        [InlineData(0, true)]
        [InlineData(1, false)]
        public async Task GetElevatorTest(int id, bool success)
        {
            // Arrange
            Elevator elevator = new Elevator(10, 0);
            Mock<IElevatorService> mockElevatorService = new Mock<IElevatorService>();

            mockElevatorService.Setup(mock => mock.GetElevator(id)).ReturnsAsync(elevator);

            ElevatorsController controller = new ElevatorsController(mockElevatorService.Object);

            // Act
            ActionResult<Elevator> result = await controller.GetElevator(0);

            // Assert
            Assert.IsType<ActionResult<Elevator>>(result);
            if (success)
            {
                Assert.Equal(result.Value, elevator);
            }
            else
            {
                Assert.NotEqual(result.Value, elevator);
                Assert.Null(result.Value);
            }
        }
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task RequestElevatorTest( bool success)
        {
            // Arrange
            int id = 0;
            int targetFloor = 1;
            Elevator elevator = new Elevator(10, 0);
            Mock<IElevatorService> mockElevatorService = new Mock<IElevatorService>();
            ElevatorRequest elevatorRequest = new ElevatorRequest(targetFloor);
            mockElevatorService.Setup(mock => mock.GetElevator(id)).ReturnsAsync(elevator);
            mockElevatorService.Setup(mock => mock.CallElevator(id, elevatorRequest)).Returns(success);

            ElevatorsController controller = new ElevatorsController(mockElevatorService.Object);

            // Act
            ActionResult<Elevator> result = await controller.RequestElevator(id, elevatorRequest);

            // Assert
            var viewResult = Assert.IsType<ActionResult<Elevator>>(result);
            if (success)
            {
                Assert.IsType<OkObjectResult>(viewResult.Result);
            }
            else
            {
                Assert.IsType<BadRequestObjectResult>(viewResult.Result);
            }
        }

    }
}
