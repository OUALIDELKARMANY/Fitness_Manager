using System.Collections.Generic;

namespace Fitness_Manager.Models
{
    public class PlanSportif
    {
        public int Id { get; set; }
        public string Nom { get; set; }
        public string Objectif { get; set; }
        public int DureeSemaines { get; set; }

        public List<Seance> Seances { get; set; }
        public List<CoachPlanSportif> CoachPlanSportifs { get; set; }
    }
}
