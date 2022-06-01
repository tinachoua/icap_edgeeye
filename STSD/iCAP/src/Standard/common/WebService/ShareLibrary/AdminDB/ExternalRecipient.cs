using System;
using System.Collections.Generic;
using System.Text;

namespace ShareLibrary.AdminDB
{
    public class ExternalRecipient
    {
        public ExternalRecipient()
        {
            ThresholdExternalRecipientList = new HashSet<ThresholdExternalRecipientList>();
        }
        public int Id { get; set; }
        public string Email { get; set; }

        public virtual ICollection<ThresholdExternalRecipientList> ThresholdExternalRecipientList { get; set; }
    }
}
