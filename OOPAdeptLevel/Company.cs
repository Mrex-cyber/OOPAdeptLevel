namespace OOPAdeptLevel
{
    class Company
    {
        public string Name { get; set; }
        public List<Person>? Employees { get; set; }
        public Company(string name, List<Person>? employees)
        {
            Name = name;
            Employees = new List<Person>();
        }
    }
}
