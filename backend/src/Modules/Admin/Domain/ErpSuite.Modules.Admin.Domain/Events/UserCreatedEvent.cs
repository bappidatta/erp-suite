using ErpSuite.BuildingBlocks.Domain.Events;
using ErpSuite.Modules.Admin.Domain.Entities;

namespace ErpSuite.Modules.Admin.Domain.Events;

public record UserCreatedEvent(User User, string CreatedBy) : DomainEvent;
