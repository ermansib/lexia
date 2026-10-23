using Lexia.BuildingBlocks.Domain;

namespace Lexia.Books.Domain.Books;

public sealed record BookCreatedDomainEvent(Guid BookId, Guid OwnerId, BookType Type, DateTimeOffset OccurredAt) : IDomainEvent;

public sealed record BookStageChangedDomainEvent(Guid BookId, BookStage From, BookStage To, DateTimeOffset OccurredAt) : IDomainEvent;
