using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.ApplicationModel.Appointments;
using Windows.ApplicationModel.Appointments.AppointmentsProvider;

namespace SpeechCustomUi
{
    class Event
    {
        //create event and add to calendar
        async void create(String subject, DateTimeOffset time)
        {
            Rect rect = new Rect(1, 1, 1, 1);

            Appointment appointment = new Appointment();
            appointment.Subject = subject;
            appointment.AllDay = false;
            appointment.StartTime = time;
            appointment.Duration = new TimeSpan(1); //default 1 hour duration

            await AppointmentManager.ShowAddAppointmentAsync(appointment, rect, Windows.UI.Popups.Placement.Default);
        }
    }
}
