using Howestprime.Movies.Domain.Movies.Events;
using Howestprime.Movies.Domain.Movies.ValueObjects;
using Howestprime.Movies.Domain.Shared;

namespace Howestprime.Movies.Domain.Movies;

public readonly record struct RoomId(Guid Value) : IEntityId;

public sealed class Room : AggregateRoot<RoomId>
{
    public string Name { get; private set; }
    public int Capacity { get; private set; }

    public static Room Create(
        RoomId? Id,
        string Name,
        int Capacity
    )
    {
        Asserts.EnsureNotEmpty(Name);
        Asserts.EnsureGreaterThan(Capacity, 0);

        Room room = new Room(
            Id ?? EntityId.New<RoomId>(),
            Name,
            Capacity
        );

        return room;
    }

    private Room(
        RoomId id,
        string Name,
        int Capacity
    ) : base(id)
    {
        this.Name = Name;
        this.Capacity = Capacity;

        ValidateState();
    }

    // Required for ORM / Serialization
#pragma warning disable CS8618 
    private Room() : base(default!) { }
#pragma warning restore CS8618


    public override void ValidateState()
    {
        Asserts.EnsureNotEmpty(Name);
        Asserts.EnsureGreaterThan(Capacity, 0);

    }
}