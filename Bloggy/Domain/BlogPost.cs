using System;
using System.Collections.Generic;
using System.Text;

namespace Bloggy.Domain
{
    class BlogPost
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Description { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }

        public List<Tag> Tags { get; set; } = new List<Tag>();

    }
}
