using CoLearn.Domain.DTOs;
using CoLearn.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoLearn.Domain.Interfaces.Services
{
    public interface INotificationService
    {
        Task SendBookingCreatedAsync(BookingEmailDto bookingDto);
        Task SendReminderAsync(BookingEmailDto bookingDto);
    }
}

