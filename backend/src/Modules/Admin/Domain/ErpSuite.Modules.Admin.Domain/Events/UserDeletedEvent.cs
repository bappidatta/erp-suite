using ErpSuite.BuildingBlocks.Domain.Events;

namespace ErpSuite.Modules.Admin.Domain.Events;

public record UserDeletedEvent(long UserId, string DeletedBy) : DomainEvent;
