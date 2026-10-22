using System;
using System.Collections.Generic;
using System.IO;
namespace Project
{
    enum Role
    {
        Programmer,
        Designers,
        Composer,
        Tester,
        Moderator
    }
    class Employee
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Role Position { get; set; }
        public Employee(int id, string name, Role position)
        {
            Id = id;
            Name = name;
            Position = position;
        }
    }
    class WorkLog
    {
        public int EmployeeId { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int hours { get; set; }
        public WorkLog(int employeeId, DateTime date, TimeSpan startTime, TimeSpan endTime)
        {
            EmployeeId = employeeId;
            Date = date;
            StartTime = startTime;
            EndTime = endTime;
            hours = (int)(EndTime - StartTime).TotalHours;
        }
    }
    class OverTime : EventArgs
    {
        public int EmployeeId { get; }
        public Role Role { get; }
        public DateTime Date { get; }
        public int OvertimeHours { get; }
        public OverTime(int employeeId, Role role, DateTime date, int overtimeHours)
        {
            Role = role;
            EmployeeId = employeeId;
            Date = date;
            OvertimeHours = overtimeHours;
        }
    }
    class CompanySystem
    {
        public List<Employee> Employees { get; } = new List<Employee>();
        public List<WorkLog> Logs { get; } = new List<WorkLog>();
        public event EventHandler<OverTime> OverTimeDetected;
        public void AddEmployee(Employee emp) => Employees.Add(emp);
        public void RegisterShift(int empId, DateTime date, TimeSpan start, TimeSpan End)
        {
            Employee emp = Employees.Find(e => e.Id == empId);
            if (emp == null) { return; }
            TimeSpan companyOpen = new TimeSpan(9, 0, 0);
            TimeSpan companyClose = new TimeSpan(19, 0, 0);

            if (start < companyOpen || End > companyClose)
            {
                Console.WriteLine($"[SYSTEM WARNING]: Employee ID {empId} out of company hours limit (09:00-19:00)!");
            }
            WorkLog log = new WorkLog(empId, date, start, End);
            Logs.Add(log);
            if (log.hours > 8)
            {
                Console.WriteLine($"[INFO]: Employee ID {empId} completed a long shift ({log.hours} hours). Standard 8-hour limit exceeded.");
            }
            bool isWeekend = date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday;
            int overtimeFromHours = log.hours > 8 ? log.hours - 8 : 0;
            int overtimeFromWeekend = isWeekend ? log.hours : 0;
            int OvertimeHours = overtimeFromHours + overtimeFromWeekend;
            if (OvertimeHours > 0)
            {
                OverTime args = new OverTime(empId, emp.Position, date, OvertimeHours);
                OverTimeDetected?.Invoke(this, args);
            }
        }
    }
    class PrintPayroll
    {
        public void Print(CompanySystem system, ISalaryCalculator calculator)
        {
            foreach (var emp in system.Employees)
            {
                int totalHours = 0;
                foreach (var log in system.Logs)
                {
                    if (log.EmployeeId == emp.Id) { totalHours += log.hours; }
                }
                decimal salary = calculator.CalculateSalary(emp.Position, totalHours);
                Console.WriteLine($"Employee: {emp.Name}, Position: {emp.Position}, Total Hours: {totalHours}, Salary: {salary}");
            }
        }
    }
    interface ISalaryCalculator
    {
        decimal CalculateSalary(Role Position, int totalHours);
    }
    class CompanySalaryCalculator : ISalaryCalculator
    {
        public decimal CalculateSalary(Role Position, int totalHours)
        {
            decimal hourlyRate;
            switch (Position)
            {
                case Role.Programmer:
                    hourlyRate = 800;
                    break;
                case Role.Designers:
                    hourlyRate = 650;
                    break;
                case Role.Composer:
                    hourlyRate = 650;
                    break;
                case Role.Tester:
                    hourlyRate = 550;
                    break;
                case Role.Moderator:
                    hourlyRate = 350;
                    break;
                default:
                    hourlyRate = 350;
                    break;
            }
            ;
            return totalHours * hourlyRate;
        }
    }

    class Program
    {
        static void Main()
        {
            Console.WriteLine("Corporation Employee Management System");
            CompanySystem system = new CompanySystem();
            string empFile = "employees.txt";
            string logFile = "worklogs.txt";
            system.OverTimeDetected += (sender, e) =>
            {
                Console.WriteLine($"[ALERT] Overtime for Employee ID: {e.EmployeeId} on {e.Date.ToShortDateString()}: +{e.OvertimeHours} hours.");
            };
            Employee emp1 = new Employee(1, "Programmer_Rust_1", Role.Programmer); //Nicknames for employees
            system.AddEmployee(emp1);
            File.AppendAllText(empFile, $"{emp1.Id};{emp1.Name};{emp1.Position}\n");
            Employee emp2 = new Employee(2, "Designers_1", Role.Designers);
            system.AddEmployee(emp2);
            File.AppendAllText(empFile, $"{emp2.Id};{emp2.Name};{emp2.Position}\n");
            Employee emp3 = new Employee(3, "Composer_1", Role.Composer);
            system.AddEmployee(emp3);
            File.AppendAllText(empFile, $"{emp3.Id};{emp3.Name};{emp3.Position}\n");
            Employee emp4 = new Employee(4, "Tester_1", Role.Tester);
            system.AddEmployee(emp4);
            File.AppendAllText(empFile, $"{emp4.Id};{emp4.Name};{emp4.Position}\n");
            Employee emp5 = new Employee(5, "Moderator_1", Role.Moderator);
            system.AddEmployee(emp5);
            File.AppendAllText(empFile, $"{emp5.Id};{emp5.Name};{emp5.Position}\n");
            Employee emp6 = new Employee(6, "Programmer_C#_1", Role.Programmer);
            system.AddEmployee(emp6);
            File.AppendAllText(empFile, $"{emp6.Id};{emp6.Name};{emp6.Position}\n");
            Console.WriteLine("\nRegistering work shifts...");
            system.RegisterShift(1, DateTime.Today, new TimeSpan(11, 0, 0), new TimeSpan(17, 0, 0));
            system.RegisterShift(2, DateTime.Today, new TimeSpan(10, 0, 0), new TimeSpan(19, 0, 0));
            File.AppendAllText(logFile, $"ID: 2; Date: {DateTime.Today.ToShortDateString()}; Time: 10:00-19:00\n");
            system.RegisterShift(3, DateTime.Today, new TimeSpan(11, 0, 0), new TimeSpan(19, 0, 0));
            File.AppendAllText(logFile, $"ID: 3; Date: {DateTime.Today.ToShortDateString()}; Time: 11:00-19:00\n");
            system.RegisterShift(4, DateTime.Today, new TimeSpan(9, 0, 0), new TimeSpan(15, 0, 0));
            File.AppendAllText(logFile, $"ID: 4; Date: {DateTime.Today.ToShortDateString()}; Time: 9:00-15:00\n");
            system.RegisterShift(5, DateTime.Today, new TimeSpan(11, 0, 0), new TimeSpan(17, 0, 0));
            File.AppendAllText(logFile, $"ID: 5; Date: {DateTime.Today.ToShortDateString()}; Time: 13:00-19:00\n");
            system.RegisterShift(6, DateTime.Today, new TimeSpan(10, 0, 0), new TimeSpan(17, 0, 0));
            File.AppendAllText(logFile, $"ID: 6; Date: {DateTime.Today.ToShortDateString()}; Time: 10:00-17:00\n");
            CompanySalaryCalculator calculator = new CompanySalaryCalculator();
            PrintPayroll payroll = new PrintPayroll();
            Console.WriteLine("\nPayroll Report:");
            payroll.Print(system, calculator);
        }
    }
}
