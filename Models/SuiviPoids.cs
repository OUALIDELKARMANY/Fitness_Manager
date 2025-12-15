using System;

namespace Fitness_Manager.Models
{
    public class SuiviPoids
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public double Poids { get; set; }

        public int ClientId { get; set; }
        public Client Client { get; set; }
    }
}
