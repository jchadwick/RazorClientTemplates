using System.Collections.Generic;

namespace RazorClientTemplates.Website.Models
{
    public class Movie
    {
        public string Title { get; set; }
        public string ReleaseDate { get; set; }
        public string RunningTime { get; set; }
        public IEnumerable<Actor> Actors { get; set; }
    }

    public class Actor
    {
        public string Name { get; set; }

        public Actor(string name)
        {
            Name = name;
        }
    }
}