// ...
using SaloonOS.Application.Common.Contracts;

public interface IUnitOfWork : IDisposable
{
    IShopRepository Shops { get; }
    IServiceRepository Services { get; } // <-- ADD THIS
    Task<int> CompleteAsync();
    IStaffMemberRepository StaffMembers { get; }
    IRepository<T> GetRepository<T>() where T : class;
    IAppointmentRepository Appointments { get; }
    ICustomerRepository Customers { get; }
    IWorkScheduleRepository WorkSchedules { get; }
}