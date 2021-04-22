using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ElevatorControl.Models;
using ElevatorControl.Interfaces;

namespace ElevatorControl.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ElevatorsController : ControllerBase
    {
        private readonly IElevatorService _elevators;

        

        public ElevatorsController(IElevatorService elevators)
        {
            _elevators = elevators;
        }

        // GET: api/Elevators
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Elevator>>> GetElevator()
        {
            return await _elevators.GetElevator();
        }

        // GET: api/Elevators/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Elevator>> GetElevator(int id)
        {
            var elevator = await _elevators.GetElevator((int)id);

            if (elevator == null)
            {
                return NotFound();
            }

            return elevator;
        }

        /// <summary>
        /// call the elevator
        /// </summary>
        /// <param name="id">elevator id</param>
        /// <param name="elevatorRequest">ElevatorRequest - only targetFloor is required</param>
        /// <returns></returns>
        [HttpPost("{id}/call")]
        public async Task<ActionResult<Elevator>> RequestElevator(int id, ElevatorRequest elevatorRequest)
        {
            Elevator elevator = await _elevators.GetElevator(id);
            if (elevator == null)
            {
                return NotFound();
            }
            bool retVal = elevator.CallMe(elevatorRequest);
            if (!retVal)
                return BadRequest(elevatorRequest.TargetFloor);

            return Ok(elevatorRequest);
        }


    }
}
