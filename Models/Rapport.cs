using System;

namespace Fitness_Manager.Models
{
    public class Rapport
    {
        public int Id { get; set; }
        public DateTime DateCreation { get; set; }
        public string Chemin_Rapport { get; set; }

        public int ClientId { get; set; }
        public Client Client { get; set; }
    }
}
