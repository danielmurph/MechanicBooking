using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using FileHelpers;

//Please note that the following command may need to be run in the nuget package manager console, if the application doesn't build "Update-Package -Reinstall"
namespace MechanicBooking
{
    [DelimitedRecord(",")]
    public class Bookings
    {
        public string mechanicName;

        public string bookingDate;

        public string startTime;

        public string endTime;

        public string description;
    }

    class MechanicBooking
    {

        private void GetBookings(string selection)
        {
            var engine = new FileHelperEngine<Bookings>();
            var result = engine.ReadFile(selection);

            foreach (Bookings book in result)
            {
                Console.WriteLine("Date: " + book.bookingDate);
                Console.WriteLine("Mechanic: " + book.mechanicName);
                Console.WriteLine("Start Time: " + book.startTime);
                Console.WriteLine("End Time: " + book.endTime);
                Console.WriteLine("Desc: " + book.description);
                Console.WriteLine();
            }
        }

        void ViewBooking()
        {
            //Assumes that there are files in folder already.
            DirectoryInfo dir = new DirectoryInfo("C:\\Bookings");
            dir.Create();
            FileInfo[] files = dir.GetFiles("*.txt");
            Console.ForegroundColor = ConsoleColor.Yellow;

            foreach (FileInfo info in files)
            {
                Console.WriteLine("FileName:- " + info.Name);
            }

            if (files.Length == 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("No files found.");
                Main();
            }

            Console.Write("Please enter the file name to view: (eg 20190528) ");
            string selection = Console.ReadLine();

            try
            {
                GetBookings("C:\\Bookings\\" + selection + ".txt");
                Main();
            }
            catch (Exception e)
            {
                Console.WriteLine("The was a problem with your selection.");
            }
        }

        private void AddBooking()
        {
            Console.Clear();
            Console.WriteLine("Please enter name(date) for the file:");
            string selected = Console.ReadLine();
            Console.Clear();
            Console.WriteLine("Name of the Mechanic: ");
            string mechanicNameIn = Console.ReadLine();
            Console.WriteLine("Please enter a date for the booking:- \t(YYYY-MM-DD)");
            string bookingDateIn = Console.ReadLine();
            Console.WriteLine("Start time: ");
            string startTimeIn = Console.ReadLine();
            Console.WriteLine("End time: ");
            string endTimeIn = Console.ReadLine();
            Console.WriteLine("Description of work: ");
            string descriptionIn = Console.ReadLine();

            string file = "c:\\Bookings\\" + selected + ".txt";
            var bookingsIn = new List<Bookings>();

            var booking = new Bookings()
            {
                mechanicName = mechanicNameIn,
                bookingDate = bookingDateIn,
                startTime = startTimeIn,
                endTime = endTimeIn,
                description = descriptionIn
            };

            if (File.Exists(file))
            {
                try
                {
                    var engine = new FileHelperAsyncEngine<Bookings>();

                    using (engine.BeginReadFile(file))
                    {
                        bool exists = false;
                        foreach (Bookings bookingOut in engine)
                        {
                            //Check if booking already exists before adding to file.
                            if (!String.Equals(bookingOut.startTime, startTimeIn, StringComparison.OrdinalIgnoreCase) && !String.Equals(bookingOut.mechanicName, mechanicNameIn, StringComparison.OrdinalIgnoreCase))
                            {
                                bookingsIn.Add(bookingOut);
                                continue;
                            }
                            else
                            {
                                exists = true;
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("Unable to make booking at this time.");
                                break;
                            }
                        }

                        if (exists == false)
                        {
                            bookingsIn.Add(booking);
                        }
                        else
                        {
                            Main();
                        }
                    }

                    //Remove file if exists to prevent errors when writing new file.
                    File.Delete(file);
                    using (engine.BeginWriteFile(file))
                    {
                        foreach (Bookings bookingOut in bookingsIn)
                        {
                            engine.WriteNext(bookingOut);
                            Console.WriteLine("File written successfully!");
                        }
                    }
                    Main();
                }
                catch (Exception e)
                {
                    Console.WriteLine("There was an issue with the booking.");
                    Main();
                }
            }
            else
            {
                var engine = new FileHelperEngine<Bookings>();
                bookingsIn.Add(booking);
                engine.WriteFile(file, bookingsIn);
                Console.WriteLine("Booking Added!");
                Main();
            }
        }

        static void Main()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            MechanicBooking bookApp = new MechanicBooking();
            Console.WriteLine("Booking Application.\n\n");
            Console.WriteLine("1. Make Booking.\n");
            Console.WriteLine("2. View Bookings.\n");
            Console.WriteLine("0.Exit\n");
            Console.Write("\nPlease enter selection: ");
            int i = 0;

            try
            {
                i = Convert.ToInt32(Console.ReadLine());
            }
            catch
            {
                Console.WriteLine("Invalid format of selection.");
            }

            if (i == 1)
            {
                Console.Clear();
                bookApp.AddBooking();

            }
            else if (i == 2)
            {
                Console.Clear();
                bookApp.ViewBooking();
            }
            else if (i == 0)
            {
                Environment.Exit(0);
            }
            else
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("Invalid Selection.\n\n");
                Main();
            }
        }
    }
}
