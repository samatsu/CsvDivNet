using CsvDivNet.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsvDivNet.Cli
{
    class Program
    {
        public static readonly int EXIT_SUCCESS = 0;
        public static readonly int EXIT_HELP = 1;
        public static readonly int EXIT_ERROR = 2;

        static void Main(string[] args)
        {
            ExecuteMain(args);
        }
        private static void ExecuteMain(string[] args)
        {
            Environment.ExitCode = EXIT_SUCCESS;
            try
            {
                CommandLineOption cmd = new CommandLineOption(args);
                if (cmd.HasHelpSwitch())
                {
                    Console.WriteLine(CommandLineOption.GetHelpMessage());
                    System.Environment.Exit(EXIT_HELP);
                }
                CsvDivConfig config = cmd.CreateConfig();
                ValidationResult valid = config.Valid();
                if (valid == ValidationResult.Success)
                {
                    ConsoleExecutor.Execute(config);
                }
                else
                {
                    Console.WriteLine(CommandLineOption.GetHelpMessage());
                    Console.WriteLine(valid.ErrorMessage);
                    System.Environment.Exit(EXIT_ERROR);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Environment.ExitCode = EXIT_ERROR;
            }

        }

    }
}
