using System.Collections.Generic;

namespace Fitness_Manager.Models
{
    public class PlanNutritionnel
    {
        public int Id { get; set; }
        public string Objectif { get; set; }
        public int CaloriesCible { get; set; }

        public List<Aliment> Aliments { get; set; }
    }
}
