namespace ElevatorControl.Models
{
    /// <summary>
    /// Possible state of the elevator doors
    /// </summary>
    public enum DoorState
    {
        Unknown,
        Opened,
        Closed,
        Opening,
        Closing
    }

    /// <summary>
    /// Possible state of the elevator
    /// </summary>
    public enum ElevatorState
    {
        Unknown,
        Ready,
        MovingUp,
        MovingDown
    }

    /// <summary>
    /// Possible moving directions
    /// </summary>
    public enum MovingDirection
    {
        Up,
        Down
    }

    /// <summary>
    /// Possible request for door operation
    /// </summary>
    public enum DoorOperationRequest
    {
        Open,
        Close
    }

}
