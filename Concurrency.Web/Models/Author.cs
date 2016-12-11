namespace Concurrency.Web.Models
{
    public class Author
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string OriginalFirstName { get; set; }
        public string OriginalLastName { get; set; }

        public void ToWebModel(Core.Models.Author author)
        {
            this.Id = author.Id;
            this.FirstName = author.FirstName;
            this.LastName = author.LastName;
            this.OriginalFirstName = author.FirstName;
            this.OriginalLastName = author.LastName;
        }
    }
}