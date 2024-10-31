using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DanteAPI;
using Microsoft.Extensions.Configuration;

namespace DanteAPIEnrolment
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await MainAsync();
        }

        static async Task MainAsync()
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();

            using var api = new DanteAPI.Main(configuration["DanteURL"], configuration["DanteAPIKey"]);

            Console.WriteLine("Welcome to Dante Enrolment!");

            Console.Write("Please enter a schedule reference to begin: ");
            string scheduleReference = Console.ReadLine();

            if (string.IsNullOrEmpty(scheduleReference))
            {
                Console.WriteLine("Schedule Reference cannot be blank.");
                return;
            }

            var schedule = await GetScheduleAsync(api, scheduleReference);
            if (schedule == null)
            {
                Console.WriteLine($"Schedule does not exist with reference: {scheduleReference}");
                return;
            }

            Console.WriteLine($"Enrolling on course {schedule.CourseName} on {schedule.StartDate.ToShortDateString()}");

            var scheduleDelegateGroup = await GetGroupBookingAsync(api, schedule.ID);
            if (scheduleDelegateGroup == null)
            {
                Console.WriteLine($"A group booking doesn't exist on schedule: {scheduleReference}");
                return;
            }

            Console.WriteLine($"Group booking for {scheduleDelegateGroup.Booking.Company.Name} has {scheduleDelegateGroup.Quantity} place(s) remaining.");

            string firstName = PromptInput("Enter your first name: ");
            string surname = PromptInput("Enter your surname: ");
            string email = PromptInput("Enter your email: ");

            if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(surname) || string.IsNullOrEmpty(email))
            {
                Console.WriteLine("You must enter your first name, surname, and email.");
                return;
            }

            var delegateEntity = await GetOrCreateDelegateAsync(api, scheduleDelegateGroup.Booking.Company.ID, firstName, surname, email);
            if (delegateEntity == null)
            {
                Console.WriteLine("Failed to create or update delegate.");
                return;
            }

            bool enrollSuccess = await EnrollDelegateAsync(api, schedule.ID, scheduleDelegateGroup.Booking.ID, scheduleDelegateGroup.DelegateID, delegateEntity.ID);
            if (enrollSuccess)
            {
                Console.WriteLine("Delegate has been added to this schedule.");
            }
            else
            {
                Console.WriteLine("Failed to enroll delegate to the schedule.");
            }
        }

        static string PromptInput(string message)
        {
            Console.Write(message);
            return Console.ReadLine();
        }

        static async Task<DanteAPI.Entities.Schedule> GetScheduleAsync(DanteAPI.Main api, string scheduleReference)
        {
            var selectResponse = await api.Select<DanteAPI.Entities.Schedule>(
                new List<string> { "BookingID", "CourseName", "StartDate" },
                new List<DanteAPI.Main.Filter>
                {
                    new DanteAPI.Main.Filter { FieldName = "Reference", Operator = "=", Value = scheduleReference }
                });

            if (!selectResponse.IsSuccess)
            {
                Console.WriteLine($"Error fetching schedule: {selectResponse.ErrorMessage}");
                return null;
            }

            return selectResponse.Data.FirstOrDefault();
        }

        static async Task<DanteAPI.Entities.ScheduleDelegate> GetGroupBookingAsync(DanteAPI.Main api, int scheduleID)
        {
            var selectResponse = await api.Select<DanteAPI.Entities.ScheduleDelegate>(
                new List<string> { "Booking", "Booking.Company", "DelegateID", "Quantity" },
                new List<DanteAPI.Main.Filter>
                {
                    new DanteAPI.Main.Filter { FieldName = "ScheduleID", Operator = "=", Value = scheduleID.ToString() },
                    new DanteAPI.Main.Filter { FieldName = "GroupBooking", Operator = "=", Value = true.ToString() }
                });

            if (!selectResponse.IsSuccess)
            {
                Console.WriteLine($"Error fetching group booking: {selectResponse.ErrorMessage}");
                return null;
            }

            return selectResponse.Data.FirstOrDefault();
        }

        static async Task<DanteAPI.Entities.Delegate> GetOrCreateDelegateAsync(DanteAPI.Main api, int companyID, string firstName, string surname, string email)
        {
            var selectResponse = await api.Select<DanteAPI.Entities.Delegate>(
                new List<string>(),
                new List<DanteAPI.Main.Filter>
                {
                    new DanteAPI.Main.Filter { FieldName = "CompanyID", Operator = "=", Value = companyID.ToString() },
                    new DanteAPI.Main.Filter { FieldName = "Email", Operator = "=", Value = email }
                });

            if (!selectResponse.IsSuccess)
            {
                Console.WriteLine($"Error fetching delegate: {selectResponse.ErrorMessage}");
                return null;
            }

            var delegateEntity = selectResponse.Data.FirstOrDefault();

            if (delegateEntity == null)
            {
                Console.WriteLine("Delegate not found, creating a new one.");
                var insertResponse = await api.Insert<DanteAPI.Entities.Delegate>(new Dictionary<string, string>
                {
                    { "CompanyID", companyID.ToString() },
                    { "FirstName", firstName },
                    { "Surname", surname },
                    { "Email", email }
                });

                if (!insertResponse.IsSuccess)
                {
                    Console.WriteLine($"Error creating delegate: {insertResponse.ErrorMessage}");
                    return null;
                }

                delegateEntity = insertResponse.Data;
            }
            else
            {
                Console.WriteLine("Delegate found, updating information.");
                var updateResponse = await api.Update<DanteAPI.Entities.Delegate>(delegateEntity.ID, new Dictionary<string, string>
                {
                    { "CompanyID", companyID.ToString() },
                    { "FirstName", firstName },
                    { "Surname", surname },
                    { "Email", email }
                });

                if (!updateResponse.IsSuccess)
                {
                    Console.WriteLine($"Error updating delegate: {updateResponse.ErrorMessage}");
                    return null;
                }

                delegateEntity = updateResponse.Data;
            }

            return delegateEntity;
        }

        static async Task<bool> EnrollDelegateAsync(DanteAPI.Main api, int scheduleID, int bookingID, int delegateID, int newDelegateID)
        {
            var customActionResponse = await api.CustomAction<DanteAPI.Entities.ScheduleDelegate>("Exchange", new Dictionary<string, string>
            {
                { "ScheduleID", scheduleID.ToString() },
                { "BookingID", bookingID.ToString() },
                { "DelegateID", delegateID.ToString() },
                { "NewDelegateID", newDelegateID.ToString() }
            });

            if (!customActionResponse.IsSuccess)
            {
                Console.WriteLine($"Error enrolling delegate: {customActionResponse.ErrorMessage}");
                return false;
            }

            return true;
        }
    }
}
