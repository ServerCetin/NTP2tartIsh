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
    
    public partial class RoomComplaint
    {
        public int Id { get; set; }
        public string Reason { get; set; }
        public int SenderId { get; set; }
        public int TartIshRoomId { get; set; }
    
        public virtual TartIshRoom TartIshRoom { get; set; }
    }
}
