//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace tartIsh.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class PollOption
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public PollOption()
        {
            this.UserSelectedPolls = new HashSet<UserSelectedPoll>();
        }
    
        public int Id { get; set; }
        public string PoolTitle { get; set; }
        public int PoolId { get; set; }
        public int VoteCount { get; set; }
    
        public virtual Poll Poll { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UserSelectedPoll> UserSelectedPolls { get; set; }
    }
}
