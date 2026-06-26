namespace LitXusCount.Application.Accounts;

public record AccTransferDto(
    long Id,
    long SenderAccountId,
    string SenderAccountName,
    long ReceiverAccountId,
    string ReceiverAccountName,
    DateTime TransferDate,
    decimal Amount,
    string? Note,
    bool IsActive);

public record AccTransferCreateDto(
    long SenderAccountId,
    long ReceiverAccountId,
    DateTime TransferDate,
    decimal Amount,
    string? Note);
