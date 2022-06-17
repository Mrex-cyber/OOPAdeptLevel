namespace OOPAdeptLevel
{
    class Person
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public List<string> Languages { get; set; }
        public Person(string name, int age, List<string> languages) 
        {
            Name = name;
            Age = age;
            Languages = languages;
        }

        public void Print() => Console.WriteLine($"Name: {Name} Age: {Age}");

        public override bool Equals(object? obj)
        {
            if (obj is Person person) return person.Name == Name;
            return false;
        }
        public override int GetHashCode() => Languages.GetHashCode();
    }
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
    class Student : Person
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public List<string> Languages { get; set; }
        public Student(string name, int age, List<string> languages) : base(name, age, languages)
        {
            Name = name;
            Age = age;
            Languages = languages;
        }
    }
    class Employee : Person
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public List<string> Languages { get; set; }
        public Company Company { get; set; }
        public Employee(string name, Company company, int age, List<string>? languages) : base(name, age, languages)
        {
            Name = name;
            Company = company;
            Age = age;
            Languages = languages;
        }
        public override bool Equals(object? obj)
        {
            if (obj is Employee emp) return Name == emp.Name;
            return false;
        }
        public override int GetHashCode() => Name.GetHashCode();
    }
    class Manager : Employee
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public List<string> Languages { get; set; }
        public Manager(string name, Company company, int age, List<string>? languages) : base(name, company, age, languages)
        {
            Name = name;
            Age = age;
            Languages = languages;
        }
    }    
}
