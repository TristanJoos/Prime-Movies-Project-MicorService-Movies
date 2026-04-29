using Howestprime.Movies.Domain.Shared;
using Howestprime.Movies.Domain.Movies;
using Xunit;

namespace UnitTests.Domain.Movies;

public class RoomTests
{
    [Fact]
    public void Create_WithValidData_ReturnsRoom()
    {
        var roomId = new RoomId(Guid.NewGuid());
        var room = Room.Create(roomId, "Room A", 50);

        Assert.NotNull(room);
        Assert.Equal(roomId, room.Id);
        Assert.Equal("Room A", room.Name);
        Assert.Equal(50, room.Capacity);
    }

    [Fact]
    public void Create_WithoutId_GeneratesNewId()
    {
        var room = Room.Create(null, "Room B", 50);

        Assert.NotNull(room);
        Assert.NotEqual(default, room.Id.Value);
    }

    [Fact]
    public void Create_WithEmptyName_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => Room.Create(null, "", 50));
    }

    [Fact]
    public void Create_WithZeroCapacity_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => Room.Create(null, "Room A", 0));
    }
}