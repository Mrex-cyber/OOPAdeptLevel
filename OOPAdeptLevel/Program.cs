using System.Linq;
using System.Reflection;
using System.Dynamic;

namespace OOPAdeptLevel
{
    class Program
    {
        static async Task Main()
        {
            //                         THREADS

            Thread nameThread = new (new ThreadStart(PrintName));
            Thread ageThread = new (PrintAge);
            Thread bookThread = new (() => PrintBook());

            nameThread.Start();
            ageThread.Start();
            bookThread.Start();

            int x = 0;

            AutoResetEvent waitForThread = new (true);
            object lockerForThread = new();

            Mutex mutex = new();
            for (int i = 0; i <= 5; i++)
            {
                x = 1;
                Thread thread = new(SomeProcess);
                thread.Name = $"Thread number {i}";
                thread.Start();
            }

            void SomeProcess()
            {
                //mutex.WaitOne();
                waitForThread.WaitOne();
                x = 1;
                for (int i = 0; i < 5; i++)
                {
                    Console.WriteLine($"Some process with THREAD number {Thread.CurrentThread.Name}: {x}");
                    x++;
                    Thread.Sleep(100);
                }
                waitForThread.Set();
                //mutex.ReleaseMutex();
            }

            for (int i = 0; i <= 5; i++)
            {
                x = 1;
                Thread thread = new(SomeProcessWithMonitor);
                thread.Name = $"Thread with MONITOR number {i}";
                thread.Start();
            }

            void SomeProcessWithMonitor()
            {
                bool isBlocked = false;
                try
                {
                    Monitor.Enter(lockerForThread, ref isBlocked);
                    x = 1;
                    for (int i = 0; i < 5; i++)
                    {
                        Console.WriteLine($"Some process with THREAD of MONITOR number {Thread.CurrentThread.Name}: {x}");
                        x++;
                        Thread.Sleep(100);
                    }
                }
                finally
                {
                    if (isBlocked) Monitor.Exit(lockerForThread);
                }
            }

            for (int i = 0; i < 5; i++)
            {
                Consumer consumer = new (i);
            }

            Console.WriteLine("main start");
            Task outerTask = new (() =>
            {
                Console.WriteLine("Outer task start");
                for (int i = 0; i < 10; i++)
                {
                    Task innerTask = new (() => { Console.WriteLine($"Inner task №{i}"); });
                    innerTask.Start();
                    Thread.Sleep(1000);
                    innerTask.Wait();
                }
                Console.WriteLine("Outer task end");
            });
            outerTask.Start();
            Console.WriteLine("Main end");
            outerTask.Wait();


            int firstNumber = 5;
            int secondNumber = 6;
            Task<int> sumTask = new (() => Sum(firstNumber, secondNumber));

            Task<int> substractTask = sumTask.ContinueWith(task => Substract(sumTask.Result, secondNumber));
            sumTask.Start();
            Console.WriteLine(sumTask.Result);
            Console.WriteLine(substractTask.Result);
            substractTask.Wait();

            ParallelLoopResult parallelResult = Parallel.For(1, 5, Square);
            if (!parallelResult.IsCompleted) Console.WriteLine($"Parallel result is ended on the {parallelResult.LowestBreakIteration} iteration");

            CancellationTokenSource cancelToken = new ();
            CancellationToken token = cancelToken.Token;

            Task tokenTask = new (() =>
            {
                for (int i = 1; i < 10; i++)
                {
                    if (token.IsCancellationRequested)
                    {
                        Console.WriteLine("Operation was canceled");
                        return;
                    }
                    Console.WriteLine($"Token task number {i}");
                    Thread.Sleep(200);
                }
            }, token);

            tokenTask.Start();

            Thread.Sleep(1350);
            cancelToken.Cancel();
            Thread.Sleep(1000);
            Console.WriteLine($"Token status: {tokenTask.Status}");            

            await SumAsync(firstNumber, secondNumber);

            var firstSubstract = SubstractAsync(5, 3);
            var secondSubstract = SubstractAsync(10, 3);
            var thirdSubstract = SubstractAsync(18, 3);

            await firstSubstract;
            Console.WriteLine("Main after 1");
            await secondSubstract;
            Console.WriteLine("Main after 2");
            await thirdSubstract;
            Console.WriteLine("Main after 3");

            int anyTaskResult = Task.WaitAny(firstSubstract, secondSubstract, thirdSubstract);
            int[] taskArray = await Task.WhenAll(firstSubstract, secondSubstract, thirdSubstract);
            switch (anyTaskResult)
            {
                case 0:
                    Console.WriteLine("It is first Substract");
                    break;
                case 1:
                    Console.WriteLine("It is first Substract");
                    break;
                case 2:
                    Console.WriteLine("It is third Substract");
                    break;
                default:
                    Console.WriteLine("Underfined");
                    break;
            }
            foreach (var taskResult in taskArray)
            {
                Console.WriteLine($"Result of task is {taskResult}");
            }

            //                            ASYNCHRONOUS PROGRAMMING


            var taskWithShortName = PrintAsync("My nm s MREX");
            var taskWithLongName = PrintAsync("I am MREX");
            var taskWithVeryLongName = PrintAsync("My name is MREX");
            var taskWithVeryShortName = PrintAsync("I'm MREX");
            var allTasks = Task.WhenAll(taskWithShortName, taskWithLongName);
            try
            {
                await allTasks;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine($"Is faulted long name: {taskWithLongName.IsFaulted}");
                Console.WriteLine($"Is faulted short name: {taskWithShortName.IsFaulted}");
                Console.WriteLine($"Is faulted very long name: {taskWithVeryLongName.IsFaulted}");
                Console.WriteLine($"Is faulted very short name: {taskWithVeryShortName.IsFaulted}");

                if (allTasks.Exception is not null)
                {
                    foreach (var task in allTasks.Exception.InnerExceptions)
                    {
                        Console.WriteLine($"Inner exception is: {task.Message}");
                    }
                }
            }

            ReposytoryWithClassmates reposytory = new ();
            IAsyncEnumerable<string> data = reposytory.GetDataAsync();
            await foreach (var name in data)
            {
                Console.WriteLine(name);
            }

            //                                LINQ 


            string[] names = new string[] { "Ivan", "Stas", "Vasya", "Maks", "Valik", "Diana", "Dasha", "Vika", "Vasylyna" };

            var listLinqOperators = from pupils in names where pupils.Length < 6 orderby pupils.Length select pupils;
            var listLinqMethods = names.Where(people => people.Length > 4).OrderBy(names => names);
            Console.WriteLine("\t\tLinq request with operators");
            foreach (var name in listLinqOperators)
            {
                Console.WriteLine(name);
            }
            Console.WriteLine("\t\tLinq request with methods");
            foreach (var name in listLinqMethods)
            {
                Console.WriteLine(name);
            }

            var companies = new List<Company>
            {
                new Company("Microsoft", new List<Person>{new Person("Valik", 25, new List<string> { "English", "French" }), new Person("Ivan", 25, new List<string> { "English", "French" }), new Person("Vika", 25, new List<string> { "Japan", "French" }) }),
                new Company("Amazon", new List<Person>{new Person("Nastya", 25, new List<string> { "English", "French" }), new Person("Vasya", 25, new List<string> { "English", "Ukrainian" }), new Person("Nika", 25, new List<string> { "Germany", "Spain" }) })
            };

            var employeesToPrint = from company in companies
                            from employee in company.Employees!
                            select new { Name = employee.Name, Company = company.Name };
            foreach (var employee in employeesToPrint)
            {
                Console.WriteLine($"{employee.Name} - {employee.Company};");
            }

            var people = new List<Person>()
            {
                new Person("Vasya", 25, new List<string>{"English", "French"}),
                new Student("Misha", 37, new List<string>{"Bolgarian", "English"}),
                new Student("Mike", 52, new List<string>{"English", "Russian"}),
                new Employee("Veronika", companies[0], 20, new List<string>{"Japanese", "Chinese"}),
                new Person("David", 15, new List<string>{ "French", "Germany"}),
                new Manager("Gustav", companies[0], 34, new List<string>{"English", "French"}),
                new Employee("Daryna", companies[1], 29, new List<string>{"Mexic", "Ukrainian"}),
                new Student("Natasha", 20, new List<string>{"Japanese", "Germany"})
            };

            var peopleForCompare = new List<Person>()
            {
                new Person("Vasya", 25, new List<string>{"English", "French"}),
                new Student("Ivan", 37, new List<string>{"Bolgarian", "English"}),
                new Student("Daniel", 52, new List<string>{"English", "Russian"}),
                new Employee("Misha", companies[0], 20, new List<string>{"Japanese", "Chinese"}),
                new Person("Vasya", 15, new List<string>{ "French", "Germany"}),
                new Manager("Taras", companies[1], 34, new List<string>{"English", "French"}),
                new Employee("Anastasia", companies[0], 29, new List<string>{"Mexic", "Ukrainian"}),
                new Student("Valerka", 20, new List<string>{"Japanese", "Germany"})
            };

            var studentList = people.OfType<Student>();
            foreach (var student in studentList) Console.WriteLine($"Student: {student.Name}");
            var employeeList = from employee in people where employee is Employee select employee;
            foreach (var employee in employeeList) Console.WriteLine($"Employee: {employee.Name}");

            var maturePeopleWithEnglish = people.SelectMany(people => people.Languages,
                (people, language) => new { Person = people, Language = language })
                .Where(people => people.Language == "English" && people.Person.Age > 25)
                .Select(people => people.Person).OrderByDescending(people => people.Age);
            foreach (var person in maturePeopleWithEnglish) Console.WriteLine($"{person.Name} - {person.Age}");

            Console.WriteLine("People with equals");
            Console.WriteLine(people.Equals(peopleForCompare));

            Console.WriteLine("UNION");
            var allPeople = people.Union(peopleForCompare);
            foreach (var person in allPeople) Console.WriteLine(person.Name);

            Console.WriteLine("TAKE and SKIP");
            var result = allPeople.Skip(5).Take(4).Skip(1).Take(2);
            foreach (var person in result) Console.Write(person.Name + ", ");

            Employee[] employees =
            {
                new Employee("Tom", companies[0], 35, new List<string>{"English"}),
                new Employee("Sam", companies[0], 47, new List<string>{"English"}),
                new Employee("Bob", companies[1], 26, new List<string>{"English"}),
                new Employee("Mike", companies[1], 52, new List<string>{"English"}),
                new Employee("Kate", companies[0], 32, new List<string>{"English"}),
                new Employee("Alice", companies[1], 41, new List<string>{"English"}),
            };

            var companiesToPrint = from c in employees
                            group c by c.Company.Name into g
                            select new
                            {
                                Name = g.Key,
                                Count = g.Count(),
                                Employees = from e in g select e
                            };
            foreach (var company in companiesToPrint)
            {
                Console.WriteLine($"{company.Name} : {company.Count}");

                foreach (var employee in company.Employees)
                {
                    Console.WriteLine(employee.Name);
                }
                Console.WriteLine();
            }

            var joins = companies.GroupJoin(employees,
                c => c.Name,
                e => e.Company.Name,
                (c, Employees) => new { c.Name, Employees });

            foreach (var company in joins)
            {
                Console.WriteLine($"Company: {company.Name}");
                Console.WriteLine("Employees: ");
                foreach (var employee in company.Employees)
                {
                    Console.WriteLine(employee.Name);
                }
                Console.WriteLine();
            }

            bool isTom = employees.Any(emp => emp.Name == "tom");
            Console.WriteLine(isTom);

            //                                  PARALLEL LINQ
            int[] numbers = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, };
            var squares = from num in numbers.AsParallel().AsOrdered()
                          where num >= 0
                          select SquareNumber(num);
            (from num in numbers.AsParallel() select SquareNumber(num)).ForAll(Console.WriteLine);
            foreach (var square in squares) Console.WriteLine(square);

