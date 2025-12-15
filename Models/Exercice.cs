namespace Fitness_Manager.Models
{
    public class Exercice
    {
        public int Id { get; set; }
        public string Nom { get; set; }
        public int Series { get; set; }
        public int Repetitions { get; set; }
        public int TempsRepos { get; set; }

        public int SeanceId { get; set; }
        public Seance Seance { get; set; }
    }
}
