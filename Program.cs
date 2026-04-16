using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading;
using sync.Enums;

class Program
{
    static void Menu()
    {
        Console.WriteLine("Press 1 - For printing folder content.");
        Console.WriteLine("Thread 1 is starting expensive operation...");
    }

    static void Main()
    {
        Thread t = new(new ThreadStart(() => 
        {
            Menu();
            while (true)
            {
                string? consoleLine = Console.ReadLine();
                if (consoleLine != null)
                {
                    var result = Enum.TryParse(consoleLine, ignoreCase: true, out CommandsEnum cmd);
                    
                    if (cmd.Equals(CommandsEnum.EXIT))
                    {
                        Console.WriteLine("bye bye");
                        break;
                    }               
                
                    switch (cmd)
                    {
                        case CommandsEnum.PATHS:
                            Console.WriteLine("this should print paths");
                            break;
                        default:
                            Console.WriteLine("command not found");
                            break;
                    }
                
                }
                
            }
        }));
        Thread t2 = new(new ThreadStart(() => 
        {
            Console.WriteLine("Thread 2: Expensive operation complete.");
        }));
        t2.Start();
        t.Start();

        // Wait for both threads to finish
        t.Join(); 
        t2.Join();

        Console.WriteLine("Both threads have finished and were automatically cleaned up by the CLR.");
    }
}