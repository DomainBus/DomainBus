namespace DomainBus.Audit
{
    /// <summary>
    /// It's used as a singleton. Should be thread safe
    /// </summary>
    public interface IStoreAudits
    {
        void Append(params MessageTransportAudit[] audits);
        void Append(params MessageRejectedAudit[] audits);
        void Append(MessageProcessingAudit audits);
        void Append(params  StoredMessageAudit[] audits);

    }
}