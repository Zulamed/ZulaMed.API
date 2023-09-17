using ZulaMed.API.Domain.Shared;

namespace ZulaMed.API.Domain.SpecialtyGroup;

public class SpecialtyGroup
{
    public required SequentialId Id { get; init; }
    public required SpecialtyGroupName Name { get; init; }
}