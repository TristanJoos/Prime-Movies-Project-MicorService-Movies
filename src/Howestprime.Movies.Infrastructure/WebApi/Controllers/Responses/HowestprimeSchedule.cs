using System.ComponentModel;

public sealed record HowestprimeSchedule(
    List<Guid> MovieIds,
    List<MovieEvent> MovieEvents
);