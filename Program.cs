using System;
using System.Collections.Specialized;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Numerics;

namespace KioskMainProject {
    internal class Program {

        public class Luhn {
            public static string isValid(string cardNum) {
                int cardDigits = cardNum.Length;
                int sum = 0;
                bool isSecond = false;
                string result = "";

                for (int i = cardDigits - 1; i >= 0; i--) {
                    int d = cardNum[i] - '0';

                    if (isSecond == true) {
                        d = d * 2;
                    }

                    sum += d / 10;
                    sum += d % 10;
                    isSecond = !isSecond;

                    if (sum % 10 == 0) {
                        result = "VALID";

                    } else {
                        result = "INVALID";
                    }
                }//END FOR
                Console.WriteLine(result);
                return result;
            }//END ISVALID CHECK
        }//END LUHN CLASS

        public class Bank {
            public void ChangeMaker(double change, double total) {
                double[] _cashValue = [100, 50, 20, 10, 5, 1, 0.25, 0.10, 0.05, .01];
                double[] _cashDrawer = [1, 0, 3, 4, 4, 8, 4, 6, 5, 10];
                //double[] _cashDrawer   = [0, 0, 0, 0, 0, 0, 0, 0, 0, 0];
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
                                _change = Math.Round(_change, 2);
                                _cashDrawer[i] = 0;
                                Thread.Sleep(500);
                            }//END IF

                            if (i == 9) {
                                string _response = "";
                                _change = Math.Round(_change, 2);
                                _response = Prompt($"${_change} left. This machine is out of money.\n\nWould you like to pay with card?(yes/no) ");
                                if (_response.ToLower() == "yes") {
                                    CardPay(total);
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


            }//END NEW GREED CHANGEMAKER

        }//END BANK

        static void Main(string[] args) {
            bool isDone = false;
            bool isValid = false;
            string check = "";
            string vendor = "";
            string date = "";
            string time = "";
            string[] infoCard = new string[3];
            string[] infoCash = new string[2];
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
            Console.WriteLine($"Total\t\t${total}\n-------------------------\n\n");

            isValid = false;
            i = 0;
            check = "";
            buffer = 0;
            string cardNum = "";
            while (isValid == false) {
                check = Prompt("\nPayment Method\t");
                if (check.ToLower() == "card") {
                    isValid = true;
                    infoCard = CardPay(total);
                } else if (check.ToLower() == "cash") {
                    isValid = true;
                    infoCash = CashPay(total);
                } else {
                    Console.WriteLine("You must choose either cash or card.\n");
                }
            }

            if (infoCard[0] == null || infoCard[1] == null || infoCard[2] == null) {
                for (i = 0; i < infoCard.Length; i++) {
                    infoCard[i] = "N/A";
                }
            }

            if (infoCash[1] == null) {
                infoCash[1] = "N/A";
            }

            if (infoCash[0] == null) {
                infoCash[0] = "N/A";
            }

            date = DateTime.Now.ToString("MMM-dd-yy");
            time = DateTime.Now.ToString("t");

            string transNum = DateTime.Now.ToString("HHmmssMMddyy");


            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = @"C:\Users\MCA-24\source\repos\Scratch\TransactionLogger\bin\Debug\net8.0\TransactionLogger.Exe";
            startInfo.Arguments = $"{transNum} {date} {time} {infoCash[1]} {infoCard[0]} {infoCard[1]} {infoCard[2]} {infoCash[0]}";
            Process.Start(startInfo);


        }//END MAIN

        static string[] MoneyRequest(string account_number, decimal amount) {
            Random rnd = new Random();

            bool pass = rnd.Next(100) < 50;
            bool declined = rnd.Next(100) < 50;
            if (pass) {
                return new string[] { account_number, amount.ToString() };
            } else {
                if (!declined) {
                    return new string[] { account_number, (amount / rnd.Next(2, 6)).ToString() };
                } else {
                    return new string[] { account_number, "declined" };
                }
            }
        }

