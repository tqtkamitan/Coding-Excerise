using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using OrientTest.model;

namespace OrientTest
{
    internal static class Program
    {
        public static DateTime OfficeStartDate = DateTime.MinValue;
        public static DateTime OfficeEndDate = DateTime.MaxValue;
        public static string[] DatetimeFormat = { "yyyy-MM-dd HH:mm:ss", "yyyy-MM-dd HH:mm" };

        static void Main(string[] args)
        {
            string fileName = "Input.txt";
            const Int32 BufferSize = 128;
            List<BookingRequest> bookingRequests = new List<BookingRequest>();
            try
            {
                using (var fileStream = File.OpenRead(@"Input\" + fileName))
                using (var streamReader = new StreamReader(fileStream, Encoding.UTF8, true, BufferSize))
                {
                    string firstLine = streamReader.ReadLine();
                    string[] officeHours = firstLine.Trim().Split(" ");

                    // Get Office Hour
                    DateTime StartDate;
                    DateTime.TryParseExact(officeHours[0], "HHmm", CultureInfo.InvariantCulture, DateTimeStyles.None, out StartDate);
                    OfficeStartDate = StartDate;
                    DateTime EndDate;
                    DateTime.TryParseExact(officeHours[1], "HHmm", CultureInfo.InvariantCulture, DateTimeStyles.None, out EndDate);
                    OfficeEndDate = EndDate;

                    string line;
                    List<string> bookingRequestData = new List<string>();
                    int index = 1;
                    while ((line = streamReader.ReadLine()) != null)
                    {
                        bookingRequestData.Add(line);
                        if (bookingRequestData.Count == 3)
                        {
                            string[] lineThree = bookingRequestData[2].Trim().Split(" ");
                            string MeetingStartTime = lineThree[0] + " " + lineThree[1];
                            int MeetingDuration = 0;
                            Int32.TryParse(lineThree[2],out MeetingDuration);
                            DateTime meetingStart, meetingEnd, requestSubmission;
                            DateTime.TryParseExact(MeetingStartTime, DatetimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out meetingStart);
                            DateTime.TryParseExact(bookingRequestData[0], DatetimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out requestSubmission);
                            meetingEnd = meetingStart.AddHours(MeetingDuration);
                            BookingRequest bookingRequest = new BookingRequest(){ 
                                id = index,
                                RequestSubmission = requestSubmission,
                                EmployeeId = bookingRequestData[1],
                                MeetingStartTime = meetingStart,
                                MeetingDuration = MeetingDuration,
                                TimeStart = meetingStart.ToString("HH:mm"),
                                TimeEnd = meetingEnd.ToString("HH:mm")
                            };
                            bookingRequests.Add(bookingRequest);
                            bookingRequestData.Clear();
                            index++;
                        }
                    }
                    streamReader.Close();
                }
                bookingRequests = ValidateBookingRequest(bookingRequests);
                OutputResult(bookingRequests);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                // TODO: Remove Inner Exception
                Console.WriteLine(e.InnerException);
            }
        }

        static List<BookingRequest> ValidateBookingRequest(List<BookingRequest> bookingRequests)
        {
            bookingRequests = bookingRequests.OrderBy(bookingRequest => bookingRequest.RequestSubmission).ToList();
            List<BookingRequest> bookingRequest = new List<BookingRequest>();
            foreach (var item in bookingRequests.ToList())
            {
                DateTime meetingEnd = item.MeetingStartTime.AddHours(item.MeetingDuration);
                
                if (item.MeetingStartTime.TimeOfDay >= OfficeStartDate.TimeOfDay && meetingEnd.TimeOfDay <= OfficeEndDate.TimeOfDay)
                {
                    var validBooking = bookingRequests.SingleOrDefault(bookingRequest => bookingRequest.id == item.id);
                    //add valid Booking from List
                    if (!bookingRequest.Any(bookingRequest => IsBewteenTwoDates(validBooking.MeetingStartTime, bookingRequest.MeetingStartTime, bookingRequest.MeetingStartTime.AddHours(bookingRequest.MeetingDuration))
                        && IsBewteenTwoDates(validBooking.MeetingStartTime.AddHours(validBooking.MeetingDuration), bookingRequest.MeetingStartTime, bookingRequest.MeetingStartTime.AddHours(bookingRequest.MeetingDuration))))
                    {
                        bookingRequest.Add(validBooking);
                    }
                }
            }
            return bookingRequest;
        }

        static void OutputResult(List<BookingRequest> bookingRequests)
        {
            bookingRequests = bookingRequests.OrderBy(bookingRequest => bookingRequest.MeetingStartTime).ToList();
            foreach (var item in bookingRequests)
            {
                Console.WriteLine(item.MeetingStartTime.ToString("yyyy-MM-dd"));
                Console.WriteLine(item.TimeStart + " " + item.TimeEnd);
                Console.WriteLine(item.EmployeeId);
            }
        }

        static bool IsBewteenTwoDates(this DateTime dateTarget, DateTime start, DateTime end)
        {
            return dateTarget >= start && dateTarget <= end;
        }
    }
}
