using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrientTest.model
{
    public class BookingRequest
    {

        public int id { get; set; } 
        public DateTime RequestSubmission { get; set; }
        public string EmployeeId { get; set; }
        public DateTime MeetingStartTime { get; set; }
        public int MeetingDuration { get; set; }
        public string TimeStart { get; set; }
        public string TimeEnd { get; set; }

        public BookingRequest()
        {
            string[] DatetimeFormat = { "yyyy-MM-dd hh:mm:ss", "yyyy-MM-dd hh:mm" };
            DateTime endDate;
            endDate = MeetingStartTime.AddHours(MeetingDuration);
            TimeStart = MeetingStartTime.ToString("HH:mm");
            TimeEnd = endDate.ToString("HH:mm");
        }
    }
}
