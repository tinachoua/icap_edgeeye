using System;
using System.Collections.Generic;
using System.Text;

namespace ShareLibrary.AdminDB
{
    public class ThresholdExternalRecipientList
    {
        public int Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool DeletedFlag { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public int ThresholdId { get; set; }
        public int ExternalRecipientId { get; set; }
        public virtual ExternalRecipient ExternalRecipient { get; set; }
        public virtual Threshold Threshold { get; set; }
    }
}
