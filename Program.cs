using System.Diagnostics;
using System.Numerics;

namespace KioskMainProject {
    internal class Program {

        public class Bank {
            public void ChangeMaker(double change, double total) {
                double[] _cashValue = [100, 50, 20, 10, 5, 1, 0.25, 0.10, 0.05, .01];
                double[] _cashDrawer   = [1, 0, 3, 4, 4, 8, 4, 6, 5, 10];
                double _buffer = 0;
                double _total = total;
                double _change = change;


                for (int i = 0; i < _cashValue.Length; i++) {
                    if ((_change % _cashValue[i]) < _change) {
                        if (_change / _cashValue[i] <= _cashDrawer[i]) {
                            _buffer = _cashDrawer[i] - (int)(_change / _cashValue[i]);
                            _change = _change % _cashValue[i];
                            _change = Math.Round(_change, 2);
                            Console.WriteLine($"{_cashDrawer[i] - _buffer} ${_cashValue[i]} dispensed...");
                            _cashDrawer[i] = _buffer;
                                Thread.Sleep(500);
                        } else {

                            _buffer = (_cashDrawer[i] * _cashValue[i]);

                            if (_buffer > 0) {
                                Console.WriteLine($"{_cashDrawer[i]} ${_cashValue[i]} dispensed...");
                                _change -= _buffer;
                                _change = Math.Round( _change, 2);
                                _cashDrawer[i] = 0;
                                Thread.Sleep(500);
                            }//END IF

                            if (i == 9) {
                                string _response = "";
                                _change = Math.Round( _change, 2);
                                _response = Prompt($"${_change} left. This machine is out of money.\n\nTransaction canceled...\n\nWould you like to pay another way?\n\n\t");
                                if (_response.ToLower() == "yes") {
                                    Console.WriteLine($"\n\nPlease input card. Balance due - ${_total}");
                                } else if (_response.ToLower() == "no") {
                                    Console.Clear();
                                    Console.Write("TRANSACTION VOIDING");
                                    Thread.Sleep(1000);
                                    Console.Write(".");
                                    Thread.Sleep(1000);
                                    Console.Write(".");
                                    Thread.Sleep(1000);
                                    Console.Write(".");
                                    Thread.Sleep(1000);
                                    Console.WriteLine("\n\nYOUR GROCERIES ARE NOW TERMINATED.");
                                }
                            }//END INNER IF
                        }//END IF ELSE
                    }//END IF
                }//END FOR LOOP
                Console.WriteLine($"\n${_change} due. Have a great day!!");
    
            }//END NEW GREED CHANGEMAKER

        }//END BANK
        
        static void Main(string[] args) {
            bool isDone = false;
            bool isValid = false;
            string check = "";
            string date = "";
            string time = "";
            double total = 0;
            double payment = 0;
            double buffer = 0;
            double change = 0;
            int i = 0;

            while (!isDone) {
                while (!isValid) {
                    check = Prompt($"Item {i + 1}\t\t$");
                    if (string.IsNullOrEmpty(check)) {
                        isDone = true;
                        isValid = true;
                    } else {
                        double.TryParse(check, out buffer);
                        if (buffer > 0 && double.TryParse(check, out buffer)) {
                            total += buffer;
                            isValid = true;
                        } else {
                            Console.WriteLine("Not real number.");
                        }
                        
                    }//END IF ELSE
                }
                isValid = false;
                i++;
            }//END FOR LOOP

            total = Math.Round(total, 2);
            Console.WriteLine($"\n\nTotal  \t\t${total}\n\n");
            
            isValid = false;
            i = 0;
            check = "";
            buffer = 0;
            while (payment < total) {
                while (!isValid) {
                    check = Prompt($"Payment {i + 1}\t$");
                    if (check.ToLower() == "card") {
                        Console.WriteLine("Would you like to pay with card?");
                    } else {
                        double.TryParse(check, out buffer);

                        if (buffer > 0) {
                            isValid = true;
                            payment += buffer;
                            change = payment - total;
                            change = Math.Round(change, 2);
                            if (payment < total) {
                                Console.WriteLine($"Remaining \t${Math.Abs(change)}");
                            }//END INNER IF
                        } else {
                            Console.WriteLine("\nNot real number.\n");
                        }//END IF ELSE
                    }
                }//END WHILE INVALID
                i++;
                isValid = false;
            }//END WHILE

            change = Math.Round(change, 2);
            Console.WriteLine($"\nChange\t\t${change}\n-------------------------\n");



            Bank bank = new Bank();
            bank.ChangeMaker(change, total);

            date = DateTime.Now.ToString("MMM-dd-yy");
            time = DateTime.Now.ToString("t");


            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = @"C:\Users\MCA-24\source\repos\Scratch\TransactionLogger\bin\Debug\net8.0\TransactionLogger.Exe";
            startInfo.Arguments = $"12 {date} {time} {payment} red 0 {change}";
            Process.Start(startInfo);


        }//END MAIN
        #region HELPER FUNCTIONS
        static void Fancify(string text) {

            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("**##=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=##**");

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"\t--->>\\{text}/<<---");

            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("**##=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=##**");

            Console.ResetColor();

        }//END FANCIFY
        static string Prompt(string request) {
            //Variables
            string userInput = "";

            //Request Information From User
            Console.Write(request);

            //Receive Response
            userInput = Console.ReadLine();

            return userInput;

        }//END PROMPT HELPER
        static int PromptInt(string request) {

            int userInput = 0;
            bool isValid = false;

            Console.Write(request);
            isValid = int.TryParse(Console.ReadLine(), out userInput);

            while (isValid == false) {

                Console.WriteLine("ERROR: NO REAL NUMBER");
                Console.Write(request);
                isValid = int.TryParse(Console.ReadLine(), out userInput);
            }//END WHILE

            return userInput;

        }//END PROMPT TRY INT
        static double PromptDouble(string request) {

            double userInput = 0;
            bool isValid = false;

            Console.Write(request);
            isValid = double.TryParse(Console.ReadLine(), out userInput);


            return userInput;

        }//END PROMPT DOUBLE
        #endregion
    }//END CLASS
}//END NAMESPACE