            new Task(() =>
            {
                Thread.Sleep(400);
                cancelToken.Cancel();
            }).Start();

            try
            {
                int[] nums = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
                var sqrs = from num in numbers.AsParallel().WithCancellation(cancelToken.Token)
                              select SquareNumber(num);
                foreach (var square in squares) Console.WriteLine(square);
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Operation was cancelled.");
            }
            catch (AggregateException aggException)
            {
                if (aggException.InnerException != null)
                {
                    foreach (Exception exception in aggException.InnerExceptions)
                    {
                        Console.WriteLine($"Was thrown Exception: {exception.Message}");
                    }
                }
            }
            finally
            {
                cancelToken.Dispose();
            }



            //                                          REFLECTION

            var typeOfPerson = typeof(Person);
            Console.WriteLine(typeOfPerson.Name);
            Console.WriteLine(typeOfPerson.FullName);

            foreach (var type in typeOfPerson.GetMembers(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic))
            {
                Console.WriteLine($"{type.DeclaringType}: {type.MemberType} {type.Name};");
            }

            var personMembers = typeOfPerson.GetMember("Print", BindingFlags.Instance | BindingFlags.Public);
            foreach (var item in personMembers)
            {
                Console.WriteLine($"{item.MemberType} {item.Name}");
            }

