using System;

namespace SupportBank
{
    static class ConsoleInterface
    {


        public static string PromptCommand(out string arguments)
        {
            while (true)
            {
                Console.WriteLine("Please enter a command:");
                var input = Console.ReadLine()?.Split(new []{' '}, 2, StringSplitOptions.None);
                arguments = input.Length == 1 ? String.Empty : input[1];
                return input[0].ToLower();
            }
        }

        public static void MainPrompt()
        {
            while (true)
            {
                var command = ConsoleInterface.PromptCommand(out var argument);
                switch (command)
                {
                    case "import":
                    {
                        if (argument.ToLower().StartsWith("file "))
                        {
                            argument = argument.Split(' ')[1];
                            var filename = argument.Length == 1 ? string.Empty : argument;
                            FileManager.LoadTransactionFile(filename);

                        }
                        else
                        {
                            Console.WriteLine("usage: import file filepath");
                        }
                        
                        break;
                    }
                    case "quit":
                    case "q":
                    case "exit":
                        return;
                    case "list":
                    {
                        if (argument == "all")
                            Accountant.ListAccounts();
                        else if (argument == string.Empty)
                            Console.WriteLine("Please enter a name or \"all\" after list");
                        else
                            Accountant.ListAccountTransactions(argument);
                        break;
                    }
                    default:
                    {
                        Console.WriteLine("Command not found");
                        break;
                    }

                }

            }
        }
    }
}