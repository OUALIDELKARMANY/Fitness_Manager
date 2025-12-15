using System.Collections.Generic;


namespace Fitness_Manager.Models
{
    public class Aliment
    {
        public int Id { get; set; }
        public string Nom { get; set; }
        public int Grammage { get; set; }

        public List<PlanNutritionnel> PlansNutritionnels { get; set; }
    }
}