            foreach (FieldInfo field in typeOfPerson.GetFields(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public))
            {
                string modificator = "";

                if (field.IsPublic) modificator += "public ";
                else if (field.IsPrivate) modificator += "private ";
                else if (field.IsAssembly) modificator += "internal ";
                else if (field.IsFamily) modificator += "protected ";
                else if (field.IsFamilyAndAssembly) modificator += "private protected ";
                else if (field.IsFamilyOrAssembly) modificator += "protected internal ";

                if (field.IsStatic) modificator += "static ";

                Console.WriteLine($"{modificator} {field.FieldType.Name} {field.Name}");
            }

            Person reflectionTom = new ("Tom", 34, new List<string> { "English" });
            Console.WriteLine(reflectionTom.Name);
            var nameProperty = typeOfPerson.GetProperty("Name");
            var nameOfReflectionTom = nameProperty?.GetValue(reflectionTom);

            nameProperty?.SetValue(reflectionTom, "Tomas");

            Console.WriteLine(reflectionTom.Name);
        }
        public static int SquareNumber(int n) 
        {
            int result = n * n;
            Console.WriteLine($"Square of the number {n} is {result}");
            Thread.Sleep(1000);
            return result;
        }
        public static async Task PrintAsync(string? message)
        {
            if (message == null || message.Contains('a'))
                throw new ArgumentException("Message contain \"a\"");
            await Task.Delay(1000);
            Console.WriteLine(message);            
        }
        public static async Task SumAsync(int firstNumber, int secondNumber)
        {
            Console.WriteLine("Sum async started");
            Thread.Sleep(1000);
            await Task.Run(() => Sum(firstNumber, secondNumber));
            Console.WriteLine("Sum async ended");
        }
        public static async Task<int> SubstractAsync(int firstNumber, int secondNumber)
        {
            Console.WriteLine("Substract async started");
            // Thread.Sleep(1000);
            await Task.Delay(3000);
            await Task.Run(() => Substract(firstNumber, secondNumber));
            Console.WriteLine("Substract async ended");
            return firstNumber - secondNumber;
        }
        public static void Square(int x, ParallelLoopState parallelLoopState)
        {
            if (x >= 2) parallelLoopState.Break();
            Console.WriteLine($"Square of number {x} is {x * x}");
            Thread.Sleep(1000);
        }
        public static int Sum(int x, int y) => x + y;
        public static int Divide(int x, int y) => x / y;
        public static int Substract(int x, int y) => x - y;
        public static int Multiply(int x, int y) => x * y;

        public static void PrintName()
        {
            for (int i = 0; i < 5; i++)
            {
                Console.WriteLine($"NAME thread {i}");
                Thread.Sleep(1000);
            }
        }
        public static void PrintAge()
        {
            for (int i = 0; i < 5; i++)
            {
                Console.WriteLine($"AGE thread {i}");
                Thread.Sleep(2000);
            }
        }
        public static void PrintBook()
        {
            for (int i = 0; i < 5; i++)
            {
                Console.WriteLine($"BOOK thread {i}");
                Thread.Sleep(3000);
            }
        }

    }
}