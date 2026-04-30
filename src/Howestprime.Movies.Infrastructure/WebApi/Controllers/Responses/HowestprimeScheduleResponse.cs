using System.ComponentModel;

public sealed record HowestprimeScheduleResponse(
    List<Guid> MovieIds,
    List<MovieEventResponse> MovieEvents
);