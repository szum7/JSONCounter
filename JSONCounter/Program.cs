using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace JSONCounter
{
    class JSONCounter
    {
        readonly string _PATH; 
        public List<Course> Courses { get; set; }
        public int TotalJSONCount
        {
            get
            {
                if (Courses != null && Courses.Count > 0)
                    return Courses.Sum(c => c.JSONCount);
                return 0;
            }
        }
        public Course CurrentCourse
        {
            get
            {
                if (Courses != null && Courses.Count > 0)
                    return Courses[Courses.Count - 1];
                return null;
            }
        }

        public JSONCounter()
        {
            _PATH = AppDomain.CurrentDomain.BaseDirectory + "JSONCount.txt";

            ReadFromFile();
        }

        public void AddJSONCount()
        {
            CurrentCourse.JSONCount++;
        }

        public void UndoJSONCount()
        {
            CurrentCourse.JSONCount--;
        }

        public void NewJSONTopic(string title, string duration)
        {
            Courses.Add(new Course()
            {
                title = title,
                duration = duration
            });
        }

        void ReadFromFile()
        {
            using (StreamReader r = new StreamReader(_PATH))
            {
                string json = r.ReadToEnd();
                Courses = JsonConvert.DeserializeObject<List<Course>>(json);
            }
        }

        public void SaveToFile()
        {
            string json = JsonConvert.SerializeObject(Courses.ToArray(), Formatting.Indented);
            File.WriteAllText(_PATH, json);
        }

        public override string ToString()
        {
            return string.Join(Environment.NewLine, Courses.Select(c => $"{c.title}: {c.JSONCount}"));
        }
    }

    public class Course
    {
        public string title;
        public string duration;
        public int JSONCount;
    }

    class Program
    {
        static void Main(string[] args)
        {
            JSONCounter jc = null;
            ConsoleKeyInfo input = new ConsoleKeyInfo();
            while (input.Key != ConsoleKey.Escape)
            {
                if (jc == null)
                    jc = new JSONCounter();

                input = Console.ReadKey();

                if(input.Key == ConsoleKey.Tab)
                {
                    Console.Clear();
                }
                else if (input.Key == ConsoleKey.Spacebar)
                {
                    Console.WriteLine(jc.ToString());
                }
                else if (input.Key == ConsoleKey.Escape)
                {
                    jc.SaveToFile();
                }
                else if (input.Key == ConsoleKey.Enter)
                {
                    Console.WriteLine($"Provide the course name:");
                    string title = Console.ReadLine();
                    Console.WriteLine($"Provide the course's duration:");
                    string duration = Console.ReadLine();

                    jc.NewJSONTopic(title, duration);
                }
                else
                {
                    if (input.Key == ConsoleKey.Add)
                    {
                        jc.AddJSONCount();
                    }
                    else if (input.Key == ConsoleKey.Subtract)
                    {
                        jc.UndoJSONCount();
                    }
                    // write to file
                    Console.WriteLine($"Total JSON count is: {jc.TotalJSONCount}");
                    Console.WriteLine($"Current JSON count is: {jc.CurrentCourse.JSONCount}");
                }
            }
        }
    }
}
