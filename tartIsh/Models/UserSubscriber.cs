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
    
    public partial class UserSubscriber
    {
        public int Id { get; set; }
        public int SubscriberId { get; set; }
        public int SubscribedId { get; set; }
        public System.DateTime SubscriptionDate { get; set; }
    
        public virtual TartIshUser TartIshUser { get; set; }
    }
}
