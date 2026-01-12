using System;

namespace PluginSample.Services.DTO
{
    public class QueueMessage
    {
        public string EntityName { get; set; }
        public Guid EntityId { get; set; }
        public string MessageName { get; set; }
        public int Stage { get; set; }
        public Guid UserId { get; set; }
    }
}