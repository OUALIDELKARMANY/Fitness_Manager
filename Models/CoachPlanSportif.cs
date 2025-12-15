namespace Fitness_Manager.Models
{
    public class CoachPlanSportif
    {
        public int CoachId { get; set; }
        public Coach Coach { get; set; }

        public int PlanSportifId { get; set; }
        public PlanSportif PlanSportif { get; set; }
    }
}
