using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElevatorControl.Models
{
    /// <summary>
    /// class representing requests for elevator to perform
    /// </summary>
    public class ElevatorRequest
    {
        /// <summary>
        /// Request time
        /// </summary>
        public DateTime TimeStamp { get; private set; }
        /// <summary>
        /// Requests
        /// </summary>
        public int TargetFloor { get; set; }

        public ElevatorRequest()
        {
            Init();
        }
        private void Init()
        {
            TimeStamp = DateTime.Now;
        }
        public ElevatorRequest(int targetFloor)
        {
            TargetFloor = targetFloor;
            Init();
        }
    }
}
