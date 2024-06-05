namespace GenerateData.Models
{
    public class GenerateDataViewModel
    {
        public List<Hobby>? Hobbies { get; set; }
        public List<Gender>? Genders { get; set; }
    }

    public class Hobby
    {
        public int Id { get; set; }
        public string? Nama { get; set; }
    }

    public class Gender
    {
        public int Id { get; set; }
        public string? Nama { get; set; }
    }

}