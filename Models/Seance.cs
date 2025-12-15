using System.Collections.Generic;

namespace Fitness_Manager.Models
{
    public class Seance
    {
        public int Id { get; set; }
        public string Nom { get; set; }
        public int DureeMinutes { get; set; }

        public int PlanSportifId { get; set; }
        public PlanSportif PlanSportif { get; set; }

        public List<Exercice> Exercices { get; set; }
    }
}
