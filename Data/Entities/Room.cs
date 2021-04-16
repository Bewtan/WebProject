using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entities
{
    public class Room
    {
        public int Id { get; set; }

        public int Capacity { get; set; }

        public string Type { get; set; }

        public decimal PriceForAdult { get; set; }

        public decimal PriceForKid { get; set; }

        public int Number { get; set; }
        public virtual Reservation Reservation { get; set; }

    }
}
