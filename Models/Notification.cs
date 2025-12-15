using System;


namespace Fitness_Manager.Models
{
    public class Notification
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public DateTime DateEnvoi { get; set; }

        public int ClientId { get; set; }
        public Client Client { get; set; }
    }
}
