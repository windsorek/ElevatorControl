using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Timers;

namespace ElevatorControl.Models
{
    /// <summary>
    /// class representing Elevator instance
    /// it represents one vertical shaft with one carriage in it
    /// </summary>
    public partial class Elevator : INotifyPropertyChanged
    {
        /// <summary>
        /// Public Key
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// State of elevator doors
        /// </summary>
        public DoorState DoorState { get { return doorState; } private set { doorState = value; OnPropertyChanged("DoorState"); } }
        /// <summary>
        /// State of the elevator
        /// </summary>
        public ElevatorState ElevatorState { get { return elevatorState; } private set { elevatorState = value; OnPropertyChanged("ElevatorState"); } }
        /// <summary>
        /// Current floor of the elevator
        /// </summary>
        public int? CurrentFloor { get { return currentFloor; }  private set { currentFloor = value; OnPropertyChanged("CurrentFloor"); } }
        /// <summary>
        /// total number of floors in elevator shaft
        /// </summary>
        public uint TotalFloors { get; set; } = 10;

        /// <summary>
        /// Lowest possible floor for elevator - default 0 - ground floor - but maybe "-10" for 10 undeground levels
        /// </summary>
        public int LowestFloor { get; set; } = 0;

        /// <summary>
        /// Elevator speed  in [floors/s]
        /// </summary>
        private const float ElevatorMovingSpeed = 1;

        /// <summary>
        /// Speed at which elevator door opens or closes [s]
        /// </summary>
        private const float DoorOperatingSpeed = 1;

        /// <summary>
        /// Raised when Property was changed
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        private DoorState doorState;
        private ElevatorState elevatorState;
        private int? currentFloor;
        private static Timer processorTimer;
        private const double processorTimerInterval = 500;

        public ILogger<Elevator> Logger { get; set; }
        

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        /// <summary>
        /// Requests FIFO queue
        /// </summary>
        private ConcurrentQueue<ElevatorRequest> ElevatorRequests { get; set; }

        public Elevator()
        {
            Init();
        }

        public Elevator(uint totalFloors, int lowestFloor)
        {
            TotalFloors = totalFloors;
            LowestFloor = lowestFloor;
            Init();
        }
        private void Init()
        {
            CurrentFloor = LowestFloor;
            DoorState = DoorState.Unknown;
            ElevatorState = ElevatorState.Unknown;
            ElevatorRequests = new ConcurrentQueue<ElevatorRequest>();
            this.PropertyChanged += Elevator_PropertyChanged;

            //Logger = Logger<Elevator>.Instance;

            processorTimer = new Timer(processorTimerInterval);
            processorTimer.Elapsed += ProcessorTimer_Elapsed;

            processorTimer.AutoReset = true;
            processorTimer.Enabled = true;
            processorTimer.Start();
        }

        private void ProcessorTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (ElevatorRequests.Count > 0)
            {
                RequestProcessor();
            }else
            {
                Logger?.LogDebug($"{Id} No Requests to do, ElevatorIdle:{ElevatorIdle()}");
            }
        }

        private bool ElevatorIdle()
        {
            bool retVal = false;
            if(     DoorState != DoorState.Opening 
                &&  DoorState != DoorState.Closing
                &&  ElevatorState != ElevatorState.MovingDown
                &&  ElevatorState != ElevatorState.MovingUp
                &&  CurrentFloor != null)
            {
                retVal = true;
            }
            return retVal;
        }

        private void RequestProcessor()
        {
            if (ElevatorIdle())
            {
                bool getRequest = ElevatorRequests.TryDequeue(out ElevatorRequest elevatorRequest);
                if (getRequest)
                {
                    ProcessRequest(elevatorRequest);
                }
                else
                {
                    ///Elevator Malfunction
                }
            }else
            {
                /// Elevator is working
            }
        }

        private async void ProcessRequest(ElevatorRequest elevatorRequest)
        {
            await MoveElevator(elevatorRequest.TargetFloor);
        }

        public bool CallMe(ElevatorRequest elevatorRequest)
        {
            int targetFloor = elevatorRequest.TargetFloor;
            bool retVal = false;
            if (targetFloor >= LowestFloor && targetFloor <= LowestFloor + TotalFloors)
            {
                ElevatorRequests.Enqueue(elevatorRequest);
                retVal = true;
            }
            return retVal;
        }

        public bool CallMe(int targetFloor)
        {
            ElevatorRequest elevatorRequest = new ElevatorRequest(targetFloor);
            return CallMe(elevatorRequest);
        }
        private void Elevator_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            //Logger?.LogInformation($"Property has Been updated: {e.PropertyName}");
            string additionalInfo = "";
            switch (e.PropertyName)
            {
                case "DoorState":
                    additionalInfo = $"Door: {Enum.GetName(typeof(Models.DoorState), this.DoorState)}";
                    break;
                case "ElevatorState":
                    additionalInfo = $"Elevator: {Enum.GetName(typeof(Models.ElevatorState), this.ElevatorState)}";
                    break;
                case "CurrentFloor":
                    if (CurrentFloor != null)
                        additionalInfo = $"Last Floor: {CurrentFloor}";
                    break;

            }
            if (additionalInfo != "")
            {
                Logger?.LogInformation($"Elevator:{Id} : {additionalInfo}");
            }
        }

        private async Task<bool> MoveElevator(int targetFloor)
        {
            MovingDirection movingDirection = targetFloor > CurrentFloor ? MovingDirection.Up : MovingDirection.Down;
            try
            {
                uint floorsToMove = (uint)Math.Abs(targetFloor - CurrentFloor.Value);
                int lastFloor = CurrentFloor.Value;
                await OperateDoor(DoorOperationRequest.Close);
                ElevatorState = movingDirection == MovingDirection.Up ? ElevatorState.MovingUp : ElevatorState.MovingDown;
                bool floorChangeResult;
                for (int i = 0; i < floorsToMove; i++)
                {
                    floorChangeResult = await MoveOne(lastFloor, movingDirection);
                    if (floorChangeResult)
                    {
                        lastFloor = CurrentFloor.Value;
                    }
                    else
                    {
                        ///future extension - set elevator state to Malfunction and proceed accordingly
                        throw new NotImplementedException();
                    }

                }
                await OperateDoor(DoorOperationRequest.Open);
                ElevatorState = ElevatorState.Ready;
                return true;
            }
            catch
            {
                return false;
            }
        }

        private async Task<bool> MoveOne(int lastFloor, MovingDirection movingDirection = MovingDirection.Up)
        {
            CurrentFloor = null;
            await Task.Delay((int)Math.Round(ElevatorMovingSpeed * 1000, 0));
            if (movingDirection == MovingDirection.Up)
            {
                CurrentFloor = lastFloor + 1;
            }
            else
            {
                CurrentFloor = lastFloor - 1;
            }
            return true;
        }

        private async Task<bool> OperateDoor(DoorOperationRequest doorOperationRequest)
        {
            ///lacks logic for when doors is already operating
            if (doorOperationRequest == DoorOperationRequest.Close)
            {

                DoorState = DoorState.Closing;
                await Task.Delay((int)Math.Round(DoorOperatingSpeed * 1000, 0));
                DoorState = DoorState.Closed;
            }
            else
            {
                DoorState = DoorState.Opening;
                await Task.Delay((int)Math.Round(DoorOperatingSpeed * 1000, 0));
                DoorState = DoorState.Opened;

            }
            return true;
        }
    }
}
