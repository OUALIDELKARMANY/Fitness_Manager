using System.Collections.Generic;

namespace Fitness_Manager.Models
{
    public class Coach : Utilisateur
    {
        public List<Client> Clients { get; set; }
        public List<CoachPlanSportif> CoachPlanSportifs { get; set; }
    }
}
