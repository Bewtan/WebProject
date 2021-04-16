using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entities
{
    public class Reservation
    {
        public int Id { get; set; }
        public int RoomId { get; set; }
        public virtual Room Room { get; set; }
        public string UserId { get; set; }
        public virtual User User { get; set; }
        public virtual ICollection<Client> Clients { get; set; }
        public void a(int b)
        {
            this.Clients.Select(a => a.FirstName + a.LastName).ToList();
        }
        public DateTime DateOfArrival { get; set; }
        public DateTime DateOfLeaving { get; set; }
        public bool IsBreakfastIncluded { get; set; }
        public bool IsAllInclusive { get; set; }
        public decimal Cost { get; set; }
       
    }
}
