using DeviceManagement_WebApp.Models;
using System;
using System.Collections.Generic;

namespace DeviceManagement_WebApp.Repositories
{
    public interface IDeviceRepository: IGenericRepository<Device>
    {
        new Device GetById(Guid? id);
        new IEnumerable<Device> GetAll();

        IEnumerable<Category> GetCategories();

        IEnumerable<Zone> GetZones();
    }
}