        static string[] CardPay(double total) {

            bool isValid = false;
            double buffer = 0;
            string cardNum = "";
            string vendor = "";
            string check = "";
            int cashBack = 0;
            Console.Clear();
            while (isValid == false) {

                cardNum = Prompt("Credit Card #\t--\t");

                switch (cardNum[0]) {
                    case '4':
                        vendor = "Visa";
                        isValid = true;
                        break;
                    case '5':
                        vendor = "MasterCard";
                        isValid = true;
                        break;
                    case '3':
                        vendor = "AmericanExpress";
                        isValid = true;
                        break;
                    case '6':
                        vendor = "Discover";
                        isValid = true;
                        break;
                    case > '9':
                        isValid = false;
                        break;
                    case < '0':
                        isValid = false;
                        break;
                    default:
                        vendor = "Unkown";
                        isValid = true;
                        break;
                }//END VENDOR SWITCH
                        Console.WriteLine(vendor);
            }//END VALIDATION WHILE

            isValid = false;

            if (Luhn.isValid(cardNum) == "VALID") {
                while (isValid == false) {

                    check = Prompt("Would you like cash-back on this transaction?(yes/no)");

                    if (check.ToLower() == "yes") {
                        isValid = true;
                        cashBack = PromptInt($"Current total\t{total}\nHow much cash would you like back? (no coins)");
                        total += Math.Abs(cashBack);
                        string[] result = (MoneyRequest(cardNum, (decimal)total));
                        if (result[1] == "declined") {
                            isValid = false;

                            while (isValid == false) {
                            check = Prompt($"Card declined, but ${total} is still due.\nWould you like to complete payment with a different card or cancel transaction?(card/cancel)");
                                if (check.ToLower() == "card") {
                                    isValid = true;
                                    CardPay(total);
                                } else if (check.ToLower() == "cancel") {
                                    isValid = true;
                                    Console.WriteLine("Transaction voided. Have a nice day :)");
                                } else { Console.WriteLine("Please input card or cancel.\n"); }
                            }
                        } else {
                            double.TryParse(result[1], out buffer);
                            Console.WriteLine($"${buffer} paid with card...");
                            total -= buffer;
                            Console.WriteLine($"${total} due.");
                            if (total != 0) {
                                isValid = false;
                                while (isValid == false){
                                    check = Prompt("Would you like to pay the rest with cash, card, or cancel transaction?");
                                    if (check.ToLower() == "cash") { CashPay(total); isValid = true; } else if (check.ToLower() == "card") { CardPay(total); isValid = true; } else if (check.ToLower() == "cancel") { Console.WriteLine("Goodbye :)"); isValid = true; } else { Console.WriteLine("Please input something."); }
                                }
                            } else {
                                Bank bank2 = new Bank();
                                bank2.ChangeMaker(cashBack, total);
                                Console.WriteLine("Have a great day!");
                            }

                        }
                    } else if (check.ToLower() == "no") {
                        isValid = true;
                        string[] result = (MoneyRequest(cardNum, (decimal)total));
                        if (result[1] == "declined") {
                            double.TryParse(result[1], out buffer);
                            total -= buffer;
                            isValid = false;
                            while (isValid == false) {
                                check = Prompt($"Card declined, but ${total} is still due.\nWould you like to complete payment with a different card or cancel transaction?(card/cash/cancel)");
                                if (check.ToLower() == "card") {
                                    isValid = true;
                                    CardPay(total);
                                } else if (check.ToLower() == "cash") {
                                    isValid = true;
                                    CashPay(total);
                                } else if (check.ToLower() == "cancel") {
                                    isValid = true;
                                    Console.WriteLine("Transaction voided. Have a nice day :)");
                                } else {
                                    Console.WriteLine("Please input cash, card, or cancel.\n");
                                }
                            }
                        } else {
                            double.TryParse(result[1], out buffer);
                            Console.WriteLine($"${buffer} paid with card...");
                            total -= buffer;
                            Console.WriteLine($"APPROVED\n${total} due");
                            if (total != 0) {

                                isValid = false;
                                while (isValid == false) {
                                    check = Prompt("Would you like to pay the rest with cash, card, or cancel transaction?");

                                    if (check.ToLower() == "cash") {
                                        isValid = true;
                                        CashPay(total);
                                    } else if (check.ToLower() == "card") { 
                                        isValid = true;
                                        CardPay(total); 
                                    } else if (check.ToLower() == "cancel") { 
                                        isValid = true;
                                        Console.WriteLine("Goodbye :)"); 
                                    } else { 
                                        Console.WriteLine("Please input something."); }
                                }
                            }

                        }
                    } else {
                        Console.WriteLine("Please enter yes or no for cashback.");
                    }
                }
            } else {

                isValid = false;
                while (isValid == false) {
                    check = Prompt("Your card is invalid. Would you like to pay with cash, try a new card, or cancel transaction?(cash/card/cancel)\t");

                    if (check.ToLower() == "cash") {
                        isValid = true;
                        CashPay(total);
                    } else if (check.ToLower() == "card") {
                        isValid = true;
                        CardPay(total);
                    } else if (check.ToLower() == "cancel") {
                        isValid = true;
                        Console.WriteLine("Transaction voided. Have a nice day :)");
                    } else {
                        Console.WriteLine("Please input cash, card, or cancel.");
                    }
                }//END VALID LOOP
               
            }

            return new string[] { cardNum, vendor, buffer.ToString() };

        }

        static string[] CashPay(double total) {
            double change = 0;
            double payment = 0;
            double buffer = 0;
            bool isValid = false;
            string check = "";

            int i = 0;
            while (payment < total) {
                while (!isValid) {
                    check = Prompt($"Payment {i + 1}\t$");
                    double.TryParse(check, out buffer);
                    if (check == "100" || check == "50" || check == "20" || check == "10" || check == "5" || check == "1" || check == ".25" || check == ".1" || check == ".10" || check == ".05" || check == ".01") {
                        isValid = true;
                        payment += buffer;
                        change = payment - total;
                        change = Math.Round(change, 2);
                        if (payment < total) {
                            Console.WriteLine($"Remaining \t${Math.Abs(change)}");
                        }//END INNER IF
                    } else {
                        isValid = false;
                        Console.WriteLine("Must input legal U.S. tender");

                    }//END IF ELSE
                }

                i++;
                isValid = false;
            }

            change = Math.Round(change, 2);
            Console.WriteLine($"\nChange\t\t${change}\n-------------------------\n");



            Bank bank = new Bank();
            bank.ChangeMaker(change, total);

            return new string[] { change.ToString(), payment.ToString() };
        }
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
