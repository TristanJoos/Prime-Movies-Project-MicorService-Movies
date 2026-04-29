public record RoomData(
    Guid Id,
    string Name,
    int Capacity
)
{
    public RoomData() : this(Guid.Empty, string.Empty, 0)
    {
    }
}
