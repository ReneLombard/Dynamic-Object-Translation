using System;
using System.Collections.Generic;

namespace ReneLombard.Translation.Sample.Models
{
    [Serializable]
    public class BlogPost
    {
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public BlogContent Content { get; set; }
        public Author Author { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public DateTime? PublishedDate { get; set; }
    }

    [Serializable]
    public class BlogContent
    {
        public List<BlogSubSection> Sections { get; set; }
    }
    [Serializable]
    public class BlogSubSection
    {
        public SectionType Type { get; set; }
        public string Data { get; set; }
        public string Heading { get; set; }
        public string Description { get; set; }
    }
    [Serializable]
    public enum SectionType
    {
        Paragraph = 1,
        Div = 2,
        Gallery = 3
    }
    [Serializable]
    public class Author
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public int Age { get; set; }
    }
}
