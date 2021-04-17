using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entities
{
    public class Reservation
    {
        public Reservation()
        {
            Clients = new List<Client>();
            ClientsIDs = new List<int>();
        }
        public int Id { get; set; }
        public int RoomId { get; set; }
        public virtual Room Room { get; set; }
        public string UserId { get; set; }
        public virtual User User { get; set; }
        [NotMapped]
        public ICollection<int> ClientsIDs { get; set; }
        public virtual ICollection<Client> Clients { get; set; }
        public DateTime DateOfArrival { get; set; }
        public DateTime DateOfLeaving { get; set; }
        public bool IsBreakfastIncluded { get; set; }
        public bool IsAllInclusive { get; set; }
        public decimal Cost { get; set; }
       
    }
}
